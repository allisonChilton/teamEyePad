using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    //this shouldn't be abstract. When drawing a KeyboardControl2, it does not instantiate the child class. It instantiates this class
    //and draws on it according to the child class specifications. It is causing the error for keyboardControl2 design view. 
    //I'm leaving it because it isn't causing a problem and fixing it is a low priority, but for general polish and correct design
    //methodology, this should be fixed in the future
    public abstract class Keyboard : UserControl, ICloneable
    {
        protected TextBox _textBox;
        protected bool _confirmClear = false;
        protected ALSButton[] predictionKeys;
        protected PresagePredictor presage;
        protected ALSKey[,] keyboard;
        protected List<string> predictionWords;
        protected string mostRecentEntry;
        protected bool browserMode = false;
        protected string internalBuffer;
        protected ClearTextConfirmation frmClearTextConfirmation;

        public delegate void Event(object sender, EventArgs e);
        public event Event OnPressed;
        public event Event ClearText_Click;

        #region Constrcutors
        public Keyboard()
        {
            this.initialSetup();
            
        }

        public Keyboard(bool browserMode)
        {
            this.browserMode = browserMode;
            this.initialSetup();
            

        }

        protected void initialSetup()
        {
            try { presage = new PresagePredictor(); }catch(Exception e) { ALS_App.LogCrash(e); }

            _textBox = new TextBox();
            _textBox.Font = new Font(_textBox.Font.FontFamily, 24);
            _textBox.Multiline = true;
            Controls.Add(_textBox);

            frmClearTextConfirmation = new ClearTextConfirmation();
            frmClearTextConfirmation.ClearText_Click += FrmClearTextConfirmation_ClearText_Click;

            predictionKeys = new ALSButton[5];
            predictionWords = new List<string>();

            for (int i = 0; i < predictionKeys.Length; i++)
            {
                predictionKeys[i] = new ALSButton();
                this.Controls.Add(predictionKeys[i]);
                predictionKeys[i].btnType = ALSButton.ButtonType.key;
                predictionKeys[i].Click += new System.EventHandler(this.Predictionkey_Click);
            }
            this.Resize += new System.EventHandler(this.Keyboard_Resize);
        }
        #endregion

        #region Public Methods
        public void SetBrowserMode(bool isBrowserMode)
        {
            browserMode = isBrowserMode;
            Keyboard_Resize(this, EventArgs.Empty);

        }

        public void SetText(string text)
        {
            _textBox.Text = text;
        }

        public void SetTextBoxLocation(Point location)
        {
            _textBox.Location = location;
        }

        public void SetTextBoxSize(Size size)
        {
            _textBox.Size = size;
        }

        public void SetSelection(int start, int length)
        {
            _textBox.SelectionStart = start;
            _textBox.SelectionLength = length;
        }

        public void SetTextBoxFocus()
        {
            _textBox.Focus();
        }

        public int GetSelectionStart()
        {
            return _textBox.SelectionStart;
        }

        public string GetText()
        {
            return _textBox.Text;
        }

        public void HideTextBox()
        {
            _textBox.Hide();
            _textBox.Size = new Size(0, 0);
            Keyboard_Resize(this, null);
        }

        public void setClearConfirmation(bool confirmClear)
        {
            _confirmClear = confirmClear;
        }

        public void FillPredictKeys(object sender, EventArgs e)
        {
            ALSButton btn = ((ALSButton)sender);
            String[] predictions;

            try
            {
                predictions = presage.getPredictions(btn.Text);
            }catch
            {
                predictions = new String[0];
            }

            for (int i = 0; i < predictionKeys.GetLength(0); i++)
            {
                try
                {
                    predictionKeys[i].Text = predictions[i];
                }
                catch (IndexOutOfRangeException)
                {
                    predictionKeys[i].Text = "";
                }
            }
        }

        public void ResetPrediction()
        {
            predictionWords.Clear();
            try { presage.reset(); } catch { }

            foreach (ALSButton btn in predictionKeys)
            {
                btn.Text = "";
            }
        }

        public string GetMostRecentEntry()
        {
            string result = mostRecentEntry;

            switch (result)
            {
                case "+":
                case "^":
                case "~":
                case "(":
                case ")":
                case "{":
                case "}":
                    result = "{" + result + "}";
                    break;
            }
            return result;
        }

        #endregion

        #region Events
        public void Predictionkey_Click(object sender, EventArgs e)
        {
            ALSButton key = ((ALSButton)sender);
            if (key.Text.Length == 0)
                return;

            string text = _textBox.Text.Substring(0, _textBox.SelectionStart);
            var match = Regex.Match(text, @"\S+\s*$");

            string newWord;
            int charactersAfterCaret = _textBox.TextLength - _textBox.SelectionStart;

            if (match.Value.Contains(" "))
            {
                if (needsCapitalization())
                    newWord = key.Text.Substring(0, 1).ToUpper() + key.Text.Substring(1);
                else
                    newWord = key.Text;
                _textBox.Text = _textBox.Text.Substring(0, _textBox.SelectionStart) + newWord + " " + _textBox.Text.Substring(_textBox.SelectionStart);
            }
            else
            {
                //Remove typed characters
                int numCharactersToRemove = 0;
                for (int i = 0; i < text.Length; i++)
                    if (text.ToLower()[i].Equals(key.Text.ToLower()[i]))
                        numCharactersToRemove++;
                    else
                        break;
                _textBox.Text = _textBox.Text.Substring(0, _textBox.SelectionStart - numCharactersToRemove) + _textBox.Text.Substring(_textBox.SelectionStart);

                if (needsCapitalization())
                    newWord = key.Text.Substring(0, 1).ToUpper() + key.Text.Substring(1);
                else
                    newWord = key.Text;

                _textBox.Text = text.Substring(0, match.Index) + newWord + " " + _textBox.Text.Substring(_textBox.SelectionStart);
            }
            _textBox.SelectionStart = _textBox.TextLength - charactersAfterCaret + 1;

            ResetPrediction();
            Populate_Predictkeys();
        }

        protected void Clear(object sender, EventArgs e)
        {
            if (_confirmClear)
            {
                frmClearTextConfirmation.Show();
            }
            else
                ClearText();

        }

        protected void DeleteWord(object sender, EventArgs e)
        {
            int numCharacterToDelete;
            string text = _textBox.Text.Substring(0, _textBox.SelectionStart);


            var match = Regex.Match(text, @"\s+\S+\s*$");
            var selectionStart = _textBox.SelectionStart;
            if (match.Success)
            {
                _textBox.Text = text.Substring(0, match.Index) + " " + _textBox.Text.Substring(_textBox.SelectionStart);
                numCharacterToDelete = selectionStart - match.Length + 1;
                _textBox.SelectionStart = selectionStart - match.Length + 1;
            }
            else
            {
                numCharacterToDelete = _textBox.SelectionStart;
                _textBox.Text = _textBox.Text.Substring(_textBox.SelectionStart);
                _textBox.SelectionStart = 0;
            }

            mostRecentEntry = "";
            for (int i = 0; i < numCharacterToDelete; i++)
                mostRecentEntry += "\b";
            if (OnPressed != null)
                OnPressed(this, EventArgs.Empty);


            ResetPrediction();
            Populate_Predictkeys();
        }

        protected void Backspace(object sender, EventArgs e)
        {
            if (_textBox.Text.Length > 0)
            {
                int selectionStart = _textBox.SelectionStart;
                _textBox.Text = _textBox.Text.Substring(0, selectionStart - 1) + _textBox.Text.Substring(selectionStart);

                _textBox.SelectionStart = selectionStart - 1;
                ResetPrediction();
                Populate_Predictkeys();
            }
            mostRecentEntry = "\b";
            if (OnPressed != null)
                OnPressed(this, EventArgs.Empty);
        }

        protected void TypeCharacter(object sender, EventArgs e)
        {
            if (!(sender is ALSButton))
                return;
            ALSButton button = (ALSButton)sender;

            switch (button.Text)
            {
                case "Space":
                    Insert(" ");
                    break;
                case "&&":
                    Insert("&");
                    break;
                case ".":
                case "!":
                case "?":
                case ",":
                    string text = _textBox.Text.Substring(0, _textBox.SelectionStart);
                    var match = Regex.Match(text, @"\s*$");
                    _textBox.SelectionStart -= match.Length;
                    Insert(button.Text);
                    ResetPrediction();
                    break;
                default:
                    if (needsCapitalization())
                        Insert(button.Text.ToUpper());
                    else
                        Insert(button.Text);
                    break;
            }

            var isPunctuation = Regex.Match(button.Text, @"[\p{P}^+=|]");

            if (!isPunctuation.Success && button.Text.Length > 0)
            {
                this.Populate_Predictkeys();
            }
        }

        protected abstract void Keyboard_Resize(object sender, EventArgs e);
        #endregion

        #region Private Methods
        private void FrmClearTextConfirmation_ClearText_Click(bool confirm)
        {
            if (confirm)
                ClearText();
        }

        protected void ClearText()
        {
            _textBox.Text = "";
            if (ClearText_Click != null)
                ClearText_Click(this, EventArgs.Empty);
        }

        protected void Populate_Predictkeys()
        {
            try
            {
                string lastWord = "";
                string text = _textBox.Text.Substring(0, _textBox.SelectionStart);
                var match = Regex.Match(text, @"[.!?][^.!?]*$");

                if (match.Success)
                    lastWord = match.Length > 1 ? text.Substring(match.Index + 1) : "";
                else
                    lastWord = text;

                //remove inaplicable words
                match = Regex.Match(text, @"\S*$");
                string lastWord2 = "";
                if (match.Success)
                    lastWord2 = text.Substring(match.Index);
                else
                    predictionWords.Clear();    //new word
                for (int i = predictionWords.Count - 1; i >= 0; i--)
                {
                    string temp = predictionWords.ElementAt(i);
                    if (!temp.ToLower().Contains(lastWord2.ToLower()))
                        predictionWords.Remove(temp);
                }

                //Add new applicable words 

                string[] predictions = presage.getPredictions(lastWord); 
                    foreach (string word in predictions)
                        predictionWords.Add(word);

                    //Write words to buttons
                    for (int i = 0; i < predictionKeys.Length; i++)
                    {
                        try
                        {
                            string word = predictionWords.ElementAt(i);
                            predictionKeys[i].Text = word;
                            predictionKeys[i].setFontSize();
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            predictionKeys[i].Text = "";
                        }
                    }

            }
            catch { }

        }

        private ALSKey[,] GetKeyboard()
        {
            return keyboard;
        }

        private bool needsCapitalization()
        {
            string text = _textBox.Text.Substring(0, _textBox.SelectionStart);
            var match = Regex.Match(text, @"[.!?]\s*$");
            if (match.Success)
                return true;
            match = Regex.Match(text, @"^\s*$");
            if (match.Success)
                return true;
            return false;
        }

        private void Insert(string text)
        {
            int selectionStart = _textBox.SelectionStart;

            _textBox.Text = _textBox.Text.Substring(0, selectionStart) + text + _textBox.Text.Substring(selectionStart);
            _textBox.SelectionStart = selectionStart + text.Length;

            mostRecentEntry = text;
            if (OnPressed != null)
                OnPressed(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Keyboard
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.Name = "Keyboard";
            this.ResumeLayout(false);
        }
        #endregion

        #region Abstract Methods
        protected abstract void SetIsBrowser(bool isBrowser);

        public abstract object Clone();


        #endregion
    }
}