using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Technolithic
{
    public class StatusEffectsUIScript : MScript
    {
        private Dictionary<StatusEffect, MNode> effectsNodes;
        private Dictionary<MNode, StatusEffect> nodesEffects;

        public StatusEffectsUIScript() : base(true)
        {
            effectsNodes = new Dictionary<StatusEffect, MNode>();
            nodesEffects = new Dictionary<MNode, StatusEffect>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public void Open(StatusEffectsManager creatureEffectsManager)
        {
            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            foreach (var effect in creatureEffectsManager.GetStatusEffects())
            {
                if (!effectsNodes.ContainsKey(effect))
                {
                    MNode effectNode = CreateEffectNode(effect);
                    effectsNodes.Add(effect, effectNode);
                    nodesEffects.Add(effectNode, effect);
                }

                listViewScript.AddItem(effectsNodes[effect]);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private MNode CreateEffectNode(StatusEffect statusEffect)
        {
            MyText effectName = new MyText(ParentNode.Scene);
            effectName.Text = "- " + statusEffect.Name;
            effectName.Width = 100;
            effectName.Height = 32;
            effectName.X = 8;
            effectName.AddOnIntersectsCallback(ShowPopUp);
            switch (statusEffect.Type)
            {
                case StatusEffectType.Negative:
                    effectName.Color = Color.OrangeRed;
                    break;
                case StatusEffectType.Neutral:
                    effectName.Color = Color.White;
                    break;
                case StatusEffectType.Positive:
                    effectName.Color = Color.YellowGreen;
                    break;
            }
            return effectName;
        }

        private void ShowPopUp(MNode node)
        {
            if (nodesEffects.ContainsKey(node))
            {
                StatusEffect statusEffect = nodesEffects[node];
                GlobalUI.ShowTooltips(statusEffect.GetInformation());
            }
        }
    }
}