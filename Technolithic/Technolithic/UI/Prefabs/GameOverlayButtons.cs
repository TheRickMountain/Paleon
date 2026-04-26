using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class GameOverlayButtons : MNode
    {

        private GameOverlayManager _overlayManager;

        private Dictionary<GameOverlayType, SmallButton> overlayTypeButton;

        private SmallButton _lastSelectedButton;

        public GameOverlayButtons(Scene scene, GameOverlayManager overlayManager) : base(scene)
        {
            _overlayManager = overlayManager;
            _overlayManager.OverlayChanged += OnOverlayChanged;

            overlayTypeButton = new Dictionary<GameOverlayType, SmallButton>();

            int buttonIndex = 0;
            int distanceBetweenButtons = 5;
            foreach (GameOverlay overlay in overlayManager.Overlays)
            {
                SmallButton smallButton = new SmallButton(scene, overlay.Icon);
                smallButton.X = (smallButton.Width + distanceBetweenButtons) * buttonIndex;
                smallButton.Tooltips = overlay.Name;
                smallButton.ButtonScript.AddOnClickedCallback(OnOverlayButtonPressed);
                smallButton.SetMetadata("data", overlay);
                AddChildNode(smallButton);
                overlayTypeButton.Add(overlay.Type, smallButton);

                Width += smallButton.Width;
                Height = Math.Max(Height, smallButton.Height);

                buttonIndex++;
            }

            Width += distanceBetweenButtons * (buttonIndex - 1);
        }

        private void OnOverlayChanged(GameOverlayType overlayType)
        {
            if (_lastSelectedButton != null)
            {
                _lastSelectedButton.ButtonScript.IsSelected = false;
                _lastSelectedButton = null;
            }

            if (overlayType != GameOverlayType.None)
            {
                _lastSelectedButton = overlayTypeButton[overlayType];
                _lastSelectedButton.ButtonScript.IsSelected = true;
            }
        }

        private void OnOverlayButtonPressed(bool value, ButtonScript buttonScript)
        {
            if (_lastSelectedButton != null)
            {
                _lastSelectedButton.ButtonScript.IsSelected = false;
                _lastSelectedButton = null;
            }

            GameOverlay gameOverlay = buttonScript.ParentNode.GetMetadata<GameOverlay>("data");

            if (_overlayManager.CurrentOverlayType == gameOverlay.Type)
            {
                _overlayManager.SetOverlay(GameOverlayType.None);
            }
            else
            {
                _overlayManager.SetOverlay(gameOverlay.Type);

                _lastSelectedButton = buttonScript.ParentNode as SmallButton;
                _lastSelectedButton.ButtonScript.IsSelected = true;
            }
        }
    }
}
