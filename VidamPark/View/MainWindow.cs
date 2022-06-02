using MapModel.Building;
using Model;
using Ookii.Dialogs.Wpf;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace View
{
    public enum MouseClickState
    {
        ClickOrInfo,
        Build,
        Destroy,
    }
    public partial class MainWindow : GameWindow
    {


        #region Fields

        private GameModel _model;

        private InformationWindow _informationWindow;
        public MouseClickState mouseState;


        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;

        private Camera _camera;

        private uint[] _indices = {
            0,1,3,
            1,2,3,
        };

        private Control _child;

        private int _displayedImages = 0;

        float _mouseScaleX = 1;
        float _mouseScaleY = 1;
        float _mouseCenterX = 0;
        float _mouseCenterY = 0;
        float _width;
        float _height;

        public int BuildId = -1;

        private Vector2 _mousePositionToTable = new Vector2(-1, -1);
        private Vector2 _mousePositionRaw = new Vector2(-1, -1);

        bool _shiftDown = false;
        byte _delay = 0;
        #endregion

        #region Properties
        public Control Child
        {
            get { return _child; }
            set
            {
                _child = value;
                Point point = new Point();
                switch (_child.HorizontalOrientation)
                {
                    case HorizontalOrientation.Left:
                        point.X = Child.Position.X;
                        break;
                    case HorizontalOrientation.Center:
                        point.X = Child.Position.X + (int)Control.RefW / 2;
                        break;
                    case HorizontalOrientation.Right:
                        point.X = Child.Position.X + (int)Control.RefW;
                        break;
                }
                switch (_child.VerticalOrientation)
                {
                    case VerticalOrientation.Top:
                        point.Y = Child.Position.Y;
                        break;
                    case VerticalOrientation.Center:
                        point.Y = Child.Position.Y + (int)Control.RefH / 2;
                        break;
                    case VerticalOrientation.Bottom:
                        point.Y = Child.Position.Y + (int)Control.RefH;
                        break;
                    default:
                        break;
                }

                _child.ActPosition = point;
            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Alkalmazásablak létrhozása.
        /// </summary>
        /// <param name="gws">magas szintű tulajdonásokat hordozó osztály</param>
        /// <param name="nws">alacsony szintű tulajdonásokat hordozó osztály</param>
        /// <param name="model">Játékmodell ami a játékműködés alapját adja</param>
        public MainWindow(GameWindowSettings gws, NativeWindowSettings nws, GameModel model) : base(gws, nws)
        {
            _model = model;
            VisualData.LoadTextures();
            _informationWindow = new InformationWindow(new Buildings(-1, -1, new Point(-1, -1)), _model);
            CursorVisible = true;
            Cursor = OpenTK.Windowing.Common.Input.MouseCursor.Default;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Ablak betöltésekor lefutó függvény.
        /// </summary>
        protected override void OnLoad()
        {
            _camera = new Camera(Vector3.UnitZ * 1, Size.X / (float)Size.Y);
            _camera.Fov = 90;
            CursorVisible = true;
            // Egymásra rájzolásnál problémát okoz.
            //GL.Enable(EnableCap.DepthTest);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.CullFace);


            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            //GL.ClearColor(1f, 1f, 1f, 0f);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices2.Length * sizeof(float), vertices2, BufferUsageHint.StaticDraw);


            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);



            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _camera.Position = new Vector3(0, 0, _camera.GetProjectionMatrix().Column1[1]);

            TextInput += MainWindow_TextInput;

            BuildMenu.InitBuildMenu(this, _model);
            if (OnGameLoad != null)
                OnGameLoad(this, EventArgs.Empty);
        }
        /// <summary>
        /// Fájlmentéshez megynitja a mentésablakot.
        /// </summary>
        /// <returns>elmentet fájlnév</returns>
        public async Task<string> OpenSaveFileDialog()
        {
            var fileDialog = new VistaSaveFileDialog();
            fileDialog.Title = "Játék mentése";
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var t = Task.Run(() => fileDialog.ShowDialog());
            await t;

            return fileDialog.FileName;
        }

        /// <summary>
        /// Fájlbetöltéshez megynitja a betöltésablakot.
        /// </summary>
        /// <returns>a betöltött fájl neve</returns>
        public string OpenLoadFileDialog()
        {
            var fileDialog = new VistaOpenFileDialog();
            fileDialog.Title = "Játék betöltése";
            fileDialog.Multiselect = false;
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var t = Task.Run(() => fileDialog.ShowDialog());
            t.Wait();

            return fileDialog.FileName;
        }



        /// <summary>
        /// Szövegbemenet kezelése a főablakra.
        /// </summary>
        /// <param name="e">billentyűzetenleütött karakter</param>
        private void MainWindow_TextInput(TextInputEventArgs e)
        {

            if (Control.focused != null && Control.focused.GetControlType() == ControlType.Textbox)
            {
                TextBox textbox = (TextBox)Control.focused;
                textbox.AddChar((char)e.Unicode);
            }
        }

        /// <summary>
        /// Az ablak képfrissítő függvénye.
        /// </summary>
        /// <param name="e">Képesemény történeteit összefoglaló paraméter</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {


            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            if (_model.IsGameRunning)
            {
                DrawTable();
            }

            _displayedImages = 0;
            if (Child != null)
            {
                DrawControl(Child);
            }

            SwapBuffers();


            base.OnRenderFrame(e);
        }

        /// <summary>
        /// Ablak újraméretezés lekezelése.
        /// </summary>
        /// <param name="e">újraméretezés paraméterei</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            _width = e.Width;
            _height = e.Height;
            int ratioH = e.Height * 16;
            int ratioW = e.Width * 9;
            if (ratioH == ratioW)
            {
                _mouseScaleX = 1;
                _mouseScaleY = 1;
                GL.Viewport(0, 0, e.Width, e.Height);
                _mouseCenterX = 0;
                _mouseCenterY = 0;
            }
            else if (ratioH > ratioW)
            {
                _mouseScaleX = 1;
                _mouseScaleY = (float)e.Height * 16 / ratioW;

                _mouseCenterX = 0;
                _mouseCenterY = (e.Height - (float)ratioW / 16) / 2;

                GL.Viewport(0, (e.Height - ratioW / 16) / 2, e.Width, ratioW / 16);
            }
            else
            {
                _mouseScaleX = (float)e.Width * 9 / ratioH;
                _mouseScaleY = 1;

                _mouseCenterX = (e.Width - (float)ratioH / 9) / 2;
                _mouseCenterY = 0;

                GL.Viewport((e.Width - ratioH / 9) / 2, 0, ratioH / 9, e.Height);
            }

        }




        /// <summary>
        /// Egér mozgatásának lekezelése.
        /// </summary>
        /// <param name="e">egér mozgatásának információi</param>
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (_model.IsGameRunning)
            {
                _mousePositionRaw = new Vector2((e.Position.X - _mouseCenterX) * _mouseScaleX / _width, (e.Position.Y - _mouseCenterY) * _mouseScaleY / _height);
                int size = _model.GameTable.TableSize;


                float refMelysegY = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column1[1]);

                float refMelysegX = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column0[0]);

                float gridPositionY = (_mousePositionRaw.Y - 0.5f) * refMelysegY - _camera.Position.Y;
                float gridPositionX = (_mousePositionRaw.X - 0.5f) * refMelysegX + _camera.Position.X;

                _mousePositionRaw.X = (_mousePositionRaw.X * 2) - 1;
                _mousePositionRaw.Y = 1 - (_mousePositionRaw.Y * 2);

                _mousePositionToTable = new Vector2(gridPositionX, gridPositionY);
            }
            base.OnMouseMove(e);
        }


        /// <summary>
        /// Vezérlői gyerekeire klikkelés lekezelése.
        /// </summary>
        /// <param name="position">klikkelés helye</param>
        /// <param name="control">vezérlő aminek a gyerekeit kezeljük le</param>
        /// <returns>lehetsége gyerek amire klikkeltünk</returns>
        private Control? GetChildOnClick(Vector2 position, Control control)
        {
            Control? result = null;
            bool found = false;
            for (int i = 0; i < control.Children.Count; i++)
            {
                var currC = control.Children[i];
                if (!currC.IsVisible)
                    continue;


                var currCH = GetChildOnClick(position, currC);
                if (currCH != null)
                {
                    currC = currCH;
                }
                else
                {
                    float descarteX = (position.X * 2) - 1;
                    float descarteY = 1 - (position.Y * 2);


                    if (currC.Vertices[5] < descarteX || currC.Vertices[0] > descarteX)
                        continue;

                    if (currC.Vertices[6] > descarteY || currC.Vertices[11] < descarteY)
                        continue;
                }


                if (!found)
                {
                    result = currC;
                    found = true;
                }
                else if (result != null && result.DisplayId > currC.DisplayId)
                {
                    result = currC;
                }
            }
            if (found && result != null && (result.OnClick != null || result.GetControlType() == ControlType.Textbox))
                return result;
            return null;
        }
        /// <summary>
        /// klikkelés lekezelés rekurziójának kezdete.
        /// </summary>
        /// <param name="position"></param>
        private void OnClick(Vector2 position)
        {

            Control? result = null;
            bool found = false;
            for (int i = 0; i < Child.Children.Count; i++)
            {
                var currC = Child.Children[i];
                if (!currC.IsVisible)
                    continue;

                var currCH = GetChildOnClick(position, currC);
                if (currCH != null)
                {
                    currC = currCH;
                }
                else
                {
                    float centralizedX = (position.X * 2) - 1;
                    float centralizedY = 1 - (position.Y * 2);


                    if (currC.Vertices[5] < centralizedX || currC.Vertices[0] > centralizedX)
                        continue;

                    if (currC.Vertices[6] > centralizedY || currC.Vertices[11] < centralizedY)
                        continue;
                }


                if (!found)
                {
                    result = currC;
                    found = true;
                }
                else if (result != null && result.DisplayId > currC.DisplayId)
                {
                    result = currC;
                }
            }
            if (result != Control.focused)
            {

                if (Control.focused != null && Control.focused.GetControlType() == ControlType.Textbox)
                {
                    ((TextBox)Control.focused).Caret.IsVisible = false;
                }
                Control.focused = result;
                if (result != null && result.GetControlType() == ControlType.Textbox)
                {
                    ((TextBox)result).Caret.IsVisible = true;
                }
            }
            if (found && result != null && result.OnClick != null)
            {
                mouseState = MouseClickState.ClickOrInfo;
                result.OnClick(result, EventArgs.Empty);
            }
            else if (!found && _model.IsGameRunning)
            {
                int size = _model.GameTable.TableSize;

                if (_mousePositionToTable.X < 0 || _mousePositionToTable.Y < 0)
                    return;
                if (_mousePositionToTable.X >= size || _mousePositionToTable.Y >= size)
                    return;

                switch (mouseState)
                {
                    case MouseClickState.ClickOrInfo:
                        {
                            Buildings clickedBuilding = _model.GameTable[(int)_mousePositionToTable.X, (int)_mousePositionToTable.Y].Building;

                            bool wasShowed = false;

                            wasShowed = Child.Children.Contains(_informationWindow.Container);
                            if (wasShowed)
                            {
                                Child.Children.Remove(_informationWindow.Container);
                            }

                            bool visitorClicked = false;
                            if (clickedBuilding is Pier || clickedBuilding is Road || clickedBuilding is null)
                            {

                                foreach (var v in _model.GameTable.Visitors)
                                {
                                    Point clickedTile = new Point((int)_mousePositionToTable.X, (int)_mousePositionToTable.Y);
                                    if (clickedTile == v.Coords || clickedTile == v.NextTile)
                                    {

                                        Vector2? qmPos = GetMousePrecisionPoint(v);
                                        if (qmPos == null)
                                            continue;
                                        Vector2 mPos = qmPos.Value;
                                        if (MathF.Abs(mPos.X - _mousePositionToTable.X) <= 0.15 && MathF.Abs(mPos.Y + _mousePositionToTable.Y) <= 0.15)
                                        {
                                            visitorClicked = true;
                                            if (!_informationWindow.IsBuilding || !wasShowed || _informationWindow.ActualVisitor != v)
                                            {
                                                _informationWindow = new InformationWindow(v, _model);
                                                Child.Children.Add(_informationWindow.Container);
                                            }
                                            break;
                                        }
                                    }
                                }

                            }
                            if (visitorClicked)
                                break;


                            if (clickedBuilding != null)
                            {

                                if (_informationWindow.ActualBuilding != clickedBuilding || !wasShowed || !_informationWindow.IsBuilding)
                                {
                                    _informationWindow = new InformationWindow(clickedBuilding, _model);
                                    Child.Children.Add(_informationWindow.Container);
                                }
                            }

                        }
                        break;
                    case MouseClickState.Build:
                        {
                            var bSize = _model.BuildingData.GetBuildingType(BuildId).Size;
                            float XRadius = bSize.X / 2.0f;
                            float YRadius = bSize.Y / 2.0f;

                            float refBX = (_mousePositionToTable.X - XRadius + 0.5f);
                            float refBY = (_mousePositionToTable.Y - YRadius + 0.5f);
                            if (refBX < 0)
                                refBX--;
                            if (refBY < 0)
                                refBY--;

                            int buildX = (int)refBX;
                            int buildY = (int)refBY;
                            if (_model.Build(BuildId, new Point(buildX, buildY)))
                            {
                                if (!_shiftDown)
                                {
                                    mouseState = MouseClickState.ClickOrInfo;
                                }
                            }
                        }
                        break;
                    case MouseClickState.Destroy:
                        {
                            if (_model.Destroy((int)_mousePositionToTable.X, (int)_mousePositionToTable.Y))
                            {

                                if (!_shiftDown)
                                    mouseState = MouseClickState.ClickOrInfo;
                            }
                        }
                        break;
                }

            }

        }


        /// <summary>
        /// Egérkattintás lekezelése.
        /// </summary>
        /// <param name="e">egérkattintás adatai</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.IsPressed)
            {
                var mouse = MouseState;
                switch (e.Button)
                {
                    case MouseButton.Left:
                        OnClick(new Vector2((mouse.Position.X - _mouseCenterX) * _mouseScaleX / _width, (mouse.Position.Y - _mouseCenterY) * _mouseScaleY / _height));
                        break;
                    case MouseButton.Right:
                        break;
                    case MouseButton.Middle:
                        break;
                    default:
                        break;
                }
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Billenytűlenyomás kezelése.
        /// </summary>
        /// <param name="e">billenytűlenyomás adatai</param>
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            var input = KeyboardState;
            switch (e.Key)
            {
                case Keys.Backspace:
                    {
                        if (Control.focused != null && Control.focused.GetControlType() == ControlType.Textbox)
                        {
                            TextBox textbox = (TextBox)Control.focused;
                            if (e.Key == Keys.Backspace)
                                textbox.RemoveLastChar();
                        }
                    }
                    break;
                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Valósidejű futást lekezelőesemény
        /// </summary>
        /// <param name="e">időzítés adatai</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused)
            {
                base.OnUpdateFrame(e);
                return;
            }
            if (_model.IsGameRunning && _model.GameTable.ParkStatus == MapModel.ParkStatusEnum.Open)
            {
                switch (_model.GameSpeed)
                {
                    case SpeedEnum.Pause:
                        break;
                    case SpeedEnum.Slow:
                        _delay += 1;
                        break;
                    case SpeedEnum.Normal:
                        _delay += 2;
                        break;
                    case SpeedEnum.Fast:
                        _delay += 4;
                        break;
                    default:
                        break;
                }
                if (_delay >= 4)
                {
                    _delay = 0;
                    _model.Tick();
                    _informationWindow.InformationStatusChanged();
                }
            }

            if (VisualData.AnimationStepper != null)
            {
                VisualData.AnimationStepper(this, EventArgs.Empty);
            }

            var input = KeyboardState;


            _shiftDown = input.IsKeyDown(Keys.LeftShift);

            var mouse = MouseState;


            if (!(Control.focused != null && Control.focused.GetControlType() == ControlType.Textbox) && _model.IsGameRunning)
            {
                float cameraSpeedArea = 1f * _camera.Position.Z;
                float cameraSpeedDeep = 1f + _camera.Position.Z + 10;
                bool isMoved = false;
                if (input.IsKeyDown(Keys.W))
                {
                    var nextPos = _camera.Position + _camera.Up * cameraSpeedArea * (float)e.Time;
                    if (nextPos.Y > 0)
                        nextPos.Y = 0;
                    _camera.Position = nextPos;
                    isMoved = true;
                    //_camera.Position += _camera.Up * cameraSpeedArea * (float)e.Time;
                }

                if (input.IsKeyDown(Keys.S))
                {
                    var nextPos = _camera.Position - _camera.Up * cameraSpeedArea * (float)e.Time;
                    if (nextPos.Y < -_model.GameTable.TableSize)
                        nextPos.Y = -_model.GameTable.TableSize;
                    _camera.Position = nextPos;
                    isMoved = true;
                    //_camera.Position -= _camera.Up * cameraSpeedArea * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.A))
                {
                    var nextPos = _camera.Position - _camera.Right * cameraSpeedArea * (float)e.Time;
                    if (nextPos.X < 0)
                        nextPos.X = 0;
                    _camera.Position = nextPos;
                    isMoved = true;
                    //_camera.Position -= _camera.Right * cameraSpeedArea * (float)e.Time;

                }
                if (input.IsKeyDown(Keys.D))
                {
                    var nextPos = _camera.Position + _camera.Right * cameraSpeedArea * (float)e.Time;
                    if (nextPos.X > _model.GameTable.TableSize)
                        nextPos.X = _model.GameTable.TableSize;
                    _camera.Position = nextPos;
                    isMoved = true;
                    //_camera.Position += _camera.Right * cameraSpeedArea * (float)e.Time;
                }

                Vector2 scroll = mouse.ScrollDelta;
                if (scroll.Y != 0)
                {
                    float upperBound = 1.6f * _model.GameTable.TableSize;
                    var nextPos = _camera.Position + scroll.Y * _camera.Front * cameraSpeedDeep * (float)e.Time;
                    if (nextPos.Z > 2 && nextPos.Z < upperBound)
                        _camera.Position += scroll.Y * _camera.Front * cameraSpeedDeep * (float)e.Time;
                    else if (_camera.Position.Z <= 2)
                    {
                        _camera.Position = new Vector3(_camera.Position.X, _camera.Position.Y, 2);
                    }
                    else if (nextPos.Z >= upperBound)
                    {
                        _camera.Position = new Vector3(_camera.Position.X, _camera.Position.Y, upperBound);
                    }
                    isMoved = true;
                }

                if (isMoved)
                {
                    Vector2 position = new Vector2((mouse.Position.X - _mouseCenterX) * _mouseScaleX / _width, (mouse.Position.Y - _mouseCenterY) * _mouseScaleY / _height);
                    int size = _model.GameTable.TableSize;


                    float refMelysegY = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column1[1]);

                    float refMelysegX = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column0[0]);

                    float GridPositionY = (position.Y - 0.5f) * refMelysegY - _camera.Position.Y;
                    float GridPositionX = (position.X - 0.5f) * refMelysegX + _camera.Position.X;

                    _mousePositionToTable = new Vector2(GridPositionX, GridPositionY);
                }

            }


            base.OnUpdateFrame(e);
        }

        /// <summary>
        /// Játék elindításakor alphelyzetbe visszaállító függvény
        /// </summary>
        public void InitGameScene()
        {
            _camera.Position = new Vector3(_model.GameTable.TableSize / (float)2, (-_model.GameTable.TableSize / (float)2), 10);

            _model.OnMoneyChanged?.Invoke(this, _model.GameTable.Balance);
            _model.OnTimeChanged?.Invoke(this, _model.GameTable.CurrentTime);
            _model.OnGuestNumChanged?.Invoke(this, _model.GameTable.Visitors.Count);
        }

        #endregion

        #region EvenHandlers
        public EventHandler OnGameLoad;
        #endregion

    }
}
