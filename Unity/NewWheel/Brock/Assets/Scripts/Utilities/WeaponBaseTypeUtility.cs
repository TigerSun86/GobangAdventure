public static class WeaponBaseTypeUtility
{
    public static WeaponBaseTypeMatchResult GetMatchResult(WeaponBaseType first, WeaponBaseType second)
    {
        if (first == second)
        {
            return WeaponBaseTypeMatchResult.TIE;
        }

        if (first == WeaponBaseType.ROCK && second == WeaponBaseType.SCISSOR)
        {
            return WeaponBaseTypeMatchResult.STRONG;
        }

        if (first == WeaponBaseType.PAPER && second == WeaponBaseType.ROCK)
        {
            return WeaponBaseTypeMatchResult.STRONG;
        }

        if (first == WeaponBaseType.SCISSOR && second == WeaponBaseType.PAPER)
        {
            return WeaponBaseTypeMatchResult.STRONG;
        }

        return WeaponBaseTypeMatchResult.WEAK;
    }
}
