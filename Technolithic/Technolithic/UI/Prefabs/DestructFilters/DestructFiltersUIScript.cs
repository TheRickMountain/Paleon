using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Technolithic
{
    public class DestructFiltersUIScript : MScript
    {
        private readonly Dictionary<DestructFilter, MNode> _rows;
        private IReadOnlyList<DestructFilter> _shownFilters = new List<DestructFilter>();

        public DestructFiltersUIScript(Dictionary<DestructFilter, MNode> rows) : base(true)
        {
            _rows = rows;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            foreach (var (filter, row) in _rows)
            {
                GetToggle(row).AddOnValueChangedCallback((isOn, _) => DestructFilters.SetEnabled(filter, isOn));
            }
        }

        // Shows only the unlocked filters; their order defines the hotkeys.
        public void Refresh(ProgressTree progressTree)
        {
            _shownFilters = DestructFilters.GetAvailable(progressTree);

            int y = DestructFiltersUI.Padding + DestructFiltersUI.TitleHeight;

            foreach (var (filter, row) in _rows)
            {
                int index = IndexOf(filter);
                row.Active = index >= 0;

                if (row.Active == false)
                    continue;

                row.Y = y;
                row.Tooltips = GetTooltip(filter, hotkeyNumber: index + 1);
                y += DestructFiltersUI.RowHeight + DestructFiltersUI.RowSpacing;
            }

            ParentNode.Height = y - DestructFiltersUI.RowSpacing + DestructFiltersUI.Padding;

            SyncToggles();
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

            for (int i = 0; i < _shownFilters.Count && i < 9; i++)
            {
                if (MInput.Keyboard.Pressed(Keys.D1 + i) == false)
                    continue;

                if (isShiftDown)
                    DestructFilters.EnableOnly(_shownFilters[i]);
                else
                    DestructFilters.Toggle(_shownFilters[i]);
            }
        }

        private void SyncToggles()
        {
            foreach (var (filter, row) in _rows)
            {
                GetToggle(row).SilentCheck(DestructFilters.IsEnabled(filter));
            }
        }

        private int IndexOf(DestructFilter filter)
        {
            for (int i = 0; i < _shownFilters.Count; i++)
            {
                if (_shownFilters[i] == filter)
                    return i;
            }

            return -1;
        }

        private static ToggleScript GetToggle(MNode row)
        {
            return row.GetChildByName("Toggle").GetComponent<ToggleScript>();
        }

        private static string GetTooltip(DestructFilter filter, int hotkeyNumber)
        {
            return $"{filter.Name}\n/c[#919090]" +
                Localization.GetLocalizedText("destruct_filter_toggle_hotkey", hotkeyNumber) + "\n" +
                Localization.GetLocalizedText("destruct_filter_only_hotkey", hotkeyNumber) + "\n" +
                Localization.GetLocalizedText("destruct_filter_all_hotkey") + "/cd";
        }
    }
}
