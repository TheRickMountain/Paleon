using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class UnitCommandUIScript : MScript
    {

        private BigButton moveToButton;
        private BigButton attackButton;
        private BigButton disbandButton;

        private ListViewUIScript listViewScript;

        private Tile targetTile;
        private AnimalCmp targetAnimal;

        public UnitCommandUIScript() : base(false)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            moveToButton = new BigButton(ParentNode.Scene, ResourceManager.MoveToIcon, false);
            moveToButton.ButtonScript.SetDefaultColor(Color.Cyan, Color.Cyan, Color.Cyan);
            moveToButton.Tooltips = Localization.GetLocalizedText("come_here");
            moveToButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnMoveToButtonPressed);

            attackButton = new BigButton(ParentNode.Scene, ResourceManager.AttackIcon, false);
            attackButton.ButtonScript.SetDefaultColor(Color.Cyan, Color.Cyan, Color.Cyan);
            attackButton.Tooltips = Localization.GetLocalizedText("attack");
            attackButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnAttackButtonPressed);

            disbandButton = new BigButton(ParentNode.Scene, ResourceManager.DisbandIcon, false);
            disbandButton.ButtonScript.SetDefaultColor(Color.Cyan, Color.Cyan, Color.Cyan);
            disbandButton.Tooltips = Localization.GetLocalizedText("disband");
            disbandButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnDisbandButtonPressed);

            listViewScript = ParentNode.GetComponent<ListViewUIScript>();
        }

        public void Open(Tile tile)
        {
            listViewScript.Clear();

            int buttonsAmount = 0;

            if (tile.IsWalkable)
            {
                targetTile = tile;
                listViewScript.AddItem(moveToButton);
                buttonsAmount++;
            }

            foreach(var selectableCmp in GameplayScene.WorldManager.GetCreature((int)GameplayScene.MouseWorldPosition.X, (int)GameplayScene.MouseWorldPosition.Y))
            {
                if(selectableCmp.SelectableType == SelectableType.Animal)
                {
                    AnimalCmp animal = selectableCmp.Entity.Get<AnimalCmp>();
                    if(animal != null && animal.IsDomesticated == false)
                    {
                        targetAnimal = animal;
                        listViewScript.AddItem(attackButton);
                        buttonsAmount++;
                    }
                }
            }

            listViewScript.AddItem(disbandButton);

            if (buttonsAmount > 0)
            {
                ParentNode.Width = buttonsAmount * moveToButton.Height + (buttonsAmount - 1) * 5;
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private void OnMoveToButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.MoveSelectedSettlersToTile(targetTile);

            GameplayScene.UIRootNodeScript.CloseUnitCommandUI();
        }

        private void OnDisbandButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.DisbandSelectedSettlers();

            GameplayScene.UIRootNodeScript.CloseUnitCommandUI();
        }

        private void OnAttackButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.WorldManager.AttackCreatureWithSelectedSettlers(targetAnimal);

            GameplayScene.UIRootNodeScript.CloseUnitCommandUI();
        }
    }
}
