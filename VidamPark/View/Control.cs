using System;
using System.Collections.Generic;
using System.Drawing;

namespace View
{
    /// <summary>
    /// Vezérlő típusát megadó enumerátor.
    /// </summary>
    public enum ControlType
    {
        Image = 1,
        AnimatedImage = 2,
        Button = 4,
        Textbox = 8,
        Label = 16,
        BuildMenu = 32,
    }
    /// <summary>
    /// Kirajzolás típusát adja meg.
    /// </summary>
    public enum DrawType
    {
        Image,
        Color,
    }
    /// <summary>
    /// Függőleges elhelyezést megadó enumerátor.
    /// </summary>
    public enum VerticalOrientation
    {
        Top,
        Center,
        Bottom,
    }
    /// <summary>
    /// Vízszintes elhelyezést megadó enumerátor.
    /// </summary>
    public enum HorizontalOrientation
    {
        Left,
        Center,
        Right,
    }

    /// <summary>
    /// Gyerek vezérlők tárolására alkalmas konténer.
    /// </summary>
    public class ContainerList : List<Control>
    {

        #region Fields
        Control Parent;
        #endregion

        #region Contructor
        /// <summary>
        /// Konténer létrehozása.
        /// </summary>
        /// <param name="p">szűlő vezérlő, mely rendelkezni fog gyerekekkel</param>
        public ContainerList(Control p)
        {
            Parent = p;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gyerek hozzáadás a szülőhöz.
        /// </summary>
        /// <param name="child">gyerek amit hozzáadunk a szülőhöz</param>
        public new void Add(Control child)
        {
            child.Parent = Parent;
            base.Add(child);
            RefreshChildrenPos();
        }
        /// <summary>
        /// Gyerekek poziciójának frissítése a szülőhöz
        /// </summary>
        internal void RefreshChildrenPos()
        {
            foreach (var child in this)
            {
                Point point = new Point();
                switch (child.HorizontalOrientation)
                {
                    case HorizontalOrientation.Left:
                        point.X = Parent.ActPosition.X + child.Position.X;
                        break;
                    case HorizontalOrientation.Center:
                        point.X = Parent.ActPosition.X + child.Position.X + Parent.Size.Width / 2;
                        break;
                    case HorizontalOrientation.Right:
                        point.X = Parent.ActPosition.X + child.Position.X + Parent.Size.Width;
                        break;
                }
                switch (child.VerticalOrientation)
                {
                    case VerticalOrientation.Top:
                        point.Y = Parent.ActPosition.Y + child.Position.Y;
                        break;
                    case VerticalOrientation.Center:
                        point.Y = Parent.ActPosition.Y + child.Position.Y + Parent.Size.Height / 2;
                        break;
                    case VerticalOrientation.Bottom:
                        point.Y = Parent.ActPosition.Y + child.Position.Y + Parent.Size.Height;
                        break;
                }
                switch (Parent.HorizontalOrientation)
                {
                    case HorizontalOrientation.Left:
                        break;
                    case HorizontalOrientation.Center:
                        point.X -= Parent.Size.Width / 2;
                        break;
                    case HorizontalOrientation.Right:
                        point.X -= Parent.Size.Width;
                        break;
                }
                switch (Parent.VerticalOrientation)
                {
                    case VerticalOrientation.Top:
                        break;
                    case VerticalOrientation.Center:
                        point.Y -= Parent.Size.Height / 2;
                        break;
                    case VerticalOrientation.Bottom:
                        point.Y -= Parent.Size.Height;
                        break;
                }
                child.ActPosition = point;
            }
        }
        #endregion




    }

    /// <summary>
    /// Vezérlő ősosztály.
    /// </summary>
    public abstract class Control
    {
        #region Constants
        public static readonly float RefH = 1080;
        public static readonly float RefW = 1920;
        #endregion

        #region Fields
        internal static Control? focused;

        public object DataContext;

        internal Control? Parent;

        internal int DisplayId = -1;

        public int Visualization = -1;

        public bool IsVisible;

        internal DrawType DrawType;

        public ContainerList Children;

        protected Point _position;

        private VerticalOrientation _verticalOrientation = VerticalOrientation.Top;

        private HorizontalOrientation _horizontalOrientation = HorizontalOrientation.Left;

        private Point _actPosition;

        private Size _size;

        internal float[] Vertices =
        {
            0, 0, 0, 0, 1, // bal felső
            0, 0, 0, 1, 1, // jobb felső
            0, 0, 0, 1, 0, // jobb alsó
            0, 0, 0, 0, 0, // bal alsó
        };
        #endregion

        #region Properties
        public string? ImageName { get; protected set; }
        public VerticalOrientation VerticalOrientation
        {
            get { return _verticalOrientation; }
            set
            {
                if (_verticalOrientation != value)
                {
                    _verticalOrientation = value;
                    ActPosition = ActPosition;
                }
                else
                {
                    _verticalOrientation = value;
                }
            }
        }
        public HorizontalOrientation HorizontalOrientation
        {
            get { return _horizontalOrientation; }
            set
            {
                if (_horizontalOrientation != value)
                {
                    _horizontalOrientation = value;
                    ActPosition = ActPosition;
                }
                else
                {
                    _horizontalOrientation = value;
                }
            }
        }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (Parent != null)
                    Parent.Children.RefreshChildrenPos();
                _position = value;
            }
        }


        public Point ActPosition
        {
            get { return _actPosition; }
            internal set
            {
                switch (HorizontalOrientation)
                {
                    case HorizontalOrientation.Left:
                        Vertices[0] = (value.X * 2) / RefW - 1;
                        Vertices[15] = (value.X * 2) / RefW - 1;
                        break;
                    case HorizontalOrientation.Center:
                        Vertices[0] = (value.X * 2 - _size.Width) / RefW - 1;
                        Vertices[15] = (value.X * 2 - _size.Width) / RefW - 1;
                        break;
                    case HorizontalOrientation.Right:
                        Vertices[0] = (value.X - _size.Width) * 2 / RefW - 1;
                        Vertices[15] = (value.X - _size.Width) * 2 / RefW - 1;
                        break;
                }
                switch (VerticalOrientation)
                {
                    case VerticalOrientation.Top:
                        Vertices[11] = 1 - (value.Y * 2) / RefH;
                        Vertices[16] = 1 - (value.Y * 2) / RefH;
                        break;
                    case VerticalOrientation.Center:
                        Vertices[11] = 1 - (value.Y * 2 - _size.Height) / RefH;
                        Vertices[16] = 1 - (value.Y * 2 - _size.Height) / RefH;
                        break;
                    case VerticalOrientation.Bottom:
                        Vertices[11] = 1 - (value.Y - _size.Height) * 2 / RefH;
                        Vertices[16] = 1 - (value.Y - _size.Height) * 2 / RefH;
                        break;
                }

                _actPosition = value;
                Children.RefreshChildrenPos();
                Size = _size; // ez kell!! hogy frissüljön a vertices size része.
            }
        }



        public Size Size
        {
            get { return _size; }
            set
            {
                Vertices[5] = Vertices[0] + (value.Width * 2) / RefW;
                Vertices[10] = Vertices[15] + (value.Width * 2) / RefW;
                Vertices[1] = Vertices[11] - (value.Height * 2) / RefH;
                Vertices[6] = Vertices[16] - (value.Height * 2) / RefH;
                _size = value;
            }
        }

        public EventHandler? OnClick
        {
            get { return _onClick; }
            set
            {
                _onClick = value;
            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// konstruktor a default értékek felvételére a leszármazott osztályokhoz.
        /// </summary>
        public Control()
        {
            DataContext = this;
            IsVisible = true;
            Children = new ContainerList(this);
            Size = new Size(0, 0);
            Position = new Point(0, 0);
            DrawType = DrawType.Color;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Vezérlőhöz tartozó shader lekérdezése.
        /// </summary>
        /// <returns>shader mely a vezérlő kirajzolásához kell</returns>
        internal Shader GetShader()
        {
            return VisualData.GetShader(ShaderPosition.StaticImage);
        }
        /// <summary>
        /// Vezérlőhöz tartozó textúra lekérdezése.
        /// </summary>
        /// <returns>textúra mely a vezérlő kirajzolásához kell</returns>
        abstract internal Texture GetTexture();

        /// <summary>
        /// Vezérlő típusát lekérdező függvény.
        /// </summary>
        /// <returns>vezérlő típusa</returns>
        abstract internal ControlType GetControlType();
        #endregion

        #region EvenHandlers
        private EventHandler? _onClick;

        #endregion
        

        

    }

}