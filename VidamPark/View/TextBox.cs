using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
    /// <summary>
    /// Felhasználó által írható szövegdoboz osztály
    /// </summary>
    public class TextBox : ImageControl
    {

        #region Fields
        public bool IsReadOnly = false;
        public bool IsDecimalOnly = false;

        internal Label Content;

        internal AnimatedImageControl Caret;
        private Graphics _textDrawer;
        #endregion

        #region Properties
        public int FontSize
        {
            get { return Content.FontSize; }
            set { Content.FontSize = value; }
        }

        public string Text
        {
            get
            {
                return Content.Text;
            }
            set
            {
                Content.Text = value;
                Caret.Position = new Point(Content.Size.Width / 2, 0);
                Children.RefreshChildrenPos();
                if (TextChanged != null)
                    TextChanged(this, value);
            }
        }
        public bool IsFocused
        {
            get
            {
                return focused == this;
            }
            set
            {
                if (value)
                    focused = this;
                else if (IsFocused)
                    focused = null;

            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Szövegdoboz létrehozása alapértelmezett adatokkal
        /// </summary>
        public TextBox()
        {
            _textDrawer = Graphics.FromImage(new Bitmap(1, 1));
            Content = new Label()
            {
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Position = new Point(0, 0),
            };
            Children.Add(Content);

            Caret = new AnimatedImageControl()
            {
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Position = new Point(0, 0),
                IsVisible = false,
                Size = new Size(5, (int)(FontSize * 1.5f)),
            };
            var colors = new List<Color4>() { new Color4(Color.Black.R, Color.Black.G, Color.Black.B, Color.Black.A), new Color4(0, 0, 0, 0) };
            var times = new List<int>() { 30, 30 };
            Caret.SetColors(colors, times);

            Children.Add(Caret);
        }
        #endregion

        #region Methods
        internal override ControlType GetControlType()
        {
            return ControlType.Textbox;
        }

        public void AddChar(char ch)
        {
            if (IsReadOnly)
                return;
            string proText = Text + ch;
            SizeF textSize = _textDrawer.MeasureString(proText, new Font(new FontFamily("Times New Roman"), FontSize, FontStyle.Bold));
            if (textSize.Width + 20 < Size.Width)
            {
                if (IsDecimalOnly)
                {
                    if (char.IsDigit(ch))
                        Text += ch;
                }
                else
                {
                    Text += ch;
                }
            }
        }

        public void RemoveLastChar()
        {
            if (IsReadOnly)
                return;
            if (Text.Length > 0)
                Text = Text.Remove(Text.Length - 1);
        }
        #endregion

        #region EvenHandlers

        public event EventHandler<string> TextChanged;

        #endregion

    }
}
