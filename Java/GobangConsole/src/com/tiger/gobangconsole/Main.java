package com.tiger.gobangconsole;

import java.util.Scanner;

import com.tiger.gobangcore.interfaces.Board;
import com.tiger.gobangcore.interfaces.BoardFactory;
import com.tiger.gobangcore.interfaces.Game;
import com.tiger.gobangcore.interfaces.Judge;
import com.tiger.gobangcore.models.BasicJudge;
import com.tiger.gobangcore.models.BoardContext;
import com.tiger.gobangcore.models.GameStatus;
import com.tiger.gobangcore.models.GobangGame;
import com.tiger.gobangcore.models.PieceType;
import com.tiger.gobangcore.models.Position;
import com.tiger.gobangcore.models.TwoDArrayBoardFactory;

public class Main {

    public static void main(String[] args) {
        final Scanner s = new Scanner(System.in);
        final HumanPlayer player1 = new HumanPlayer(s);
        final HumanPlayer player2 = new HumanPlayer(s);
        final BoardContext context = new BoardContext(2, 2, 2);
        final BoardFactory boardFactory = new TwoDArrayBoardFactory(context);
        final Judge judge = new BasicJudge();
        final Game game = new GobangGame(boardFactory, player1, player2, judge);

        game.start();
        
        display(context, game.getBoard());

        do {
            game.run();
        
            display(context, game.getBoard());
            
            if (game.getGameStatus() == GameStatus.P1_WIN)
            {
                System.out.print("Winner is player 1.");
            }
            else if (game.getGameStatus() == GameStatus.P2_WIN)
            {
                System.out.print("Winner is player 2.");
            }
            else if (game.getGameStatus() == GameStatus.TIE)
            {
                System.out.print("Game ties.");
            }

        } while (game.getGameStatus() == GameStatus.NOT_END);
    }

    private static void display(BoardContext context, Board board) {
        for (int i = 0; i < context.rowSize; i++) {
            for (int j = 0; j < context.colSize; j++) {
                PieceType piece = board.get(new Position(i, j));
                System.out.print(piece + ", ");
            }
            System.out.println();
        }
        System.out.println();
    }

}
