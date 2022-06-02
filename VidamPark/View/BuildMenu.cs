using Model.BuildingConstructors;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Model;

namespace View
{
    /// <summary>
    /// Építőlista típusát adja meg.
    /// </summary>
    public enum ActiveBuildList
    {
        Roads,
        Rides,
        Utilities,
        Generators,
        Hide,
    }
    /// <summary>
    /// Építőmenü vezérlő osztálya
    /// </summary>
    public class BuildMenuControl : ImageControl
    {
        internal override ControlType GetControlType()
        {
            return ControlType.BuildMenu;
        }
    }

    /// <summary>
    /// Építőmenü adattárolója
    /// </summary>
    public static class BuildMenu
    {

        #region Fields

        internal static MainWindow GameWindow;
        internal static GameModel Model;
        internal static Button Roads;
        internal static Button Rides;
        internal static Button Utilities;
        internal static Button Generators;
        internal static Button Hide;


        internal static List<Control> RoadsList = new List<Control>();
        internal static List<Control> RoadsInfoList = new List<Control>();
        internal static List<Control> RidesList = new List<Control>();
        internal static List<Control> RidesInfoList = new List<Control>();
        internal static List<Control> UtilitiesList = new List<Control>();
        internal static List<Control> UtilitiesInfoList = new List<Control>();
        internal static List<Control> GeneratorsList = new List<Control>();
        internal static List<Control> GeneratorsInfoList = new List<Control>();

        public static BuildMenuControl Container;



        internal static ActiveBuildList activeBuildList;

        static int _listButtonWidth = 160;
        static readonly int ButtonWidth = 180;
        internal static readonly int HideHeight = 160;
        #endregion

        #region Contructor
        /// <summary>
        /// Építőmenü adatároló inicializálása
        /// </summary>
        /// <param name="mw">Főablak meylre vonatkozni fog a építőlista</param>
        /// <param name="model">Modell ami a háttérinformációit kezeli a építőlistáknak</param>
        public static void InitBuildMenu(MainWindow mw, GameModel model)
        {
            GameWindow = mw;
            Model = model;
            activeBuildList = ActiveBuildList.Hide;
            Container = new BuildMenuControl()
            {
                Position = new Point(0, HideHeight),
                Size = new Size(0, 224),
                VerticalOrientation = VerticalOrientation.Bottom,
                HorizontalOrientation = HorizontalOrientation.Left,
                BackgroundColor = new OpenTK.Mathematics.Color4(0, 0, 0, 0),
            };

            Roads = GenButton(0 * ButtonWidth);
            Roads.Text = "Roads";
            Roads.DataContext = ActiveBuildList.Roads;
            Roads.OnClick += button_click;

            Rides = GenButton(1 * ButtonWidth);
            Rides.Text = "Rides";
            Rides.DataContext = ActiveBuildList.Rides;
            Rides.OnClick += button_click;

            Utilities = GenButton(2 * ButtonWidth);
            Utilities.Text = "Utilities";
            Utilities.DataContext = ActiveBuildList.Utilities;
            Utilities.OnClick += button_click;

            Generators = GenButton(3 * ButtonWidth);
            Generators.Text = "Generators";
            Generators.DataContext = ActiveBuildList.Generators;
            Generators.OnClick += button_click;

            Hide = GenButton(4 * ButtonWidth);
            Hide.SetImage("ButtonClose");
            Hide.Size = new Size(60, 60);
            Hide.Text = "";
            Hide.DataContext = ActiveBuildList.Hide;
            Hide.OnClick += button_click;
            Container.Children.Remove(Hide);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gomb építőmetódus, redundáns kód csökkentésére
        /// </summary>
        /// <param name="leftX">gomb x szerinti eltolása</param>
        /// <returns></returns>
        private static Button GenButton(int leftX)
        {
            Button button = new Button()
            {
                Position = new Point(leftX, 0),
                Size = new Size(ButtonWidth, 60),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            button.SetImage("ButtonCheese");
            button.FontSize = 18;
            Container.Children.Add(button);
            return button;
        }
        /// <summary>
        /// Megadott gombra a megfelelő építőlista jelenjen meg.
        /// </summary>
        /// <param name="sender">A gomb ami küldte az eseményt.</param>
        /// <param name="e">Eseménytulajodonságok hordozója.</param>
        private static void button_click(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                activeBuildList = (ActiveBuildList)(sender as Button).DataContext;
            }
        }

        /// <summary>
        /// Megadott építési kategóriához hozzáadja az épületlistát
        /// </summary>
        /// <param name="listType">épületkategória</param>
        /// <param name="buildList">épületlista</param>
        public static void SetBuildList(ActiveBuildList listType, List<Tuple<string, int>> buildList)
        {
            List<Control> currentList = new List<Control>();
            List<Control> currentInfoList = new List<Control>();
            switch (listType)
            {
                case ActiveBuildList.Roads:
                    currentList = RoadsList;
                    currentInfoList = RoadsInfoList;
                    break;
                case ActiveBuildList.Rides:
                    currentList = RidesList;
                    currentInfoList = RidesInfoList;
                    break;
                case ActiveBuildList.Utilities:
                    currentList = UtilitiesList;
                    currentInfoList = UtilitiesInfoList;
                    break;
                case ActiveBuildList.Generators:
                    currentList = GeneratorsList;
                    currentInfoList = GeneratorsInfoList;
                    break;
                case ActiveBuildList.Hide:
                    break;
            }
            BuildingData buildings = Model.BuildingData;
            for (int i = 0; i < buildList.Count; i++)
            {

                Button bb = new Button()
                {
                    DataContext = buildList[i].Item2,
                    Position = new Point(i * (_listButtonWidth + 2), 62),
                    Size = new Size(_listButtonWidth, _listButtonWidth),
                    VerticalOrientation = VerticalOrientation.Top,
                    HorizontalOrientation = HorizontalOrientation.Left,
                };
                bb.SetImage(buildList[i].Item1);
                bb.OnClick += BuildClick;

                currentList.Add(bb);

                ImageControl container = new ImageControl()
                {
                    Position = new Point(0, 0),
                    Size = new Size(1, 1),
                    VerticalOrientation = VerticalOrientation.Top,
                    HorizontalOrientation = HorizontalOrientation.Left,
                };
                container.SetColor(new Color4(0, 0, 0, 0));
                currentInfoList.Add(container);

                Label name = CreateLabel(new Point(i * (_listButtonWidth + 2), 62), $"{buildList[i].Item1}");
                container.Children.Add(name);

                Label prize = CreateLabel(new Point(i * (_listButtonWidth + 2), name.Position.Y + name.Size.Height), $"Price: {buildings.GetPrice(buildList[i].Item1)}");
                container.Children.Add(prize);

                string data = $"{buildings.GetBuildTime(buildList[i].Item1)}";
                if (data.Length == 0)
                    continue;
                Label buildTime = CreateLabel(new Point(i * (_listButtonWidth + 2), prize.Position.Y + prize.Size.Height), data);
                container.Children.Add(buildTime);

                data = $"{buildings.GetCapacityORPowerRadius(buildList[i].Item1)}";
                if (data.Length == 0)
                    continue;
                Label extraInfo = CreateLabel(new Point(i * (_listButtonWidth + 2), buildTime.Position.Y + buildTime.Size.Height), data);
                container.Children.Add(extraInfo);

                data = $"{buildings.GetSize(buildList[i].Item1)}";
                if (data.Length == 0)
                    continue;
                Label sizeInfo = CreateLabel(new Point(i * (_listButtonWidth + 2), extraInfo.Position.Y + extraInfo.Size.Height), data);
                container.Children.Add(sizeInfo);

                data = $"{buildings.GetUC(buildList[i].Item1)}";
                if (data.Length == 0)
                    continue;
                Label upkeepCost = CreateLabel(new Point(i * (_listButtonWidth + 2), sizeInfo.Position.Y + sizeInfo.Size.Height), data);
                container.Children.Add(upkeepCost);
            }
        }

        /// <summary>
        /// Cím építőmetódus, redundáns kód csökkentésére
        /// </summary>
        /// <param name="position">cím pozíciója</param>
        /// <param name="content">cím szövege</param>
        /// <returns></returns>
        private static Label CreateLabel(Point position, string content)
        {
            Label label = new Label()
            {
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
                Position = position,
                BackgroundColor = Color.White,
                FontSize = 12,
            };
            label.Text = content;

            return label;
        }
        /// <summary>
        /// épület id beállítása a pályára való építéshez
        /// </summary>
        /// <param name="sender">gomb ami küldi az eseményt</param>
        /// <param name="e">eseményparaméter</param>
        private static void BuildClick(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            int id = (int)((Control)sender).DataContext;
            if (id != -1)
            {
                GameWindow.mouseState = MouseClickState.Build;
            }
            else
            {
                GameWindow.mouseState = MouseClickState.Destroy;
            }
            GameWindow.BuildId = id;
        }
        #endregion


    }
}
