using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Technolithic
{
    public class CreatureEquipment
    {

        private ItemContainer toolItemContainer;
        private ItemContainer clothingItemContainer;
        private ItemContainer topClothingItemContainer;

        public Action<Item> EquipmentChanged { get; set; }

        public int Defense { get; private set; }
        public int MeleeDamage { get; private set; }
        public float RechargeTime { get; private set; }

        private int nativeDefense;
        private int nativeMeleeDamage;
        private float nativeRechargeTime;

        private List<Sprite> sprites = new List<Sprite>();

        private Sprite toolImage;
        private Sprite clothingImage;
        private Sprite topClothingImage;

        private Dictionary<ToolType, ItemContainer> tools = new();

        public CreatureEquipment(int nativeDefense, int nativeMeleeDamage, float nativeRechargeTime)
        {
            this.nativeDefense = nativeDefense;
            this.nativeMeleeDamage = nativeMeleeDamage;
            this.nativeRechargeTime = nativeRechargeTime;

            foreach(ToolType toolType in Enum.GetValues(typeof(ToolType)))
            {
                tools.Add(toolType, null);
            }

            CalculateStats();
        }

        private void CalculateStats()
        {
            MeleeDamage = nativeMeleeDamage;
            RechargeTime = nativeRechargeTime;
            Defense = nativeDefense;

            if (toolItemContainer != null)
            {
                int toolMeleeDamage = toolItemContainer.Item.Tool.MeleeDamage;
                if (toolMeleeDamage != 0)
                {
                    MeleeDamage = toolMeleeDamage;
                }

                float rechargeTime = toolItemContainer.Item.Tool.RechargeTime;
                if (rechargeTime != 0)
                {
                    RechargeTime = rechargeTime;
                }
            }

            if (ClothingItemContainer != null)
            {
                Defense += ClothingItemContainer.Item.Outfit.Defense;
            }

            if (TopClothingItemContainer != null)
            {
                Defense += TopClothingItemContainer.Item.Outfit.Defense;
            }
        }

        public void InitializeSprites(Entity parentEntity)
        {
            toolImage = new Sprite(RenderManager.Pixel, 1, 1);
            parentEntity.Add(toolImage);

            clothingImage = new Sprite(RenderManager.Pixel, 1, 1);
            parentEntity.Add(clothingImage);

            topClothingImage = new Sprite(RenderManager.Pixel, 1, 1);
            parentEntity.Add(topClothingImage);
        }

        public ItemContainer ToolItemContainer
        {
            get { return toolItemContainer; }
            set
            {
                if (toolItemContainer == value)
                    return;

                toolItemContainer = value;

                OnItemContainerUpdated(toolItemContainer, toolImage);
            }
        }

        public ItemContainer ClothingItemContainer
        {
            get { return clothingItemContainer; }
            set
            {
                if (clothingItemContainer == value)
                    return;

                clothingItemContainer = value;

                OnItemContainerUpdated(clothingItemContainer, clothingImage);
            }
        }

        public ItemContainer TopClothingItemContainer
        {
            get { return topClothingItemContainer; }
            set {
                if (topClothingItemContainer == value)
                    return;

                topClothingItemContainer = value;

                OnItemContainerUpdated(topClothingItemContainer, topClothingImage);
            }
        }

        private void OnItemContainerUpdated(ItemContainer itemContainer, Sprite sprite)
        {
            if (itemContainer == null)
            {
                sprites.Remove(sprite);
                EquipmentChanged?.Invoke(null);
            }
            else
            {
                EquipmentChanged?.Invoke(itemContainer.Item);
                UpdateSpriteTexture(sprite, itemContainer.Item);
                UpdateSpritesOrdering();
            }

            CalculateStats();
        }

        private void UpdateSpriteTexture(Sprite sprite, Item item)
        {
            if (item.Equipment.TextureData != null)
            {
                sprite.Texture = item.Equipment.TextureData.Texture;
            }
            else
            {
                sprite.Texture = item.Icon;
            }

            sprite.SetTextureSize();
            sprite.CenterOrigin();
        }

        public IEnumerable<ItemContainer> GetTools()
        {
            foreach(var kvp in tools)
            {
                ItemContainer itemContainer = kvp.Value;

                if (itemContainer == null)
                    continue;

                yield return itemContainer;
            }
        }

        public void DegradeTool(ToolType toolType, float value)
        {
            ItemContainer itemContainer = tools[toolType];

            itemContainer.Durability -= value;
            if(itemContainer.Durability <= 0)
            {
                tools[toolType] = null;

                if (ToolItemContainer == itemContainer)
                {
                    ToolItemContainer = null;
                }
            }
        }

        public void EquipTool(ItemContainer itemContainer)
        {
            ToolType toolType = itemContainer.Item.Tool.ToolType;

            tools[toolType] = itemContainer;

            // Achievement
            int toolsAmount = 0;
            foreach(var kvp in tools)
            {
                if(kvp.Value != null)
                {
                    toolsAmount++;
                }
            }

            if(toolsAmount >= 3)
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.MULTITOOL);
            }
            // Achievement
        }

        public ItemContainer TryGetTool(ToolType toolType)
        {
            return tools[toolType];
        }

        public ItemContainer TryGetTool(LaborType laborType)
        {
            foreach(var kvp in tools)
            {
                ItemContainer itemContainer = kvp.Value;

                if (itemContainer == null)
                    continue;

                Tool tool = itemContainer.Item.Tool;

                for (int i = 0; i < tool.LaborTypes.Length; i++)
                {
                    if (tool.LaborTypes[i] == laborType)
                        return itemContainer;
                }
            }

            return null;
        }

        public bool HasTool(ToolType toolType)
        {
            return tools[toolType] != null;
        }

        public bool HasTool(LaborType laborType)
        {
            foreach(var kvp in tools)
            {
                ItemContainer itemContainer = kvp.Value;

                if (itemContainer == null)
                    continue;

                Tool tool = itemContainer.Item.Tool;

                for (int i = 0; i < tool.LaborTypes.Length; i++)
                {
                    if (tool.LaborTypes[i] == laborType)
                        return true;
                }
            }

            return false;
        }

        public void ThrowAllTools(Tile tile)
        {
            ToolItemContainer = null;

            foreach(var kvp in tools)
            {
                ToolType toolType = kvp.Key;
                ItemContainer itemContainer = kvp.Value;

                if (itemContainer == null)
                    continue;

                tile.Inventory.AddCargo(itemContainer);

                tools[toolType] = null;
            }
        }

        public void ThrowTool(ToolType toolType, Tile tile)
        {
            ItemContainer itemContainer = tools[toolType];

            if(ToolItemContainer == itemContainer)
            {
                ToolItemContainer = null;
            }

            if(itemContainer == null)
            {
                throw new InvalidOperationException($"Creature has no '{toolType}' tool");
            }

            tile.Inventory.AddCargo(itemContainer);

            tools[toolType] = null;
        }

        public void ThrowClothing(Tile tile)
        {
            if (ClothingItemContainer != null)
            {
                Item item = ClothingItemContainer.Item;
                float durability = ClothingItemContainer.Durability;
                ClothingItemContainer = null;
                tile.Inventory.AddCargo(new ItemContainer(item, 1, durability));
            }
        }

        public void ThrowTopClothing(Tile tile)
        {
            if(TopClothingItemContainer != null)
            {
                Item item = TopClothingItemContainer.Item;
                float durability = TopClothingItemContainer.Durability;
                TopClothingItemContainer = null;
                tile.Inventory.AddCargo(new ItemContainer(item, 1, durability));
            }
        }

        private void UpdateSpritesOrdering()
        {
            sprites.Clear();

            if (ClothingItemContainer != null)
            {
                sprites.Add(clothingImage);
            }

            if (TopClothingItemContainer != null)
            {
                sprites.Add(topClothingImage);
            }

            if (ToolItemContainer != null)
            {
                sprites.Add(toolImage);
            }
        }

        public void UpdateSpritesPositionings(float baseX, float baseY, bool flipped)
        {
            if(flipped)
            {
                if (toolItemContainer != null)
                {
                    Equipment equipment = toolItemContainer.Item.Equipment;
                    toolImage.X = baseX - equipment.PositionX;
                    toolImage.Y = baseY + equipment.PositionY;
                    toolImage.Rotation = MathHelper.ToRadians(-equipment.Rotation);
                    toolImage.FlipX = true;
                }

                if (clothingItemContainer != null)
                {
                    Equipment equipment = clothingItemContainer.Item.Equipment;
                    clothingImage.X = baseX - equipment.PositionX;
                    clothingImage.Y = baseY + equipment.PositionY;
                    clothingImage.Rotation = MathHelper.ToRadians(-equipment.Rotation);
                    clothingImage.FlipX = true;
                }

                if (topClothingItemContainer != null)
                {
                    Equipment equipment = topClothingItemContainer.Item.Equipment;
                    topClothingImage.X = baseX - equipment.PositionX;
                    topClothingImage.Y = baseY + equipment.PositionY;
                    topClothingImage.Rotation = MathHelper.ToRadians(-equipment.Rotation);
                    topClothingImage.FlipX = true;
                }
            }
            else
            {
                if (toolItemContainer != null)
                {
                    Equipment equipment = toolItemContainer.Item.Equipment;
                    toolImage.X = baseX + equipment.PositionX;
                    toolImage.Y = baseY + equipment.PositionY;
                    toolImage.Rotation = MathHelper.ToRadians(equipment.Rotation);
                    toolImage.FlipX = false;
                }

                if (clothingItemContainer != null)
                {
                    Equipment equipment = clothingItemContainer.Item.Equipment;
                    clothingImage.X = baseX + equipment.PositionX;
                    clothingImage.Y = baseY + equipment.PositionY;
                    clothingImage.Rotation = MathHelper.ToRadians(equipment.Rotation);
                    clothingImage.FlipX = false;
                }

                if (topClothingItemContainer != null)
                {
                    Equipment equipment = topClothingItemContainer.Item.Equipment;
                    topClothingImage.X = baseX + equipment.PositionX;
                    topClothingImage.Y = baseY + equipment.PositionY;
                    topClothingImage.Rotation = MathHelper.ToRadians(equipment.Rotation);
                    topClothingImage.FlipX = false;
                }
            }
        }

        public void UpdateDurability(float deltaTime)
        {
            foreach(var kvp in tools)
            {
                ToolType toolType = kvp.Key;
                ItemContainer itemContainer = kvp.Value;
               
                if (itemContainer == null)
                    continue;

                Item item = itemContainer.Item;

                if (item.IsDecayable)
                {
                    float spoilagePerMinute = item.SpoilageRate / (float)(WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
                    itemContainer.Durability -= spoilagePerMinute * deltaTime;

                    if (itemContainer.Durability <= 0)
                    {
                        if (ToolItemContainer == itemContainer)
                        {
                            ToolItemContainer = null;
                        }

                        tools[toolType] = null;
                    }
                }
            }

            if(ClothingItemContainer != null)
            {
                Item item = ClothingItemContainer.Item;
                if (item.IsDecayable)
                {
                    float spoilagePerMinute = item.SpoilageRate / (float)(WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
                    ClothingItemContainer.Durability -= spoilagePerMinute * deltaTime;

                    if (ClothingItemContainer.Durability <= 0)
                    {
                        ClothingItemContainer = null;
                    }
                }
            }

            if(TopClothingItemContainer != null)
            {
                Item item = TopClothingItemContainer.Item;
                if (item.IsDecayable)
                {
                    float spoilagePerMinute = item.SpoilageRate / (float)(WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
                    TopClothingItemContainer.Durability -= spoilagePerMinute * deltaTime;

                    if (TopClothingItemContainer.Durability <= 0)
                    {
                        TopClothingItemContainer = null;
                    }
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].Render();
            }
        }

    }
}
