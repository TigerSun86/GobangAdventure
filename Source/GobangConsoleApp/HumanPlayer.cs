﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GobangGameLib.GameBoard;
using GobangGameLib.Players;

namespace GobangConsoleApp
{
    public class HumanPlayer : IPlayer
    {
        public Position MakeAMove(IBoard board)
        {
            Console.WriteLine("Please input, sample: 1 2");
            Position move = null;
            bool isValid = false;
            do
            {
                try
                {
                    var input = Console.ReadLine().Split(' ');
                    move = new Position(int.Parse(input[0]), int.Parse(input[1]));
                    isValid = board.Get(move) == PieceType.Empty;
                }
                catch (Exception)
                {
                    Console.WriteLine("Please reinput, sample: 1 2");
                }
            } while (!isValid);
            return move;
        }
    }
}
