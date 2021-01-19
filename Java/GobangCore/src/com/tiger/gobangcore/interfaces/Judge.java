package com.tiger.gobangcore.interfaces;

import com.tiger.gobangcore.models.PieceType;

public interface Judge {
    public PieceType GetWinner(Board board);
}
