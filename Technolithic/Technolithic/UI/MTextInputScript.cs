using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        private string lastText = "";
        private bool underscore;
        private float underscoreCounter;
        private float repeatCounter = 0;
        private Keys? repeatKey = null;
        private bool hasFocus = false;
        
        private MyText text;
        private MyText underscoreText;

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

        private InputValidator validator;

        public Action<string> OnCurrentTextChanges { get; set; }

        public MTextInputScript() : base(true)
        {
            validator = new InputValidator();

            text = new MyText(null);
            text.Text = "";
            text.X = 8;
            text.Y = 6;
            
            underscoreText = new MyText(null);
            underscoreText.Text = "|";
            underscoreText.X = 8;
            underscoreText.Y = 6;
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
            ParentNode.AddChildNode(text);
            ParentNode.AddChildNode(underscoreText);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                if (ParentNode.Intersects(mouseX, mouseY))
                {
                    hasFocus = true;
                }
                else
                {
                    hasFocus = false;
                }
            }

            if (hasFocus == false)
            {
                underscoreText.Active = false;
                return;
            }

            UpdateCursor();
            HandleKeyboardInput();
        }

        public MTextInputScript AddValidationRule(IInputValidationRule rule)
        {
            validator.AddRule(rule);
            return this;
        }

        private void UpdateCursor()
        {
            underscoreCounter += Engine.DeltaTime;
            while (underscoreCounter >= UNDERSCORE_TIME)
            {
                underscoreCounter -= UNDERSCORE_TIME;
                underscore = !underscore;
            }

            underscoreText.Active = underscore && hasFocus;
        }

        private void HandleKeyboardInput()
        {
            oldState = currentState;
            currentState = Keyboard.GetState();

            HandleKeyRepeat();

            foreach (Keys key in currentState.GetPressedKeys())
            {
                if (oldState[key] == KeyState.Up)
                {
                    ProcessKey(key);
                    break;
                }
            }
        }

        private void HandleKeyRepeat()
        {
            if (repeatKey.HasValue && currentState[repeatKey.Value] == KeyState.Down)
            {
                repeatCounter += Engine.DeltaTime;
                if (repeatCounter >= REPEAT_DELAY)
                {
                    ProcessKey(repeatKey.Value);
                    repeatCounter -= REPEAT_EVERY;
                }
            }
            else
            {
                repeatKey = null;
                repeatCounter = 0;
            }
        }

        private void ProcessKey(Keys key)
        {
            string newText = currentText;
            char charToAdd = GetCharFromKey(key);

            if (charToAdd != '\0')
            {
                if (validator.ValidateInput(currentText, charToAdd, currentText.Length))
                {
                    newText += charToAdd;
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Специальные клавиши
                switch (key)
                {
                    case Keys.Space:
                        if (validator.ValidateInput(currentText, ' ', currentText.Length))
                        {
                            repeatKey = Keys.Space;
                            newText += " ";
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Keys.Back:
                        repeatKey = Keys.Back;
                        if (currentText.Length > 0)
                            newText = currentText.Substring(0, currentText.Length - 1);
                        break;
                    case Keys.Delete:
                        newText = "";
                        break;
                    default:
                        return;
                }
            }

            CurrentText = newText;
        }

        private char GetCharFromKey(Keys key)
        {
            bool shift = currentState[Keys.LeftShift] == KeyState.Down ||
                        currentState[Keys.RightShift] == KeyState.Down;

            // Letters
            if (key >= Keys.A && key <= Keys.Z)
                return shift ? key.ToString()[0] : key.ToString().ToLower()[0];

            // Numbers and symbols
            switch (key)
            {
                case Keys.D1: return shift ? '!' : '1';
                case Keys.D2: return shift ? '@' : '2';
                case Keys.D3: return shift ? '#' : '3';
                case Keys.D4: return shift ? '$' : '4';
                case Keys.D5: return shift ? '%' : '5';
                case Keys.D6: return shift ? '^' : '6';
                case Keys.D7: return shift ? '&' : '7';
                case Keys.D8: return shift ? '*' : '8';
                case Keys.D9: return shift ? '(' : '9';
                case Keys.D0: return shift ? ')' : '0';
                case Keys.OemMinus: return shift ? '_' : '-';
                case Keys.OemPlus: return shift ? '+' : '=';
                case Keys.OemComma: return shift ? '<' : ',';
                case Keys.OemPeriod: return shift ? '>' : '.';
                case Keys.OemQuestion: return shift ? '?' : '/';
                case Keys.OemSemicolon: return shift ? ':' : ';';
                case Keys.OemQuotes: return shift ? '"' : '\'';
                case Keys.OemOpenBrackets: return shift ? '{' : '[';
                case Keys.OemCloseBrackets: return shift ? '}' : ']';
                case Keys.OemBackslash: return shift ? '|' : '\\';
                default: return '\0';
            }
        }
    }
}
