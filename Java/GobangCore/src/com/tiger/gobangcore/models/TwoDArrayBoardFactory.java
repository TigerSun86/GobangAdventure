package com.tiger.gobangcore.models;

import com.tiger.gobangcore.interfaces.Board;
import com.tiger.gobangcore.interfaces.BoardFactory;

public class TwoDArrayBoardFactory implements BoardFactory {

    private final BoardContext boardContext;

    public TwoDArrayBoardFactory(BoardContext boardContext) {
        this.boardContext = boardContext;
    }

    @Override
    public Board create() {
        return new TwoDArrayBoard(boardContext);
    }

}
