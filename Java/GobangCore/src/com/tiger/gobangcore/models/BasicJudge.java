package com.tiger.gobangcore.models;

import com.tiger.gobangcore.interfaces.Board;
import com.tiger.gobangcore.interfaces.Judge;

public class BasicJudge implements Judge {

    @Override
    public PieceType GetWinner(Board board) {
        PieceType winner = this.checkRow(board);
        if (winner != PieceType.EMPTY) {
            return winner;
        }

        winner = this.checkCol(board);
        if (winner != PieceType.EMPTY) {
            return winner;
        }

        return PieceType.EMPTY;
    }

    private PieceType checkRow(Board board) {
        int connectedCount;
        PieceType winner;
        for (int r = 0; r < board.getBoardContext().rowSize; r++) {
            connectedCount = 0;
            winner = PieceType.EMPTY;
            for (int c = 0; c < board.getBoardContext().colSize; c++) {
                Position p = new Position(r, c);
                PieceType currentPiece = board.get(p);
                if (currentPiece == PieceType.EMPTY) {
                    winner = PieceType.EMPTY;
                    connectedCount = 0;
                } else {
                    if (currentPiece != winner) {
                        connectedCount = 0;
                    }

                    connectedCount++;
                    winner = currentPiece;

                    if (connectedCount == board.getBoardContext().numOfPiecesToWin) {
                        return winner;
                    }
                }
            }
        }

        return PieceType.EMPTY;
    }

    private PieceType checkCol(Board board) {
        int connectedCount;
        PieceType winner;
        for (int c = 0; c < board.getBoardContext().colSize; c++) {
            connectedCount = 0;
            winner = PieceType.EMPTY;
            for (int r = 0; r < board.getBoardContext().rowSize; r++) {
                Position p = new Position(r, c);
                PieceType currentPiece = board.get(p);
                if (currentPiece == PieceType.EMPTY) {

                    winner = PieceType.EMPTY;
                    connectedCount = 0;
                } else {
                    if (currentPiece != winner) {
                        connectedCount = 0;
                    }

                    connectedCount++;
                    winner = currentPiece;

                    if (connectedCount == board.getBoardContext().numOfPiecesToWin) {
                        return winner;
                    }
                }
            }
        }

        return PieceType.EMPTY;
    }
}
