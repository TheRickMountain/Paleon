using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MTextInputScript : MScript
    {
        private const float UNDERSCORE_TIME = .5f;
        private const float REPEAT_DELAY = .5f;
        private const float REPEAT_EVERY = 1 / 30f;
        
        private KeyboardState oldState;
        private KeyboardState currentState;
        private string currentText = "";
        public string CurrentText 
        { 
            get { return currentText; }
            set
            {
                currentText = value;

                text.Text = currentText;
                underscoreText.X = text.TextWidth + 4;

                OnCurrentTextChanges?.Invoke(currentText);
            }
        }
        private string lastText = "";
        private bool underscore;
        private float underscoreCounter;
        private float repeatCounter = 0;
        private Keys? repeatKey = null;
        
        private MyText text;
        private MyText underscoreText;

        public int CharactersLimit { get; set; } = 20;
        public bool AllowIllegalSigns { get; set; } = false;

        public Action<string> OnCurrentTextChanges { get; set; }

        public MTextInputScript() : base(true)
        {

        }

        public void ResetText()
        {
            lastText = "";
            CurrentText = "";
            underscoreText.X = 8;
            underscoreText.Y = 6;
            text.Text = "";
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            text = new MyText(ParentNode.Scene);
            text.Text = "";
            text.X = 8;
            text.Y = 6;
            ParentNode.AddChildNode(text);

            underscoreText = new MyText(ParentNode.Scene);
            underscoreText.Text = "|";
            underscoreText.X = 8;
            underscoreText.Y = 6;
            ParentNode.AddChildNode(underscoreText);
        }

        public override void Update(int mouseX, int mouseY)
        {
            oldState = currentState;
            currentState = Keyboard.GetState();

            underscoreCounter += Engine.DeltaTime;
            while (underscoreCounter >= UNDERSCORE_TIME)
            {
                underscoreCounter -= UNDERSCORE_TIME;
                underscore = !underscore;
            }

            underscoreText.Active = underscore;

            if (repeatKey.HasValue)
            {
                if (currentState[repeatKey.Value] == KeyState.Down)
                {
                    underscoreText.Active = true;
                    repeatCounter += Engine.DeltaTime;

                    while (repeatCounter >= REPEAT_DELAY)
                    {
                        HandleKey(repeatKey.Value);
                        repeatCounter -= REPEAT_EVERY;

                        text.Text = CurrentText;
                        underscoreText.X = text.TextWidth + 4;
                    }
                }
                else
                {
                    repeatKey = null;
                    repeatCounter = 0;
                }
            }

            foreach (Keys key in currentState.GetPressedKeys())
            {
                if (oldState[key] == KeyState.Up)
                {
                    lastText = CurrentText;
                    HandleKey(key);
                    
                    if(CurrentText.Length > CharactersLimit)
                    {
                        CurrentText = lastText;
                    }

                    text.Text = CurrentText;
                    underscoreText.X = text.TextWidth + 4;
                    break;
                }
            }
        }

        private void HandleKey(Keys key)
        {
            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                            CurrentText += key.ToString();
                        else
                            CurrentText += key.ToString().ToLower();
                    }
                    break;

                case (Keys.D1):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '!';
                    }
                    else
                    {
                        CurrentText += '1';
                    }
                    break;
                case (Keys.D2):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '@';
                    }
                    else
                    {
                        CurrentText += '2';
                    }
                    break;
                case (Keys.D3):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '#';
                    }
                    else
                    {
                        CurrentText += '3';
                    }
                    break;
                case (Keys.D4):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '$';
                    }
                    else
                    {
                        CurrentText += '4';
                    }
                    break;
                case (Keys.D5):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '%';
                    }
                    else
                    {
                        CurrentText += '5';
                    }
                    break;
                case (Keys.D6):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '^';
                    }
                    else
                    {
                        CurrentText += '6';
                    }
                    break;
                case (Keys.D7):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '&';
                    }
                    else
                    {
                        CurrentText += '7';
                    }
                    break;
                case (Keys.D8):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '*';
                    }
                    else
                    {
                        CurrentText += '8';
                    }
                    break;
                case (Keys.D9):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '(';
                    }
                    else
                    {
                        CurrentText += '9';
                    }
                    break;
                case (Keys.D0):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += ')';
                    }
                    else
                    {
                        CurrentText += '0';
                    }
                    break;
                case (Keys.OemComma):
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '<';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += ',';
                    }
                    break;
                case Keys.OemPeriod:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '>';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '.';
                    }
                    break;
                case Keys.OemQuestion:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '?';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '/';
                    }
                    break;
                case Keys.OemSemicolon:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += ':';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += ';';
                    }
                    break;
                case Keys.OemQuotes:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '"';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '\'';
                    }
                    break;
                case Keys.OemBackslash:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '|';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '\\';
                    }
                    break;
                case Keys.OemOpenBrackets:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '{';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '[';
                    }
                    break;
                case Keys.OemCloseBrackets:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '}';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += ']';
                    }
                    break;
                case Keys.OemMinus:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '_';
                    }
                    else
                    {
                        CurrentText += '-';
                    }
                    break;
                case Keys.OemPlus:
                    if (currentState[Keys.LeftShift] == KeyState.Down || currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '+';
                    }
                    else
                    {
                        if (AllowIllegalSigns)
                            CurrentText += '=';
                    }
                    break;

                case Keys.Space:
                    repeatKey = Keys.Space;
                    CurrentText += " ";
                    break;
                case Keys.Back:
                    repeatKey = Keys.Back;
                    if (CurrentText.Length > 0)
                        CurrentText = CurrentText.Substring(0, CurrentText.Length - 1);
                    break;
                case Keys.Delete:
                    CurrentText = "";
                    break;
            }
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
