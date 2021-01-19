package com.tiger.gobangcore.interfaces;

import com.tiger.gobangcore.models.BoardContext;
import com.tiger.gobangcore.models.PieceType;
import com.tiger.gobangcore.models.Position;

public interface Board {

    public PieceType get(Position position);

    public void set(Position position, PieceType piece);

    public boolean isFull();
    
    public BoardContext getBoardContext();
}
