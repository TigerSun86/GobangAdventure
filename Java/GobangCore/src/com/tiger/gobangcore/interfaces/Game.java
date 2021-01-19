package com.tiger.gobangcore.interfaces;

import com.tiger.gobangcore.models.GameStatus;
import com.tiger.gobangcore.models.PieceType;

public interface Game {
    public void start();

    public void run();

    public Board getBoard();

    public PieceType getCurrentPlayerType();

    public GameStatus getGameStatus();
}
