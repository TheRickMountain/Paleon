using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class AnimalCmp : CreatureCmp
    {
        private bool hunt;
        private bool domesticate;
        private bool gatherProduct;

        private ActionWithAnimalLabor gatherProductLabor;

        public AnimalPenBuildingCmp TargetAnimalPen { get; set; }

        private static bool wasInformed = false;

        public bool Hunt 
        { 
            get { return hunt; } 
            set
            {
                if (hunt == value)
                    return;

                hunt = value;

                if (hunt)
                {
                    if (wasInformed == false)
                    {
                        GameplayScene.UIRootNodeScript?.NotificationsUI.GetComponent<NotificationsUIScript>()
                        .AddNotification(Localization.GetLocalizedText("to_do_the_job_of_x_you_need_tools", $"\"{Labor.GetLaborString(LaborType.Hunt)}\""), 
                        NotificationLevel.INFO, Entity);

                        wasInformed = true;
                    }

                    if (Domesticate)
                        Domesticate = false;

                    indicator.Active = true;
                    indicator.Texture = ResourceManager.HuntIcon;

                    GameplayScene.WorldManager.AnimalsToHunt.Add(this);
                }
                else
                {
                    indicator.Active = false;

                    GameplayScene.WorldManager.AnimalsToHunt.Remove(this);
                }
            }
        }

        public bool Domesticate 
        {
            get { return domesticate; }
            set
            {
                if (CanDomesticate == false)
                    return;

                if (domesticate == value)
                    return;

                domesticate = value;

                if (domesticate)
                {
                    // Can't be haunted and tamed at the same time
                    if (Hunt)
                        Hunt = false;
                
                    indicator.Active = true;
                    indicator.Texture = ResourceManager.DomesticateIcon;

                    GameplayScene.WorldManager.AnimalsToDomesticate.Add(this);
                }
                else
                {
                    indicator.Active = false;

                    GameplayScene.WorldManager.AnimalsToDomesticate.Remove(this);
                }
            }
        }

        public bool GatherProduct
        {
            get { return gatherProduct; }
            set
            {
                if (gatherProduct == value)
                    return;

                gatherProduct = value;

                if (gatherProduct)
                {

                }
                else
                {
                    if (gatherProductLabor != null)
                    {
                        gatherProductLabor.CancelAndClearAllTasksAndComplete();
                        gatherProductLabor = null;
                    }
                }
            }
        }

        public bool CanGatherProduct => AnimalTemplate.AnimalProduct != null;

        public bool CanHunt
        {
            get
            {
                if (IsDomesticated)
                    return false;

                if (IsHidden)
                    return false;

                return true;
            }
        }

        public bool CanDomesticate
        {
            get
            {
                if (IsDomesticated)
                    return false;

                if (WasAttacked) // нельзя приручить атакованное животное
                    return false;

                DomesticationData domesticationData = AnimalTemplate.DomesticationData;

                if (domesticationData == null)
                    return false;

                return GameplayScene.Instance.ProgressTree.IsAnimalUnlocked(domesticationData.TamedFormAnimalTemplate);
            }
        }

        public AnimalTemplate AnimalTemplate { get; private set; }

        private bool readyToGatherProduct = false;
        public float ProductReadyPercent { get; set; } = 0;
        private float percentPerMinute;

        private AnimalSleepLabor sleepLabor;
        private AnimalEatLabor eatLabor;
        private AnimalWanderLabor wanderLabor;
        private AnimalFertilizationLabor animalFertilizationLabor;

        // TODO: remove after migration to interaction system
        private Sprite indicator;

        public bool WasAttacked { get; set; } = false;

        private bool isReserved = false; // Animal can't work, eat, sleep when it's reserved
        public bool IsReserved 
        { 
            get => isReserved;
            set
            {
                isReserved = value;

                if(isReserved)
                {
                    if (CurrentLabor != null)
                    {
                        CurrentLabor.CancelAndClearTasks(this);
                        CurrentLabor = null;
                        CurrentTask = null;
                    }
                }
            }
        }

        public int DaysUntilAging { get; set; }

        public AnimalCmp(CreatureStats stats, AnimalTemplate animalTemplate) 
            : base(animalTemplate.Texture, null, animalTemplate.Name, stats, animalTemplate.MovementSpeed, CreatureType.Animal)
        {
            AnimalTemplate = animalTemplate;

            AgeState = animalTemplate.AgeState;

            if (AnimalTemplate.IsWild)
            {
                IsDomesticated = false;
            }
            else
            {
                IsDomesticated = true;
            }

            foreach (LaborType laborType in Enum.GetValues(typeof(LaborType)))
            {
                SetLaborPriority(laborType, -1);
            }

            foreach (LaborType laborType in AnimalTemplate.AllowedLabors)
            {
                AllowedLabors.Add(laborType);
            }

            sleepLabor = new AnimalSleepLabor();
            sleepLabor.Repeat = true;

            eatLabor = new AnimalEatLabor();
            eatLabor.Repeat = true;

            wanderLabor = new AnimalWanderLabor();
            wanderLabor.Repeat = true;

            animalFertilizationLabor = new AnimalFertilizationLabor();
            animalFertilizationLabor.Repeat = true;

            indicator = new Sprite(RenderManager.Pixel);
            indicator.Color = Color.White * 0.75f;
            indicator.Width = 16;
            indicator.Height = 16;
            indicator.Active = false;
            indicator.X = 4;
            indicator.Y = -AnimalTemplate.TextureHeight;

            if (AnimalTemplate.AnimalProduct != null)
            {
                percentPerMinute = AnimalTemplate.AnimalProduct.PercentPerDay / (WorldState.HOURS_PER_CYCLE * WorldState.MINUTES_PER_HOUR);
            }

            BodyImage.Width = AnimalTemplate.TextureWidth;
            BodyImage.Height = AnimalTemplate.TextureHeight;
            BodyImage.SetOrigin(0, 0);
            BodyImage.X = (Engine.TILE_SIZE / 2) - (AnimalTemplate.TextureWidth / 2);
            BodyImage.Y = -(AnimalTemplate.TextureHeight - Engine.TILE_SIZE);

            if(IsDomesticated)
            {
                CanBeRenamed = true;
            }

            DaysUntilAging = AnimalTemplate.DaysUntilAging;
        }

        // TODO: нужно перестать вызывать этот метод вручную, так как при изменениях, я могу случайно забыть вызвать его
        // и система взаимодействий перестанет работать
        public void Initialize()
        {
            if (AnimalTemplate.IsWild)
            {

            }
            else
            {
                AddAvailableInteraction(InteractionType.Slaughter, LaborType.Ranching, false);
                SetInteractionDuration(InteractionType.Slaughter, 1.0f);

                ActivateInteraction(InteractionType.Slaughter);
            }
        }

        public override void Begin()
        {
            base.Begin();

            indicator.Entity = Entity;

            NextFertilizationHoursSum = MyRandom.Range(WorldState.HOURS_PER_CYCLE, WorldState.HOURS_PER_CYCLE + WorldState.HOURS_PER_CYCLE / 2);

            GameplayScene.Instance.WorldState.OnNextDayStartedCallback += OnNextDayStarted;
            GameplayScene.Instance.WorldState.NextHourStarted += OnNextHourStarted;

            if (AnimalTemplate.IsWild)
            {
                GameplayScene.WorldManager.WildAnimalsNumber++;
            }
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.Slaughter:
                    {
                        Die("");
                    }
                    break;
            }
        }

        protected override void ChangeDirection(Direction direction)
        {
            base.ChangeDirection(direction);

            if (direction == Direction.LEFT)
            {
                CargoImage.X = 28;
                CargoImage.Y = 0;
            }
            else if (direction == Direction.RIGHT)
            {
                CargoImage.X = -12;
                CargoImage.Y = 0;
            }
        }

        public override void FallAsleep(bool value)
        {
            base.FallAsleep(value);

            if (CreatureStats.IsAsleep)
            {
                BodyImage.Texture = AnimalTemplate.SleepTexture;
            }
            else
            {
                BodyImage.Texture = AnimalTemplate.Texture;
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsDead == false)
            {
                if(TargetAnimalPen != null && TargetAnimalPen.IsFullOfManure())
                {
                    StatusEffectsManager.AddStatusEffect(StatusEffectId.DirtyAnimalPen);
                }
                else
                {
                    StatusEffectsManager.RemoveStatusEffect(StatusEffectId.DirtyAnimalPen);
                }

                if(CreatureStats.Hunger.IsDissatisfied())
                {
                    StatusEffectsManager.AddStatusEffect(StatusEffectId.Hunger);
                }
                else
                {
                    StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Hunger);
                }

                if (AnimalTemplate.AnimalProduct != null)
                {
                    if (readyToGatherProduct)
                    {
                        if (gatherProduct && (gatherProductLabor == null || gatherProductLabor.IsCompleted))
                        {
                            gatherProductLabor = new ActionWithAnimalLabor(LaborType.Ranching, this, ActionWithAnimal.GatherProduct);
                            GameplayScene.WorldManager.LaborManager.Add(gatherProductLabor);
                        }
                    }
                    else
                    {
                        float productivityModificator = CreatureStats.Productivity.CurrentValue / 100f;
                        ProductReadyPercent += (percentPerMinute * productivityModificator) * Engine.GameDeltaTime;
                        if (ProductReadyPercent >= 100)
                        {
                            readyToGatherProduct = true;
                            ProductReadyPercent = 100;
                        }
                    }
                }

                if (IsReserved == false)
                {
                    if (CreatureStats.Hunger.IsDissatisfied() && (CurrentLabor == null || CurrentLabor.LaborType != LaborType.Eat))
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
                        }
                    }

                    if (CreatureStats.Energy.IsDissatisfied() && (CurrentLabor == null ||
                        (CurrentLabor.LaborType != LaborType.Sleep && CurrentLabor.LaborType != LaborType.Eat)))
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
                        }
                    }

                    if (CurrentLabor == null)
                    {
                        CurrentLabor = LookForLabor(GameplayScene.WorldManager.LaborManager.LaborsByType);
                        if (CurrentLabor != null)
                        {
                            if(CurrentLabor.LaborType != LaborType.Eat &&
                                CurrentLabor.LaborType != LaborType.Waner &&
                                CurrentLabor.LaborType != LaborType.Sleep &&
                                CurrentLabor.LaborType != LaborType.Fertilization)
                            {
                                if (TargetAnimalPen != null)
                                {
                                    TargetAnimalPen.RemoveAnimal(this);
                                }
                            }

                            CurrentLabor.InitTasks(this);
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
        }

        public override void Render()
        {
            base.Render();

            if (indicator.Active)
                indicator.Render();
        }

        private Timer wanderTimer = new Timer();
        private int idleTime = 8;

        private int currentLaborIteration = 0;
        private int currentLaborTypeIteration = 0;

        public bool IsPregnant { get; set; } = false;
        public int PregnancyProgressInDays { get; set; } = 0;

        private void OnNextDayStarted(int day, Season season)
        {
            if (AnimalTemplate.PregnancyData != null)
            {
                if (IsPregnant)
                {
                    PregnancyProgressInDays++;

                    if (PregnancyProgressInDays >= AnimalTemplate.PregnancyData.DurationInDays)
                    {
                        GiveBirthToAChild();

                        PregnancyProgressInDays = 0;
                        IsPregnant = false;
                    }
                }
            }

            DaysUntilAging--;

            if (DaysUntilAging <= 0)
            {
                switch (AnimalTemplate.AgeState)
                {
                    case AgeState.Baby:
                        {
                            AnimalTemplate adultAnimalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(AnimalTemplate.Json.Replace("_baby", "_adult"));
                            Tile spawnTile = Movement.CurrentTile;
                            AnimalCmp adultAnimal = GameplayScene.Instance.SpawnAnimal(spawnTile.X, spawnTile.Y, adultAnimalTemplate, adultAnimalTemplate.DaysUntilAging);
                            if (IsDomesticated)
                            {
                                adultAnimal.GatherProduct = true;

                                if(IsInteractionMarked(InteractionType.Slaughter))
                                {
                                    adultAnimal.MarkInteraction(InteractionType.Slaughter);
                                }
                            }
                            else
                            {
                                adultAnimal.Domesticate = Domesticate;
                            }

                            Die(null, false);
                        }
                        break;
                    case AgeState.Adult:
                        {
                            AnimalTemplate oldAnimalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(AnimalTemplate.Json.Replace("_adult", "_old"));
                            Tile spawnTile = Movement.CurrentTile;
                            AnimalCmp oldAnimal = GameplayScene.Instance.SpawnAnimal(spawnTile.X, spawnTile.Y, oldAnimalTemplate, oldAnimalTemplate.DaysUntilAging);
                            if (IsDomesticated)
                            {
                                oldAnimal.MarkInteraction(InteractionType.Slaughter);
                            }
                            else
                            {
                                oldAnimal.Domesticate = Domesticate;
                            }

                            Die(null, false);
                        }
                        break;
                    case AgeState.Old:
                        {
                            if (IsDomesticated)
                            {
                                Die(Localization.GetLocalizedText("x_died_of_old_age", Name), true);
                            }
                            else
                            {
                                Die("", false);
                            }
                        }
                        break;
                }
            }
        }

        public void GiveBirthToAChild()
        {
            AnimalTemplate offspringAnimalTemplate = Calc.Random.Choose(AnimalTemplate.PregnancyData.OffspringTemplates);

            AnimalCmp child = GameplayScene.Instance.SpawnAnimal(Movement.CurrentTile.X, Movement.CurrentTile.Y, 
                offspringAnimalTemplate, offspringAnimalTemplate.DaysUntilAging);
        }

        public int HoursPassedFromLastFertilization { get; set; } = 0;
        public int NextFertilizationHoursSum { get; set; } = 0;
        public bool IsReadyToFertilization { get; set; } = false;

        private void OnNextHourStarted(int hour)
        {
            if(IsDomesticated && AgeState == AgeState.Adult && AnimalTemplate.Gender == Gender.M)
            {
                if (IsReadyToFertilization == false)
                {
                    HoursPassedFromLastFertilization++;

                    if (HoursPassedFromLastFertilization >= NextFertilizationHoursSum)
                    {
                        HoursPassedFromLastFertilization = 0;
                        IsReadyToFertilization = true;
                        NextFertilizationHoursSum = MyRandom.Range(WorldState.HOURS_PER_CYCLE, WorldState.HOURS_PER_CYCLE + WorldState.HOURS_PER_CYCLE / 2);
                    }
                }
            }
        }

        public void Fertilize(AnimalCmp fertilizator)
        {
            if (fertilizator != null)
            {
                fertilizator.IsReadyToFertilization = false;
            }
            IsPregnant = true;
            PregnancyProgressInDays = 0;
        }

        private Labor LookForLabor(Dictionary<LaborType, List<Labor>> laborsByType)
        {
            if (IsDomesticated && Parent == null)
            {
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

                if (animalFertilizationLabor.Check(this))
                {
                    return animalFertilizationLabor;
                }
            }

            if(AnimalTemplate.IsPet && Parent != null && Parent.GetRoomId() == GetRoomId())
            {
                if(Parent.CurrentLabor != null && Parent.CurrentLabor.LaborType == LaborType.Hunt)
                {
                    if (Parent.CurrentLabor is SettlerHuntLabor)
                    {
                        SettlerHuntLabor parentHuntLabor = Parent.CurrentLabor as SettlerHuntLabor;

                        AnimalCmp parentTargetAnimal = parentHuntLabor.GetTasks(Parent)
                            .Where(x => x is SettlerHuntTask)
                            .Select(x => (x as SettlerHuntTask).TargetAnimal)
                            .First();

                        AnimalHuntLabor huntLabor = new AnimalHuntLabor(parentTargetAnimal);

                        if (huntLabor.Check(this))
                        {
                            GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.BEST_FRIENDS_FOREVER);

                            return huntLabor;
                        }
                    }
                    else if(Parent.CurrentLabor is AttackLabor)
                    {
                        AttackLabor parentAttackLabor = Parent.CurrentLabor as AttackLabor;

                        AttackLabor attackLabor = new AttackLabor(parentAttackLabor.TargetCreature);

                        if(attackLabor.Check(this))
                        {
                            attackLabor.CreateTasks(this);
                            attackLabor.InitTasks(this);

                            GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.BEST_FRIENDS_FOREVER);

                            return attackLabor;
                        }
                    }
                }
            }

            if (wanderTimer.GetTime() > idleTime && wanderLabor.Check(this))
            {
                wanderTimer.Reset();
                return wanderLabor;
            }

            return null;
        }

        private void NextLaborType()
        {
            currentLaborTypeIteration++;
            if (currentLaborTypeIteration >= SortedLaborTypes.Count)
            {
                currentLaborTypeIteration = 0;
            }
        }

        public override float TakeDamage(CreatureCmp damager, float damage)
        {
            float damageValue = base.TakeDamage(damager, damage);

            WasAttacked = true;

            if(MyRandom.ProbabilityChance(AnimalTemplate.AttackChance))
            {
                if (CurrentLabor == null || CurrentLabor.LaborType != LaborType.Hunt)
                {
                    if (CurrentLabor != null)
                    {
                        CurrentLabor.CancelAndClearTasks(this);
                        CurrentLabor = null;
                        CurrentTask = null;
                    }

                    AnimalHuntLabor animalHuntLabor = new AnimalHuntLabor(damager);
                    CurrentLabor = animalHuntLabor;
                    CurrentLabor.Check(this);
                    CurrentLabor.InitTasks(this);
                }
            }
            else
            {
                if (CurrentLabor == null || CurrentLabor.LaborType != LaborType.Hide)
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

            return damageValue;
        }

        public AnimalCmp TryToDomesticate(CreatureCmp creature)
        {
            Domesticate = false;

            int domesticationChance = AnimalTemplate.DomesticationData.Chance;

            if(MyRandom.ProbabilityChance(domesticationChance))
            {
                return TurnToDomesticated();
            }

            CreatureThoughts?.AddThought("nevermind", 3);

            return null;
        }

        public AnimalCmp TurnToDomesticated()
        {
            AnimalTemplate tamedAnimalTemplate = AnimalTemplate.DomesticationData.TamedFormAnimalTemplate;
            Tile spawnTile = Movement.CurrentTile;
            Die(null, false);
            return GameplayScene.Instance.SpawnAnimal(spawnTile.X, spawnTile.Y, tamedAnimalTemplate, DaysUntilAging);
        }

        public void TryToGatherProduct()
        {
            readyToGatherProduct = false;
            ProductReadyPercent = 0;

            Item product = AnimalTemplate.AnimalProduct.Product;
            Movement.CurrentTile.Inventory.AddCargo(new ItemContainer(product, 1, product.Durability));
        }

        public override void Die(string reasonMessage, bool throwLoot = true)
        {
            indicator.Active = false;

            GameplayScene.Instance.WorldState.OnNextDayStartedCallback -= OnNextDayStarted;
            GameplayScene.Instance.WorldState.NextHourStarted -= OnNextHourStarted;

            base.Die(reasonMessage);

            GatherProduct = false;
            Domesticate = false;

            if(TargetAnimalPen != null)
            {
                TargetAnimalPen.RemoveAnimal(this);
            }

            if (throwLoot)
            {
                Tile tile = Movement.CurrentTile;
                foreach (var kvp in AnimalTemplate.Drop)
                {
                    Item item = kvp.Key;
                    int count = kvp.Value;

                    tile.Inventory.AddCargo(new ItemContainer(item, count, item.Durability));
                }
            }

            if(AnimalTemplate.IsWild)
            {
                GameplayScene.WorldManager.WildAnimalsNumber--;
            }
            else
            {
                if (string.IsNullOrEmpty(reasonMessage) == false)
                {
                    GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                        .AddNotification(reasonMessage, NotificationLevel.INFO, Entity);
                }
            }
        }

        public override string GetInformation()
        {
            string baseInfo = base.GetInformation();

            switch(AnimalTemplate.AgeState)
            {
                case AgeState.Baby:
                case AgeState.Adult:
                    baseInfo += $"\n{Localization.GetLocalizedText("days_until_aging_x", DaysUntilAging)}";
                    break;
                case AgeState.Old:
                    baseInfo += $"\n{Localization.GetLocalizedText("days_until_death_x", DaysUntilAging)}";
                    break;
            }

            PregnancyData pregnancyData = AnimalTemplate.PregnancyData;

            if (pregnancyData != null)
            {
                if (IsPregnant)
                {
                    baseInfo += $"\n{Localization.GetLocalizedText("pregnancy")}: {PregnancyProgressInDays}/{pregnancyData.DurationInDays} " +
                        $"{Localization.GetLocalizedText("days")}";
                }
            }

            if (AnimalTemplate.AnimalProduct != null)
            {
                baseInfo += $"\n{AnimalTemplate.AnimalProduct.Product.Name}: {(int)ProductReadyPercent}%";
            }

            baseInfo += $"\n{Localization.GetLocalizedText("food")}: ";

            int count = 0;
            foreach (var item in FoodRation.GetAllowedRation())
            {
                if (count == FoodRation.Count - 1)
                    baseInfo += $"{item.Name}";
                else
                    baseInfo += $"{item.Name}, ";
                count++;
            }

            if (AnimalTemplate.IsPet && Parent != null)
            {
                baseInfo += $"\n{Localization.GetLocalizedText("assigned_to", Parent.Name)}";
            }

            return baseInfo;
        }

        public override CreatureSaveData GetSaveData()
        {
            CreatureSaveData creatureSaveData = base.GetSaveData();
            creatureSaveData.Id = Id;

            if(Parent != null)
            {
                creatureSaveData.ParentId = Parent.Id;
            }

            if(Child != null)
            {
                creatureSaveData.ChildId = Child.Id;
            }

            creatureSaveData.X = Movement.CurrentTile.X;
            creatureSaveData.Y = Movement.CurrentTile.Y;
            creatureSaveData.Name = Name;
            creatureSaveData.HairColor = GetHairColor();

            creatureSaveData.Ration = new Dictionary<int, bool>();
            foreach (var kvp in FoodRation.GetFilters())
            {
                creatureSaveData.Ration.Add(kvp.Key.Id, kvp.Value);
            }

            creatureSaveData.OutfitId = CreatureEquipment.ClothingItemContainer == null ? -1 : CreatureEquipment.ClothingItemContainer.Item.Id;
            if (creatureSaveData.OutfitId != -1)
            {
                creatureSaveData.OutfitFactWeight = CreatureEquipment.ClothingItemContainer.FactWeight;
                creatureSaveData.OutfitDurability = CreatureEquipment.ClothingItemContainer.Durability;
            }

            creatureSaveData.TopOutfitId = CreatureEquipment.TopClothingItemContainer == null ? -1 : CreatureEquipment.TopClothingItemContainer.Item.Id;
            if (creatureSaveData.TopOutfitId != -1)
            {
                creatureSaveData.TopOutfitFactWeight = CreatureEquipment.TopClothingItemContainer.FactWeight;
                creatureSaveData.TopOutfitDurability = CreatureEquipment.TopClothingItemContainer.Durability;
            }

            creatureSaveData.LaborTypePriorityPair = new Dictionary<string, int>();
            foreach (var kvp in LaborTypePriorityPair)
            {
                creatureSaveData.LaborTypePriorityPair.Add(kvp.Key.ToString(), kvp.Value);
            }

            creatureSaveData.InventoryItems = new List<Tuple<int, int, float>>();
            foreach (var kvp in Inventory.ItemItemContainerPair)
            {
                List<ItemContainer> itemContainers = kvp.Value;

                foreach (var itemContainer in itemContainers)
                {
                    creatureSaveData.InventoryItems.Add(Tuple.Create(itemContainer.Item.Id, itemContainer.FactWeight, itemContainer.Durability));
                }
            }

            creatureSaveData.Hunger = CreatureStats.Hunger.CurrentValue;
            creatureSaveData.Health = CreatureStats.Health.CurrentValue;
            creatureSaveData.Energy = CreatureStats.Energy.CurrentValue;
            creatureSaveData.Temperature = CreatureStats.Temperature.CurrentValue;
            creatureSaveData.Happiness = CreatureStats.Happiness.CurrentValue;
            creatureSaveData.CreatureType = CreatureType;

            creatureSaveData.DaysUntilAging = DaysUntilAging;

            creatureSaveData.Hunt = Hunt;
            creatureSaveData.Domesticate = Domesticate;
            creatureSaveData.GatherProduct = GatherProduct;
            creatureSaveData.WasAttacked = WasAttacked;
            creatureSaveData.ProductReadyPercent = ProductReadyPercent;
            creatureSaveData.AnimalTemplateName = AnimalTemplate.Json;

            creatureSaveData.IsPregnant = IsPregnant;
            creatureSaveData.PregnancyProgressInDays = PregnancyProgressInDays;

            creatureSaveData.HoursPassedFromLastFertilization = HoursPassedFromLastFertilization;
            creatureSaveData.NextFertilizationHoursSum = NextFertilizationHoursSum;
            creatureSaveData.IsReadyToFertilization = IsReadyToFertilization;

            creatureSaveData.StatusEffects = new Dictionary<StatusEffectId, float>();
            foreach (var statusEffect in StatusEffectsManager.GetStatusEffects())
            {
                if (statusEffect.IsPassive)
                    continue;

                creatureSaveData.StatusEffects.Add(statusEffect.Id, statusEffect.Progress);
            }

            return creatureSaveData;
        }
    }
}
