using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TimeControllerUIScript : MScript
    {

        private SmallButton firstButton;
        private SmallButton secondButton;
        private SmallButton fourthButton;
        private SmallButton pauseButton;

        private Dictionary<SmallButton, int> speeds = new Dictionary<SmallButton, int>();

        private SmallButton lastSelectedButton;

        private int currentSpeed = 1;
        private bool inPause = false;

        public TimeControllerUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            firstButton = (SmallButton)ParentNode.GetChildByName("First");
            firstButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetTimeSpeed);
            firstButton.Tooltips = Localization.GetLocalizedText("speed") + $" x1 [1]";

            secondButton = (SmallButton)ParentNode.GetChildByName("Second");
            secondButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetTimeSpeed);
            secondButton.Tooltips = Localization.GetLocalizedText("speed") + $" x2 [2]";

            fourthButton = (SmallButton)ParentNode.GetChildByName("Fourth");
            fourthButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetTimeSpeed);
            fourthButton.Tooltips = Localization.GetLocalizedText("speed") + $" x4 [3]";

            pauseButton = (SmallButton)ParentNode.GetChildByName("Pause");
            pauseButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetPause);
            pauseButton.Tooltips = Localization.GetLocalizedText("pause") + $" [Space]";

            speeds.Add(firstButton, 1);
            speeds.Add(secondButton, 2);
            speeds.Add(fourthButton, 4);

            SetTimeSpeed(firstButton);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;
            }

            if (MInput.Keyboard.Pressed(Keys.D1))
            {
                SetTimeSpeed(firstButton);
            }
            else if (MInput.Keyboard.Pressed(Keys.D2))
            {
                SetTimeSpeed(secondButton);
            }
            else if (MInput.Keyboard.Pressed(Keys.D3))
            {
                SetTimeSpeed(fourthButton);
            }
            else if (MInput.Keyboard.Pressed(Keys.Space))
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

                Engine.GameSpeed = currentSpeed;

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
                Engine.GameSpeed = speeds[button];

            currentSpeed = speeds[button];
        }
    }
}
