package com.tiger.gobangconsole;

import java.util.Scanner;

import com.tiger.gobangcore.interfaces.Board;
import com.tiger.gobangcore.interfaces.Player;
import com.tiger.gobangcore.models.PieceType;
import com.tiger.gobangcore.models.Position;

public class HumanPlayer implements Player {
    private final Scanner s;

    public HumanPlayer(final Scanner s) {
        this.s = s;
    }

    @Override
    public Position makeMove(final Board board) {
        while (true) {
            final String[] in = s.nextLine().split(" ");

            final int r = Integer.parseInt(in[0]);
            final int c = Integer.parseInt(in[1]);
            if (r >= 0 && r < board.getBoardContext().rowSize && c >= 0 && c < board.getBoardContext().colSize) {
                final Position position = new Position(r, c);
                if (board.get(position) == PieceType.EMPTY) {
                    return position;
                }
            }
            System.out.println("Illegal input. Try again");
        }
    }

}
