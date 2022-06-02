using System.Drawing;

namespace View
{
    /// <summary>
    /// Gomb vezérlő
    /// </summary>
    public class Button : ImageControl
    {

        #region Fields
        public int FontSize
        {
            get => TextControl.FontSize;
            set => TextControl.FontSize = value;
        }
        public Brush Brush = Brushes.Black;

        internal Label TextControl;

        private string _text = "";
        #endregion

        #region Properties
        public string Text
        {
            get => _text;
            set
            {
                TextControl.Text = value;
                _text = value;
            }
        }

        #endregion

        #region Contructor
        /// <summary>
        /// gomb vezérlő konstruktora
        /// </summary>
        public Button() : base()
        {
            TextControl = new Label()
            {
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Position = new Point(0, 0),
            };
            Children.Add(TextControl);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Vezérlőtypus felülírása gombra
        /// </summary>
        /// <returns></returns>
        internal override ControlType GetControlType()
        {
            return ControlType.Button;
        }
        #endregion

    }
}
