using System;

namespace Game
{
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
}
