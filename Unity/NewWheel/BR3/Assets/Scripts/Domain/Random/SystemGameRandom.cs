namespace BR3.Domain.Random
{
    public sealed class SystemGameRandom : IGameRandom
    {
        private readonly System.Random _random;

        public SystemGameRandom()
            : this(new System.Random())
        {
        }

        public SystemGameRandom(int seed)
            : this(new System.Random(seed))
        {
        }

        public SystemGameRandom(System.Random random)
        {
            _random = random ?? throw new System.ArgumentNullException(nameof(random));
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }
    }
}
