using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Drawing;
using View;
using Model;
using System.Collections.Generic;

namespace App
{
    /// <summary>
    /// A program belépési pontja, központi vezérlője
    /// </summary>
    internal class Program
    {
        static MainWindow game;
        static ImageControl mainMenu;
        static ImageControl newGameMenu;
        static ImageControl verifyExitGameMenu;
        static ImageControl verifyBackToGameMenu;
        static ImageControl gameWindow;

        static string parkName = "";
        static TextBox givenNameTextBox;
        static TextBox tableSizeTextBox;

        static TextBox moneyTextBox;
        static TextBox guestNumberTextBox;
        static TextBox timeTextBox;
        static Button pauseButton;

        static GameModel model;
        /// <summary>
        /// A program belépési pontja
        /// </summary>
        /// <param name="args">argumentomok</param>
        static void Main(string[] args)
        {
            GameWindowSettings gws = new GameWindowSettings();
            NativeWindowSettings nws = new NativeWindowSettings();

            nws.Size = new Vector2i(1280, 720);
            gws.UpdateFrequency = 60;
            //nws.WindowState = WindowState.Fullscreen;
            
            nws.Title = "\"Vidám\"Park";
            model = new GameModel();
            game = new MainWindow(gws,nws, model);
            game.VSync = VSyncMode.On;

            game.OnGameLoad += GameLoading; 

            game.Run();
        }

        /// <summary>
        /// Játék betöltése
        /// </summary>
        /// <param name="sender">Esemény küldő</param>
        /// <param name="e">Esemény argumentum</param>
        static void GameLoading(object sender, EventArgs e)
        {
            BuildMainMenu();
            game.Child = mainMenu;

            BuildNewGameMenu();
            BuildVerifyExitGameMenu();
            BuildVerifyLeaveGameMenu();
        }

        #region Menu systems building functions
        /// <summary>
        /// Új játék létrehozásának a menüje
        /// </summary>
        static void BuildNewGameMenu()
        {
            newGameMenu = new ImageControl()
            {
                Size = new Size(1920, 1080),
            };

            int fSize = 32;
            int height = 125;

            Button startButton = new Button()
            {
                Position = new Point(-250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            startButton.SetImage("ButtonCheese");
            startButton.FontSize = fSize;
            startButton.Text = "Start";
            newGameMenu.Children.Add(startButton);
            startButton.OnClick = StartGameClicked;

            Button backButton = new Button()
            {
                Position = new Point(250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            backButton.SetImage("ButtonCheese");
            backButton.FontSize = fSize;
            backButton.Text = "Back";
            newGameMenu.Children.Add(backButton);
            backButton.OnClick = BackGameClicked;

            TextBox parkNameTextBox = new TextBox()
            {
                Position = new Point(0, -200),
                Size = new Size((int)(height * 3.5f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            parkNameTextBox.SetImage("ButtonSilver");
            parkNameTextBox.TextChanged += ParkNameTextChanged;
            parkNameTextBox.Text = "Parkname";
            newGameMenu.Children.Add(parkNameTextBox);

            tableSizeTextBox = new TextBox()
            {
                Position = new Point(0, -40),
                Size = new Size((int)(height * 3.5f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            tableSizeTextBox.SetImage("ButtonSilver");
            tableSizeTextBox.IsDecimalOnly = true;
            tableSizeTextBox.Text = "10";
            tableSizeTextBox.TextChanged += MapSizeBoxTextChanged;
            newGameMenu.Children.Add(tableSizeTextBox);
        }

        /// <summary>
        /// A játék főmenüjének létrehozása
        /// </summary>
        static void BuildMainMenu()
        {
            mainMenu = new ImageControl()
            {
                Size = new Size(1920, 1080),
            };

            SolidBrush color = new SolidBrush(Color.FromArgb(255,0,0,0));
            int fSize = 48;
            int height = 200;

            Button newGameButton = new Button()
            {
                Position = new Point(0, -300),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Brush = color,
            };
            newGameButton.SetImage("ButtonCheese");
            newGameButton.FontSize = fSize;
            newGameButton.Text = "New game";
            mainMenu.Children.Add(newGameButton); 
            newGameButton.OnClick = NewGameClicked;

            Button loadGameButton = new Button()
            {
                Position = new Point(0, -50),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Brush = color,
            };
            loadGameButton.SetImage("ButtonCheese");
            loadGameButton.FontSize = fSize;
            loadGameButton.Text = "Load game";
            mainMenu.Children.Add(loadGameButton); 
            loadGameButton.OnClick = LoadGameClicked;

            Button exitGameButton = new Button()
            {
                Position = new Point(0, 200),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
                Brush = color,
            };
            exitGameButton.SetImage("ButtonCheese");
            exitGameButton.FontSize = fSize;
            exitGameButton.Text = "Exit";
            mainMenu.Children.Add(exitGameButton);
            exitGameButton.OnClick = ExitGameClicked;
        }
        /// <summary>
        /// Biztos ki akar-e lépni a játékbő menü
        /// </summary>
        static void BuildVerifyExitGameMenu()
        {
            verifyExitGameMenu = new ImageControl()
            {
                Size = new Size(1920, 1080),
            };

            int fSize = 32;
            int height = 125;

            Button cancelButton = new Button()
            {
                Position = new Point(-250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            cancelButton.SetImage("ButtonCheese");
            cancelButton.FontSize = fSize;
            cancelButton.Text = "Cancel";
            verifyExitGameMenu.Children.Add(cancelButton);
            cancelButton.OnClick = BackGameClicked;

            Button closeButton = new Button()
            {
                Position = new Point(250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            closeButton.SetImage("ButtonCheese");
            closeButton.FontSize = fSize;
            closeButton.Text = "Close";
            verifyExitGameMenu.Children.Add(closeButton);
            closeButton.OnClick = CloseGameClicked;

            Label messageLabel = new Label()
            {
                Position = new Point(0, -100),
                Size = new Size((0), 0),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            messageLabel.FontSize = fSize;
            messageLabel.Text = "Are you sure you want to exit the game?";
            verifyExitGameMenu.Children.Add(messageLabel);
        }
        //VerifyLeaveGameMenu: Cancel, Close, [MessageLabel]...
        /// <summary>
        /// Biztos ki akar-e lépni az éppen futó játékból menü
        /// </summary>
        static void BuildVerifyLeaveGameMenu()
        {
            verifyBackToGameMenu = new ImageControl()
            {
                Size = new Size(1920, 1080),
            };

            int fSize = 32;
            int height = 125;

            Button cancelButton = new Button()
            {
                Position = new Point(-250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            cancelButton.SetImage("ButtonCheese");
            cancelButton.FontSize = fSize;
            cancelButton.Text = "Cancel";
            verifyBackToGameMenu.Children.Add(cancelButton);
            cancelButton.OnClick = BackToRunningGameClicked;

            Button confirmButton = new Button()
            {
                Position = new Point(250, 100),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            confirmButton.SetImage("ButtonCheese");
            confirmButton.FontSize = fSize;
            confirmButton.Text = "Confirm";
            verifyBackToGameMenu.Children.Add(confirmButton);
            confirmButton.OnClick = BackToMainMenuClicked;

            Label messageLabel = new Label()
            {
                Position = new Point(0, -100),
                Size = new Size((0), 0),
                VerticalOrientation = VerticalOrientation.Center,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            messageLabel.FontSize = fSize;
            messageLabel.Text = "Are you sure you leave the current game?";
            verifyBackToGameMenu.Children.Add(messageLabel);
        }
        /// <summary>
        /// Játék elemeinek létrehozása
        /// </summary>
        static void BuildGame()
        {
            gameWindow = new ImageControl()
            {
                Size = new Size(1920, 1080),
            };
            int fSize = 15;
            int height = 50;
            int yDist = 10;
            string buttonImage = "ButtonSilver";

            Button mainMenuButton = new Button()
            {
                Position = new Point(10, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            mainMenuButton.SetImage(buttonImage);
            mainMenuButton.FontSize = fSize;
            mainMenuButton.Text = "Main Menu";
            gameWindow.Children.Add(mainMenuButton);
            mainMenuButton.OnClick = MainMenuClicked;

            Button saveButton = new Button()
            {
                Position = new Point(2+mainMenuButton.Position.X+mainMenuButton.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            saveButton.SetImage(buttonImage);
            saveButton.FontSize = fSize;
            saveButton.Text = "Save";
            gameWindow.Children.Add(saveButton);
            saveButton.OnClick = SaveClicked;
            
            pauseButton = new Button()
            {
                Position = new Point(2 + saveButton.Position.X + saveButton.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            pauseButton.SetImage(buttonImage);
            pauseButton.FontSize = fSize;
            pauseButton.Text = "Pause";
            gameWindow.Children.Add(pauseButton);
            pauseButton.OnClick =PauseClicked ;
            
            Button slowerButton = new Button()
            {
                Position = new Point(2 + pauseButton.Position.X + pauseButton.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            slowerButton.SetImage(buttonImage);
            slowerButton.FontSize = fSize;
            slowerButton.Text = "Slower";
            gameWindow.Children.Add(slowerButton);
            slowerButton.OnClick = SlowerClicked;
            
            Button fasterButton = new Button()
            {
                Position = new Point(2 + slowerButton.Position.X + slowerButton.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
            };
            fasterButton.SetImage(buttonImage);
            fasterButton.FontSize = fSize;
            fasterButton.Text = "Faster";
            gameWindow.Children.Add(fasterButton);
            fasterButton.OnClick = FasterClicked;

            givenNameTextBox = new TextBox()
            {
                Position = new Point(0, 10),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            givenNameTextBox.IsReadOnly = true;
            givenNameTextBox.SetImage(buttonImage);
            givenNameTextBox.FontSize = fSize;
            givenNameTextBox.Text = "";
            gameWindow.Children.Add(givenNameTextBox);

            moneyTextBox = new TextBox()
            {
                Position = new Point(-10, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
            };
            moneyTextBox.IsReadOnly = true;
            moneyTextBox.SetImage(buttonImage);
            moneyTextBox.FontSize = fSize;
            moneyTextBox.Text = "Money: ";
            gameWindow.Children.Add(moneyTextBox);
            
            guestNumberTextBox = new TextBox()
            {
                Position = new Point(2+moneyTextBox.Position.X-moneyTextBox.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
            };
            guestNumberTextBox.IsReadOnly = true;
            guestNumberTextBox.SetImage(buttonImage);
            guestNumberTextBox.FontSize = fSize;
            guestNumberTextBox.Text = "Guests: ";
            gameWindow.Children.Add(guestNumberTextBox);
            
            timeTextBox = new TextBox()
            {
                Position = new Point(2 + guestNumberTextBox.Position.X - guestNumberTextBox.Size.Width, yDist),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
            };
            timeTextBox.IsReadOnly = true;
            timeTextBox.SetImage(buttonImage);
            timeTextBox.FontSize = fSize;
            timeTextBox.Text = "Time: ";
            gameWindow.Children.Add(timeTextBox);

            Button openParkButton = new Button()
            {
                Position = new Point(0, 120),
                Size = new Size((int)(height * 3.3f), height),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
            };
            openParkButton.SetImage(buttonImage);
            openParkButton.FontSize = fSize;
            openParkButton.Text = "Open Park!";
            openParkButton.OnClick += OpenParkClicked;
            gameWindow.Children.Add(openParkButton);

            SetUpMenu();
        }
        #endregion

        #region Build BuildMenu structure
        /// <summary>
        /// Játékbeli menü létrehozása
        /// </summary>
        private static void SetUpMenu()
        {
            Tuple<string, int> destroy = new Tuple<string, int>("Destroy", -1);
            List<Tuple<string, int>> buildList = new List<Tuple<string, int>>();
            buildList.Add(new Tuple<string, int>(model.BuildingData.RoadData.Name, model.BuildingData.RoadData.Id));
            buildList.Add(new Tuple<string, int>(model.BuildingData.PierData.Name, model.BuildingData.PierData.Id));
            buildList.Add(new Tuple<string, int>(model.BuildingData.WaterData.Name, model.BuildingData.WaterData.Id));
            buildList.Add(new Tuple<string, int>(model.BuildingData.GrassData.Name, model.BuildingData.GrassData.Id));
            buildList.Add(destroy);
            BuildMenu.SetBuildList(ActiveBuildList.Roads,buildList);

            buildList = new List<Tuple<string, int>>();
            foreach (var rideData in model.BuildingData.RideData)
            {
                buildList.Add(new Tuple<string, int>(rideData.Value.Name, rideData.Key));
            }
            buildList.Add(destroy);
            BuildMenu.SetBuildList(ActiveBuildList.Rides, buildList);

            buildList = new List<Tuple<string, int>>();
            buildList.Add(new Tuple<string, int>(model.BuildingData.RestRoomData.Name, model.BuildingData.RestRoomData.Id));
            foreach (var cafeteriaData in model.BuildingData.ShopData)
            {
                buildList.Add(new Tuple<string, int>(cafeteriaData.Value.Name, cafeteriaData.Key));
            }
            buildList.Add(destroy);
            BuildMenu.SetBuildList(ActiveBuildList.Utilities, buildList);

            buildList = new List<Tuple<string, int>>();
            foreach (var generatorData in model.BuildingData.GeneratorData)
            {
                if(generatorData.Value.Name != "Entrance")
                    buildList.Add(new Tuple<string, int>(generatorData.Value.Name, generatorData.Key));
            }
            buildList.Add(destroy);
            BuildMenu.SetBuildList(ActiveBuildList.Generators, buildList);

            gameWindow.Children.Add(BuildMenu.Container);
        }
        #endregion

        #region Changed methods
        /// <summary>
        /// Pályaméret beálltás korlátozása
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        private static void MapSizeBoxTextChanged(object sender, string e)
        {
            TextBox textBox = (TextBox)sender;
            int size = 10;
            if (!int.TryParse(e, out size))
            {
                size = 10;
            }

            if (size > 100)
            {
                size = 100;
                textBox.Text = $"{size}";
            }
        }
        /// <summary>
        /// Pálya név beálltása
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        private static void ParkNameTextChanged(object sender, string e)
        {
            parkName = e;
        }

        /// <summary>
        /// Pénz változásának megjelenítése
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void MoneyChanged(object sender, int e)
        {
            moneyTextBox.Text = $"Money: {e}";
        }

        /// <summary>
        /// Látogatók száma változásának megjelenítése
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void GuestChanged(object sender, int e)
        {
            guestNumberTextBox.Text = $"Guests: {e}";
        }

        /// <summary>
        /// Idő változásának megjelenítése
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void TimeChanged(object sender, TimeSpan e)
        {
            timeTextBox.Text = $"Time: {e}";
        }
        #endregion

        #region ButtonClicked events

        /// <summary>
        /// Start gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void StartGameClicked(object sender, EventArgs e)
        {
            int size = 0;
            if(!int.TryParse(tableSizeTextBox.Text, out size))
            {
                size = 10;
            }

            if (size < 10 || size > 100)
            {
                tableSizeTextBox.Text = $"{10}";
                return;
            }

            BuildGame();
            model.OnMoneyChanged = MoneyChanged;
            model.OnGuestNumChanged = GuestChanged;
            model.OnTimeChanged = TimeChanged;

            model.NewGame(parkName, size);
            game.InitGameScene();
            givenNameTextBox.Text = parkName;
            game.Child = gameWindow;
        }

        /// <summary>
        /// Park megnyitása gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void OpenParkClicked(object sender, EventArgs e)
        {
            model.OpenPark();
            Button b = (Button)sender;
            b.IsVisible = false;
        }

        /// <summary>
        /// Vissza gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void BackGameClicked(object sender, EventArgs e)
        {
            game.Child = mainMenu;
        }
        /// <summary>
        /// Új játék gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void NewGameClicked(object sender, EventArgs e)
        {
            game.Child = newGameMenu;
        }
        /// <summary>
        /// Játék betöltése gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void LoadGameClicked(object sender, EventArgs e)
        {
            int size = int.Parse(tableSizeTextBox.Text);

            var path = game.OpenLoadFileDialog();
            if (path.Length == 0)
                return;
            model.LoadGame(path);

            BuildGame();
            model.OnMoneyChanged = MoneyChanged;
            model.OnGuestNumChanged = GuestChanged;
            model.OnTimeChanged = TimeChanged;

            game.InitGameScene();
            givenNameTextBox.Text = parkName;
            game.Child = gameWindow;
        }

        /// <summary>
        /// Kilépés gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void ExitGameClicked(object sender, EventArgs e)
        {
            game.Child = verifyExitGameMenu;

        }
        /// <summary>
        /// Close gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void CloseGameClicked(object sender, EventArgs e)
        {
            game.Close();
        }
        /// <summary>
        /// Back to game gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void BackToRunningGameClicked(object sender, EventArgs e)
        {
            game.Child = gameWindow;

        }
        /// <summary>
        /// Back to menu gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void BackToMainMenuClicked(object sender, EventArgs e)
        {
            model.IsGameRunning = false;
            game.Child = mainMenu;
        }

        /// <summary>
        /// Main menu gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void MainMenuClicked(object sender, EventArgs e)
        {
            model.GameSpeed = SpeedEnum.Pause;
            game.Child = verifyBackToGameMenu;
        }
        /// <summary>
        /// Save mentése gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static async void SaveClicked(object sender, EventArgs e)
        {
            var path = game.OpenSaveFileDialog();
            await path;
            if (path.Result.Length == 0)
                return;
            model.SaveGame(path.Result);
        }
        /// <summary>
        /// Pause gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void PauseClicked(object sender, EventArgs e)
        {
            if (model.IsGameRunning)
            {
                switch (model.GameSpeed)
                {
                    case SpeedEnum.Pause:
                        model.GameSpeed = SpeedEnum.Normal;
                        pauseButton.Text = "Pause";
                        break;
                    default:
                        model.GameSpeed = SpeedEnum.Pause;
                        pauseButton.Text = "Continue";
                        break;
                };
            }
        }

        /// <summary>
        /// Slower gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void SlowerClicked(object sender, EventArgs e)
        {
            if (model.IsGameRunning)
            {
                switch (model.GameSpeed)
                {
                    case SpeedEnum.Pause:
                        break;
                    case SpeedEnum.Slow:
                        break;
                    case SpeedEnum.Normal:
                        model.GameSpeed = SpeedEnum.Slow;
                        break;
                    case SpeedEnum.Fast:
                        model.GameSpeed = SpeedEnum.Normal;
                        break;
                    default:
                        break;
                };
            }
        }
        /// <summary>
        /// Faster gomb megnyomását érzékelő esemény
        /// </summary>
        /// <param name="sender">Esemény küldő azonosítója</param>
        /// <param name="e">Esemény argumentuma</param>
        static void FasterClicked(object sender, EventArgs e)
        {
            if (model.IsGameRunning)
            {
                switch (model.GameSpeed)
                {
                    case SpeedEnum.Pause:
                        break;
                    case SpeedEnum.Slow:
                        model.GameSpeed = SpeedEnum.Normal;
                        break;
                    case SpeedEnum.Normal:
                        model.GameSpeed = SpeedEnum.Fast;
                        break;
                    case SpeedEnum.Fast:
                        break;
                    default:
                        break;
                };
            }
        }
        #endregion
    }
}
