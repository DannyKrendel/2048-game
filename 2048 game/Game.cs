using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    class Game : IGame
    {
        // Length & Width of game field (doesn't have to be equal)
        int length;
        int width;

        public int Length
        {
            get => length;
            private set
            {
                if (value >= 2)
                    length = value;
                else
                    length = 2;
            }
        }
        public int Width
        {
            get => width;
            private set
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

        int InitialCellAmount { get; set; }

        Random random = new Random();

        // Initializing the game with field parameters and cell amount at the beginning
        public Game(int length, int width, int initialCellAmount)
        {
            Length = length;
            Width = width;
            InitialCellAmount = initialCellAmount;
            Initialize();
        }

        public void Initialize()
        {
            cells = new List<Cell>();
            prevCells = new List<Cell>();

            Score = 0;
            Highscore = 0;
            Moves = 0;

            for (int i = 0; i < InitialCellAmount; i++)
                AddCell();
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
                cell = new Cell(random.NextDouble() < 0.9 ? 2 : 4, random.Next(1, Length + 1), random.Next(1, Width + 1));
            } while (cells.Any(c => c.X == cell.X && c.Y == cell.Y));
            cells.Add(cell);
        }

        // Drawing the field
        public void DisplayField()
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
        public void DisplayCells()
        {
            foreach (Cell cell in cells)
                cell.Display();
        }

        // Displaying score, highscore and moves
        public void DisplayStats()
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
        public void HandleKey(ConsoleKeyInfo cki)
        {
            prevCells = cells.Select(c => (Cell)c.Clone()).ToList();

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

            if (IsMoved())
                AddCell();
        }

        // Checking for field overflow
        bool IsFieldFull()
        {
            return cells.Count() == Length * Width;
        }

        // Checking if there is no cell that can be moved
        public bool IsOver()
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
        public bool IsWon()
        {
            return cells.Any(c => c.Value == 2048);
        }
    }
}
