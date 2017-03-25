using AI;
using GobangGameLib.GameBoard;
using GobangGameLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.Players;
using AI.Scorer;

namespace GobangConsoleApp
{
    public class Program
    {
        private static readonly Dictionary<PieceType, string> PieceToDisplayChar =
            new Dictionary<PieceType, string>() {
                { PieceType.Empty," " },
                { PieceType.P1,"X" },
                { PieceType.P2,"O" },
            };

        public static void Main(string[] args)
        {
            //IGame game = new GameFactory().CreateRandomGame();
            IGame game = new GameFactory().CreateGame(
                //new RandomPlayer(),
                new MinmaxSearchAi(1, new CenterScorer()) { Player = PieceType.P1 },
                new MinmaxSearchAi(1, new PatternScorer()) { Player = PieceType.P2 });
            game.Start();
            var board = game.Board;
            DisplayBoard(board);

            do
            {
                game.Run();
                DisplayBoard(board);
                if (game.GameStatus == GameStatus.P1Win)
                {
                    Console.WriteLine("Winner is player 1.");
                }
                else if (game.GameStatus == GameStatus.P2Win)
                {
                    Console.WriteLine("Winner is player 2.");
                }
                else if (game.GameStatus == GameStatus.Tie)
                {
                    Console.WriteLine("Game ties.");
                }
                System.Threading.Thread.Sleep(500);
                //Console.ReadLine();
            } while (game.GameStatus == GameStatus.NotEnd);
            Console.ReadLine();
        }

        private static void DisplayBoard(IBoard board)
        {
            Console.Write("   ");
            for (var i = 0; i < BoardProperties.ColSize; i++)
            {
                Console.Write(i);
            }
            Console.WriteLine();
            for (var i = 0; i < BoardProperties.RowSize; i++)
            {
                Console.Write(string.Format("{0,2:00} ", i));
                for (var j = 0; j < BoardProperties.ColSize; j++)
                {
                    Console.Write(PieceToDisplayChar[board.Get(new Position(i, j))]);
                }
                Console.WriteLine();
            }
        }
    }

}
