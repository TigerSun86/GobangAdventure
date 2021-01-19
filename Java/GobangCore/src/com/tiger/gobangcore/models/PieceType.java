package com.tiger.gobangcore.models;

public enum PieceType {
    EMPTY, P1, P2;

    public PieceType getOther() {
        if (this == PieceType.EMPTY) {
            return PieceType.EMPTY;
        }
        return (this == PieceType.P1 ? PieceType.P2 : PieceType.P1);
    }
}
