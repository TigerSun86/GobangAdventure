package com.tiger.gobangcore.models;

public class BoardProperties {
    public final int rowSize;
    public final int colSize;
    public final int numOfPiecesToWin;

    public BoardProperties(int rowSize, int colSize, int numOfPiecesToWin) {
        this.rowSize = rowSize;
        this.colSize = colSize;
        this.numOfPiecesToWin = numOfPiecesToWin;
    }

    public BoardProperties() {
        this(10, 10, 5);
    }
}
