using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class OverlayManager<T> where T : Enum
    {
        private Dictionary<T, IOverlay> _overlays;

        private T _defaultOverlayType;

        private IOverlay _currentOverlay;

        public T CurrentOverlayType { get; private set; }

        public bool IsDefaultOverlayActive => EqualityComparer<T>.Default.Equals(CurrentOverlayType, _defaultOverlayType);

        public event Action<T> OverlayChanged;

        public OverlayManager(T defaultOverlayType)
        {
            _overlays = new Dictionary<T, IOverlay>();

            _defaultOverlayType = defaultOverlayType;

            RegisterOverlay(_defaultOverlayType, new DefaultOverlay());

            SetOverlay(_defaultOverlayType);
        }

        protected void RegisterOverlay(T overlayType, IOverlay overlay)
        {
            if (overlay == null) return;

            _overlays[overlayType] = overlay;
        }

        public void SetOverlay(T overlayType)
        {
            if (_overlays.TryGetValue(overlayType, out IOverlay overlay) == false)
            {
                Program.WriteWarningLog($"Overlay {overlayType} is not registered, the default overlay is used instead");

                overlayType = _defaultOverlayType;
                overlay = _overlays[_defaultOverlayType];
            }

            CurrentOverlayType = overlayType;
            _currentOverlay = overlay;

            OverlayChanged?.Invoke(overlayType);
        }

        public void ToggleOverlay(T overlayType)
        {
            if (EqualityComparer<T>.Default.Equals(CurrentOverlayType, overlayType))
            {
                SetOverlay(_defaultOverlayType);
            }
            else
            {
                SetOverlay(overlayType);
            }
        }

        public void Render()
        {
            _currentOverlay.Render();
        }

    }
}
