using GobangGameLib.GameBoard;
using GobangGameLib.Players;
using System.Threading.Tasks;

namespace GobangDesktopApp
{
    public class HumanPlayer : IPlayer
    {
        public Position Move { get; set; } = null;

        public Position MakeAMove(IBoard board)
        {
            do
            {
                Task.Delay(50);
            } while (this.Move == null || board.Get(this.Move) != PieceType.Empty);

            Position result = this.Move;
            this.Move = null;
            return result;
        }
    }
}
