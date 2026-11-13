using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Technolithic
{
    public class DestructFiltersUIScript : MScript
    {
        private readonly Dictionary<DestructFilter, MToggleUI> toggles;

        public DestructFiltersUIScript(Dictionary<DestructFilter, MToggleUI> toggles) : base(true)
        {
            this.toggles = toggles;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            foreach (var (filter, toggle) in toggles)
            {
                toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback((isOn, _) => DestructFilters.SetEnabled(filter, isOn));
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            HandleHotkeys();
            SyncToggles();
        }

        // 1..9 toggles a filter, Shift + number keeps only that filter, 0 enables all.
        private void HandleHotkeys()
        {
            if (MInput.Keyboard.Pressed(Keys.D0))
            {
                DestructFilters.EnableAll();
                return;
            }

            bool isShiftDown = MInput.Keyboard.Check(Keys.LeftShift) || MInput.Keyboard.Check(Keys.RightShift);

            for (int i = 0; i < DestructFilters.All.Count; i++)
            {
                if (MInput.Keyboard.Pressed(Keys.D1 + i) == false)
                    continue;

                DestructFilter filter = DestructFilters.All[i];

                if (isShiftDown)
                    DestructFilters.EnableOnly(filter);
                else
                    DestructFilters.Toggle(filter);
            }
        }

        private void SyncToggles()
        {
            foreach (var (filter, toggle) in toggles)
            {
                toggle.GetComponent<ToggleScript>().SilentCheck(DestructFilters.IsEnabled(filter));
            }
        }
    }
}
