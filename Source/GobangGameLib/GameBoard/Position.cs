using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard
{
    public class Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            if (row < 0 || col < 0)
            {
                throw new ArgumentOutOfRangeException($"row: {row}, col: {col}.");
            }

            Row = row;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var item = obj as Position;
            if (item == null)
            {
                return false;
            }

            return (Row == item.Row) && (Col == item.Col);
        }

        public override int GetHashCode()
        {
            // To avoid conflict, assuming row and column length won't be greater than 32.
            return (Row << 5) ^ Col;
        }

        public override string ToString()
        {
            return $"({Row},{Col})";
        }
    }
}
