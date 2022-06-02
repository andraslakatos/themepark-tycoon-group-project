using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Persistence;

namespace Model
{
    /// <summary>
    /// Útvonalkereső algoritmus
    /// </summary>
    public class PathFinding
    {
        #region Constants

        private readonly int[] _rowInts = {0, -1, 0, 1};
        private readonly int[] _colInts = {-1, 0, 1, 0};

        #endregion

        #region Properties

        public static GameTable _table;

        public static int[,] Map { get; set; }

        #endregion

        #region Contructor

        /// <summary>
        /// PathFinding osztály példányosítása
        /// </summary>
        public PathFinding()
        {
            Map = new int[_table.TableSize, _table.TableSize];
            MapReset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Alaphelyzetbe állítja a pathfidnigra használt pályát.
        /// </summary>
        public void MapReset()
        {
            for (int i = 0; i < _table.TableSize; i++)
                for (int j = 0; j < _table.TableSize; j++)
                {
                    if (_table[i, j].BuildingType.Equals(TileType.Road) || _table[i, j].BuildingType.Equals(TileType.Pier) || _table[i, j].BuildingType.Equals(TileType.Entrance))
                    {
                        Map[i, j] = 0;
                    }
                    else
                    {
                        Map[i, j] = -1;
                    }
                }
        }

        /// <summary>
        /// Útvonal
        /// </summary>
        /// <param name="start">kiinduló pont azonosítója</param>
        /// <param name="dest">cél azonosítója</param>
        /// <returns></returns>
        public Queue<Point> PathFind(Point start, HashSet<Point> dest)
        {
            MapReset();
            var final = Bfs(start, dest);
            if (final == null) return new Queue<Point>();

            return GetPath((Point) final);
        }

        /// <summary>
        /// Breadth first search algoritmus, ami a legrövidebb utat keresi meg.
        /// </summary>
        /// <param name="start">Kiinduló pont azonosítója</param>
        /// <param name="dest">Cél azonosítója</param>
        /// <returns>Utolsó ponttal tér vissza, ha van</returns>
        private Point? Bfs(Point start, HashSet<Point> dest) // igaz ha elérte a célpontot, hamis ha nem
        {
            Map[start.X, start.Y] = 0;
            foreach (var point in dest)
            {
                Map[point.X, point.Y] = 0;    
            }

            var queue = new Queue<Point>();

            queue.Enqueue(start);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (dest.Contains(current)) return current;
                int x = current.X, y = current.Y;
                int distance = Map[x, y] + 1;

                for (int i = 0; i < 4; i++)
                {
                    int adjX = x + _rowInts[i];
                    int adjY = y + _colInts[i];

                    if (IsValid(distance, adjX, adjY) && new Point(adjX,adjY) != start)
                    {
                        queue.Enqueue(new Point(adjX, adjY));
                        Map[adjX, adjY] = distance;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Útvonal összeállítása sorba
        /// </summary>
        /// <param name="dest">Cél azonosítója</param>
        /// <returns>Útvonallal</returns>
        private Queue<Point> GetPath(Point dest)
        {
            var stack = new Stack<Point>();
            var path = new Queue<Point>();
            stack.Push(dest);

            int x = dest.X;
            int y = dest.Y;

            for (int i = Map[dest.X, dest.Y]; i > 0; i--)
            {
                for (int j = 0; j < 4; j++)
                {
                    int adjX = x + _rowInts[j];
                    int adjY = y + _colInts[j];
                    if (!(adjX < 0 || adjY < 0 || adjX >= _table.TableSize || adjY >= _table.TableSize))
                    {
                        if (Map[adjX, adjY] == i)
                        {
                            x = adjX;
                            y = adjY;
                            stack.Push(new Point(x, y));
                        }
                    }
                }
            }
            while (stack.Any())
                path.Enqueue(stack.Pop());

            return path;
        }

        /// <summary>
        /// Ellenőrzi hogy a táblán belülre léptünk
        /// </summary>
        /// <param name="x">X azonosítója</param>
        /// <param name="y">Y azonosítója</param>
        /// <returns>Érvényes-e a mező vagy sem.</returns>
        private bool IsValid(int distance, int x, int y)
        {
            if (x < 0 || y < 0 || x >= _table.TableSize || y >= _table.TableSize) return false;
            if (Map[x, y] == -1) return false;
            return (Map[x, y] == 0);
        }

        #endregion
    }
}
