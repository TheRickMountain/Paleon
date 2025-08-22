using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Technolithic
{
    public class TimeControllerUIScript : MScript
    {

        public enum GameSpeed
        {
            X1 = 1,
            X2 = 2,
            X4 = 4
        }

        private SmallButton firstButton;
        private SmallButton secondButton;
        private SmallButton fourthButton;
        private SmallButton pauseButton;

        private SmallButton lastSelectedButton;

        private GameSpeed _currentGameSpeed = GameSpeed.X1;
        private bool inPause = false;

        private Dictionary<GameSpeed, SmallButton> gameSpeedButtons = new();

        public TimeControllerUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            firstButton = (SmallButton)ParentNode.GetChildByName("First");
            AssociateButtonWithSpeed(firstButton, GameSpeed.X1);

            secondButton = (SmallButton)ParentNode.GetChildByName("Second");
            AssociateButtonWithSpeed(secondButton, GameSpeed.X2);

            fourthButton = (SmallButton)ParentNode.GetChildByName("Fourth");
            AssociateButtonWithSpeed(fourthButton, GameSpeed.X4);

            pauseButton = (SmallButton)ParentNode.GetChildByName("Pause");
            pauseButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetPause);
            pauseButton.Tooltips = Localization.GetLocalizedText("pause") + $" [Space]";

            SetTimeSpeed(firstButton);
        }

        private void AssociateButtonWithSpeed(SmallButton button, GameSpeed gameSpeed)
        {
            button.GetComponent<ButtonScript>().AddOnClickedCallback(SetTimeSpeed);
            button.Tooltips = Localization.GetLocalizedText("speed") + $" {gameSpeed.ToString()} [Tab]";
            button.SetMetadata("game_speed", gameSpeed);
            gameSpeedButtons.Add(gameSpeed, button);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;
            }

            if (MInput.Keyboard.Pressed(Keys.Tab))
            {
                _currentGameSpeed = _currentGameSpeed.NextEnum();

                SetTimeSpeed(gameSpeedButtons[_currentGameSpeed]);
            }
            
            if (MInput.Keyboard.Pressed(Keys.Space))
            {
                SetPause(true, null);
            }
        }

        private void SetTimeSpeed(bool value, ButtonScript buttonScript)
        {
            SetTimeSpeed((SmallButton)buttonScript.ParentNode);
        }

        private void SetPause(bool value, ButtonScript buttonScript)
        {
            if (pauseButton.GetComponent<ButtonScript>().IsSelected)
            {
                pauseButton.GetComponent<ButtonScript>().IsSelected = false;

                Engine.GameSpeed = (int)_currentGameSpeed;

                inPause = false;
            }
            else
            {
                pauseButton.GetComponent<ButtonScript>().IsSelected = true;

                Engine.GameSpeed = 0;

                inPause = true;
            }
        }

        private void SetTimeSpeed(SmallButton button)
        {
            if (lastSelectedButton != null)
                lastSelectedButton.GetComponent<ButtonScript>().IsSelected = false;

            lastSelectedButton = button;
            lastSelectedButton.GetComponent<ButtonScript>().IsSelected = true;

            if(inPause == false)
                Engine.GameSpeed = (int)button.GetMetadata<GameSpeed>("game_speed");

            _currentGameSpeed = button.GetMetadata<GameSpeed>("game_speed");
        }
    }
}
