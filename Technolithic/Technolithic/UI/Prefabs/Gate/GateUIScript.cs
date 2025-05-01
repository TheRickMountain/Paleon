using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class GateUIScript : MScript
    {
        private Dictionary<MNode, GateState> gateStateNodePair;

        private GateCmp gate;

        public GateUIScript() : base(true)
        {
            gateStateNodePair = new Dictionary<MNode, GateState>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            gateStateNodePair.Add(CreateListElementNode(Localization.GetLocalizedText("open")), GateState.Opened);
            gateStateNodePair.Add(CreateListElementNode(Localization.GetLocalizedText("close")), GateState.Closed);

            int count = 0;
            foreach (var kvp in gateStateNodePair)
            {
                MNode node = kvp.Key;
                node.X = 8;
                node.Y = 38 + count * (node.Height + 5);
                ParentNode.AddChildNode(node);

                count++;
            }
        }

        public void Open(GateCmp gate)
        {
            this.gate = gate;

            UpdateToggles();
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private MNode CreateListElementNode(string name)
        {
            MNode element = new MNode(ParentNode.Scene);

            MToggleUI toggleUI = new MToggleUI(ParentNode.Scene, true, true);
            toggleUI.Name = "Toggle";
            toggleUI.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetToggle);

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = name;
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = toggleUI.LocalX + toggleUI.Width + 5;

            element.Width = toggleUI.LocalX + itemName.LocalX + itemName.Width;
            element.Height = toggleUI.Height;

            element.AddChildNode(toggleUI);
            element.AddChildNode(itemName);

            return element;
        }

        private void SetToggle(bool value, MToggleUI toggleUI)
        {
            GateState state = gateStateNodePair[toggleUI.ParentNode];

            gate.SetState(state);

            UpdateToggles();
        }

        private void UpdateToggles()
        {
            foreach(var kvp in gateStateNodePair)
            {
                MNode node = kvp.Key;
                GateState state = kvp.Value;

                ToggleScript toggleScript = node.GetChildByName("Toggle").GetComponent<ToggleScript>();

                if (gate.State == state)
                {
                    toggleScript.SilentCheck(true);
                }
                else
                {
                    toggleScript.SilentCheck(false);
                }
            }
        }
    }
}
