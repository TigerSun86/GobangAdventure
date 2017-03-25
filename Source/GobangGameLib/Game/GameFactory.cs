using GobangGameLib.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;

namespace GobangGameLib.Game
{
    public class GameFactory
    {
        public IGame CreateRandomGame()
        {
            return new GobangGame(new RandomPlayer() { Player = PieceType.P1 }, new RandomPlayer() { Player = PieceType.P2 });
        }
        public IGame CreateGame(IPlayer p1, IPlayer p2)
        {
            return new GobangGame(p1, p2);
        }


    }
}
