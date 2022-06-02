using MapModel.Building;
using MapModel.Facility;
using MapModel.Visitors;
using Model;
using Model.BuildingConstructors;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Linq;

namespace View
{
    /// <summary>
    /// Információs ablak oszálya
    /// </summary>
    internal class InformationWindow 
    {
        #region Constants
        internal readonly bool IsBuilding;
        #endregion

        #region Fields
        internal Buildings ActualBuilding;
        internal ImageControl Container;
        GameModel Model;


        internal TextBox BuildingStatus;
        internal TextBox VisitorUsingContext;
        internal TextBox VisitorWaitingContext;


        internal Visitor ActualVisitor;
        internal TextBox ActivityData;
        internal TextBox MoodData;
        internal TextBox FoodData;
        internal TextBox HappinessData;
        internal TextBox MoneyData;
        internal TextBox BladderData;
        #endregion

        #region Contructor
        /// <summary>
        /// Információs ablak létrehozása vendéghez.
        /// </summary>
        /// <param name="actualVisitor">vendég akinek az információját kiírjuk</param>
        /// <param name="model">model amihhez az adatai kötve vannak</param>
        internal InformationWindow(Visitor actualVisitor, GameModel model)
        {
            ActualVisitor = actualVisitor;
            IsBuilding = false;
            Model = model;
            Container = new ImageControl()
            {
                Position = new Point(10, 80),
                Size = new Size(600, 700),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
            };

            //Container.SetColor(new Color4(30, 100, 200, 255));
            Container.SetImage("InfoWindow");

            AddMouseDynamicData(20, $"VISITOR");

            AddMouseDynamicData(100, $"Visitor id: {actualVisitor.Id}");

            ActivityData = new TextBox();
            AddMouseDynamicData(180, $"Current activity: {actualVisitor.CurrentActivity}", ActivityData);
            MoodData = new TextBox();
            AddMouseDynamicData(260, $"Current mood: {actualVisitor.CurrentMood}", MoodData);
            FoodData = new TextBox();
            AddMouseDynamicData(340, $"Current food: {actualVisitor.FoodLevel}", FoodData);
            HappinessData = new TextBox();
            AddMouseDynamicData(420, $"Current happiness: {actualVisitor.HappinessLevel}", HappinessData);
            BladderData = new TextBox();
            AddMouseDynamicData(500, $"Current bladder: {actualVisitor.BladderLevel}", BladderData);
            MoneyData = new TextBox();
            AddMouseDynamicData(580, $"Current money: {actualVisitor.Money}", MoneyData);

        }

        /// <summary>
        /// Információs ablak létrehozása épülethez.
        /// </summary>
        /// <param name="actualBuilding">épület aminek az információját kiírjuk</param>
        /// <param name="model">model amihhez az adatai kötve vannak</param>
        internal InformationWindow(Buildings actualBuilding, GameModel model)
        {
            IsBuilding = true;
            ActualBuilding = actualBuilding;
            Model = model;
            Container = new ImageControl()
            {
                Position = new Point(10, 80),
                Size = new Size(600, 720),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
            };
            //Container.SetColor(new Color4(30, 100, 200, 255));
            Container.SetImage("InfoWindow");


            switch (ActualBuilding)
            {
                case Road R:
                    SetRoad(R);
                    break;
                case Pier P:
                    SetPier(P);
                    break;
                case Entrance E:
                    SetEntrance(E);
                    break;
                case Generator G:
                    SetGenerator(G);
                    break;
                case Facilities F:
                    SetFacilities(F);
                    break;
                default:
                    break;
            }

        }

        #endregion

        #region Methods
        /// <summary>
        /// Változó státuszok frissítésének függvénye.
        /// </summary>
        public void InformationStatusChanged()
        {
            if (IsBuilding)
            {
                switch (ActualBuilding)
                {
                    case Facilities F:
                        FacilityChanged(F);
                        break;
                    case Entrance E:
                        break;
                    case Generator G:
                        GeneratorChanged(G);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ActivityData.Text = $"Current activity: {ActualVisitor.CurrentActivity}";
                MoodData.Text = $"Current mood: {ActualVisitor.CurrentMood}";
                FoodData.Text = $"Current food: {ActualVisitor.FoodLevel}";
                HappinessData.Text = $"Current happiness: {ActualVisitor.HappinessLevel}";
                BladderData.Text = $"Current bladder: {ActualVisitor.BladderLevel}";
                MoneyData.Text = $"Current money: {ActualVisitor.Money}";
            }
        }
        /// <summary>
        /// Információs sor építő konstruktora redundancia csökkentésére.
        /// </summary>
        /// <param name="Yposition">y tengelyen eltolás</param>
        /// <param name="information">kiírandó információ</param>
        private void AddMouseDynamicData(int Yposition, string information)
        {
            TextBox dataHolder = new TextBox()
            {
                Position = new Point(0, Yposition),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            dataHolder.Text = information;
            dataHolder.Size = dataHolder.Content.Size;

            Container.Children.Add(dataHolder);
        }
        /// <summary>
        /// Információs sor építő konstruktora redundancia csökkentésére.
        /// </summary>
        /// <param name="Yposition">y tengelyen eltolás</param>
        /// <param name="information">kiírandó információ</param>
        /// /// <param name="dataHolder">Adatátoló szövegdoboz</param>
        private void AddMouseDynamicData(int Yposition, string information, TextBox dataHolder)
        {
            dataHolder.Position = new Point(0, Yposition);
            dataHolder.VerticalOrientation = VerticalOrientation.Top;
            dataHolder.HorizontalOrientation = HorizontalOrientation.Center;
            dataHolder.IsReadOnly = true;
            dataHolder.FontSize = 20;
            dataHolder.Text = information;
            dataHolder.Size = dataHolder.Content.Size;

            Container.Children.Add(dataHolder);
        }

        /// <summary>
        /// Információs sor építő konstruktora redundancia csökkentésére.
        /// </summary>
        /// <param name="Yposition">y tengelyen eltolás</param>
        /// <param name="inforamtion">kiírandó információ</param>
        private void AddConstInformation(int Yposition, string inforamtion)
        {
            TextBox constData = new TextBox()
            {
                Position = new Point(0, Yposition),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            constData.Text = inforamtion;
            constData.Size = constData.Content.Size;
            Container.Children.Add(constData);
        }
        /// <summary>
        /// Stég adatok beállítása.
        /// </summary>
        /// <param name="pier">Stég mely az adatokkal rendelkezik</param>
        private void SetPier(Pier pier)
        {
            AddConstInformation(40, "PIER");

            Container.Size = new Size(500, 100);
        }
        /// <summary>
        /// Út adatok beállítása.
        /// </summary>
        /// <param name="road">Út mely az adatokkal rendelkezik</param>
        private void SetRoad(Road road)
        {
            AddConstInformation(40, "ROAD");

            Container.Size = new Size(500, 100);
        }
        /// <summary>
        /// Bejárat adatok beállítása.
        /// </summary>
        /// <param name="entrance">Bejárat mely az adatokkal rendelkezik</param>
        private void SetEntrance(Entrance entrance)
        {
            BuildingBaseData buildingTypeData = Model.BuildingData.GetBuildingType(entrance.BuildingTypeId);
            TextBox type = new TextBox()
            {
                Position = new Point(0, 60),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            type.Text = buildingTypeData.Name.ToUpper();

            TextBox radius = new TextBox()
            {
                Position = new Point(0, 100),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            radius.Text = $"Power Radius: {entrance.Radius}";
            radius.Size = radius.Content.Size;


            TextBox feeText = new TextBox()
            {
                Position = new Point(40, 180),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
                IsReadOnly = true,
                FontSize = 20,
            };
            feeText.Text = "Entrance fee:";
            feeText.Size = feeText.Content.Size;


            TextBox fee = new TextBox()
            {
                Position = new Point(-40, 180),
                Size = new Size(140, 50),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
                IsReadOnly = false,
                IsDecimalOnly = true,
                FontSize = 20,
            };
            fee.SetColor(new Color4(255, 255, 255, 255));
            fee.Text = $"{Model.GameTable.EntranceTicket}";
            fee.DataContext = entrance;
            fee.TextChanged += EntranceTicket_TextChanged;





            Container.Size = new Size(500, 300);
            Container.Children.Add(type);
            Container.Children.Add(radius);
            Container.Children.Add(feeText);
            Container.Children.Add(fee);
        }

        /// <summary>
        /// Generátor adatok beállítása.
        /// </summary>
        /// <param name="generator">Generátor mely az adatokkal rendelkezik</param>
        private void SetGenerator(Generator generator)
        {
            BuildingBaseData buildingTypeData = Model.BuildingData.GetBuildingType(generator.BuildingTypeId);

            AddConstInformation(20, buildingTypeData.Name.ToUpper());

            AddConstInformation(100, $"Power Radius: {generator.Radius}");

            BuildingStatus = new TextBox()
            {
                Position = new Point(0, 180),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            BuildingStatus.Text = $"Generator status: {generator.Status}";
            BuildingStatus.Size = BuildingStatus.Content.Size;
            Container.Children.Add(BuildingStatus);

            Container.Size = new Size(700, 250);
        }

        /// <summary>
        /// Létesítmény adatok beállítása.
        /// </summary>
        /// <param name="facility">Létesítmény mely az adatokkal rendelkezik</param>
        private void SetFacilities(Facilities facility)
        {
            FacilityData buildingTypeData = (FacilityData)Model.BuildingData.GetBuildingType(facility.BuildingTypeId);

            AddConstInformation(60, $"{buildingTypeData.Name.ToUpper()}");

            AddConstInformation(100, $"Capacity: {buildingTypeData.Capacity}");

            if (facility is Ride)
            {
                TextBox minUsageText = new TextBox()
                {
                    Position = new Point(40, 180),
                    VerticalOrientation = VerticalOrientation.Top,
                    HorizontalOrientation = HorizontalOrientation.Left,
                    IsReadOnly = true,
                    FontSize = 20,
                };
                minUsageText.Text = "Mininum Visitor%: ";
                minUsageText.Size = minUsageText.Content.Size;
                TextBox minUsage = new TextBox()
                {
                    Position = new Point(-40, 180),
                    Size = new Size(140, 50),
                    VerticalOrientation = VerticalOrientation.Top,
                    HorizontalOrientation = HorizontalOrientation.Right,
                    IsReadOnly = false,
                    IsDecimalOnly = true,
                    FontSize = 20,
                };
                minUsage.SetColor(new Color4(255, 255, 255, 255));
                minUsage.Text = $"{facility.MinUsage}";
                minUsage.DataContext = facility;
                minUsage.TextChanged += FacilityMinUsage_TextChanged;

                Container.Children.Add(minUsageText);
                Container.Children.Add(minUsage);
            }

            AddConstInformation(260, $"Build time: {buildingTypeData.BuildTime}");

            AddConstInformation(340, $"Upkeep cost: {buildingTypeData.UpkeepCost}");

            AddConstInformation(420, $"Interaction Length: {buildingTypeData.InteractionLength}");



            BuildingStatus = new TextBox()
            {
                Position = new Point(0, 500),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            BuildingStatus.Text = $"Facility status: {facility.Status}";
            BuildingStatus.Size = BuildingStatus.Content.Size;
            Container.Children.Add(BuildingStatus);

            VisitorUsingContext = new TextBox()
            {
                Position = new Point(0, 580),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            VisitorUsingContext.Text = $"Visitors using: {facility.UserList.Count()}";
            VisitorUsingContext.Size = VisitorUsingContext.Content.Size;
            Container.Children.Add(VisitorUsingContext);

            VisitorWaitingContext = new TextBox()
            {
                Position = new Point(0, 660),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Center,
                IsReadOnly = true,
                FontSize = 20,
            };
            VisitorWaitingContext.Text = $"Visitors waiting: {facility.VisitorQueue.Count()}";
            VisitorWaitingContext.Size = VisitorWaitingContext.Content.Size;
            Container.Children.Add(VisitorWaitingContext);

            TextBox feeText = new TextBox()
            {
                Position = new Point(40, 740),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Left,
                IsReadOnly = true,
                FontSize = 20,
            };
            feeText.Text = "Facility fee:";
            feeText.Size = feeText.Content.Size;


            TextBox fee = new TextBox()
            {
                Position = new Point(-40, 740),
                Size = new Size(180, 50),
                VerticalOrientation = VerticalOrientation.Top,
                HorizontalOrientation = HorizontalOrientation.Right,
                IsReadOnly = false,
                IsDecimalOnly = true,
                FontSize = 20,
            };
            fee.SetColor(new Color4(255, 255, 255, 255));
            fee.Text = $"{facility.Fee}";
            fee.DataContext = facility;
            fee.TextChanged += FacilityFee_TextChanged;

            Container.Children.Add(feeText);
            Container.Children.Add(fee);

            Container.Size = new Size(600, 860);
        }

        /// <summary>
        /// Generátor adat változása
        /// </summary>
        /// <param name="generator">generátor melynek az adatai változhatnak</param>
        private void GeneratorChanged(Generator generator)
        {
            BuildingStatus.Text = $"Facility status: {generator.Status}";
            BuildingStatus.Size = BuildingStatus.Content.Size;

        }
        /// <summary>
        /// Létesítmény adat változása
        /// </summary>
        /// <param name="facility">létesítmény melynek az adatai változhatnak</param>
        private void FacilityChanged(Facilities facility)
        {
            BuildingStatus.Text = $"Facility status: {facility.Status}";
            BuildingStatus.Size = BuildingStatus.Content.Size;

            VisitorUsingContext.Text = $"Visitors using: {facility.UserList.Count()}";
            VisitorUsingContext.Size = VisitorUsingContext.Content.Size;

            VisitorWaitingContext.Text = $"Visitors waiting: {facility.VisitorQueue.Count()}";
            VisitorWaitingContext.Size = VisitorWaitingContext.Content.Size;

        }

        #endregion

        #region EvenHandlers
        private void FacilityMinUsage_TextChanged(object? sender, string e)
        {
            if (sender == null)
                return;
            Facilities facility = (Facilities)((Control)sender).DataContext;
            try
            {
                facility.MinUsage = int.Parse(e);
            }
            catch (FormatException)
            {
                facility.MinUsage = 0;
            }
            catch (OverflowException)
            {
                facility.MinUsage = 100;
                TextBox tb = (TextBox)sender;
                tb.Text = $"{facility.MinUsage}";
            }
            if (facility.MinUsage > 100)
            {
                facility.MinUsage = 100;
                TextBox tb = (TextBox)sender;
                tb.Text = $"{facility.MinUsage}";
            }
        }
        private void FacilityFee_TextChanged(object? sender, string e)
        {
            if (sender == null)
                return;
            Facilities facility = (Facilities)((Control)sender).DataContext;
            try
            {
                facility.Fee = int.Parse(e);
            }
            catch (FormatException)
            {
                facility.Fee = 0;
            }
            catch (OverflowException)
            {
                facility.Fee = 10000;
                TextBox tb = (TextBox)sender;
                tb.Text = $"{facility.Fee}";
            }
            if (facility.Fee > 10000)
            {
                facility.Fee = 10000;
                TextBox tb = (TextBox)sender;
                tb.Text = $"{facility.Fee}";
            }


        }

        private void EntranceTicket_TextChanged(object? sender, string e)
        {
            if (sender == null)
                return;
            try
            {
                Model.SetTicket(int.Parse(e));
            }
            catch (FormatException)
            {
                Model.SetTicket(0);
            }
            catch (OverflowException)
            {
                Model.SetTicket(10000);
                TextBox tb = (TextBox)sender;
                tb.Text = $"{Model.GameTable.EntranceTicket}";
            }
            if (Model.GameTable.EntranceTicket > 10000)
            {
                Model.SetTicket(10000);
                TextBox tb = (TextBox)sender;
                tb.Text = $"{Model.GameTable.EntranceTicket}";
            }
        }

        #endregion

    }
}
