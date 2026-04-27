namespace BR3.Domain.Random
{
    public interface IGameRandom
    {
        int NextInt(int minInclusive, int maxExclusive);
    }
}
