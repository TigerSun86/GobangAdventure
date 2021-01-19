package com.tiger.gobangcore.models;

public class BoardContext {
    public final int rowSize;
    public final int colSize;
    public final int numOfPiecesToWin;

    public BoardContext(int rowSize, int colSize, int numOfPiecesToWin) {
        this.rowSize = rowSize;
        this.colSize = colSize;
        this.numOfPiecesToWin = numOfPiecesToWin;
    }

    public BoardContext() {
        this(10, 10, 5);
    }
}
