using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public class PositionManager : IAllLineGroups, ILines, IPositions
    {
        private readonly List<ILines> _lineGroups;
        private static PositionManager _positionManager;

        private PositionManager()
        {
            _lineGroups = GetAllLineGroups().ToList();
        }

        public static PositionManager Instance()
        {
            if (_positionManager == null)
            {
                _positionManager = new PositionManager();
            }

            return _positionManager;
        }

        public IEnumerable<ILines> LineGroups
        {
            get
            {
                return _lineGroups;
            }
        }

        public IEnumerable<IPositions> Lines
        {
            get
            {
                return LineGroups.SelectMany(g => g.Lines);
            }
        }

        /// <summary>
        /// Gets all positions in order of first row, second row, etc..
        /// </summary>
        public IEnumerable<Position> Positions
        {
            get
            {
                return LineGroups.First().Lines.SelectMany(l => l.Positions);
            }
        }

        private IEnumerable<ILines> GetAllLineGroups()
        {
            yield return new LineGroup(LineType.Row);
            yield return new LineGroup(LineType.Column);
            yield return new LineGroup(LineType.DiagonalOne);
            yield return new LineGroup(LineType.DiagonalTwo);
        }
    }
}
