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
            var item = obj as Position;
            if (item == null)
            {
                return false;
            }

            return (Row == item.Row) && (Col == item.Col);
        }

        public override int GetHashCode()
        {
            return (Row << 16) + Col;
        }

        public override string ToString()
        {
            return $"({Row},{Col})";
        }
    }
}
