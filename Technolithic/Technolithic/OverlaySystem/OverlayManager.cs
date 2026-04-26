using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class OverlayManager<T> where T : Enum
    {
        private World _world;

        private Dictionary<T, IOverlay> _overlays;

        private T _defaultOverlayType;

        private IOverlay _currentOverlay;

        public T CurrentOverlayType { get; private set; }

        public Action<T> OverlayChanged;

        public OverlayManager(World world, T defaultOverlayType)
        {
            _world = world;

            _overlays = new Dictionary<T, IOverlay>();

            _defaultOverlayType = defaultOverlayType;
            
            AddOverlay(_defaultOverlayType, new DefaultOverlay());

            SetOverlay(_defaultOverlayType);
        }

        protected void AddOverlay(T overlayType, IOverlay overlay)
        {
            if (overlay == null) return;

            _overlays[overlayType] = overlay;
        }

        public void SetOverlay(T overlayType)
        {
            CurrentOverlayType = overlayType;

            if (_overlays.TryGetValue(overlayType, out IOverlay overlay))
            {
                _currentOverlay = overlay;
            }
            else
            {
                // TODO: add warning log
                _currentOverlay = _overlays[_defaultOverlayType];
            }

            OverlayChanged?.Invoke(overlayType);
        }

        public void Render()
        {
            _currentOverlay.Render(_world);
        }

    }
}
