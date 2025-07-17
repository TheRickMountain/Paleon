using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class SettlerCmp : CreatureCmp
    {
        public bool IsSelected { get; set; } = false;
        public bool IsUnderControl { get; private set; } = false;
        private float controlTimer = 0;

        public Tile OccupiedTile { get; set; } = null;

        private SettlerSleepLabor sleepLabor;
        private HealLabor healLabor;
        private EatLabor eatLabor;
        private GetWarmLabor getWarmLabor;
        private DrinkLabor drinkLabor;
        private SettlerWanderLabor wanderLabor;
        private GetDressedLabor getDressedLabor;
        private GetDressedLabor getDressedTopLabor;
        private CureFoodPoisoningLaborLabor cureFoodPoisoningLaborLabor;

        private int idleTime = 8;

        private SettlerInfo settlerInfo;

        private LaborType lastLaborType = LaborType.None;

        private AnimatedSprite vomitingSprite;

        private float vomitingTimer;

        public SettlerCmp(SettlerInfo settlerInfo, CreatureStats stats, float moveSpeed, Tile spawnTile, 
            InteractablesManager interactablesManager) 
            : base(ResourceManager.CreaturesTileset[settlerInfo.BodyTextureId], 
                  ResourceManager.HairsTileset[settlerInfo.HairTextureId], settlerInfo.Name, stats, moveSpeed, CreatureType.Settler,
                  spawnTile, interactablesManager)
        {
            this.settlerInfo = settlerInfo;

            foreach (LaborType laborType in Labor.GetWorkLaborEnumerator())
            {
                AllowedLabors.Add(laborType);
            }

            IsDomesticated = true;
            AgeState = AgeState.Adult;

            HairImage.Color = settlerInfo.HairColor;

            eatLabor = new EatLabor();
            eatLabor.Repeat = true;

            getWarmLabor = new GetWarmLabor();
            getWarmLabor.Repeat = true;

            drinkLabor = new DrinkLabor();
            drinkLabor.Repeat = true;

            sleepLabor = new SettlerSleepLabor();
            sleepLabor.Repeat = true;

            healLabor = new HealLabor();
            healLabor.Repeat = true;

            wanderLabor = new SettlerWanderLabor();
            wanderLabor.Repeat = true;

            getDressedLabor = new GetDressedLabor(false);
            getDressedLabor.Repeat = true;

            getDressedTopLabor = new GetDressedLabor(true);
            getDressedTopLabor.Repeat = true;

            cureFoodPoisoningLaborLabor = new CureFoodPoisoningLaborLabor();
            cureFoodPoisoningLaborLabor.Repeat = true;

            BodyImage.Width = 24;
            BodyImage.Height = 24;
            BodyImage.SetOrigin(0, 0);
            BodyImage.X = (Engine.TILE_SIZE / 2) - (24 / 2);
            BodyImage.Y = -(24 - Engine.TILE_SIZE);

            CanBeRenamed = true;
        }

        public override void Begin()
        {
            base.Begin();

            vomitingSprite = new AnimatedSprite(24, 24);
            vomitingSprite.Add("idle", new Animation(ResourceManager.GetTexture("vomiting"), 4, 0, 24, 24, 0, 0));
            vomitingSprite.Play("idle");
            vomitingSprite.Active = false;
            vomitingSprite.SetOrigin(0, 0);
            vomitingSprite.X = (Engine.TILE_SIZE / 2) - (24 / 2);
            vomitingSprite.Y = -(24 - Engine.TILE_SIZE);
            Entity.Add(vomitingSprite);

            StatusEffectsManager.StatusEffectAdded += OnStatusEffectAddedCallback;
        }

        public override void Update()
        {
            base.Update();

            if (IsDead)
                return;

            vomitingSprite.Active = StatusEffectsManager.ContainsStatusEffect(StatusEffectId.Vomiting);
            vomitingSprite.Visible = StatusEffectsManager.ContainsStatusEffect(StatusEffectId.Vomiting);

            if(GameplayScene.Instance.WorldState.GetCurrentHourTimeOfDay() == TimeOfDay.Night &&
                Movement.CurrentTile.IsIlluminated == false)
            {
                StatusEffectsManager.AddStatusEffect(StatusEffectId.LowLight);
            }
            else
            {
                StatusEffectsManager.RemoveStatusEffect(StatusEffectId.LowLight);
            }

            if(StatusEffectsManager.ContainsStatusEffect(StatusEffectId.FoodPoisoning))
            {
                CreatureThoughts.AddThought("poisoned", 4);

                vomitingTimer -= Engine.GameDeltaTime;

                if (vomitingTimer <= 0)
                {
                    vomitingTimer = MyRandom.Range(6 * WorldState.MINUTES_PER_HOUR, 10 * WorldState.MINUTES_PER_HOUR);

                    StatusEffectsManager.AddStatusEffect(StatusEffectId.Vomiting);
                }
            }

            if (IsUnderControl)
            {
                // Счетчик ожидания команд будет запущен только после того, как у поселенца не будет работы
                if (CurrentLabor == null)
                {
                    controlTimer += Engine.GameDeltaTime;
                    if (controlTimer >= WorldState.MINUTES_PER_HOUR)
                    {
                        Disband();
                    }
                }
            }

            if (IsUnderControl == false)
            {
                if (CreatureStats.Health.IsDissatisfied())
                {
                    if (CurrentLabor == null || 
                        (CurrentLabor.LaborType != LaborType.Heal
                        && CurrentLabor.LaborType != LaborType.Sleep))
                    {
                        if (healLabor.Check(this))
                        {
                            if (CurrentLabor != null)
                            {
                                CurrentLabor.CancelAndClearTasks(this);
                                CurrentLabor = null;
                                CurrentTask = null;
                            }

                            CurrentLabor = healLabor;
                            CurrentLabor.CreateTasks(this);
                            CurrentLabor.InitTasks(this);

                            lastLaborType = CurrentLabor.LaborType;
                        }
                    }
                }

                if (CreatureStats.Hunger.IsDissatisfied())
                {
                    if (CurrentLabor == null || (CurrentLabor.LaborType != LaborType.Eat && CurrentLabor.LaborType != LaborType.Heal))
                    {
                        if (eatLabor.Check(this))
                        {
                            if (CurrentLabor != null)
                            {
                                CurrentLabor.CancelAndClearTasks(this);
                                CurrentLabor = null;
                                CurrentTask = null;
                            }

                            CurrentLabor = eatLabor;
                            CurrentLabor.CreateTasks(this);
                            CurrentLabor.InitTasks(this);

                            lastLaborType = CurrentLabor.LaborType;
                        }
                    }
                }

                if (CreatureStats.Hunger.IsSatisfied() == false && CurrentLabor == null && lastLaborType == LaborType.Eat)
                {
                    if (eatLabor.Check(this))
                    {
                        if (CurrentLabor != null)
                        {
                            CurrentLabor.CancelAndClearTasks(this);
                            CurrentLabor = null;
                            CurrentTask = null;
                        }

                        CurrentLabor = eatLabor;
                        CurrentLabor.CreateTasks(this);
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                }

                if (CreatureStats.Energy.IsDissatisfied())
                {
                    if(CurrentLabor == null || (CurrentLabor.LaborType != LaborType.Sleep
                        && CurrentLabor.LaborType != LaborType.Eat
                        && CurrentLabor.LaborType != LaborType.Heal))
                    {
                        if (sleepLabor.Check(this))
                        {
                            if (CurrentLabor != null)
                            {
                                CurrentLabor.CancelAndClearTasks(this);
                                CurrentLabor = null;
                                CurrentTask = null;
                            }

                            CurrentLabor = sleepLabor;
                            CurrentLabor.CreateTasks(this);
                            CurrentLabor.InitTasks(this);

                            lastLaborType = CurrentLabor.LaborType;
                        }
                    }
                }

                if (CurrentLabor == null)
                {
                    if(StatusEffectsManager.ContainsStatusEffect(StatusEffectId.FoodPoisoning) && cureFoodPoisoningLaborLabor.Check(this))
                    {
                        if (CurrentLabor != null)
                        {
                            CurrentLabor.CancelAndClearTasks(this);
                            CurrentLabor = null;
                            CurrentTask = null;
                        }

                        CurrentLabor = cureFoodPoisoningLaborLabor;
                        CurrentLabor.CreateTasks(this);
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                    else if (CreatureStats.Happiness.IsDissatisfied() && drinkLabor.Check(this))
                    {
                        if (CurrentLabor != null)
                        {
                            CurrentLabor.CancelAndClearTasks(this);
                            CurrentLabor = null;
                            CurrentTask = null;
                        }

                        CurrentLabor = drinkLabor;
                        CurrentLabor.CreateTasks(this);
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                    else if (CreatureStats.Temperature.IsDissatisfied() && getWarmLabor.Check(this))
                    {
                        if (CurrentLabor != null)
                        {
                            CurrentLabor.CancelAndClearTasks(this);
                            CurrentLabor = null;
                            CurrentTask = null;
                        }

                        CurrentLabor = getWarmLabor;
                        CurrentLabor.CreateTasks(this);
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                }

                if (CreatureEquipment.ClothingItemContainer == null)
                {
                    if (CurrentLabor == null && getDressedLabor.Check(this))
                    {
                        CurrentLabor = getDressedLabor;
                        CurrentLabor.CreateTasks(this);
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                }

                if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
                {
                    if (CreatureEquipment.TopClothingItemContainer == null)
                    {
                        if (CurrentLabor == null && getDressedTopLabor.Check(this))
                        {
                            CurrentLabor = getDressedTopLabor;
                            CurrentLabor.CreateTasks(this);
                            CurrentLabor.InitTasks(this);

                            lastLaborType = CurrentLabor.LaborType;
                        }
                    }
                }
                else
                {
                    // Поселенец снимает верхнюю одежду в теплый сезон
                    if (CreatureEquipment.TopClothingItemContainer != null)
                    {
                        CreatureEquipment.ThrowTopClothing(Movement.CurrentTile);
                    }
                }

                if (CurrentLabor == null)
                {
                    CurrentLabor = LookForLabor(GameplayScene.WorldManager.LaborManager.LaborsByType);
                    if (CurrentLabor != null)
                    {
                        CurrentLabor.InitTasks(this);

                        lastLaborType = CurrentLabor.LaborType;
                    }
                }
            }

            if (CurrentLabor != null)
            {
                if (CurrentTask == null)
                {
                    if (CurrentLabor.GetTasks(this).Count > 0)
                    {
                        CurrentTask = CurrentLabor.GetTasks(this)[0];
                        CurrentTask.BeforeUpdate();
                    }
                    else
                    {
                        CurrentLabor = null;
                    }
                }
                else
                {
                    switch (CurrentTask.Update())
                    {
                        case TaskState.Success:
                            if(CurrentTask != null)
                            {
                                CurrentTask.Complete();
                            }
                            CurrentTask = CurrentLabor.GetNextTask(this);
                            if (CurrentTask != null)
                                CurrentTask.BeforeUpdate();
                            else
                                CurrentLabor = null;

                            break;
                        case TaskState.Failed:
                            CurrentLabor.CancelAndClearTasks(this);
                            CurrentTask = null;
                            CurrentLabor = null;
                            break;
                        case TaskState.Canceled:
                            CurrentTask = null;
                            CurrentLabor = null;
                            break;
                    }
                }
            }
        }

        // If not rendering - seems like component is invisible
        public override void Render()
        {
            if (IsSelected)
            {
                ResourceManager.UnitSelectorSprite.Draw(BodyImage.RenderPosition + new Vector2(0, 2), Color.GreenYellow);
            }

            if (CreatureStats.IsAsleep)
            {
                ResourceManager.BedBottom.Draw(BodyImage.RenderPosition);
            }

            base.Render();

            if (AttackSprite.Active)
                AttackSprite.Render();

            if (CreatureStats.IsAsleep)
            {
                ResourceManager.BedTop.Draw(BodyImage.RenderPosition);
            }
        }

        public override float TakeDamage(CreatureCmp damager, float damage)
        {
            float finalDamage = base.TakeDamage(damager, damage);

            if(IsDead == false && CreatureStats.Health.IsDissatisfied())
            {
                if (CurrentLabor == null || (CurrentLabor.LaborType != LaborType.Hide && CurrentLabor.LaborType != LaborType.Heal))
                {
                    if (CurrentLabor != null)
                    {
                        CurrentLabor.CancelAndClearTasks(this);
                        CurrentLabor = null;
                        CurrentTask = null;
                    }

                    HideLabor hideLabor = new HideLabor(damager);
                    CurrentLabor = hideLabor;
                    CurrentLabor.Check(this);
                    CurrentLabor.InitTasks(this);
                }
            }

            return finalDamage;
        }

        private void OnStatusEffectAddedCallback(StatusEffect statusEffect)
        {
            if(statusEffect.Id == StatusEffectId.FoodPoisoning)
            {
                vomitingTimer = MyRandom.Range(6 * WorldState.MINUTES_PER_HOUR, 10 * WorldState.MINUTES_PER_HOUR);

                GameplayScene.UIRootNodeScript.NotificationsUI
                    .GetComponent<NotificationsUIScript>()
                    .AddNotification(Localization.GetLocalizedText("x_got_the_y_effect", Name, statusEffect.Name), 
                        NotificationLevel.WARNING, Entity);
            }
        }

        protected override void ChangeDirection(Direction direction)
        {
            base.ChangeDirection(direction);

            if(direction == Direction.LEFT)
            {
                if (vomitingSprite != null)
                {
                    vomitingSprite.FlipX = false;
                }
            }
            else if(direction == Direction.RIGHT)
            {
                if (vomitingSprite != null)
                {
                    vomitingSprite.FlipX = true;
                }
            }
        }

        public void Disband()
        {
            IsUnderControl = false;
            controlTimer = 0;

            if (OccupiedTile != null)
            {
                OccupiedTile.OccupiedBy = null;
                OccupiedTile = null;
            }
        }

        public override bool SetLabor(Labor labor)
        {
            bool success = base.SetLabor(labor);

            if (success)
            {
                IsUnderControl = true;
                controlTimer = 0;
            }

            return success;
        }

        private int currentLaborIteration = 0;
        private int currentLaborTypeIteration = 0;

        private Timer wanderTimer = new Timer();

        private Labor LookForLabor(Dictionary<LaborType, List<Labor>> laborsByType)
        {
            // Иногда игрок может запретить поселенцу что-либо делать
            if (SortedLaborTypes.Count > 0)
            {
                if (currentLaborTypeIteration >= SortedLaborTypes.Count)
                {
                    currentLaborTypeIteration = 0;
                }

                LaborType checkLaborType = SortedLaborTypes[currentLaborTypeIteration];
                // Check if labor is allowed and any labors exists else got to check next labor type
                if (laborsByType[checkLaborType].Count > 0)
                {
                    List<Labor> labors = laborsByType[checkLaborType];

                    if (currentLaborIteration >= labors.Count)
                        currentLaborIteration = 0;

                    Labor labor = labors[currentLaborIteration];

                    if (labor.IsCompleted == false && labor.Check(this))
                    {
                        // Т.к. поселенец нашел работу, следующий раз он должен начать искать работу начиная с самого приоритетного типа работ
                        currentLaborTypeIteration = 0;
                        return labor;
                    }

                    currentLaborIteration++;
                    if (currentLaborIteration >= labors.Count)
                    {
                        currentLaborIteration = 0;
                        NextLaborType();
                    }
                }
                else
                {
                    NextLaborType();
                }
            }

            if (wanderTimer.GetTime() > idleTime && wanderLabor.Check(this))
            {
                wanderTimer.Reset();
                return wanderLabor;
            }

            return null;
        }

        // Переход к следующему типу работы
        private void NextLaborType()
        {
            currentLaborTypeIteration++;
            if (currentLaborTypeIteration >= SortedLaborTypes.Count)
            {
                currentLaborTypeIteration = 0;
            }
        }

        public override void Die(string reasonMessage, bool throwLoot = true)
        {
            base.Die(reasonMessage);

            if(OccupiedTile != null)
            {
                if(OccupiedTile.OccupiedBy == this)
                {
                    OccupiedTile.OccupiedBy = null;
                }

                OccupiedTile = null;
            }

            if (throwLoot)
            {
                Item corpse = ItemDatabase.GetItemByName("settler_corpse");
                Movement.CurrentTile.Inventory.AddCargo(new ItemContainer(corpse, 1, corpse.Durability));
            }

            if (string.IsNullOrEmpty(reasonMessage) == false)
            {
                GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                    .AddNotification(reasonMessage, NotificationLevel.WARNING, Entity);
            }
        }

        public override CreatureSaveData GetSaveData()
        {
            CreatureSaveData settlerSaveData = base.GetSaveData();
            settlerSaveData.Id = Id;

            if (Parent != null)
            {
                settlerSaveData.ParentId = Parent.Id;
            }

            if (Child != null)
            {
                settlerSaveData.ChildId = Child.Id;
            }

            settlerSaveData.X = Movement.CurrentTile.X;
            settlerSaveData.Y = Movement.CurrentTile.Y;
            settlerSaveData.Name = Name;
            settlerSaveData.HairColor = GetHairColor();

            settlerSaveData.Ration = new Dictionary<int, bool>();
            foreach(var kvp in FoodRation.GetFilters())
            {
                settlerSaveData.Ration.Add(kvp.Key.Id, kvp.Value);
            }

            settlerSaveData.OutfitId = CreatureEquipment.ClothingItemContainer == null ? -1 : CreatureEquipment.ClothingItemContainer.Item.Id;
            if (settlerSaveData.OutfitId != -1)
            {
                settlerSaveData.OutfitFactWeight = CreatureEquipment.ClothingItemContainer.FactWeight;
                settlerSaveData.OutfitDurability = CreatureEquipment.ClothingItemContainer.Durability;
            }

            settlerSaveData.TopOutfitId = CreatureEquipment.TopClothingItemContainer == null ? -1 : CreatureEquipment.TopClothingItemContainer.Item.Id;
            if (settlerSaveData.TopOutfitId != -1)
            {
                settlerSaveData.TopOutfitFactWeight = CreatureEquipment.TopClothingItemContainer.FactWeight;
                settlerSaveData.TopOutfitDurability = CreatureEquipment.TopClothingItemContainer.Durability;
            }

            settlerSaveData.LaborTypePriorityPair = new Dictionary<string, int>();
            foreach(var kvp in LaborTypePriorityPair)
            {
                settlerSaveData.LaborTypePriorityPair.Add(kvp.Key.ToString(), kvp.Value);
            }

            settlerSaveData.BodyTextureId = settlerInfo.BodyTextureId;
            settlerSaveData.HairTextureId = settlerInfo.HairTextureId;

            settlerSaveData.InventoryItems = new List<Tuple<int, int, float>>();
            foreach (var kvp in Inventory.ItemItemContainerPair)
            {
                List<ItemContainer> itemContainers = kvp.Value;

                foreach (var itemContainer in itemContainers)
                {
                    settlerSaveData.InventoryItems.Add(Tuple.Create(itemContainer.Item.Id, itemContainer.FactWeight, itemContainer.Durability));
                }
            }

            settlerSaveData.Hunger = CreatureStats.Hunger.CurrentValue;
            settlerSaveData.Health = CreatureStats.Health.CurrentValue;
            settlerSaveData.Energy = CreatureStats.Energy.CurrentValue;
            settlerSaveData.Temperature = CreatureStats.Temperature.CurrentValue;
            settlerSaveData.Happiness = CreatureStats.Happiness.CurrentValue;
            settlerSaveData.CreatureType = CreatureType;

            settlerSaveData.StatusEffects = new Dictionary<StatusEffectId, float>();
            foreach(var statusEffect in StatusEffectsManager.GetStatusEffects())
            {
                if (statusEffect.IsPassive)
                    continue;

                settlerSaveData.StatusEffects.Add(statusEffect.Id, statusEffect.Progress);
            }

            return settlerSaveData;
        }
    }
}
