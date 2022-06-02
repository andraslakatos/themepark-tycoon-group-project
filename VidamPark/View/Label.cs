using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
    /// <summary>
    /// Cím kiírásához vezérlő alosztály
    /// </summary>
    public class Label : ImageControl
    {
        #region Constants

        private static readonly float RefSize = 0.6085f;
        #endregion

        #region Fields
        internal bool haveTexture = false;

        private Texture? _textImage;

        private string _text = "";

        public int FontSize = 36;
        public Brush Brush = Brushes.Black;
        #endregion

        #region Properties

        internal Texture? TextImage
        {
            get { return _textImage; }
            set
            {
                haveTexture = true;
                _textImage = value;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                var bmp = CreateTextTexture(value);
                TextImage?.Delete(_text);
                TextImage = new Texture(bmp);
                Size = bmp.Size;
                _text = value;
            }
        }

        #endregion

        #region Contructor
        /// <summary>
        /// Cím létrehozása.
        /// </summary>
        public Label()
        {
            DrawType = DrawType.Image;
        }
        /// <summary>
        /// Cím törlése, hozzá tartozó textúra törlésével együtt.
        /// </summary>
        ~Label()
        {
            TextImage?.Delete(_text);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Vezérlőhöz tartozó textúra lekérdezése.
        /// </summary>
        /// <returns>textúra mely a vezérlő kirajzolásához kell</returns>
        internal override Texture GetTexture()
        {
            return TextImage;
        }
        /// <summary>
        /// Vezérlő típusát lekérdező függvény.
        /// </summary>
        /// <returns>vezérlő típusa</returns>
        internal override ControlType GetControlType()
        {
            return ControlType.Label;
        }
        /// <summary>
        /// Szöveg írása képre.
        /// </summary>
        /// <param name="text">szöveg melyet képre akarunk írni</param>
        /// <returns>kép mely tartalmazza a szöveget.</returns>
        internal Bitmap CreateTextTexture(string text)
        {

            if (text.Length == 0)
                return new Bitmap(1, 1);

            Graphics gfx;
            gfx = Graphics.FromImage(new Bitmap(1, 1));
            SizeF textSize = gfx.MeasureString(text, new Font(new FontFamily("Times New Roman"), FontSize, FontStyle.Bold));

            float scale = FontSize / textSize.Height * (text.Count(c => c == '\n') + 1) / RefSize;

            Bitmap textImage = new Bitmap((int)(textSize.Width * scale), (int)(textSize.Height * scale));


            gfx = Graphics.FromImage(textImage);
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            gfx.DrawString(text, new Font(new FontFamily("Times New Roman"), FontSize * scale, FontStyle.Bold), Brush, new PointF(0, 0));

            gfx.Dispose();
            return textImage;
        }
        #endregion




    }
}
