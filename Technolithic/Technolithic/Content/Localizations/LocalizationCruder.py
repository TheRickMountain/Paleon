#!filepath LocalizationCruder.py
import tkinter as tk
from tkinter import ttk, messagebox
import csv
import os
import sys
import logging
from typing import Dict, List, Set, Optional, Any, Tuple

# Set up basic logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

class LocalizationEditor:
    """
    A GUI application for editing localization CSV files.

    Provides functionalities to load, display, search, filter, add, edit (in-place),
    delete, and save localization entries across multiple language files.
    Includes visual tracking for unsaved changes.
    """
    def __init__(self, root: tk.Tk) -> None:
        """
        Initializes the LocalizationEditor application.

        Args:
            root: The main Tkinter window.
        """
        self.root: tk.Tk = root
        self.root.title("Localization File Editor")
        self.root.geometry("1000x600")

        # Store loaded data
        self.files_data: Dict[str, Dict[str, str]] = {}  # {filename: {key: translation}}
        self.languages: List[str] = []   # List of language filenames (column headers)
        self.all_keys: Set[str] = set() # Set of all unique keys across files
        self.filtered_keys: Set[str] = set() # For search/filter functionality
        self.filter_untranslated: bool = False # Toggle for untranslated entries

        # --- State for tracking unsaved changes ---
        self.unsaved_changes: Set[str] = set()

        # In-place editing state
        self._edit_entry: Optional[ttk.Entry] = None
        self._edit_item_id: Optional[str] = None
        self._edit_column_id: Optional[str] = None

        # Get current directory (where the program is located)
        try:
            self.current_directory: str = os.path.dirname(os.path.abspath(__file__))
        except NameError: # Handle case where __file__ is not defined (e.g., interactive)
             self.current_directory: str = os.getcwd()
        logging.info(f"Working directory set to: {self.current_directory}")

        # --- GUI Setup ---
        self._setup_ui()

        # Load files automatically at startup
        self.load_files()

    def _setup_ui(self) -> None:
        """Sets up the graphical user interface elements."""
        # Create main frame
        self.main_frame = ttk.Frame(self.root, padding="10")
        self.main_frame.grid(row=0, column=0, sticky="nsew")

        # Top controls frame
        top_frame = ttk.Frame(self.main_frame)
        top_frame.grid(row=0, column=0, sticky="ew", pady=(0, 5))
        top_frame.columnconfigure(1, weight=1) # Allow directory label to expand

        # Load button
        ttk.Button(top_frame, text="Load Files", command=self.load_files).grid(row=0, column=0, padx=5)

        # Show current directory
        self.dir_label = ttk.Label(top_frame, text=f"Working dir: {self.current_directory}", anchor="w")
        self.dir_label.grid(row=0, column=1, padx=5, sticky="ew")

        # Search frame
        search_frame = ttk.LabelFrame(self.main_frame, text="Search & Filter")
        search_frame.grid(row=1, column=0, sticky="ew", pady=5)
        search_frame.columnconfigure(1, weight=1) # Allow search entry to expand

        # Search Label and Entry
        ttk.Label(search_frame, text="Search:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        self.search_var = tk.StringVar()
        self.search_entry = ttk.Entry(search_frame, textvariable=self.search_var)
        self.search_entry.grid(row=0, column=1, padx=5, pady=5, sticky="ew")
        self.search_var.trace_add("write", self._apply_filter_callback)
        self._apply_cyrillic_bindings(self.search_entry) # Apply Cyrillic bindings

        # Search options Checkbuttons
        self.search_keys_var = tk.BooleanVar(value=True)
        self.search_translations_var = tk.BooleanVar(value=True)
        ttk.Checkbutton(search_frame, text="Keys", variable=self.search_keys_var,
                        command=self.apply_filter).grid(row=0, column=2, padx=5)
        ttk.Checkbutton(search_frame, text="Translations", variable=self.search_translations_var,
                        command=self.apply_filter).grid(row=0, column=3, padx=5)

        # Filter untranslated Checkbutton
        self.untranslated_var = tk.BooleanVar(value=False)
        ttk.Checkbutton(search_frame, text="Show only untranslated",
                        variable=self.untranslated_var, command=self.toggle_untranslated
                        ).grid(row=0, column=4, padx=5)

        # --- Treeview Setup ---
        tree_frame = ttk.Frame(self.main_frame)
        tree_frame.grid(row=2, column=0, sticky="nsew", pady=(5, 0))
        tree_frame.columnconfigure(0, weight=1)
        tree_frame.rowconfigure(0, weight=1)

        self.tree = ttk.Treeview(tree_frame, show="headings")
        self.tree.grid(row=0, column=0, sticky="nsew")

        # --- Configure Treeview Tags ---
        self.tree.tag_configure('untranslated', background='#ffe6e6') # Light red background
        self.tree.tag_configure('modified', background='#e6f7ff') # Light blue background for modified

        # Scrollbars
        v_scrollbar = ttk.Scrollbar(tree_frame, orient="vertical", command=self.tree.yview)
        v_scrollbar.grid(row=0, column=1, sticky="ns")
        self.tree.configure(yscrollcommand=v_scrollbar.set)

        h_scrollbar = ttk.Scrollbar(self.main_frame, orient="horizontal", command=self.tree.xview)
        h_scrollbar.grid(row=3, column=0, sticky="ew") # Place below tree frame
        self.tree.configure(xscrollcommand=h_scrollbar.set)

        # Bind double-click for in-place editing
        self.tree.bind("<Double-1>", self._on_double_click)

        # Status bar
        self.status_var = tk.StringVar(value="Ready")
        status_bar = ttk.Label(self.main_frame, textvariable=self.status_var, relief=tk.SUNKEN, anchor=tk.W)
        status_bar.grid(row=4, column=0, sticky="ew", pady=(5, 0))

        # Buttons frame
        btn_frame = ttk.Frame(self.main_frame)
        btn_frame.grid(row=5, column=0, pady=5, sticky="ew")
        # Center buttons in the frame
        btn_frame.columnconfigure(0, weight=1)
        btn_frame.columnconfigure(1, weight=1)
        btn_frame.columnconfigure(2, weight=1)
        btn_frame.columnconfigure(3, weight=1)
        btn_frame.columnconfigure(4, weight=1)

        ttk.Button(btn_frame, text="Add Entry", command=self.add_entry).grid(row=0, column=0, padx=5)
        ttk.Button(btn_frame, text="Edit Key", command=self.edit_key_popup).grid(row=0, column=1, padx=5) # Renamed for clarity
        ttk.Button(btn_frame, text="Delete Entry", command=self.delete_entry).grid(row=0, column=2, padx=5)
        ttk.Button(btn_frame, text="Save All Files", command=self.save_files).grid(row=0, column=3, padx=5)
        ttk.Button(btn_frame, text="Clear Filters", command=self.clear_filters).grid(row=0, column=4, padx=5)

        # Configure main grid weights
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        self.main_frame.columnconfigure(0, weight=1)
        self.main_frame.rowconfigure(2, weight=1) # Allow treeview frame to expand

    def _apply_cyrillic_bindings(self, widget: tk.Widget) -> None:
        """
        Applies Cyrillic Cut/Copy/Paste bindings for Windows compatibility.

        Args:
            widget: The Tkinter widget (typically an Entry) to apply bindings to.
        """
        if sys.platform == "win32": # Apply only on Windows
            # Using key symbols directly might be more robust than Cyrillic names
            # Map based on standard QWERTY <-> ЙЦУКЕН layout physical key positions
            widget.bind("<Control-KeyPress-x>", lambda e: widget.event_generate("<<Cut>>"))
            widget.bind("<Control-KeyPress-X>", lambda e: widget.event_generate("<<Cut>>"))
            widget.bind("<Control-KeyPress-c>", lambda e: widget.event_generate("<<Copy>>"))
            widget.bind("<Control-KeyPress-C>", lambda e: widget.event_generate("<<Copy>>"))

            # Add explicit Cyrillic bindings as fallback/primary method if needed
            widget.bind("<Control-KeyPress-Cyrillic_che>", lambda e: widget.event_generate("<<Cut>>")) # ч -> x
            widget.bind("<Control-KeyPress-Cyrillic_es>", lambda e: widget.event_generate("<<Copy>>"))  # с -> c
            widget.bind("<Control-KeyPress-Cyrillic_em>", lambda e: widget.event_generate("<<Paste>>")) # м -> v
            widget.bind("<Control-KeyPress-Cyrillic_CHE>", lambda e: widget.event_generate("<<Cut>>")) # Ч -> X
            widget.bind("<Control-KeyPress-Cyrillic_ES>", lambda e: widget.event_generate("<<Copy>>"))  # С -> C
            widget.bind("<Control-KeyPress-Cyrillic_EM>", lambda e: widget.event_generate("<<Paste>>")) # М -> V


    def _mark_as_modified(self, key: str) -> None:
        """Adds the key to unsaved changes and updates its tag in the Treeview."""
        if not key: return
        self.unsaved_changes.add(key)
        if self.tree.exists(key):
            current_tags = list(self.tree.item(key, 'tags'))
            if 'modified' not in current_tags:
                current_tags.append('modified')
                self.tree.item(key, tags=tuple(current_tags))
            logging.debug(f"Marked key '{key}' as modified.")

    def _clear_modified_flag(self, key: str) -> None:
        """Removes the key from unsaved changes and updates its tag in the Treeview."""
        if not key: return
        self.unsaved_changes.discard(key)
        if self.tree.exists(key):
            current_tags = list(self.tree.item(key, 'tags'))
            if 'modified' in current_tags:
                current_tags.remove('modified')
                self.tree.item(key, tags=tuple(current_tags))
            logging.debug(f"Cleared modified flag for key '{key}'.")

    def load_files(self) -> None:
        """Loads all CSV files from the current directory."""
        logging.info("Attempting to load files...")
        # --- Clear unsaved changes state ---
        self.unsaved_changes.clear()
        self._cancel_cell_edit() # Ensure no lingering edit entry
        # Clear existing data
        self.files_data.clear()
        self.languages.clear()
        self.all_keys.clear()
        self.tree.delete(*self.tree.get_children())
        self.tree["columns"] = ()

        directory = self.current_directory
        csv_files: List[str] = []
        try:
            csv_files = [f for f in os.listdir(directory) if f.lower().endswith('.csv')]
        except FileNotFoundError:
            messagebox.showerror("Error", f"Directory not found: {directory}")
            self.status_var.set(f"Error: Directory not found: {directory}")
            logging.error(f"Directory not found: {directory}")
            return
        except Exception as e:
            messagebox.showerror("Error", f"Failed to list directory contents: {str(e)}")
            self.status_var.set("Error reading directory")
            logging.error(f"Failed to list directory contents: {e}", exc_info=True)
            return

        if not csv_files:
            messagebox.showinfo("Information", f"No CSV files found in {directory}")
            self.status_var.set(f"No CSV files found in {directory}")
            logging.info(f"No CSV files found in {directory}")
            return

        loaded_count = 0
        for filename in sorted(csv_files): # Sort for consistent column order
            file_path = os.path.join(directory, filename)
            data: Dict[str, str] = {}
            try:
                with open(file_path, 'r', encoding='utf-8', newline='') as f:
                    reader = csv.DictReader(f)
                    if 'key' not in reader.fieldnames or 'translation' not in reader.fieldnames:
                         logging.warning(f"Skipping {filename}: Missing 'key' or 'translation' column.")
                         messagebox.showwarning("Warning", f"Skipping {filename}: Missing 'key' or 'translation' column.")
                         continue
                    for row_num, row in enumerate(reader, 2): # Start row count from 2 for logging
                        key = row.get('key')
                        translation = row.get('translation', '') # Handle missing translation value gracefully
                        if key: # Ensure key is not empty
                            if key in data:
                                logging.warning(f"Duplicate key '{key}' found in {filename} (row {row_num}). Using last value.")
                            data[key] = translation if translation is not None else ""
                            self.all_keys.add(key)
                        else:
                            logging.warning(f"Empty key found in {filename} (row {row_num}). Skipping row: {row}")

                self.files_data[filename] = data
                self.languages.append(filename)
                loaded_count += 1
                logging.info(f"Successfully loaded {filename}")
            except Exception as e:
                messagebox.showerror("Error", f"Failed to load {filename}: {str(e)}")
                logging.error(f"Failed to load {filename}: {e}", exc_info=True)

        if not self.languages:
            self.status_var.set("Failed to load any valid CSV files.")
            return

        # Initialize filtered keys and populate tree
        self.clear_filters(update_status=False) # This calls populate_treeview

        # Configure Treeview columns
        columns = ["key"] + self.languages
        self.tree["columns"] = columns
        self.tree.column("key", width=200, stretch=True, anchor="w")
        self.tree.heading("key", text="Key")
        for lang in self.languages:
            self.tree.column(lang, width=200, stretch=True, anchor="w")
            self.tree.heading(lang, text=lang)

        self.populate_treeview() # Populate happens here, unsaved_changes is already empty
        self.status_var.set(f"Loaded {loaded_count} language files with {len(self.all_keys)} unique keys. No unsaved changes.")
        logging.info(f"Finished loading: {loaded_count} files, {len(self.all_keys)} keys.")

    def populate_treeview(self) -> None:
        """Populates the Treeview with the current filtered data."""
        self._cancel_cell_edit() # Ensure no lingering edit entry
        self.tree.delete(*self.tree.get_children())

        # Sort keys for consistent display
        sorted_filtered_keys = sorted(list(self.filtered_keys))

        for key in sorted_filtered_keys:
            values: List[str] = [key]
            has_empty_translation = False
            for lang in self.languages:
                translation = self.files_data.get(lang, {}).get(key, "")
                values.append(translation)
                if not translation.strip():
                    has_empty_translation = True

            # Determine tags based on state
            tags_list = []
            if has_empty_translation:
                tags_list.append('untranslated')
            # --- Check if modified ---
            if key in self.unsaved_changes:
                tags_list.append('modified')
            # --- ---

            final_tags = tuple(tags_list)

            # Use key as item ID (iid) for easy reference
            try:
                self.tree.insert("", "end", iid=key, values=values, tags=final_tags)
            except tk.TclError as e:
                logging.warning(f"Could not insert key '{key}' as IID: {e}. Skipping row.")


        # Update status after population
        count = len(sorted_filtered_keys)
        total = len(self.all_keys)
        unsaved_count = len(self.unsaved_changes)
        status_parts = [f"Showing {count} of {total} keys"]
        if unsaved_count > 0:
            status_parts.append(f"{unsaved_count} unsaved change{'s' if unsaved_count != 1 else ''}")
        self.status_var.set(". ".join(status_parts))
        logging.debug(f"Treeview populated. Showing {count}/{total} keys. {unsaved_count} unsaved.")

    def _apply_filter_callback(self, *args: Any) -> None:
        """Callback wrapper for search variable trace."""
        self.apply_filter()

    def apply_filter(self) -> None:
        """Applies search and untranslated filters to the data."""
        self._cancel_cell_edit() # Ensure no lingering edit entry
        search_text = self.search_var.get().lower().strip()
        search_in_keys = self.search_keys_var.get()
        search_in_translations = self.search_translations_var.get()

        # Determine base set of keys (all or only untranslated)
        if self.filter_untranslated:
            base_keys = {
                k for k in self.all_keys
                if any(not self.files_data.get(lang, {}).get(k, "").strip() for lang in self.languages)
            }
        else:
            base_keys = self.all_keys.copy()

        # Apply search text filter
        if search_text and (search_in_keys or search_in_translations):
            search_results = set()
            for key in base_keys:
                # Search in key
                if search_in_keys and search_text in key.lower():
                    search_results.add(key)
                    continue # Found in key, no need to check translations

                # Search in translations
                if search_in_translations:
                    for lang in self.languages:
                        translation = self.files_data.get(lang, {}).get(key, "").lower()
                        if search_text in translation:
                            search_results.add(key)
                            break # Found in one translation, move to next key
            self.filtered_keys = search_results
        else:
            # No search text or no search areas selected, use the base set
            self.filtered_keys = base_keys

        logging.debug(f"Filter applied. Search: '{search_text}', Untranslated: {self.filter_untranslated}. Result count: {len(self.filtered_keys)}")
        self.populate_treeview() # Repopulate will apply tags correctly

    def toggle_untranslated(self) -> None:
        """Toggles the filter for untranslated entries."""
        self.filter_untranslated = self.untranslated_var.get()
        logging.debug(f"Untranslated filter toggled to: {self.filter_untranslated}")
        self.apply_filter()

    def clear_filters(self, update_status: bool = True) -> None:
        """Clears all search and filter settings."""
        self._cancel_cell_edit()
        self.search_var.set("")
        self.untranslated_var.set(False)
        self.filter_untranslated = False
        self.search_keys_var.set(True)
        self.search_translations_var.set(True)
        self.filtered_keys = self.all_keys.copy()
        logging.debug("Filters cleared.")
        self.populate_treeview() # Repopulate will apply tags correctly
        # Status is updated within populate_treeview


    # --- In-Place Editing Methods ---

    def _on_double_click(self, event: tk.Event) -> None:
        """Handles double-click events on the Treeview for editing."""
        # If an edit is already in progress, save it first
        if self._edit_entry:
            # Check if the click is on the active edit entry itself
            widget_at_click = self.root.winfo_containing(event.x_root, event.y_root)
            if widget_at_click == self._edit_entry:
                 return # Ignore double click inside the edit entry
            self._save_cell_edit() # Save previous edit if clicking elsewhere

        region = self.tree.identify("region", event.x, event.y)
        if region != "cell":
            return # Click was not on a cell

        column_id = self.tree.identify_column(event.x) # e.g., '#1', '#2'
        item_id = self.tree.identify_row(event.y) # This is the key (our iid)

        # Allow editing only for language columns (not the 'key' column, #0)
        try:
            col_index = int(column_id.replace('#', '')) -1 # 0-based index
        except ValueError:
             logging.warning(f"Could not parse column index from '{column_id}'")
             return


        if col_index < 1 or not item_id: # Column 0 is the key column, cannot edit
             logging.debug(f"Double click on non-editable area: col_index={col_index}, item_id={item_id}")
             return

        language = self.languages[col_index - 1] # Adjust index for self.languages list

        # Get cell dimensions
        try:
            bbox = self.tree.bbox(item_id, column=column_id)
            if not bbox: # Item might be scrolled out of view
                logging.debug(f"Could not get bbox for item '{item_id}', column '{column_id}'. Cell might be hidden.")
                # Attempt to scroll to the item and try again? Or just abort.
                # self.tree.see(item_id) # Scroll to make it visible
                # bbox = self.tree.bbox(item_id, column=column_id)
                # if not bbox: return
                return # Abort if not visible initially
            x, y, width, height = bbox
        except Exception as e:
            logging.error(f"Error getting bounding box for cell: {e}", exc_info=True)
            return

        # Get current value
        current_value = self.tree.set(item_id, column=column_id)

        # Create Entry widget
        self._edit_entry = ttk.Entry(self.tree, width=width) # Use tree as parent
        # Adjust placement slightly if needed (e.g., border width)
        self._edit_entry.place(x=x, y=y, width=width, height=height)
        self._edit_entry.insert(0, current_value)
        self._edit_entry.select_range(0, tk.END)
        self._edit_entry.focus_set()

        # Store editing context
        self._edit_item_id = item_id
        self._edit_column_id = column_id # Store the '#n' identifier

        # Apply Cyrillic/standard bindings
        self._apply_cyrillic_bindings(self._edit_entry)

        # Bind events to Entry
        self._edit_entry.bind("<Return>", self._save_cell_edit)
        self._edit_entry.bind("<KP_Enter>", self._save_cell_edit) # Numpad Enter
        self._edit_entry.bind("<Escape>", self._cancel_cell_edit)
        self._edit_entry.bind("<FocusOut>", self._save_cell_edit) # Save on clicking away
        # Prevent double-click inside the entry from triggering another _on_double_click
        self._edit_entry.bind("<Double-1>", lambda e: "break")


        logging.debug(f"Started editing cell: item='{item_id}', column='{column_id}', language='{language}'")


    def _save_cell_edit(self, event: Optional[tk.Event] = None) -> None:
        """Saves the content of the in-place edit Entry."""
        if not self._edit_entry or not self._edit_item_id or not self._edit_column_id:
            self._cleanup_edit_entry()
            return

        try:
            new_value = self._edit_entry.get()
            key = self._edit_item_id
            col_index = int(self._edit_column_id.replace('#', '')) - 1 # 0-based index
            language = self.languages[col_index - 1]

            # Update data source only if value changed
            if language in self.files_data and key in self.files_data[language]:
                old_value = self.files_data[language][key]
                if old_value != new_value:
                    self.files_data[language][key] = new_value
                    logging.info(f"Updated data for key='{key}', lang='{language}': '{old_value}' -> '{new_value}'")

                    # --- Mark as modified ---
                    self._mark_as_modified(key)

                    # Update Treeview display
                    self.tree.set(key, column=self._edit_column_id, value=new_value)

                    # Re-evaluate 'untranslated' tag for the row
                    has_empty_translation = False
                    for i, lang in enumerate(self.languages):
                         current_trans = self.files_data.get(lang, {}).get(key, "")
                         if not current_trans.strip():
                              has_empty_translation = True
                              break

                    current_tags = list(self.tree.item(key, 'tags'))
                    # Manage untranslated tag
                    if has_empty_translation and 'untranslated' not in current_tags:
                         current_tags.append('untranslated')
                    elif not has_empty_translation and 'untranslated' in current_tags:
                         current_tags.remove('untranslated')
                    # Manage modified tag (already added by _mark_as_modified)
                    # Ensure modified tag is present (should be)
                    if 'modified' not in current_tags:
                        current_tags.append('modified')

                    self.tree.item(key, tags=tuple(current_tags)) # Set final combination of tags

                    # --- Update status bar ---
                    self.populate_treeview() # Easiest way to update status counts

                else:
                    logging.debug("Cell edit saved, but value did not change.")
            else:
                logging.warning(f"Could not save edit. Data mismatch for key='{key}', lang='{language}'")

        except Exception as e:
            logging.error(f"Error saving cell edit: {e}", exc_info=True)
            messagebox.showerror("Error", f"Failed to save edit: {e}")
        finally:
            self._cleanup_edit_entry()


    def _cancel_cell_edit(self, event: Optional[tk.Event] = None) -> None:
        """Cancels the in-place edit and destroys the Entry."""
        if self._edit_entry:
            logging.debug(f"Cancelled editing cell: item='{self._edit_item_id}', column='{self._edit_column_id}'")
            self._cleanup_edit_entry()

    def _cleanup_edit_entry(self) -> None:
        """Destroys the edit entry and resets state variables."""
        if self._edit_entry:
            self._edit_entry.destroy()
        self._edit_entry = None
        self._edit_item_id = None
        self._edit_column_id = None

    # --- Add/Edit(Key)/Delete Methods ---

    def add_entry(self) -> None:
        """Opens a popup window to add a new localization key and its translations."""
        self._cancel_cell_edit() # Ensure no lingering edit entry
        if not self.languages:
            messagebox.showwarning("Warning", "Please load language files first.")
            logging.warning("Add Entry cancelled: No languages loaded.")
            return

        popup = tk.Toplevel(self.root)
        popup.title("Add New Entry")
        popup.transient(self.root) # Keep popup on top of main window
        popup.grab_set() # Make popup modal
        popup.resizable(False, True) # Not horizontally resizable

        entries: Dict[str, ttk.Entry] = {} # Store lang -> entry widget

        # Key Entry
        ttk.Label(popup, text="Key:", anchor="e").grid(row=0, column=0, padx=5, pady=5, sticky="ew")
        key_entry = ttk.Entry(popup, width=40)
        key_entry.grid(row=0, column=1, padx=5, pady=5, sticky="ew")
        self._apply_cyrillic_bindings(key_entry)

        # Translation Entries
        for i, lang in enumerate(self.languages, 1):
            ttk.Label(popup, text=f"{lang}:", anchor="e").grid(row=i, column=0, padx=5, pady=5, sticky="ew")
            entry = ttk.Entry(popup, width=40)
            entry.grid(row=i, column=1, padx=5, pady=5, sticky="ew")
            self._apply_cyrillic_bindings(entry)
            entries[lang] = entry

        popup.columnconfigure(1, weight=1) # Allow entries to expand if window resized vertically

        def save_new_entry() -> None:
            key = key_entry.get().strip()
            if not key:
                messagebox.showwarning("Input Required", "Key cannot be empty.", parent=popup)
                return
            if key in self.all_keys:
                messagebox.showwarning("Duplicate Key", f"The key '{key}' already exists.", parent=popup)
                return

            # Add to data structures
            for lang in self.languages:
                translation = entries[lang].get().strip()
                if lang not in self.files_data:
                    self.files_data[lang] = {}
                self.files_data[lang][key] = translation
            self.all_keys.add(key)
            self.filtered_keys.add(key) # Add to filtered set

            # --- Mark as modified ---
            self._mark_as_modified(key) # Mark before populate/filter

            logging.info(f"Added new key: '{key}'")
            popup.destroy()
            self.apply_filter() # Re-apply filter to show/hide the new item correctly and update status
            # Status update happens within populate_treeview called by apply_filter


        button_frame = ttk.Frame(popup)
        button_frame.grid(row=len(self.languages) + 1, column=0, columnspan=2, pady=10)
        ttk.Button(button_frame, text="Save", command=save_new_entry).pack(side=tk.LEFT, padx=5)
        ttk.Button(button_frame, text="Cancel", command=popup.destroy).pack(side=tk.LEFT, padx=5)

        key_entry.focus_set() # Focus on key entry initially
        self.root.wait_window(popup) # Wait for popup to close


    def edit_key_popup(self) -> None:
        """
        Opens a popup to edit the selected key name. Translations are edited in-place.
        """
        self._cancel_cell_edit() # Ensure no lingering edit entry
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Selection Required", "Please select an entry to edit its key.")
            logging.warning("Edit Key cancelled: No entry selected.")
            return
        if len(selected) > 1:
             messagebox.showwarning("Single Selection Only", "Please select only one entry to edit its key.")
             logging.warning("Edit Key cancelled: Multiple entries selected.")
             return

        item_id = selected[0] # This is the old key (our iid)
        old_key = item_id # Explicitly name it

        popup = tk.Toplevel(self.root)
        popup.title(f"Edit Key: {old_key}")
        popup.transient(self.root)
        popup.grab_set()
        popup.resizable(False, False)

        ttk.Label(popup, text="Current Key:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        ttk.Label(popup, text=old_key).grid(row=0, column=1, padx=5, pady=5, sticky="w")

        ttk.Label(popup, text="New Key:", anchor="e").grid(row=1, column=0, padx=5, pady=5, sticky="ew")
        key_entry = ttk.Entry(popup, width=40)
        key_entry.insert(0, old_key)
        key_entry.grid(row=1, column=1, padx=5, pady=5, sticky="ew")
        self._apply_cyrillic_bindings(key_entry)

        popup.columnconfigure(1, weight=1)

        def save_edited_key() -> None:
            new_key = key_entry.get().strip()
            if not new_key:
                messagebox.showwarning("Input Required", "New key cannot be empty.", parent=popup)
                return
            if new_key == old_key:
                popup.destroy() # No change needed
                return
            if new_key in self.all_keys:
                messagebox.showwarning("Duplicate Key", f"The key '{new_key}' already exists.", parent=popup)
                return

            # Update data source: Rename key in all language dictionaries
            for lang in self.languages:
                if lang in self.files_data and old_key in self.files_data[lang]:
                    self.files_data[lang][new_key] = self.files_data[lang].pop(old_key)

            # Update key sets
            self.all_keys.remove(old_key)
            self.all_keys.add(new_key)
            if old_key in self.filtered_keys:
                self.filtered_keys.remove(old_key)
                self.filtered_keys.add(new_key)

            # --- Update unsaved changes tracking ---
            was_modified = old_key in self.unsaved_changes
            self.unsaved_changes.discard(old_key)
            if was_modified: # Mark the new key as modified if the old one was
                 self.unsaved_changes.add(new_key)
            else: # If only the key changed, it's still a modification
                 self.unsaved_changes.add(new_key)


            # Update Treeview item (delete old, insert new)
            try:
                item_data = self.tree.item(old_key) # Get current values and tags
                values = item_data['values']
                tags = item_data['tags']
                values[0] = new_key # Update the key in the displayed values

                # Update tags based on the new key's modification status
                final_tags_list = list(tags)
                if 'modified' in final_tags_list: # Remove old modified status if present
                    final_tags_list.remove('modified')
                if new_key in self.unsaved_changes: # Add modified tag if new key is marked
                    final_tags_list.append('modified')

                self.tree.delete(old_key)
                self.tree.insert("", self.tree.index(old_key) if self.tree.exists(old_key) else "end", iid=new_key, values=values, tags=tuple(final_tags_list))
                self.tree.selection_set(new_key) # Reselect the renamed item
                self.tree.focus(new_key) # Set focus
            except tk.TclError as e:
                logging.error(f"Failed to update tree item for key rename {old_key} -> {new_key}: {e}", exc_info=True)
                messagebox.showerror("Tree Update Error", "Failed to update the view for the renamed key. Data is saved, but you might need to reload.", parent=popup)


            logging.info(f"Renamed key: '{old_key}' -> '{new_key}'")
            popup.destroy()
            # --- Update status bar ---
            self.populate_treeview() # Easiest way to update status counts

        button_frame = ttk.Frame(popup)
        button_frame.grid(row=2, column=0, columnspan=2, pady=10)
        ttk.Button(button_frame, text="Save Key", command=save_edited_key).pack(side=tk.LEFT, padx=5)
        ttk.Button(button_frame, text="Cancel", command=popup.destroy).pack(side=tk.LEFT, padx=5)

        key_entry.focus_set()
        key_entry.select_range(0, tk.END)
        self.root.wait_window(popup) # Wait for popup


    def delete_entry(self) -> None:
        """Deletes the selected entry (key and all its translations)."""
        self._cancel_cell_edit() # Ensure no lingering edit entry
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Selection Required", "Please select an entry to delete.")
            logging.warning("Delete cancelled: No entry selected.")
            return

        confirm_msg = f"Are you sure you want to delete {len(selected)} selected entr{'ies' if len(selected) > 1 else 'y'}?"
        if messagebox.askyesno("Confirm Deletion", confirm_msg):
            deleted_count = 0
            keys_to_delete = list(selected) # Copy selection

            for item_id in keys_to_delete:
                key = item_id # Our IID is the key
                if key not in self.all_keys:
                    logging.warning(f"Attempted to delete key '{key}' which is not in all_keys set. Skipping.")
                    continue

                # Remove from data source
                for lang in self.languages:
                    self.files_data.get(lang, {}).pop(key, None)

                # Remove from key sets
                self.all_keys.discard(key)
                self.filtered_keys.discard(key)
                # --- Remove from unsaved changes ---
                self.unsaved_changes.discard(key)

                # Remove from Treeview
                if self.tree.exists(item_id):
                    self.tree.delete(item_id)
                    deleted_count += 1
                else:
                     logging.warning(f"Attempted to delete key '{key}' from tree, but item ID not found.")


            if deleted_count > 0:
                logging.info(f"Deleted {deleted_count} entr{'ies' if deleted_count > 1 else 'y'}.")
                # --- Update status bar ---
                self.populate_treeview() # Easiest way to update counts
            else:
                 logging.warning("Delete confirmation received, but no entries were actually deleted.")
                 self.status_var.set("Deletion confirmed, but no matching entries found to delete.")


    def save_files(self) -> None:
        """Saves all current localization data back to their respective CSV files."""
        self._cancel_cell_edit() # Ensure any active edit is saved/cancelled before saving files
        if not self.languages:
            messagebox.showwarning("No Data", "No files loaded to save.")
            logging.warning("Save cancelled: No languages loaded.")
            return
        if not self.unsaved_changes:
            messagebox.showinfo("No Changes", "There are no unsaved changes to save.")
            logging.info("Save cancelled: No unsaved changes.")
            return


        directory = self.current_directory
        logging.info(f"Attempting to save {len(self.languages)} files with {len(self.unsaved_changes)} modified keys to {directory}...")
        saved_count = 0
        error_occurred = False

        # Use the sorted list of all keys to ensure consistent order in saved files
        keys_to_save = sorted(list(self.all_keys))

        for lang in self.languages:
            file_path = os.path.join(directory, lang)
            try:
                with open(file_path, 'w', encoding='utf-8', newline='') as f:
                    writer = csv.writer(f, quoting=csv.QUOTE_MINIMAL) # Use minimal quoting
                    writer.writerow(['key', 'translation']) # Write header
                    lang_data = self.files_data.get(lang, {})
                    for key in keys_to_save:
                        translation = lang_data.get(key, "") # Get translation, default to empty string if key missing for this lang
                        writer.writerow([key, translation])
                saved_count += 1
                logging.debug(f"Successfully saved {lang}")
            except Exception as e:
                error_occurred = True
                messagebox.showerror("Save Error", f"Failed to save {lang}:\n{str(e)}")
                logging.error(f"Failed to save file {lang}: {e}", exc_info=True)
                # Stop saving process on first error? Or continue? Currently continues.
                break # Let's stop on the first error to avoid potential cascading issues

        if error_occurred:
            self.status_var.set(f"Save failed for {lang}. {saved_count}/{len(self.languages)} files processed before error.")
            messagebox.showwarning("Save Failed", f"Failed saving {lang}. No further files were saved. Please check permissions or errors.")
        else:
             # --- Clear unsaved changes state ---
            keys_that_were_unsaved = self.unsaved_changes.copy()
            self.unsaved_changes.clear()

            # --- Update Treeview to remove 'modified' tags ---
            # Option 1: Iterate and remove tag (might be faster for large trees than full rebuild)
            # for key in keys_that_were_unsaved:
            #      if self.tree.exists(key):
            #           current_tags = list(self.tree.item(key, 'tags'))
            #           if 'modified' in current_tags:
            #                current_tags.remove('modified')
            #                self.tree.item(key, tags=tuple(current_tags))
            # Option 2: Full repopulate (simpler code)
            self.populate_treeview() # This will redraw without 'modified' and update status bar

            logging.info(f"All {saved_count} files saved successfully.")
            # Status update happens in populate_treeview
            messagebox.showinfo("Success", f"All {saved_count} files saved successfully to {directory}")


if __name__ == "__main__":
    root = tk.Tk()
    app = LocalizationEditor(root)
    root.mainloop()