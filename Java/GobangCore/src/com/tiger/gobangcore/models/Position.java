package com.tiger.gobangcore.models;

public class Position {
    public Position(int row, int col) {
        if (row < 0 || col < 0) {
            throw new IllegalArgumentException(String.format("row: %d, col: %d", row, col));
        }

        this.row = row;
        this.col = col;
    }

    public int row;
    public int col;
}
