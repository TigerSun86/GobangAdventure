using System.Collections.Generic;
using BR3.Config;
using BR3.Domain;

namespace BR3.Presentation.DebugUi
{
    public static class TraitListFormatter
    {
        public static string Format(IReadOnlyList<TraitType> traits, TraitTuning traitTuning = null)
        {
            if (traits == null || traits.Count == 0)
            {
                return "Traits: None";
            }

            string[] parts = new string[traits.Count];
            for (int index = 0; index < traits.Count; index++)
            {
                parts[index] = FormatTrait(traits[index], traitTuning);
            }

            return $"Traits: {string.Join(", ", parts)}";
        }

        public static string FormatTrait(TraitType traitType, TraitTuning traitTuning = null)
        {
            if (traitTuning == null)
            {
                return traitType.ToString();
            }

            switch (traitType)
            {
                case TraitType.Empower:
                    return $"Empower(+{traitTuning.empowerBonus})";
                case TraitType.AdjacentAid:
                    return $"AdjacentAid(+{traitTuning.adjacentAidBonus})";
                case TraitType.Suppress:
                    return $"Suppress(-{traitTuning.suppressPenalty})";
                case TraitType.Regrow:
                    return $"Regrow(+{traitTuning.regrowHeal} HP)";
                case TraitType.Growth:
                    return $"Growth(+{traitTuning.growthBonus} Perm)";
                default:
                    return traitType.ToString();
            }
        }
    }
}
