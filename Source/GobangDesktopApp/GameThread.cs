using AI.Scorer;
using GobangGameLib.Game;
using GobangGameLib.GameBoard;
using GobangGameLib.GameBoard.Patterns;
using GobangGameLib.GameBoard.PositionManagement;
using GobangGameLib.GameJudge;
using GobangGameLib.Players;
using System;
using System.Threading.Tasks;

namespace GobangDesktopApp
{
    public class GameThread
    {
        public bool Running { get; private set; } = true;
        public IGame game;
        public IBoard board { get => this.game.Board; }
        public BoardProperties context;
        public IPlayer p1;
        public IPlayer p2;

        public async void Start()
        {
            context = new BoardProperties();
            var positions = new PositionFactory().Create(context);
            var patterns = new PatternFactory().Create();
            var matcher = new PatternMatcher(patterns);
            var boardFactory = new BoardFactory(context, positions);
            var patternBoardFactory = new PatternBoardFactory(context, positions, matcher);
            var centerScorer = new CenterScorer(context, positions);
            var patternScorer = new PatternScorer(positions, patterns, matcher);
            var aggregatedScorer = new AggregatedScorer(new[]
            {
                new Tuple<IScorer, double>(patternScorer, 1),
                new Tuple<IScorer, double>(centerScorer, 0.01)
            });


            var judge = new PatternJudge(positions, patterns, matcher);
            this.p1 = new HumanPlayer();
            this.p2 = new HumanPlayer();
            game = new GameFactory().CreateGame(boardFactory,
                this.p1,
                this.p2,
                judge
                );

            game.Start();

            do
            {

                await Task.Run(() => game.Run());

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

                await Task.Delay(8);
                //System.Threading.Thread.Sleep(500);
                //Console.ReadLine();
            } while (game.GameStatus == GameStatus.NotEnd);
        }

        private IPlayer GetCurrentPlayer()
        {
            if (this.game.CurPiece == PieceType.P1)
            {
                return this.p1;
            }

            if (this.game.CurPiece == PieceType.P2)
            {
                return this.p2;
            }
            throw new ArgumentException($"Unsupported player: {this.game.CurPiece.ToString()}.");
        }

        public void MakeHumanMove(int r, int c)
        {
            HumanPlayer player = this.GetCurrentPlayer() as HumanPlayer;
            if (player != null)
            {
                player.Move = new Position(r, c);
            }
        }
    }
}
