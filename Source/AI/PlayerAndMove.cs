using GobangGameLib.GameBoard;

namespace AI
{
    internal class PlayerAndMove
    {
        public PlayerAndMove(PieceType player, Position move)
        {
            this.Player = player;
            this.Move = move;
        }

        public PieceType Player { get; private set; }

        public Position Move { get; private set; }

        public override string ToString()
        {
            return $"{Player}:{Move}";
        }
    }
}
