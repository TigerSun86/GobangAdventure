using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard
{
    public enum PieceType
    {
        Empty,
        P1,
        P2
    }

    public static class PieceTypeExtensions
    {
        public static PieceType GetOther(this PieceType t)
        {
            if (t == PieceType.Empty)
            {
                return PieceType.Empty;
            }
            return (t == PieceType.P1 ? PieceType.P2 : PieceType.P1);
        }

        public static IEnumerable<PieceType> GetAll()
        {
            return Enum.GetValues(typeof(PieceType)).Cast<PieceType>();
        }

        public static IEnumerable<PieceType> GetAllPieces()
        {
            return GetAll().Where(e => e != PieceType.Empty);
        }
    }
}
