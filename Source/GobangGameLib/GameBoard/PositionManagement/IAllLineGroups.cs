using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobangGameLib.GameBoard.PositionManagement
{
    public interface IAllLineGroups
    {
        IDictionary<LineType, ILines> LineGroups { get; }
    }
}
