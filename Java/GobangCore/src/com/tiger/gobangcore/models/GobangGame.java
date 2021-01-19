package com.tiger.gobangcore.models;

import com.tiger.gobangcore.interfaces.Board;
import com.tiger.gobangcore.interfaces.BoardFactory;
import com.tiger.gobangcore.interfaces.Game;
import com.tiger.gobangcore.interfaces.Judge;
import com.tiger.gobangcore.interfaces.Player;

public class GobangGame implements Game {

    private final BoardFactory boardFactory;
    private final Player player1;
    private final Player player2;
    private final Judge judge;

    private Board board;
    private PieceType currentPlayerType;
    private GameStatus gameStatus;

    public GobangGame(BoardFactory boardFactory, Player p1, Player p2, Judge judge) {
        this.boardFactory = boardFactory;
        this.player1 = p1;
        this.player2 = p2;
        this.judge = judge;
    }

    @Override
    public void start() {
        this.board = this.boardFactory.create();
        this.currentPlayerType = PieceType.P1;
        this.gameStatus = GameStatus.NOT_END;
    }

    @Override
    public void run() {
        if (this.gameStatus != GameStatus.NOT_END) {
            throw new IllegalStateException("Failed to run the game after it's over.");
        }

        if (this.board == null) {
            throw new IllegalStateException("Failed to run the game before it's started.");
        }

        Player curPlayer = this.getPlayer();
        Position move = curPlayer.makeMove(this.board);
        this.board.set(move, this.currentPlayerType);

        this.currentPlayerType = this.currentPlayerType.getOther();

        PieceType winner = this.judge.GetWinner(this.board);
        if (winner == PieceType.P1) {
            this.gameStatus = GameStatus.P1_WIN;
        } else if (winner == PieceType.P2) {
            this.gameStatus = GameStatus.P2_WIN;
        } else if (this.board.isFull()) {
            this.gameStatus = GameStatus.TIE;
        }
    }

    @Override
    public Board getBoard() {
        return this.board;
    }

    @Override
    public PieceType getCurrentPlayerType() {
        return this.currentPlayerType;
    }

    @Override
    public GameStatus getGameStatus() {
        return this.gameStatus;
    }

    private Player getPlayer() {
        if (this.currentPlayerType == PieceType.P1) {
            return player1;
        }

        if (this.currentPlayerType == PieceType.P2) {
            return player2;
        }

        throw new IllegalArgumentException(String.format("Unsupported player: %s.", this.currentPlayerType));
    }
}
