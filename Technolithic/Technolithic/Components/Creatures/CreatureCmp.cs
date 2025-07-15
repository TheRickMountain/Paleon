using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum MovementState
    {
        Running,
        Success,
        Failed,
        Completion
    }

    public enum CreatureType
    {
        Settler,
        Animal
    }

    public enum Gender
    {
        F,
        M
    }

    public enum AgeState
    {
        Baby = 0,
        Adult = 1,
        Old = 2
    }

    public abstract class CreatureCmp : Interactable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public CreatureCmp Parent { get; set; }
        public CreatureCmp Child { get; set; }

        public HutBuildingCmp AssignedHut { get; set; }

        public HutBuildingCmp EnteredBuilding { get; set; }

        public Slider Slider { get; private set; }
        private Slider rechargeSlider;
        public CreatureStats CreatureStats { get; private set; }

        public bool IsHidden { get; set; }

        public Labor CurrentLabor { get; protected set; }

        public MovementCmp Movement { get; set; }

        public Inventory Inventory { get; private set; }
        public CreatureEquipment CreatureEquipment { get; private set; }

        public CreatureThoughts CreatureThoughts { get; private set; }

        public float Speed { get; private set; }

        public bool IsDead { get; private set; }

        public CreatureRation FoodRation { get; private set; }
        public CreatureRation BeverageRation { get; private set; }

        public CreatureType CreatureType { get; private set; }

        public Action<CreatureCmp> OnCreatureDieCallback { get; set; }   

        public bool IsDomesticated { get; protected set; }

        public Direction CurrentDirection { get; set; } = Direction.LEFT;

        public virtual AgeState AgeState { get; protected set; }

        public bool CanBeRenamed { get; set; } = false;

        protected Dictionary<LaborType, int> LaborTypePriorityPair { get; private set; }
        protected List<LaborType> SortedLaborTypes { get; private set; }
        protected List<LaborType> AllowedLabors { get; private set; }

        public Sprite CargoImage { get; private set; }
        protected Sprite BodyImage { get; private set; }
        protected Sprite HairImage { get; private set; }

        protected Task CurrentTask { get; set; }

        protected AnimatedSprite AttackSprite;

        private Entity bodyEntity;

        private Vector2 knockbackOffset;

        private Timer attackRechargeTimer;
        private float rechargeTime;

        private bool canAttack = true;

        public StatusEffectsManager StatusEffectsManager { get; private set; }

        public CreatureCmp(MyTexture bodyTexture, MyTexture hairTexture, string name, CreatureStats stats, float moveSpeed, CreatureType creatureType) : base(true, true)
        {
            Id = Guid.NewGuid();

            Name = name;
            CreatureStats = stats;
            Speed = moveSpeed;
            CreatureType = creatureType;
            FoodRation = new CreatureRation();
            BeverageRation = new CreatureRation();
            
            CargoImage = new Sprite(RenderManager.Pixel, 16, 16);
            CargoImage.Origin = new Vector2(8, 8);
            CargoImage.X = 8;
            CargoImage.Y = 8;
            CargoImage.Visible = false;

            LaborTypePriorityPair = new Dictionary<LaborType, int>();
            SortedLaborTypes = new List<LaborType>();

            foreach (LaborType laborType in Labor.GetWorkLaborEnumerator())
            {
                LaborTypePriorityPair.Add(laborType, 2);
                SortedLaborTypes.Add(laborType);
            }

            AllowedLabors = new List<LaborType>();

            Slider = new Slider(16, 4, Color.Black, Color.Orange);
            Slider.Active = false;

            rechargeSlider = new Slider(16, 2, Color.Black, Color.Cyan);
            rechargeSlider.Active = false;

            Inventory = new Inventory(this);
            Inventory.OnItemAddedCallback += ChangeCargoView;
            Inventory.OnItemRemovedCallback += ChangeCargoView;

            CreatureEquipment = new CreatureEquipment(
                CreatureStats.NativeDefense, 
                CreatureStats.NativeMeleeDamage,
                CreatureStats.NativeRechargeTime);

            CreatureEquipment.EquipmentChanged += OnCreatureEquipmentChanged;

            bodyEntity = new Entity();

            if (bodyTexture != null)
            {
                BodyImage = new Sprite(bodyTexture, 24, 24, true);
                BodyImage.CenterOrigin();
                BodyImage.X = 8;
                BodyImage.Y = 4;
                bodyEntity.Add(BodyImage);
            }

            if (hairTexture != null)
            {
                HairImage = new Sprite(hairTexture, 24, 24, true);
                HairImage.CenterOrigin();
                HairImage.X = 8;
                HairImage.Y = 4;
                bodyEntity.Add(HairImage);
            }

            CreatureEquipment.InitializeSprites(bodyEntity);

            MyTexture fightTexture = ResourceManager.GetTexture("fight");

            AttackSprite = new AnimatedSprite(16, 24);
            AttackSprite.Add("Weapon", new Animation(fightTexture, 5, 0, 16, 24, 0, 0, 20));
            AttackSprite.Add("Teeth", new Animation(fightTexture, 5, 0, 16, 24, 0, 24, 12));
            AttackSprite.Add("Claws", new Animation(fightTexture, 5, 0, 16, 24, 0, 48, 12));
            AttackSprite.Add("Horns", new Animation(fightTexture, 5, 0, 16, 24, 0, 72, 12));
            AttackSprite.Active = false;

            attackRechargeTimer = new Timer();

            StatusEffectsManager = new StatusEffectsManager();
        }

        public override void Awake()
        {
            
        }

        public override void Begin()
        {
            base.Begin();

            ChangeDirection(CurrentDirection);

            CreatureThoughts = Entity.Get<CreatureThoughts>();
            Movement = Entity.Get<MovementCmp>();
            Movement.RegisterOnDirectionChangedCallback(ChangeDirection);
            Movement.Speed = Speed;

            CargoImage.Entity = Entity;
            AttackSprite.Entity = Entity;
        }

        public override void Update()
        {
            if (knockbackOffset.X != 0)
            {
                if (knockbackOffset.X > 0)
                {
                    knockbackOffset.X = Math.Max(0, knockbackOffset.X - 50 * Engine.GameDeltaTime);
                }
                else if (knockbackOffset.X < 0)
                {
                    knockbackOffset.X = Math.Min(0, knockbackOffset.X + 50 * Engine.GameDeltaTime);
                }
                else
                {
                    knockbackOffset.X = 0;
                }
            }

            if (knockbackOffset.Y != 0)
            {
                if (knockbackOffset.Y > 0)
                {
                    knockbackOffset.Y = Math.Max(0, knockbackOffset.Y - 50 * Engine.GameDeltaTime);
                }
                else if (knockbackOffset.Y < 0)
                {
                    knockbackOffset.Y = Math.Min(0, knockbackOffset.Y + 50 * Engine.GameDeltaTime);
                }
                else
                {
                    knockbackOffset.Y = 0;
                }
            }

            bodyEntity.X = Entity.X + knockbackOffset.X;
            bodyEntity.Y = Entity.Y + knockbackOffset.Y;

            if (IsDead == false)
            {
                StatusEffectsManager.Update(Engine.GameDeltaTime);

                if(GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
                {
                    StatusEffectsManager.AddStatusEffect(StatusEffectId.WinterCold);
                }
                else
                {
                    StatusEffectsManager.RemoveStatusEffect(StatusEffectId.WinterCold);
                }

                if(CreatureEquipment.TopClothingItemContainer != null)
                {
                    StatusEffectsManager.AddStatusEffect(StatusEffectId.WarmClothing);
                }
                else
                {
                    StatusEffectsManager.RemoveStatusEffect(StatusEffectId.WarmClothing);
                }

                CreatureEquipment.UpdateDurability(Engine.GameDeltaTime);

                Movement.Speed = StatusEffectsManager.CalculateMovementSpeed(Speed);

                if (AttackSprite.Active)
                    AttackSprite.Update();

                if (Movement.CurrentTile.Room == null)
                {
                    if (CurrentLabor != null)
                    {
                        CurrentLabor.CancelAndClearTasks(this);
                        CurrentLabor = null;
                        CurrentTask = null;
                    }

                    MChunk currentChunk = Movement.CurrentTile.Chunk;
                    Tile walkableTile = null;
                    foreach(var room in currentChunk.Rooms)
                    {
                        foreach(var tile in room.Tiles)
                        {
                            walkableTile = tile;
                            break;
                        }
                    }

                    TilePath  path = PathAStar.CreateStrengthPath(Movement.CurrentTile, walkableTile);

                    foreach(Tile targetTile in path.Tiles)
                    {
                        if(targetTile.IsWalkable)
                        {
                            Movement.Teleport(targetTile);
                            break;
                        }
                    }
                }

                for (int i = 0; i < CreatureStats.Attribures.Count; i++)
                {
                    MAttribute status = CreatureStats.Attribures[i];
                    if (status.Active == false)
                        continue;

                    if(StatusEffectsManager.TotalAttributesChanges.ContainsKey(status.AttributeType))
                    {
                        status.ExtraChangePerDay = StatusEffectsManager.TotalAttributesChanges[status.AttributeType];
                    }

                    status.CurrentValue += (status.ChangePerSecond + status.ExtraChangePerSecond) * Engine.GameDeltaTime;

                    if (status.DeadlyIfLower && status.CurrentValue <= 0)
                    {
                        Die(Localization.GetLocalizedText(status.DeadMessage, Name));
                        break;
                    }

                    if (status.DeadlyIfMore && status.CurrentValue >= status.MaxValue)
                    {
                        Die(Localization.GetLocalizedText(status.DeadMessage, Name));
                        break;
                    }
                }

                if (CreatureStats.Hunger.Active && CreatureStats.Hunger.IsDissatisfied())
                {
                    CreatureThoughts.AddThought("hunger", 4);
                }

                if (CreatureStats.Happiness.Active && CreatureStats.Happiness.IsDissatisfied())
                {
                    CreatureThoughts.AddThought("unhappy", 4);
                }

                if (CreatureStats.Energy.Active && CreatureStats.Energy.IsDissatisfied())
                {
                    CreatureThoughts.AddThought("sleep", 4);
                }

                if (CreatureStats.Health.Active && CreatureStats.Health.IsDissatisfied())
                {
                    CreatureThoughts.AddThought("health", 4);
                }

                if(CreatureStats.Temperature.Active && CreatureStats.Temperature.IsDissatisfied())
                {
                    CreatureThoughts.AddThought("cold", 4);
                }

                if (canAttack == false)
                {
                    float currentRechargeTime = attackRechargeTimer.GetTime();

                    if (currentRechargeTime > rechargeTime)
                    {
                        canAttack = true;
                        attackRechargeTimer.Reset();
                        AttackSprite.Active = false;
                        rechargeSlider.Active = false;
                    }
                    else
                    {
                        rechargeSlider.SetValue(0, rechargeTime, currentRechargeTime, Color.Cyan);
                    }

                    if (AttackSprite.CurrentAnimation != null && AttackSprite.CurrentAnimation.IsLastFrame)
                    {
                        AttackSprite.Active = false;
                        AttackSprite.CurrentAnimation.Reset();
                    }
                }
            }
        }

        public bool IsLaborAllowed(LaborType laborType)
        {
            return AllowedLabors.Contains(laborType);
        }

        public void SetLaborPriority(LaborType labor, int value)
        {
            LaborTypePriorityPair[labor] = value;

            SortedLaborTypes = LaborTypePriorityPair.OrderByDescending(x => x.Value).Where(x => x.Value > -1).Select(x => x.Key).ToList();

            if(value == -1)
            {
                if (CreatureEquipment.HasTool(labor))
                {
                    ItemContainer toolItemContainer = CreatureEquipment.TryGetTool(labor);

                    Tool tool = toolItemContainer.Item.Tool;

                    // Проверяем, все ли типы работ заблокированы для этого инструмента, чтобы его выкинуть

                    bool allLaborTypesForbidden = true;

                    for (int i = 0; i < tool.LaborTypes.Length; i++)
                    {
                        LaborType checkLaborType = tool.LaborTypes[i];

                        if (GetLaborPriority(checkLaborType) != -1)
                        {
                            allLaborTypesForbidden = false;
                            break;
                        }
                    }

                    if (allLaborTypesForbidden)
                    {
                        if (Movement != null && Movement.CurrentTile != null)
                        {
                            CreatureEquipment.ThrowTool(tool.ToolType, Movement.CurrentTile);
                        }
                    }
                }

                if (CurrentLabor != null && CurrentLabor.LaborType == labor)
                {
                    CancelLabor();
                }
            }
        }

        public int GetLaborPriority(LaborType labor)
        {
            return LaborTypePriorityPair[labor];
        }

        public virtual float TakeDamage(CreatureCmp damager, float damage)
        {
            if (!IsDead)
            {
                if (damager != null)
                {
                    Vector2 difference = damager.Entity.Position - Entity.Position;
                    difference.Normalize();
                    knockbackOffset = new Vector2(-difference.X * 10, -difference.Y * 10);
                }

                damage = Math.Max(1, damage - CreatureEquipment.Defense);

                GameplayScene.Instance.MessageManager.ShowMessage("-" + damage, Entity.Position - new Vector2(0, 30));

                MAttribute health = CreatureStats.Health;

                health.CurrentValue -= damage;

                if(CreatureEquipment.ClothingItemContainer != null)
                {
                    CreatureEquipment.ClothingItemContainer.Durability--;
                    if(CreatureEquipment.ClothingItemContainer.Durability <= 0)
                    {
                        CreatureEquipment.ClothingItemContainer = null;
                    }
                }

                if(CreatureEquipment.TopClothingItemContainer != null)
                {
                    CreatureEquipment.TopClothingItemContainer.Durability--;
                    if(CreatureEquipment.TopClothingItemContainer.Durability <= 0)
                    {
                        CreatureEquipment.TopClothingItemContainer = null;
                    }
                }

                if (health.CurrentValue <= health.MinValue)
                {
                    Die(Localization.GetLocalizedText("was_killed_by", Name, damager.Name));
                }

                return damage;
            }

            return 0.0f;
        }

        public virtual void FallAsleep(bool value)
        {
            if (CreatureStats.IsAsleep == value)
                return;

            CreatureStats.IsAsleep = value;
        }

        public override void Render()
        {
            if (BodyImage != null && BodyImage.Active)
                BodyImage.Render();

            if (HairImage != null && HairImage.Active)
                HairImage.Render();

            CreatureEquipment.Render();

            if (CargoImage.Visible)
                CargoImage.TestRender();

            if (Slider.Active)
            {
                Slider.Position = new Vector2(Entity.Position.X, Entity.Position.Y - 15);
                Slider.Render();
            }

            if(rechargeSlider.Active)
            {
                rechargeSlider.Position = new Vector2(Entity.Position.X, Entity.Position.Y - 13);
                rechargeSlider.Render();
            }
        }

        public void DoMeleeAttack(CreatureCmp target)
        {
            if (canAttack)
            {
                rechargeTime = CreatureEquipment.RechargeTime;
                rechargeSlider.Active = true;

                canAttack = false;

                AttackSprite.Active = true;

                AttackSprite.Play("Weapon");

                Vector2 difference = target.Entity.Position - Entity.Position;
                difference.Normalize();
                knockbackOffset = new Vector2(difference.X * 10, difference.Y * 10);

                target.TakeDamage(this, CreatureEquipment.MeleeDamage);

                if(CreatureEquipment.HasTool(ToolType.HuntingMelee))
                {
                    CreatureEquipment.DegradeTool(ToolType.HuntingMelee, 1);
                }
            }
        }

        public void DoRangeAttack(CreatureCmp target)
        {
            if (canAttack)
            {
                rechargeTime = CreatureEquipment.RechargeTime;
                rechargeSlider.Active = true;

                canAttack = false;

                Tool tool = CreatureEquipment.TryGetTool(ToolType.HuntingRange).Item.Tool;

                GameplayScene.WorldManager.AddAttackInfo(new AttackInfo(this, target, tool));

                CreatureEquipment.DegradeTool(ToolType.HuntingRange, 1);
            }
        }

        public void LookAt(CreatureCmp target)
        {
            if (target.Movement.CurrentTile.X > Movement.CurrentTile.X)
                ChangeDirection(Direction.RIGHT);
            else
                ChangeDirection(Direction.LEFT);
        }

        protected virtual void ChangeDirection(Direction direction)
        {
            CurrentDirection = direction;

            if (direction == Direction.LEFT)
            {
                if (BodyImage != null && BodyImage.Active)
                    BodyImage.FlipX = false;

                if (HairImage != null && HairImage.Active)
                    HairImage.FlipX = false;

                CreatureEquipment.UpdateSpritesPositionings(BodyImage.X + BodyImage.Width, BodyImage.Y, false);

                AttackSprite.FlipX = true;
                AttackSprite.X = -10;
            }
            else if (direction == Direction.RIGHT)
            {
                if (BodyImage != null && BodyImage.Active)
                    BodyImage.FlipX = true;

                if (HairImage != null && HairImage.Active)
                    HairImage.FlipX = true;

                CreatureEquipment.UpdateSpritesPositionings(BodyImage.X, BodyImage.Y, true);

                AttackSprite.FlipX = false;
                AttackSprite.X = 10;
            }
        }

        private void ChangeCargoView(Inventory senderInventory, Item item, int weight)
        {
            Item itemToShow = null;
            foreach(var kvp in Inventory.ItemItemContainerPair)
            {
                int factWeight = Inventory.GetInventoryFactWeight(kvp.Key);
                if(factWeight > 0)
                {
                    itemToShow = kvp.Key;
                    break;
                }
            }

            if (itemToShow != null)
            {
                CargoImage.Texture = itemToShow.Icon;
                CargoImage.Visible = true;
            }
            else
            {
                CargoImage.Visible = false;
            }
        }

        public virtual bool SetLabor(Labor labor)
        {
            if (labor.Check(this) == false)
                return false;

            CancelLabor();

            CurrentLabor = labor;
            CurrentLabor.CreateTasks(this);
            CurrentLabor.InitTasks(this);

            return true;
        }

        public void CancelLabor()
        {
            if (CurrentLabor != null)
            {
                CurrentLabor.CancelAndClearTasks(this);
                CurrentLabor = null;
                CurrentTask = null;
            }
        }

        public virtual void Die(string reasonMessage, bool throwLoot = true)
        {
            IsDead = true;

            // Если существо находилось в здании, то выйти оттуда
            if (EnteredBuilding != null)
            {
                EnteredBuilding.Exit(this);
            }

            AssignedHut?.UnassignCreature(this);

            Entity.RemoveSelf();

            if (CurrentLabor != null)
            {
                CurrentLabor.CancelAndClearTasks(this);
                CurrentLabor = null;
                CurrentTask = null;
            }

            Inventory.ThrowCargo(Movement.CurrentTile);

            CreatureEquipment.ThrowAllTools(Movement.CurrentTile);
            CreatureEquipment.ThrowClothing(Movement.CurrentTile);
            CreatureEquipment.ThrowTopClothing(Movement.CurrentTile);

            if(Parent != null)
            {
                Parent.Child = null;
                Parent = null;
            }

            if(Child != null)
            {
                Child.Parent = null;
                Child = null;
            }

            Inventory.OnItemAddedCallback -= ChangeCargoView;
            Inventory.OnItemRemovedCallback -= ChangeCargoView;

            CreatureEquipment.EquipmentChanged -= OnCreatureEquipmentChanged;

            Movement.UnregisterOnDirectionChangedCallback(ChangeDirection);

            OnCreatureDieCallback?.Invoke(this);
        }

        public void WakeUp()
        {
            if (CurrentLabor != null && (CurrentLabor.LaborType == LaborType.Sleep || CurrentLabor.LaborType == LaborType.Heal))
            {
                CurrentLabor.CancelAndClearTasks(this);
                CurrentLabor = null;
                CurrentTask = null;
            }
        }

        public void OnCreatureEquipmentChanged(Item item)
        {
            if(item != null)
            {
                ChangeDirection(CurrentDirection);
            }
        }

        public MyTexture GetBodyIcon()
        {
            return BodyImage.Texture;
        }

        public MyTexture GetHairIcon()
        {
            if (HairImage == null)
                return null;

            return HairImage.Texture;
        }

        public Color GetHairColor()
        {
            if (HairImage == null)
                return Color.White;

            return HairImage.Color;
        }

        public virtual string GetInformation()
        {
            return $"";
        }

        public int GetRoomId()
        {
            return Movement.CurrentTile.GetRoomId();
        }

        public override Tile GetApproachableTile(CreatureCmp creature)
        {
            return GetApproachableTile(creature.Movement.CurrentTile.GetRoomId());
        }

        public override Tile GetApproachableTile(int zoneId)
        {
            if (Movement.CurrentTile.GetRoomId() != zoneId) return null;

            if (Movement.CurrentTile.IsWalkable == false) return null;

            return Movement.CurrentTile;
        }

        public override IEnumerable<Tile> GetApproachableTiles()
        {
            if(Movement.CurrentTile.IsWalkable)
            {
                yield return Movement.CurrentTile;
            }
        }

        public virtual CreatureSaveData GetSaveData()
        {
            CreatureSaveData creatureSaveData = new CreatureSaveData();

            creatureSaveData.Tools = new List<Tuple<int, int, float>>();

            foreach(ItemContainer toolItemContainer in CreatureEquipment.GetTools())
            {
                int itemId = toolItemContainer.Item.Id;
                int itemAmount = toolItemContainer.FactWeight;
                float itemDurability = toolItemContainer.Durability;

                creatureSaveData.Tools.Add(new Tuple<int, int, float>(itemId, itemAmount, itemDurability));
            }

            return creatureSaveData;
        }
    }
}
