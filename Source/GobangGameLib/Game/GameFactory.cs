using GobangGameLib.GameBoard;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;

namespace GobangGameLib.Game
{
    public class GameFactory
    {
        public IGame CreateGame(BoardProperties context, IPlayer p1, IPlayer p2, IJudge judge)
        {
            return new GobangGame(context, p1, p2, judge);
        }
    }
}
