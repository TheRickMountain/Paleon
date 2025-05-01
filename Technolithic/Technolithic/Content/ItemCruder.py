import json
import tkinter as tk
from tkinter import ttk, messagebox
from typing import Dict, Any, List, Tuple
from copy import deepcopy
import os
from PIL import Image, ImageTk
import logging
from tkinter import simpledialog

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s', filename='item_manager.log')

# Constants for configuration
class Config:
    WINDOW_WIDTH = 1000
    WINDOW_HEIGHT = 600
    COLUMN_WIDTHS = {"Id": 80, "Name": 150, "Category": 80, "Value": 80, "Durability": 80}
    FILES = {"ITEMS": "items.json", "CATEGORIES": "itemCategories.json", "TILESET": "Sprites//tileset.png"}
    TILE_SIZE = 16
    MAX_ICON_ID = 256
    ICON_SELECTOR_WIDTH = 600
    ICON_SELECTOR_HEIGHT = 600

class ItemManager:
    """Handles JSON file operations and data management"""
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(ItemManager, cls).__new__(cls)
            cls._instance.__init__()
        return cls._instance

    def __init__(self):
        if hasattr(self, '_initialized'):
            return
        self.filename = Config.FILES["ITEMS"]
        self.categories_filename = Config.FILES["CATEGORIES"]
        self.tileset_filename = Config.FILES["TILESET"]
        self.items: Dict[str, Any] = {}
        self.categories: Dict[str, Dict] = {}
        self.tileset_image = None
        self._initialized = True
        self.load_items()
        self.load_categories()
        self.load_tileset()

    def load_items(self) -> None:
        """Load items from JSON file with error handling and logging"""
        try:
            if os.path.exists(self.filename):
                with open(self.filename, 'r') as f:
                    self.items = json.load(f)
            else:
                self.items = {}
                logging.warning(f"Items file not found, starting with empty data: {self.filename}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to load items: {str(e)}")
            logging.error(f"Failed to load items: {str(e)}")
            self.items = {}

    def load_categories(self) -> None:
        """Load categories from JSON file"""
        try:
            if os.path.exists(self.categories_filename):
                with open(self.categories_filename, 'r') as f:
                    self.categories = json.load(f)
            else:
                self.categories = {}
                logging.warning(f"Categories file not found, starting with empty data: {self.categories_filename}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to load categories: {str(e)}")
            logging.error(f"Failed to load categories: {str(e)}")
            self.categories = {}

    def load_tileset(self) -> None:
        """Load tileset image lazily"""
        try:
            if not hasattr(self, 'tileset_image') or self.tileset_image is None:
                if os.path.exists(self.tileset_filename):
                    self.tileset_image = Image.open(self.tileset_filename)
                else:
                    logging.warning(f"Tileset file not found: {self.tileset_filename}")
                    self.tileset_image = None
        except Exception as e:
            messagebox.showerror("Error", f"Failed to load tileset: {str(e)}")
            logging.error(f"Failed to load tileset: {str(e)}")
            self.tileset_image = None

    def get_icon_photo(self, id: int) -> ImageTk.PhotoImage:
        """Get PhotoImage for a specific icon ID from tileset"""
        if not self.tileset_image or id < 0 or id >= Config.MAX_ICON_ID:
            return None
        tile_size = Config.TILE_SIZE
        x = (id % 16) * tile_size
        y = (id // 16) * tile_size
        icon = self.tileset_image.crop((x, y, x + tile_size, y + tile_size))
        photo = ImageTk.PhotoImage(icon)
        return photo

    def save_items(self) -> None:
        """Save items to JSON file atomically"""
        try:
            temp_filename = self.filename + '.tmp'
            with open(temp_filename, 'w') as f:
                json.dump(self.items, f, indent=2)
            os.replace(temp_filename, self.filename)
            logging.info(f"Items saved successfully to {self.filename}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to save items: {str(e)}")
            logging.error(f"Failed to save items: {str(e)}")

    def add_item(self, key: str, item_data: Dict) -> bool:
        """Add a new item with validation"""
        if not key or key in self.items:
            return False
        self.items[key] = self._validate_item_data(item_data)
        self.save_items()
        logging.info(f"Added item: {key}")
        return True

    def update_item(self, key: str, item_data: Dict) -> bool:
        """Update an existing item with validation"""
        if key not in self.items:
            return False
        self.items[key] = self._validate_item_data(item_data)
        self.save_items()
        logging.info(f"Updated item: {key}")
        return True

    def delete_item(self, key: str) -> bool:
        """Delete an item"""
        if key not in self.items:
            return False
        del self.items[key]
        self.save_items()
        logging.info(f"Deleted item: {key}")
        return True

    def _validate_item_data(self, item_data: Dict) -> Dict:
        """Validate and normalize item data"""
        valid_labor_types = ["Haul", "Supply", "Plant", "Harvest", "Chop", "Mine", "Hunt", "Fish"]
        result = {
            "Id": int(item_data.get("Id", 0)),
            "key": item_data.get("key", ""),
            "Durability": int(item_data.get("Durability", 0)),
            "ItemCategory": int(item_data.get("ItemCategory", 0)),
            "Value": int(item_data.get("Value", 0)),
        }
        if "Equipment" in item_data:
            result["Equipment"] = {
                "TextureData": None,
                "PositionX": int(item_data["Equipment"].get("PositionX", 0)),
                "PositionY": int(item_data["Equipment"].get("PositionY", 0)),
                "Rotation": int(item_data["Equipment"].get("Rotation", 0))
            }
        if "Tool" in item_data:
            result["Tool"] = {
                "MeleeDamage": int(item_data["Tool"].get("MeleeDamage", 0)),
                "RangeDamage": 0,
                "Level": 1,
                "Efficiency": float(item_data["Tool"].get("Efficiency", 1.0)),
                "ToolType": int(item_data["Tool"].get("ToolType", 0)),
                "CreatureType": 0,
                "RechargeTime": 0,
                "AmmoTextureId": -1,
                "ProjectileSpeed": 0,
                "LaborTypes": [lt for lt in item_data["Tool"].get("LaborTypes", []) if lt in valid_labor_types]
            }
        return result

class IconSelector(tk.Toplevel):
    """Enhanced window for selecting an icon from tileset"""
    def __init__(self, parent, item_manager: ItemManager, callback):
        super().__init__(parent)
        self.item_manager = item_manager
        self.callback = callback
        self.title("Select Icon")
        self.geometry(f"{Config.ICON_SELECTOR_WIDTH}x{Config.ICON_SELECTOR_HEIGHT}")
        self.resizable(True, True)  # Allow resizing
        self.selected_id = -1

        # Main frame with scrollbar
        main_frame = ttk.Frame(self)
        main_frame.pack(fill="both", expand=True)

        # Canvas and scrollbar for scrolling
        self.canvas = tk.Canvas(main_frame, width=Config.ICON_SELECTOR_WIDTH - 20, height=Config.ICON_SELECTOR_HEIGHT - 20)
        h_scrollbar = ttk.Scrollbar(main_frame, orient="horizontal", command=self.canvas.xview)
        v_scrollbar = ttk.Scrollbar(main_frame, orient="vertical", command=self.canvas.yview)
        self.inner_frame = ttk.Frame(self.canvas)

        # Configure scrollbars and canvas
        self.canvas.configure(xscrollcommand=h_scrollbar.set, yscrollcommand=v_scrollbar.set)
        h_scrollbar.pack(side="bottom", fill="x")
        v_scrollbar.pack(side="right", fill="y")
        self.canvas.pack(side="left", fill="both", expand=True)

        # Create inner frame in canvas for icons
        self.canvas.create_window((0, 0), window=self.inner_frame, anchor="nw")
        self.inner_frame.bind("<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all")))

        # Preview frame
        preview_frame = ttk.LabelFrame(self, text="Preview", padding="5")
        preview_frame.pack(fill="x", pady=5)
        self.preview_label = ttk.Label(preview_frame, image=None)
        self.preview_label.pack(padx=5, pady=5)

        # Progress bar
        self.progress = ttk.Progressbar(self, mode="indeterminate")
        self.progress.pack(fill="x", pady=5)

        self.load_tileset()
        # Bind <Button-1> only to the canvas for icon selection, not the entire window
        self.canvas.bind("<Button-1>", self.select_icon_canvas)
        self.bind("<Up>", self.move_up)
        self.bind("<Down>", self.move_down)
        self.bind("<Left>", self.move_left)
        self.bind("<Right>", self.move_right)
        self.bind("<Return>", self.select_current)

    def load_tileset(self):
        """Load and display tileset as a grid of icons with progress indicator and debugging"""
        self.progress.start()
        if not self.item_manager.tileset_image:
            self.progress.stop()
            self.progress.destroy()
            logging.error("Tileset image not loaded")
            messagebox.showerror("Error", "Tileset image could not be loaded. Check tileset.png exists and is valid.")
            return

        tile_size = Config.TILE_SIZE
        self.icon_photos = []  # Store PhotoImage references to prevent garbage collection
        self.icon_buttons = []  # Store button references for navigation

        logging.info("Loading tileset into grid...")
        for y in range(16):
            for x in range(16):
                icon_id = y * 16 + x
                icon = self.item_manager.tileset_image.crop((x * tile_size, y * tile_size, (x + 1) * tile_size, (y + 1) * tile_size))
                photo = ImageTk.PhotoImage(icon)
                self.icon_photos.append(photo)  # Keep reference
                btn = ttk.Button(self.inner_frame, image=photo, command=lambda id=icon_id: self.select_icon_id(id))
                btn.grid(row=y, column=x, padx=2, pady=2, sticky="nsew")  # Add padding and stretch for visibility
                btn.bind("<Enter>", lambda e, id=icon_id: self.preview_icon(id))
                btn.bind("<Leave>", lambda e: self.preview_icon(None))
                ToolTip(btn, f"Icon ID: {icon_id}")
                self.icon_buttons.append(btn)

        # Configure grid rows and columns to expand
        for i in range(16):
            self.inner_frame.grid_rowconfigure(i, weight=1)
            self.inner_frame.grid_columnconfigure(i, weight=1)

        self.progress.stop()
        self.progress.destroy()
        self.current_id = 0  # Start with the first icon selected
        self.highlight_selection()
        logging.info("Tileset loaded successfully with 256 icons")

    def select_icon_canvas(self, event):
        """Handle icon selection from canvas click, only for the icon grid"""
        # Adjust for canvas scrolling
        x = self.canvas.canvasx(event.x) // Config.TILE_SIZE
        y = self.canvas.canvasy(event.y) // Config.TILE_SIZE
        icon_id = y * 16 + x
        if 0 <= icon_id < Config.MAX_ICON_ID and 0 <= x < 16 and 0 <= y < 16:
            self.select_icon_id(icon_id)

    def select_icon_id(self, icon_id: int):
        """Select an icon by ID and call the callback"""
        self.selected_id = icon_id
        self.highlight_selection()
        self.preview_icon(icon_id)
        if self.callback:
            self.callback(icon_id)
        self.destroy()

    def highlight_selection(self):
        """Highlight the currently selected icon"""
        for btn in self.icon_buttons:
            btn.config(style='TButton')
        if 0 <= self.selected_id < Config.MAX_ICON_ID:
            row, col = divmod(self.selected_id, 16)
            button = self.inner_frame.grid_slaves(row=row, column=col)[0]
            button.config(style='Selected.TButton')
        self.style = ttk.Style()
        self.style.configure('Selected.TButton', background='lightblue', relief='raised')

    def preview_icon(self, icon_id: int):
        """Show a larger preview of the selected icon"""
        if icon_id is None:
            self.preview_label.config(image=None)
            return
        if 0 <= icon_id < Config.MAX_ICON_ID:
            photo = self.item_manager.get_icon_photo(icon_id)
            if photo:
                # Create a larger preview (e.g., 64x64)
                larger_icon = self.item_manager.tileset_image.crop((
                    (icon_id % 16) * Config.TILE_SIZE,
                    (icon_id // 16) * Config.TILE_SIZE,
                    (icon_id % 16 + 1) * Config.TILE_SIZE,
                    (icon_id // 16 + 1) * Config.TILE_SIZE
                )).resize((64, 64), Image.NEAREST)
                preview_photo = ImageTk.PhotoImage(larger_icon)
                self.preview_label.config(image=preview_photo)
                self.preview_label.image = preview_photo  # Keep reference

    def move_up(self, event):
        """Move selection up using arrow keys"""
        if self.selected_id > 15:
            self.select_icon_id(self.selected_id - 16)
            self.canvas.yview_scroll(-1, "units")

    def move_down(self, event):
        """Move selection down using arrow keys"""
        if self.selected_id < Config.MAX_ICON_ID - 16:
            self.select_icon_id(self.selected_id + 16)
            self.canvas.yview_scroll(1, "units")

    def move_left(self, event):
        """Move selection left using arrow keys"""
        if self.selected_id % 16 > 0:
            self.select_icon_id(self.selected_id - 1)
            self.canvas.xview_scroll(-1, "units")

    def move_right(self, event):
        """Move selection right using arrow keys"""
        if self.selected_id % 16 < 15:
            self.select_icon_id(self.selected_id + 1)
            self.canvas.xview_scroll(1, "units")

    def select_current(self, event):
        """Select the current icon using Enter key"""
        if 0 <= self.selected_id < Config.MAX_ICON_ID:
            self.select_icon_id(self.selected_id)

class ItemEditorWindow(tk.Toplevel):
    """Window for adding/editing items"""
    def __init__(self, parent, item_manager: ItemManager, item_key: str = None):
        super().__init__(parent)
        self.item_manager = item_manager
        self.item_key = item_key
        self.title("Edit Item" if item_key else "Add Item")
        self.geometry("600x700")
        self.resizable(False, False)
        self.style = ttk.Style()
        self.style.theme_use("clam")  # Use a modern theme
        
        self.create_widgets()
        if item_key:
            self.load_item_data()
        self.protocol("WM_DELETE_WINDOW", self.on_closing)

    def create_widgets(self):
        # Main frame with padding
        main_frame = ttk.Frame(self, padding="10")
        main_frame.pack(fill="both", expand=True)

        # Basic properties
        label = ttk.Label(main_frame, text="Key:")
        label.grid(row=0, column=0, sticky="w", pady=2)
        ToolTip(label, "Unique identifier for the item")
        self.key_entry = ttk.Entry(main_frame)
        self.key_entry.grid(row=0, column=1, sticky="ew", pady=2)
        if self.item_key:
            self.key_entry.insert(0, self.item_key)
            self.key_entry.config(state="disabled")

        label = ttk.Label(main_frame, text="Id (Icon):")
        label.grid(row=1, column=0, sticky="w", pady=2)
        ToolTip(label, "Icon ID from tileset.png (0-255)")
        self.id_frame = ttk.Frame(main_frame)
        self.id_frame.grid(row=1, column=1, sticky="ew", pady=2)
        self.id_entry = ttk.Entry(self.id_frame, width=5)
        self.id_entry.pack(side="left", padx=5)
        button = ttk.Button(self.id_frame, text="Select Icon", command=self.select_icon)
        button.pack(side="left", padx=5)
        ToolTip(button, "Select an icon from the tileset")
        self.icon_label = ttk.Label(self.id_frame, image=None)
        self.icon_label.pack(side="left", padx=5)

        label = ttk.Label(main_frame, text="Durability:")
        label.grid(row=2, column=0, sticky="w", pady=2)
        ToolTip(label, "Durability value (0 or positive integer)")
        self.durability_entry = ttk.Entry(main_frame)
        self.durability_entry.grid(row=2, column=1, sticky="ew", pady=2)

        label = ttk.Label(main_frame, text="ItemCategory:")
        label.grid(row=3, column=0, sticky="w", pady=2)
        ToolTip(label, "Category of the item")
        category_names = [self.item_manager.categories[cat]["key"] for cat in self.item_manager.categories]
        self.category_combo = ttk.Combobox(main_frame, values=category_names, state="readonly")
        self.category_combo.grid(row=3, column=1, sticky="ew", pady=2)

        label = ttk.Label(main_frame, text="Value:")
        label.grid(row=4, column=0, sticky="w", pady=2)
        ToolTip(label, "Value of the item (positive integer)")
        self.value_entry = ttk.Entry(main_frame)
        self.value_entry.grid(row=4, column=1, sticky="ew", pady=2)

        # Equipment frame
        equip_frame = ttk.LabelFrame(main_frame, text="Equipment", padding="5")
        equip_frame.grid(row=5, column=0, columnspan=2, sticky="ew", pady=5)

        label = ttk.Label(equip_frame, text="PositionX:")
        label.grid(row=0, column=0, sticky="w", pady=2)
        ToolTip(label, "X position in equipment")
        self.pos_x_entry = ttk.Entry(equip_frame)
        self.pos_x_entry.grid(row=0, column=1, sticky="ew", pady=2)

        label = ttk.Label(equip_frame, text="PositionY:")
        label.grid(row=1, column=0, sticky="w", pady=2)
        ToolTip(label, "Y position in equipment")
        self.pos_y_entry = ttk.Entry(equip_frame)
        self.pos_y_entry.grid(row=1, column=1, sticky="ew", pady=2)

        label = ttk.Label(equip_frame, text="Rotation:")
        label.grid(row=2, column=0, sticky="w", pady=2)
        ToolTip(label, "Rotation angle in equipment")
        self.rotation_entry = ttk.Entry(equip_frame)
        self.rotation_entry.grid(row=2, column=1, sticky="ew", pady=2)

        # Tool frame
        tool_frame = ttk.LabelFrame(main_frame, text="Tool", padding="5")
        tool_frame.grid(row=6, column=0, columnspan=2, sticky="ew", pady=5)

        label = ttk.Label(tool_frame, text="MeleeDamage:")
        label.grid(row=0, column=0, sticky="w", pady=2)
        ToolTip(label, "Melee damage value (positive integer)")
        self.melee_entry = ttk.Entry(tool_frame)
        self.melee_entry.grid(row=0, column=1, sticky="ew", pady=2)

        label = ttk.Label(tool_frame, text="Efficiency:")
        label.grid(row=1, column=0, sticky="w", pady=2)
        ToolTip(label, "Efficiency value (positive float)")
        self.efficiency_entry = ttk.Entry(tool_frame)
        self.efficiency_entry.grid(row=1, column=1, sticky="ew", pady=2)

        label = ttk.Label(tool_frame, text="ToolType:")
        label.grid(row=2, column=0, sticky="w", pady=2)
        ToolTip(label, "Type of tool (positive integer)")
        self.tool_type_entry = ttk.Entry(tool_frame)
        self.tool_type_entry.grid(row=2, column=1, sticky="ew", pady=2)

        label = ttk.Label(tool_frame, text="LaborTypes:")
        label.grid(row=3, column=0, sticky="w", pady=2)
        ToolTip(label, "Comma-separated list of labor types (e.g., Haul, Plant)")
        self.labor_entry = ttk.Entry(tool_frame)
        self.labor_entry.grid(row=3, column=1, sticky="ew", pady=2)

        # Buttons
        btn_frame = ttk.Frame(main_frame)
        btn_frame.grid(row=7, column=0, columnspan=2, pady=10)
        button = ttk.Button(btn_frame, text="Save", command=self.save)
        button.pack(side="left", padx=5)
        ToolTip(button, "Save the current item")
        button = ttk.Button(btn_frame, text="Cancel", command=self.destroy)
        button.pack(side="left", padx=5)
        ToolTip(button, "Cancel and close the window")

    def select_icon(self):
        """Open icon selector window with progress indicator"""
        progress = ttk.Progressbar(self, mode="indeterminate")
        progress.pack(fill="x", pady=5)
        progress.start()
        icon_selector = IconSelector(self, self.item_manager, self.set_icon_id)
        self.wait_window(icon_selector)
        progress.stop()
        progress.destroy()

    def set_icon_id(self, icon_id: int):
        """Set the ID field with the selected icon ID and update icon display"""
        self.id_entry.delete(0, tk.END)
        self.id_entry.insert(0, str(icon_id))
        icon_photo = self.item_manager.get_icon_photo(icon_id)
        if icon_photo:
            self.icon_label.config(image=icon_photo)
            self.icon_label.image = icon_photo  # Keep reference to prevent garbage collection

    def load_item_data(self):
        """Load existing item data into fields"""
        item = self.item_manager.items[self.item_key]
        self.id_entry.insert(0, item.get("Id", ""))
        icon_id = item.get("Id", 0)
        icon_photo = self.item_manager.get_icon_photo(icon_id)
        if icon_photo:
            self.icon_label.config(image=icon_photo)
            self.icon_label.image = icon_photo  # Keep reference
        self.durability_entry.insert(0, item.get("Durability", ""))
        
        # Set category combobox
        category_id = str(item.get("ItemCategory", ""))
        if category_id in self.item_manager.categories:
            category_name = self.item_manager.categories[category_id]["key"]
            self.category_combo.set(category_name)
        self.value_entry.insert(0, item.get("Value", ""))
        
        equipment = item.get("Equipment", {})
        self.pos_x_entry.insert(0, equipment.get("PositionX", ""))
        self.pos_y_entry.insert(0, equipment.get("PositionY", ""))
        self.rotation_entry.insert(0, equipment.get("Rotation", ""))
        
        tool = item.get("Tool", {})
        self.melee_entry.insert(0, tool.get("MeleeDamage", ""))
        self.efficiency_entry.insert(0, tool.get("Efficiency", ""))
        self.tool_type_entry.insert(0, tool.get("ToolType", ""))
        self.labor_entry.insert(0, ",".join(tool.get("LaborTypes", [])))

    def get_category_id(self, category_name: str) -> int:
        """Get category ID from name"""
        for cat_id, cat_data in self.item_manager.categories.items():
            if cat_data["key"] == category_name:
                return int(cat_id)
        return 0  # Default to 0 if not found

    def save(self):
        """Save item data with validation"""
        key = self.key_entry.get().strip()
        if not key:
            messagebox.showerror("Error", "Key is required")
            logging.error("Save failed: Key is required")
            return

        try:
            # Validate numerical fields
            def validate_number(value, field_name, min_value=0):
                try:
                    val = int(value) if value.strip() else min_value
                    if val < min_value:
                        raise ValueError(f"{field_name} must be non-negative")
                    return val
                except ValueError as e:
                    raise ValueError(f"Invalid {field_name}: {str(e)}")

            def validate_float(value, field_name, min_value=0.0):
                try:
                    val = float(value) if value.strip() else min_value
                    if val < min_value:
                        raise ValueError(f"{field_name} must be non-negative")
                    return val
                except ValueError as e:
                    raise ValueError(f"Invalid {field_name}: {str(e)}")

            # Get category ID from selected name
            category_name = self.category_combo.get()
            category_id = self.get_category_id(category_name) if category_name else 0

            # Validate and collect item data
            item_data = {
                "Id": validate_number(self.id_entry.get(), "ID", 0),
                "key": key,
                "Durability": validate_number(self.durability_entry.get(), "Durability"),
                "ItemCategory": category_id,
                "Value": validate_number(self.value_entry.get(), "Value"),
            }

            # Equipment data
            if any([self.pos_x_entry.get(), self.pos_y_entry.get(), self.rotation_entry.get()]):
                item_data["Equipment"] = {
                    "TextureData": None,
                    "PositionX": validate_number(self.pos_x_entry.get(), "PositionX"),
                    "PositionY": validate_number(self.pos_y_entry.get(), "PositionY"),
                    "Rotation": validate_number(self.rotation_entry.get(), "Rotation")
                }

            # Tool data
            if any([self.melee_entry.get(), self.efficiency_entry.get(), self.tool_type_entry.get()]):
                valid_labor_types = ["Haul", "Supply", "Plant", "Harvest", "Chop", "Mine", "Hunt", "Fish"]
                labor_types = [lt.strip() for lt in self.labor_entry.get().split(",") if lt.strip() and lt.strip() in valid_labor_types]
                if not labor_types and self.labor_entry.get().strip():
                    raise ValueError("Invalid LaborTypes: Must be from [Haul, Supply, Plant, Harvest, Chop, Mine, Hunt, Fish]")

                item_data["Tool"] = {
                    "MeleeDamage": validate_number(self.melee_entry.get(), "MeleeDamage"),
                    "RangeDamage": 0,
                    "Level": 1,
                    "Efficiency": validate_float(self.efficiency_entry.get(), "Efficiency"),
                    "ToolType": validate_number(self.tool_type_entry.get(), "ToolType"),
                    "CreatureType": 0,
                    "RechargeTime": 0,
                    "AmmoTextureId": -1,
                    "ProjectileSpeed": 0,
                    "LaborTypes": labor_types
                }

            if self.item_key:
                success = self.item_manager.update_item(self.item_key, item_data)
                action = "updated"
            else:
                success = self.item_manager.add_item(key, item_data)
                action = "added"

            if success:
                messagebox.showinfo("Success", f"Item {action} successfully")
                logging.info(f"Item {action}: {key}")
                self.destroy()
            else:
                messagebox.showerror("Error", f"Failed to {action} item: Key already exists")
                logging.error(f"Failed to {action} item: Key already exists for {key}")

        except ValueError as e:
            messagebox.showerror("Error", str(e))
            logging.error(f"Save failed for item {key}: {str(e)}")

    def on_closing(self):
        """Handle window close event"""
        if messagebox.askyesno("Confirm", "Are you sure you want to close without saving?"):
            self.destroy()
            logging.info("Item editor window closed without saving")

class ItemManagerApp:
    """Main application class"""
    def __init__(self, root):
        self.root = root
        self.root.title("Item Manager")
        self.root.geometry(f"{Config.WINDOW_WIDTH}x{Config.WINDOW_HEIGHT}")
        self.root.resizable(True, True)  # Allow resizing
        self.item_manager = ItemManager()
        self.sort_column = None
        self.sort_reverse = False
        self.create_widgets()
        self.root.bind("<Control-n>", lambda e: self.add_item())
        self.root.bind("<Control-e>", lambda e: self.edit_item())
        self.root.bind("<Control-d>", lambda e: self.delete_item())

    def create_widgets(self):
        # Main frame
        main_frame = ttk.Frame(self.root, padding="10")
        main_frame.pack(fill="both", expand=True)

        # Search bar with real-time search
        search_frame = ttk.Frame(main_frame)
        search_frame.pack(fill="x", pady=5)
        label = ttk.Label(search_frame, text="Search by Name:")
        label.pack(side="left", padx=5)
        ToolTip(label, "Search items by name (real-time)")
        self.search_entry = ttk.Entry(search_frame, width=30)
        self.search_entry.pack(side="left", padx=5)
        self.search_entry.bind('<KeyRelease>', lambda event: self.search_items())
        button = ttk.Button(search_frame, text="Clear Search", command=self.clear_search)
        button.pack(side="left", padx=5)
        ToolTip(button, "Clear the search filter")

        # Treeview with columns and sortable headers
        self.tree = ttk.Treeview(main_frame, columns=("Id", "Name", "Category", "Value", "Durability"), show="headings")
        self.sort_arrows = {}  # Track sort direction per column
        for column in ["Id", "Name", "Category", "Value", "Durability"]:
            self.tree.heading(column, text=column, command=lambda c=column: self.sort_tree(c))
            self.tree.column(column, width=Config.COLUMN_WIDTHS[column], stretch=True)  # Allow column resizing
            self.sort_arrows[column] = None  # Initialize sort direction
        self.tree.pack(fill="both", expand=True)

        # Context menu for treeview
        self.tree.bind("<Button-3>", self.show_context_menu)

        # Buttons with tooltips
        btn_frame = ttk.Frame(main_frame)
        btn_frame.pack(fill="x", pady=10)
        button = ttk.Button(btn_frame, text="Add Item", command=self.add_item)
        button.pack(side="left", padx=5)
        ToolTip(button, "Add a new item (Ctrl+N)")
        button = ttk.Button(btn_frame, text="Edit Item", command=self.edit_item)
        button.pack(side="left", padx=5)
        ToolTip(button, "Edit selected item (Ctrl+E)")
        button = ttk.Button(btn_frame, text="Delete Item", command=self.delete_item)
        button.pack(side="left", padx=5)
        ToolTip(button, "Delete selected item (Ctrl+D)")
        button = ttk.Button(btn_frame, text="Refresh", command=self.load_items)
        button.pack(side="left", padx=5)
        ToolTip(button, "Refresh the item list")

        # Export/Import buttons
        export_import_frame = ttk.Frame(main_frame)
        export_import_frame.pack(fill="x", pady=5)
        button = ttk.Button(export_import_frame, text="Export Items", command=self.export_items)
        button.pack(side="left", padx=5)
        ToolTip(button, "Export items to CSV")
        button = ttk.Button(export_import_frame, text="Import Items", command=self.import_items)
        button.pack(side="left", padx=5)
        ToolTip(button, "Import items from JSON")

        self.load_items()

    def sort_tree(self, column: str):
        """Sort treeview by column, toggle ascending/descending with proper handling of text and numbers"""
        if self.sort_column == column:
            self.sort_reverse = not self.sort_reverse
        else:
            self.sort_reverse = False
        self.sort_column = column

        # Update sort arrow in heading
        for col in ["Id", "Name", "Category", "Value", "Durability"]:
            arrow = " ▼" if col == column and self.sort_reverse else " ▲" if col == column and not self.sort_reverse else ""
            self.tree.heading(col, text=f"{col}{arrow}")

        # Get all items and their values
        items = [(self.tree.item(iid, "values"), iid) for iid in self.tree.get_children()]
        
        def get_sort_key(value: str, col: str) -> Any:
            try:
                if value.strip():  # Check if value is not empty
                    if col in ["Id", "Value", "Durability"]:
                        return int(value)  # Numerical sorting
                    elif col in ["Name", "Category"]:
                        return value.lower()  # Alphabetical sorting, case-insensitive
                return float('-inf') if col in ["Id", "Value", "Durability"] else ""  # Default for empty values
            except ValueError:
                return float('-inf') if col in ["Id", "Value", "Durability"] else ""  # Fallback for invalid numbers

        # Determine the column index for sorting
        col_index = {"Id": 0, "Name": 1, "Category": 2, "Value": 3, "Durability": 4}[column]
        
        # Sort items based on the selected column
        items.sort(key=lambda x: get_sort_key(x[0][col_index], column), reverse=self.sort_reverse)

        # Rebuild the treeview with sorted items
        for item in self.tree.get_children():
            self.tree.delete(item)
        for values, iid in items:
            self.tree.insert("", "end", iid=iid, values=values)

    def load_items(self, filter_text: str = ""):
        """Load items into treeview with optional filtering by name and progress indicator"""
        progress = ttk.Progressbar(self.root, mode="indeterminate")
        progress.pack(fill="x", pady=5)
        progress.start()

        for item in self.tree.get_children():
            self.tree.delete(item)
        
        filtered_items = [(key, data) for key, data in self.item_manager.items.items() if filter_text.lower() in key.lower()]
        for key, data in filtered_items:
            self.tree.insert("", "end", iid=key, values=(
                data.get("Id", ""),
                data.get("key", ""),
                self.item_manager.categories.get(str(data.get("ItemCategory", "")), {}).get("key", "Unknown"),
                data.get("Value", ""),
                data.get("Durability", "")
            ))

        progress.stop()
        progress.destroy()
        logging.info(f"Loaded {len(filtered_items)} items with filter: '{filter_text}'")

    def search_items(self):
        """Filter items based on search text (real-time)"""
        search_text = self.search_entry.get().strip()
        self.load_items(search_text)

    def clear_search(self):
        """Clear search filter and show all items"""
        self.search_entry.delete(0, tk.END)
        self.load_items()

    def add_item(self):
        """Open add item window"""
        ItemEditorWindow(self.root, self.item_manager)
        self.root.wait_window()
        self.load_items()
        logging.info("Added new item")

    def edit_item(self):
        """Open edit item window for selected item"""
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Warning", "Please select an item to edit")
            logging.warning("Edit attempted without selection")
            return
        ItemEditorWindow(self.root, self.item_manager, selected[0])
        self.root.wait_window()
        self.load_items()
        logging.info(f"Edited item: {selected[0]}")

    def delete_item(self):
        """Delete selected item with confirmation"""
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Warning", "Please select an item to delete")
            logging.warning("Delete attempted without selection")
            return
        
        if messagebox.askyesno("Confirm", "Are you sure you want to delete this item?"):
            self.item_manager.delete_item(selected[0])
            self.load_items()
            logging.info(f"Deleted item: {selected[0]}")

    def show_context_menu(self, event):
        """Show context menu for treeview"""
        context_menu = tk.Menu(self.root, tearoff=0)
        context_menu.add_command(label="Edit Item", command=self.edit_item, accelerator="Ctrl+E")
        context_menu.add_command(label="Delete Item", command=self.delete_item, accelerator="Ctrl+D")
        context_menu.add_command(label="Refresh", command=self.load_items)
        context_menu.post(event.x_root, event.y_root)

    def export_items(self):
        """Export items to CSV"""
        try:
            import csv
            with open('items_export.csv', 'w', newline='', encoding='utf-8') as f:
                writer = csv.writer(f)
                writer.writerow(["ID", "Name", "Category", "Value", "Durability"])
                for key, data in self.item_manager.items.items():
                    writer.writerow([
                        data.get("Id", ""),
                        data.get("key", ""),
                        self.item_manager.categories.get(str(data.get("ItemCategory", "")), {}).get("key", "Unknown"),
                        data.get("Value", ""),
                        data.get("Durability", "")
                    ])
            messagebox.showinfo("Success", "Items exported to items_export.csv")
            logging.info("Items exported to CSV")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to export items: {str(e)}")
            logging.error(f"Failed to export items: {str(e)}")

    def import_items(self):
        """Import items from JSON file"""
        file_path = simpledialog.askstring("Import Items", "Enter JSON file path:", parent=self.root)
        if not file_path or not os.path.exists(file_path):
            messagebox.showwarning("Warning", "Invalid file path")
            logging.warning("Import failed: Invalid file path")
            return

        try:
            with open(file_path, 'r') as f:
                new_items = json.load(f)
            for key, data in new_items.items():
                self.item_manager.add_item(key, data)
            self.load_items()
            messagebox.showinfo("Success", "Items imported successfully")
            logging.info(f"Items imported from {file_path}")
        except Exception as e:
            messagebox.showerror("Error", f"Failed to import items: {str(e)}")
            logging.error(f"Failed to import items: {str(e)}")

# Custom tooltip class
class ToolTip:
    def __init__(self, widget, text):
        self.widget = widget
        self.text = text
        self.tooltip = None
        self.widget.bind("<Enter>", self.show)
        self.widget.bind("<Leave>", self.hide)

    def show(self, event):
        if self.tooltip:
            return
        x, y, _, _ = self.widget.bbox("insert")
        x += self.widget.winfo_rootx() + 25
        y += self.widget.winfo_rooty() + 20
        self.tooltip = tk.Toplevel(self.widget)
        self.tooltip.wm_overrideredirect(True)
        self.tooltip.wm_geometry(f"+{x}+{y}")
        label = ttk.Label(self.tooltip, text=self.text, background="#ffffe0", relief="solid", borderwidth=1)
        label.pack()

    def hide(self, event):
        if self.tooltip:
            self.tooltip.destroy()
            self.tooltip = None

if __name__ == "__main__":
    root = tk.Tk()
    app = ItemManagerApp(root)
    root.mainloop()