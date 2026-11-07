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

        public Action<T> OverlayChanged;

        public event Action<T> OverlayChanged;

        public OverlayManager(T defaultOverlayType)
        {
            _overlays = new Dictionary<T, IOverlay>();

            _defaultOverlayType = defaultOverlayType;

            SetOverlay(_defaultOverlayType);
        }

        protected void AddOverlay(T overlayType, IOverlay overlay)
        protected void RegisterOverlay(T overlayType, IOverlay overlay)
        {
            if (overlay == null) return;

            _overlays[overlayType] = overlay;
        }

        public void SetOverlay(T overlayType)
        {
            {
                Program.WriteWarningLog($"Overlay {overlayType} is not registered, the default overlay is used instead");

                overlayType = _defaultOverlayType;
            CurrentOverlayType = overlayType;

            if (_overlays.TryGetValue(overlayType, out IOverlay overlay))
            {
                _currentOverlay = overlay;
            }
            else
            {
                // TODO: add warning log
                _currentOverlay = _overlays[_defaultOverlayType];
                SetOverlay(overlayType);
            }

            OverlayChanged?.Invoke(overlayType);
        }

        public void Render()
        {
            _currentOverlay.Render(_world);
        }

    }
}
