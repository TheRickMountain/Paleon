import tkinter as tk
from tkinter import ttk, messagebox, filedialog
import csv
import os

class LocalizationEditor:
    def __init__(self, root):
        self.root = root
        self.root.title("Localization File Editor")
        self.root.geometry("1000x600")

        # Store loaded data
        self.files_data = {}  # {filename: {key: translation}}
        self.languages = []   # List of language filenames
        self.all_keys = set() # Set of all unique keys across files
        self.filtered_keys = set() # For search/filter functionality
        self.filter_untranslated = False # Toggle for untranslated entries
        
        # Get current directory (where the program is located)
        self.current_directory = os.path.dirname(os.path.abspath(__file__))

        # Create main frame
        self.main_frame = ttk.Frame(self.root, padding="10")
        self.main_frame.grid(row=0, column=0, sticky="nsew")

        # Top controls frame
        top_frame = ttk.Frame(self.main_frame)
        top_frame.grid(row=0, column=0, sticky="ew")
        top_frame.columnconfigure(2, weight=1)

        # Load button
        ttk.Button(top_frame, text="Load Files", command=self.load_files).grid(row=0, column=0, pady=5, padx=5)

        # Show current directory
        self.dir_label = ttk.Label(top_frame, text=f"Working directory: {self.current_directory}")
        self.dir_label.grid(row=0, column=1, pady=5, padx=5, sticky="w")

        # Search frame
        search_frame = ttk.LabelFrame(self.main_frame, text="Search & Filter")
        search_frame.grid(row=1, column=0, sticky="ew", pady=5)
        search_frame.columnconfigure(1, weight=1)

        # Search
        ttk.Label(search_frame, text="Search:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        self.search_var = tk.StringVar()
        self.search_entry = ttk.Entry(search_frame, textvariable=self.search_var)
        self.search_entry.grid(row=0, column=1, padx=5, pady=5, sticky="ew")
        self.search_var.trace_add("write", self.apply_filter)
        
        # Search options
        self.search_keys = tk.BooleanVar(value=True)
        self.search_translations = tk.BooleanVar(value=True)
        ttk.Checkbutton(search_frame, text="Search in keys", variable=self.search_keys, 
                        command=self.apply_filter).grid(row=0, column=2, padx=5)
        ttk.Checkbutton(search_frame, text="Search in translations", variable=self.search_translations, 
                        command=self.apply_filter).grid(row=0, column=3, padx=5)
        
        # Filter untranslated
        self.untranslated_var = tk.BooleanVar(value=False)
        ttk.Checkbutton(search_frame, text="Show only untranslated entries", 
                        variable=self.untranslated_var, command=self.toggle_untranslated
                        ).grid(row=0, column=4, padx=5)

        # Treeview for displaying data
        self.tree = ttk.Treeview(self.main_frame, show="headings")
        self.tree.grid(row=2, column=0, sticky="nsew", pady=10)
        self.tree.tag_configure('untranslated', background='#ffe6e6')

        # Scrollbars
        v_scrollbar = ttk.Scrollbar(self.main_frame, orient="vertical", command=self.tree.yview)
        v_scrollbar.grid(row=2, column=1, sticky="ns")
        self.tree.configure(yscrollcommand=v_scrollbar.set)

        h_scrollbar = ttk.Scrollbar(self.main_frame, orient="horizontal", command=self.tree.xview)
        h_scrollbar.grid(row=3, column=0, sticky="ew")
        self.tree.configure(xscrollcommand=h_scrollbar.set)

        # Status bar
        self.status_var = tk.StringVar(value="Ready")
        status_bar = ttk.Label(self.main_frame, textvariable=self.status_var, relief=tk.SUNKEN, anchor=tk.W)
        status_bar.grid(row=4, column=0, columnspan=2, sticky="ew", pady=5)

        # Buttons frame
        btn_frame = ttk.Frame(self.main_frame)
        btn_frame.grid(row=5, column=0, pady=5)

        ttk.Button(btn_frame, text="Add Entry", command=self.add_entry).grid(row=0, column=0, padx=5)
        ttk.Button(btn_frame, text="Edit Entry", command=self.edit_entry).grid(row=0, column=1, padx=5)
        ttk.Button(btn_frame, text="Delete Entry", command=self.delete_entry).grid(row=0, column=2, padx=5)
        ttk.Button(btn_frame, text="Save All Files", command=self.save_files).grid(row=0, column=3, padx=5)
        ttk.Button(btn_frame, text="Clear Filters", command=self.clear_filters).grid(row=0, column=4, padx=5)

        # Configure grid weights
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        self.main_frame.columnconfigure(0, weight=1)
        self.main_frame.rowconfigure(2, weight=1)
        
        # Load files automatically at startup
        self.load_files()

    def load_files(self):
        # Clear existing data
        self.files_data.clear()
        self.languages.clear()
        self.all_keys.clear()
        self.filtered_keys.clear()
        self.filter_untranslated = False
        self.untranslated_var.set(False)
        self.search_var.set("")
        self.tree.delete(*self.tree.get_children())
        self.tree["columns"] = ()

        # Use current directory instead of dialog
        directory = self.current_directory

        # Load all CSV files in the directory
        csv_files = [f for f in os.listdir(directory) if f.endswith('.csv')]
        
        if not csv_files:
            messagebox.showinfo("Information", f"No CSV files found in {directory}")
            return
            
        for file in csv_files:
            file_path = os.path.join(directory, file)
            data = {}
            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    reader = csv.DictReader(f)
                    for row in reader:
                        data[row['key']] = row['translation']
                        self.all_keys.add(row['key'])
                self.files_data[file] = data
                self.languages.append(file)
            except Exception as e:
                messagebox.showerror("Error", f"Failed to load {file}: {str(e)}")

        if not self.languages:
            return

        # Copy all keys to filtered keys (initially show all)
        self.filtered_keys = self.all_keys.copy()

        # Configure Treeview columns
        columns = ["key"] + self.languages
        self.tree["columns"] = columns
        self.tree.heading("key", text="Key")
        for lang in self.languages:
            self.tree.heading(lang, text=lang)
            self.tree.column(lang, width=200, stretch=True)
        self.tree.column("key", width=200, stretch=True)

        # Populate Treeview
        self.populate_treeview()
        
        # Update status
        self.status_var.set(f"Loaded {len(self.languages)} language files with {len(self.all_keys)} keys")

    def populate_treeview(self):
        # Clear treeview
        self.tree.delete(*self.tree.get_children())
        
        # Add rows for filtered keys
        for key in sorted(self.filtered_keys):
            values = [key]
            has_empty_translation = False
            
            for lang in self.languages:
                translation = self.files_data[lang].get(key, "")
                values.append(translation)
                if not translation.strip():
                    has_empty_translation = True
            
            # Insert with appropriate tag
            if has_empty_translation:
                self.tree.insert("", "end", values=values, tags=('untranslated',))
            else:
                self.tree.insert("", "end", values=values)
        
        # Update status
        self.status_var.set(f"Showing {len(self.filtered_keys)} of {len(self.all_keys)} keys")

    def apply_filter(self, *args):
        search_text = self.search_var.get().lower()
        search_in_keys = self.search_keys.get()
        search_in_translations = self.search_translations.get()
        
        # Reset filtered keys to all keys or untranslated keys
        if self.filter_untranslated:
            self.filtered_keys = {k for k in self.all_keys if any(not self.files_data[lang].get(k, "").strip() for lang in self.languages)}
        else:
            self.filtered_keys = self.all_keys.copy()
        
        # If search text exists, apply search filter
        if search_text:
            search_results = set()
            
            for key in self.filtered_keys:
                # Search in keys
                if search_in_keys and search_text in key.lower():
                    search_results.add(key)
                    continue
                
                # Search in translations
                if search_in_translations:
                    for lang in self.languages:
                        translation = self.files_data[lang].get(key, "").lower()
                        if search_text in translation:
                            search_results.add(key)
                            break
            
            self.filtered_keys = search_results
        
        # Update the treeview with filtered data
        self.populate_treeview()

    def toggle_untranslated(self):
        self.filter_untranslated = self.untranslated_var.get()
        self.apply_filter()

    def clear_filters(self):
        self.search_var.set("")
        self.untranslated_var.set(False)
        self.filter_untranslated = False
        self.filtered_keys = self.all_keys.copy()
        self.populate_treeview()
        self.status_var.set("Filters cleared")

    def add_entry(self):
        if not self.languages:
            messagebox.showwarning("Warning", "Please load files first")
            return

        # Create popup window
        popup = tk.Toplevel(self.root)
        popup.title("Add New Entry")
        popup.geometry(f"300x{100 + len(self.languages) * 50}")

        ttk.Label(popup, text="Key:").grid(row=0, column=0, padx=5, pady=5)
        key_entry = ttk.Entry(popup)
        key_entry.grid(row=0, column=1, padx=5, pady=5)

        trans_entries = {}
        for i, lang in enumerate(self.languages, 1):
            ttk.Label(popup, text=f"{lang}:").grid(row=i, column=0, padx=5, pady=5)
            entry = ttk.Entry(popup)
            entry.grid(row=i, column=1, padx=5, pady=5)
            trans_entries[lang] = entry

        def save_new_entry():
            key = key_entry.get().strip()
            if not key:
                messagebox.showwarning("Warning", "Key is required")
                return
                
            if key in self.all_keys:
                messagebox.showwarning("Warning", "This key already exists")
                return

            values = [key]
            has_empty_translation = False
            
            for lang in self.languages:
                translation = trans_entries[lang].get().strip()
                self.files_data[lang][key] = translation
                values.append(translation)
                if not translation:
                    has_empty_translation = True
            
            self.all_keys.add(key)
            self.filtered_keys.add(key)
            
            # Apply appropriate tag
            if has_empty_translation:
                self.tree.insert("", "end", values=values, tags=('untranslated',))
            else:
                self.tree.insert("", "end", values=values)
                
            popup.destroy()
            
            # Re-apply current filter
            self.apply_filter()

        ttk.Button(popup, text="Save", command=save_new_entry).grid(row=len(self.languages) + 1, column=0, columnspan=2, pady=10)

    def edit_entry(self):
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Warning", "Please select an entry to edit")
            return

        item = self.tree.item(selected[0])
        old_key = item['values'][0]
        old_values = item['values'][1:]

        # Create popup window
        popup = tk.Toplevel(self.root)
        popup.title("Edit Entry")
        popup.geometry(f"300x{100 + len(self.languages) * 50}")

        ttk.Label(popup, text="Key:").grid(row=0, column=0, padx=5, pady=5)
        key_entry = ttk.Entry(popup)
        key_entry.insert(0, old_key)
        key_entry.grid(row=0, column=1, padx=5, pady=5)

        trans_entries = {}
        for i, (lang, old_trans) in enumerate(zip(self.languages, old_values), 1):
            ttk.Label(popup, text=f"{lang}:").grid(row=i, column=0, padx=5, pady=5)
            entry = ttk.Entry(popup)
            entry.insert(0, old_trans)
            entry.grid(row=i, column=1, padx=5, pady=5)
            trans_entries[lang] = entry

        def save_edited_entry():
            new_key = key_entry.get().strip()
            if not new_key:
                messagebox.showwarning("Warning", "Key is required")
                return
                
            if new_key != old_key and new_key in self.all_keys:
                messagebox.showwarning("Warning", "This key already exists")
                return

            # Update data
            if new_key != old_key:
                for lang in self.languages:
                    translation = self.files_data[lang].pop(old_key)
                    self.files_data[lang][new_key] = translation
                self.all_keys.remove(old_key)
                self.filtered_keys.discard(old_key)
                self.all_keys.add(new_key)
                self.filtered_keys.add(new_key)
            
            values = [new_key]
            has_empty_translation = False
            
            for lang in self.languages:
                new_translation = trans_entries[lang].get().strip()
                self.files_data[lang][new_key] = new_translation
                values.append(new_translation)
                if not new_translation:
                    has_empty_translation = True
            
            # Determine tags based on translations
            if has_empty_translation:
                self.tree.item(selected[0], values=values, tags=('untranslated',))
            else:
                self.tree.item(selected[0], values=values, tags=())
                
            popup.destroy()
            
            # Re-apply current filter
            self.apply_filter()

        ttk.Button(popup, text="Save", command=save_edited_entry).grid(row=len(self.languages) + 1, column=0, columnspan=2, pady=10)

    def delete_entry(self):
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Warning", "Please select an entry to delete")
            return

        if messagebox.askyesno("Confirm", "Are you sure you want to delete this entry?"):
            item = self.tree.item(selected[0])
            key = item['values'][0]
            for lang in self.languages:
                self.files_data[lang].pop(key, None)
            self.all_keys.remove(key)
            self.filtered_keys.discard(key)
            self.tree.delete(selected[0])
            
            # Update status
            self.status_var.set(f"Deleted key '{key}'")

    def save_files(self):
        if not self.languages:
            messagebox.showwarning("Warning", "No files loaded to save")
            return

        # Use current directory for saving
        directory = self.current_directory

        try:
            for lang in self.languages:
                file_path = os.path.join(directory, lang)
                with open(file_path, 'w', encoding='utf-8', newline='') as f:
                    writer = csv.writer(f)
                    writer.writerow(['key', 'translation'])
                    for key in sorted(self.all_keys):
                        translation = self.files_data[lang].get(key, "")
                        writer.writerow([key, translation])
            
            self.status_var.set(f"All files saved successfully to {directory}")
            messagebox.showinfo("Success", f"All files saved successfully to {directory}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to save files: {str(e)}")

if __name__ == "__main__":
    root = tk.Tk()
    app = LocalizationEditor(root)
    root.mainloop()