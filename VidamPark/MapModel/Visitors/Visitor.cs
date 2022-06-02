using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Xml.Serialization;
using MapModel.Building;

namespace MapModel.Visitors
{
    /// <summary>
    /// Látogatók lehetséges minimum szüségleteinek enumerátora
    /// </summary>
    enum MinNecessity
    {
        Happiness,
        Food,
        Bladder
    }

    /// <summary>
	/// Látogatók típusa
	/// </summary>
    [XmlInclude(typeof(Visitor))]
    [Serializable]
    public class Visitor
    {
        #region Constants

        public const int MINMONEY = 0;
        public const int MAXMONEY = 1500;
        public const int CRITICALLEVEL = 30;
        public const int MOVEREQUIRED = 30;
        public const int NECESSITYREQUIRED = 240;

        #endregion

        #region Properties
        public Vector4 SkinColor { get; set; }
        public int Id { get; set; }
        public Point Coords { get; set; }
        public Activity CurrentActivity { get; set; }
        public Mood CurrentMood { get; set; }
        [XmlIgnore]
        public Buildings CurrentBuilding { get; set; }
        public Vector2 InTileCoords { get; set; }
        public int FoodLevel { get; set; }
        public int HappinessLevel { get; set; }
        public int Money { get; set; }
        public int BladderLevel { get; set; }
        public int InteractionTime { get; set; }
        [XmlIgnore]
        public Queue<Point> Path { get; set; }
        public List<Point> PathSerializable { get; set; }
        public Point NextTile { get; set; }
        public Point PrevTile { get; set; }

        public int RestingTime { get; set; }
        public int MoveTick { get; set; }
        [XmlIgnore]

        public static Random Rand = new Random();

        public int NecessityTick { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Látogató példányosítása.
        /// </summary>
        public Visitor(int id, Point coords)
        {
            GenerateInTileCoords();
            Id = id;
            MoveTick = 0;
            RestingTime = 0;
            NecessityTick = 0;
            Money = Rand.Next(MINMONEY, MAXMONEY + 1);
            FoodLevel = 100;
            HappinessLevel = 100;
            BladderLevel = 0;
            Coords = coords;
            NextTile = coords;
            PrevTile = coords;
            CurrentBuilding = null;
            CurrentActivity = Activity.Nothing;
            CurrentMood = Mood.Nothing;
            Path = new Queue<Point>();
            SkinColor = new Vector4()
            {
                X = (float)Rand.NextDouble(),
                Y = (float)Rand.NextDouble(),
                Z = (float)Rand.NextDouble(),
                W = 1.0f,
            };
        }

        /// <summary>
        /// Üres példányosítás.
        /// </summary>
        public Visitor() { }
        

        #endregion

        #region Functions

        /// <summary>
        /// Tile-on belül rendomizál pozíciót.
        /// </summary>
        public void GenerateInTileCoords()
        {
            float x = Rand.Next(701) / 1000.0f - 0.35f;
            float y = Rand.Next(701) / 1000.0f - 0.35f;
            InTileCoords = new Vector2(x, y);
        }

        /// <summary>
        /// Kezeli a látogatók szükségleteinek változását.
        /// </summary>
        /// <returns>Megváltozott-e a szükségletének állapota</returns>
        public bool NecessityChange()
        {
            NecessityTick++;
            if(NecessityTick==NECESSITYREQUIRED)
            {
                NecessityTick = 0;

                Mood prevMood = CurrentMood;
                FoodChange(-2);
                BladderChange(-3);
                if (CurrentActivity == Activity.Waiting)
                {
                    HappinessChange(-8);
                }
                else
                {
                    HappinessChange(-5);
                }
                return prevMood!=CurrentMood;
            }
            return false;
        }

        /// <summary>
        /// Mozgatja a látogató koordinátáját.
        /// </summary>
        /// <returns>Mozgott-e a látogató</returns>
        public bool Move()
        {
            MoveTick++;
            if (MoveTick< MOVEREQUIRED)
            {
                return false;
            }
            MoveTick = 0;

            if (Path == null) return false;

            PrevTile = Coords;
            Coords = NextTile;
            if (!Path.Any() && Coords == NextTile)
            {
                switch (CurrentActivity)
                {
                    case Activity.Moving:
                        CurrentActivity = Activity.Arrived;
                        GenerateInTileCoords();
                        break;
                    case Activity.Arrived:
                        if (CurrentMood == Mood.WantsToLeave)
                            CurrentActivity = Activity.Left;
                        break;
                }
            }

            if (Coords == NextTile && Path.Any())
                NextTile = Path.Dequeue();

            return true;
        }
        
        /// <summary>
        /// Kezeli a látogatók pihenőjét.
        /// </summary>
        /// <returns>Igaz, ha még pihennek</returns>
        public bool Resting()
        {
            RestingTime++;
            if (RestingTime == 150)
            {
                RestingTime = 0;
                CurrentActivity = Activity.Nothing;
                MoodChange();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Szükségletekhez mérten változtatja a kedvet.
        /// </summary>
        public void MoodChange()
        {
            if (CurrentMood == Mood.WantsToLeave) return;
            switch (GetMin())
            {
                case MinNecessity.Happiness:
                    CurrentMood =  HappinessLevel == 0 ? Mood.WantsToLeave : Mood.WantsToPlay;
                    break;
                case MinNecessity.Food:
                    CurrentMood = Mood.WantsToEat;
                    break;
                case MinNecessity.Bladder:
                    CurrentMood = Mood.WantsToPee;
                    break;
                default:
                    CurrentMood = Mood.Nothing;
                    break;
            }
        }

        /// <summary>
        /// Növeli a boldogság mértékét i-vel.
        /// </summary>
        /// <param name="i">A plusz/minusz boldogság értéke</param>
        public void HappinessChange(int i)
        {
            HappinessLevel += i;

            if (HappinessLevel <= 0)
                HappinessLevel = 0;

            if (HappinessLevel>100)
                HappinessLevel = 100;

            MoodChange();
        }

        /// <summary>
        /// Növeli az éhség mértékét i-vel.
        /// </summary>
        /// <param name="i">A plusz/minusz éhség értéke</param>
        public void FoodChange(int i)
        {
            FoodLevel += i;
            
            if (FoodLevel < 0)
                FoodLevel = 0;
            
            if (FoodLevel > 100)
                FoodLevel = 100;

            if (FoodLevel <= CRITICALLEVEL)
                LowFood();

            MoodChange();
        }

        /// <summary>
        /// Növeli a szükséglet mértékét i-vel.
        /// </summary>
        /// <param name="i">A plusz/minusz szükséglet értéke</param>
        public void BladderChange(int i)
        {
            BladderLevel -= i;

            if (BladderLevel > 100)
                BladderLevel = 100;
            
            if (BladderLevel < 0)
                BladderLevel = 0; 

            if (BladderLevel >= 100-CRITICALLEVEL)
                HighBladder();

            MoodChange();
        }

        /// <summary>
        /// Levonja az adott pénzmennyiséget.
        /// </summary>
        /// <param name="i">A levonandó pénzmennyiség értéke</param>
        public void MoneyChange(int i)
        {
            Money -= Math.Abs(i);
            if (Money <= 0)
                CurrentMood = Mood.WantsToLeave;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Lementhető adattípússá változtatja az útvonalat.
        /// </summary>
        public void SavePath()
        {
            PathSerializable = new List<Point>();
            PathSerializable = Path.ToList();
        }

        /// <summary>
        /// Visszatölti sor típusba a lementett útvonalat.
        /// </summary>
        public void LoadPath()
        {
            Path = new Queue<Point>();
            if (PathSerializable == null)
                return;
            foreach (var point in PathSerializable)
            {
                Path.Enqueue(point);
            }
        }

        #endregion

        #region public methods
        /// <summary>
        /// Csökkenti a boldogságot ha alacsoncs az éh szint.
        /// </summary>
        public void LowFood()
        {
            HappinessChange(-25);
        }
        /// <summary>
        /// Csökkenti a boldogságot ha alacsoncs az szüséglet szint.
        /// </summary>
        public void HighBladder()
        {
            HappinessChange(-25);
        }

        /// <summary>
        /// Visszaadja a legalacsonyabb szükségletet.
        /// </summary>
        /// <returns>A legalacsonyabb szükséglethez tartozó enum értéket</returns>
        private MinNecessity GetMin()
        {
            if (100 - BladderLevel <= FoodLevel && 100 - BladderLevel <= HappinessLevel && 100 - BladderLevel <= 30)
            {
                return MinNecessity.Bladder;
            } 
            else if (FoodLevel <= HappinessLevel)
            {
                return MinNecessity.Food;
            }
            else
            {
                return MinNecessity.Happiness;
            }
        }
        
        #endregion
    }
}
