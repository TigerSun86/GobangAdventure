package com.tiger.gobangcore.interfaces;

import com.tiger.gobangcore.models.Position;

public interface Player {
    Position makeMove(final Board board);
}
