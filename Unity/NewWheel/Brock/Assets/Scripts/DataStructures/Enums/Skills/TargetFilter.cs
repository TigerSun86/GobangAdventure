using System;

[Flags]
public enum TargetFilter
{
    None = 0,
    Self = 1 << 0,
    RunningAway = 1 << 1,
    Neighbours = 1 << 2,
    SelfAndNeighbours = Self | Neighbours,
    All = Self | RunningAway | Neighbours,
}
