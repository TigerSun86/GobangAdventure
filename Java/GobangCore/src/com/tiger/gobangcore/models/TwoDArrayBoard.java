package com.tiger.gobangcore.models;

import com.tiger.gobangcore.interfaces.Board;

public class TwoDArrayBoard implements Board {
    private final BoardContext context;
    private final PieceType[][] data;
    private int pieceCount;

    public TwoDArrayBoard(BoardContext context) {
        this.context = context;
        this.data = new PieceType[context.rowSize][context.colSize];
        for (int i = 0; i < context.rowSize; i++) {
            for (int j = 0; j < context.colSize; j++) {
                this.data[i][j] = PieceType.EMPTY;
            }
        }
    }

    @Override
    public PieceType get(Position position) {
        return this.data[position.row][position.col];
    }

    @Override
    public void set(Position position, PieceType piece) {
        // Remove a piece.
        if (piece == PieceType.EMPTY && this.data[position.row][position.col] != PieceType.EMPTY) {
            this.pieceCount--;
        }

        // Place a piece.
        else if (piece != PieceType.EMPTY && this.data[position.row][position.col] == PieceType.EMPTY) {
            this.pieceCount++;
        }

        this.data[position.row][position.col] = piece;
    }

    @Override
    public boolean isFull() {
        return this.pieceCount == this.context.rowSize * this.context.colSize;
    }

    @Override
    public BoardContext getBoardContext() {
        return this.context;
    }
}
