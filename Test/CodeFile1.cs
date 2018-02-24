using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Game
{
    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            do
            {
                Game game = new Game(4, 8, 2);

                string key;
                do
                {
                    Console.WriteLine("1. New game");
                    Console.WriteLine("2. Quit");
                    key = Console.ReadLine();
                    Console.Clear();
                } while (key != "1" && key != "2");

                switch (key)
                {
                    case "1":
                        game.Run();
                        break;
                    case "2":
                        Environment.Exit(0);
                        break;
                }
                Console.Clear();
            } while (true);
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    class Cell : IEquatable<Cell>, ICloneable
    {
        public int Value { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int value, int x, int y)
        {
            Value = value;
            X = x;
            Y = y;
        }

        // Double the value
        public void Double() => Value *= 2;

        // Display value of cell at certain coordinates
        public void Display()
        {
            Console.SetCursorPosition((Y - 1) * 5 + 1, (X - 1) * 2 + 1);
            ColorChanger.ColorCell(Value.ToString());
        }

        // Erase value of cell at certain coordinates
        public void Erase()
        {
            Console.SetCursorPosition((Y - 1) * 5 + 1, (X - 1) * 2 + 1);
            ColorChanger.ColorCell("    ");
        }

        #region overriding section

        public static bool operator ==(Cell cell1, Cell cell2)
        {
            return cell1.Value == cell2.Value && cell1.X == cell2.X && cell1.Y == cell2.Y;
        }

        public static bool operator !=(Cell cell1, Cell cell2)
        {
            return !(cell1.Value == cell2.Value && cell1.X == cell2.X && cell1.Y == cell2.Y);
        }

        public bool Equals(Cell cell)
        {
            if (cell is null)
                return false;

            return Value == cell.Value && X == cell.X && Y == cell.Y;
        }

        public override bool Equals(object obj)
        {
            Cell cell = obj as Cell;
            if (obj == null)
                return false;
            return Value == cell.Value && X == cell.X && Y == cell.Y;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode();
        }

        public object Clone() => MemberwiseClone();

        #endregion
    }

    class Game
    {
        // Length & Width of game field (doesn't have to be equal)
        int length;
        int width;

        int Length
        {
            get => length;
            set
            {
                if (value >= 2)
                    length = value;
                else
                    length = 2;
            }
        }
        int Width
        {
            get => width;
            set
            {
                if (value >= 2)
                    width = value;
                else
                    width = 2;
            }
        }

        // Stats of current game
        int Score { get; set; }
        static int Highscore { get; set; }
        int Moves { get; set; }

        // List of current cells & cells on previous move
        List<Cell> cells;
        List<Cell> prevCells;

        Random rand = new Random();

        // Initializing the game with field parameters and cell amount at the beginning
        public Game(int length, int width, int initialCellAmount)
        {
            cells = new List<Cell>();
            prevCells = new List<Cell>();
            Length = length;
            Width = width;
            Score = 0;
            Moves = 0;

            for (int i = 0; i < initialCellAmount; i++)
                AddCell();
        }

        // Launching the game
        public void Run()
        {
            DisplayField();
            DisplayCells();
            DisplayStats();

            do
            {
                if (Console.KeyAvailable)
                {
                    HandleKey();
                    if (IsMoved())
                        AddCell();
                    DisplayCells();
                    DisplayStats();

                    bool isOver = IsOver();
                    bool isWon = IsWon();

                    if (isOver || isWon)
                    {
                        Console.SetCursorPosition(0, 11);
                        Console.WriteLine(isOver ? "You lost!" : "You won!");
                        Thread.Sleep(2000);
                        break;
                    }
                }
            } while (true);
        }

        // Checking for any cell on field that moved
        bool IsMoved()
        {
            foreach (Cell cell in prevCells)
            {
                if (cells.Contains(cell) == false)
                {
                    Moves++;
                    return true;
                }
            }
            return false;
        }

        // Adding new cell on the field (90% - '2', 10% - '4')
        void AddCell()
        {
            if (IsFieldFull())
                return;
            Cell cell;
            do
            {
                cell = new Cell(rand.NextDouble() < 0.9 ? 2 : 4, rand.Next(1, Length), rand.Next(1, Width));
            } while (cells.Any(c => c.X == cell.X && c.Y == cell.Y));
            cells.Add(cell);
        }

        // Drawing the field
        void DisplayField()
        {
            for (int i = 0; i < Length + 1; i++)
            {
                for (int j = 0; j < Width; j++)
                    Console.Write(" ----");

                Console.WriteLine();

                if (i == Length)
                    break;

                for (int j = 0; j < Width + 1; j++)
                    Console.Write("|    ");

                Console.WriteLine();
            }
        }

        // Displaying existing cells
        void DisplayCells()
        {
            foreach (Cell cell in cells)
                cell.Display();
        }

        // Displaying score, highscore and moves
        void DisplayStats()
        {
            Console.SetCursorPosition(0, 9);
            Console.WriteLine($"Score: {Score}   Highscore: {Highscore}\n" +
                $"Moves: {Moves}");
        }

        // Cells movement algorithm
        void MoveCells(Direction dir)
        {
            // Erasing existing cells on field
            foreach (Cell cell in cells)
                cell.Erase();

            // Choosing type of order based on chosen Direction
            switch (dir)
            {
                case Direction.Left:
                    cells = cells.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();
                    break;
                case Direction.Right:
                    cells = cells.OrderBy(c => c.X).ThenByDescending(c => c.Y).ToList();
                    break;
                case Direction.Up:
                    cells = cells.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
                    break;
                case Direction.Down:
                    cells = cells.OrderBy(c => c.Y).ThenByDescending(c => c.X).ToList();
                    break;
                default:
                    return;
            }

            // Get X or Y coordinate of certain cell
            int getCoord(Cell cell, bool isOpposite)
            {
                return isOpposite ^ (dir == Direction.Left || dir == Direction.Right) ? cell.Y : cell.X;
            }

            // Set X or Y coordinate of certain cell
            void setCoord(ref Cell cell, int value)
            {
                if (dir == Direction.Left || dir == Direction.Right)
                    cell.Y = value;
                else
                    cell.X = value;
            }

            // Assigning first value for every enumerated column or row
            int border = 0;
            switch (dir)
            {
                case Direction.Left:
                    border = 1;
                    break;
                case Direction.Right:
                    border = Width;
                    break;
                case Direction.Up:
                    border = 1;
                    break;
                case Direction.Down:
                    border = Length;
                    break;
            }

            // Assigning the number, that will be added to certain coordinate
            int n = dir == Direction.Left || dir == Direction.Up ? 1 : -1;

            for (int i = -1; i + 1 < cells.Count(); i++)
            { 
                Cell getCurr() => i >= 0 ? cells[i] : null;
                Cell getNext() => cells[i + 1];

                if (i == -1 || getCoord(getCurr(), true) != getCoord(getNext(), true))
                {
                    Cell temp = getNext();
                    setCoord(ref temp, border);
                    continue;
                }

                if (getCurr().Value == getNext().Value)
                {
                    getCurr().Double();
                    CalculateScore(getCurr().Value);
                    cells.Remove(getNext());
                    i--;
                }
                else
                {
                    Cell temp = getNext();
                    setCoord(ref temp, getCoord(getCurr(), false) + n);
                }
            }
        }

        // Setting new score and highscore
        void CalculateScore(int value)
        {
            Score += value;
            if (Score > Highscore)
                Highscore = Score;
        }

        // Choosing movement direction of corresponding key 
        void HandleKey()
        {
            prevCells = cells.Select(c => (Cell)c.Clone()).ToList();
            ConsoleKeyInfo cki = Console.ReadKey(true);
            switch (cki.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveCells(Direction.Left);
                    break;
                case ConsoleKey.RightArrow:
                    MoveCells(Direction.Right);
                    break;
                case ConsoleKey.UpArrow:
                    MoveCells(Direction.Up);
                    break;
                case ConsoleKey.DownArrow:
                    MoveCells(Direction.Down);
                    break;
            }
        }

        // Checking for field overflow
        bool IsFieldFull()
        {
            return cells.Count() == Length * Width;
        }

        // Checking if there is no cell that can be moved
        bool IsOver()
        {
            if (!IsFieldFull())
                return false;

            cells = cells.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();

            int[,] values = new int[Length, Width];

            foreach (Cell cell in cells)
            {
                values[cell.X - 1, cell.Y - 1] = cell.Value;
            }

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int center = values[i, j];
                    int right = j + 1 < Width ? values[i, j + 1] : 0;
                    int bottom = i + 1 < Length ? values[i + 1, j] : 0;

                    if (center == right || center == bottom)
                        return false;
                }
            }
            return true;
        }

        // Searching for cell with 2048
        bool IsWon()
        {
            return cells.Any(c => c.Value == 2048);
        }
    }

    // Class for changing color for cells
    static class ColorChanger
    {
        // Get color for corresponding value of cell
        static ConsoleColor GetCellColor(string value)
        {
            switch (value)
            {
                case "2":
                    return ConsoleColor.Blue;
                case "4":
                    return ConsoleColor.Magenta;
                case "8":
                    return ConsoleColor.Cyan;
                case "16":
                    return ConsoleColor.Green;
                case "32":
                    return ConsoleColor.Yellow;
                case "64":
                    return ConsoleColor.DarkBlue;
                case "128":
                    return ConsoleColor.DarkMagenta;
                case "256":
                    return ConsoleColor.DarkCyan;
                case "512":
                    return ConsoleColor.DarkGreen;
                case "1024":
                    return ConsoleColor.DarkYellow;
                default:
                    return ConsoleColor.Red;
            }
        }

        // Color certain cell with optional background color
        public static void ColorCell(string value, ConsoleColor bgColor = ConsoleColor.Black)
        {
            ConsoleColor defaultFg = Console.ForegroundColor;
            ConsoleColor defaultBg = Console.BackgroundColor;

            Console.ForegroundColor = GetCellColor(value);
            Console.BackgroundColor = bgColor;

            Console.WriteLine(value);

            Console.ForegroundColor = defaultFg;
            Console.BackgroundColor = defaultBg;
        }
    }
}