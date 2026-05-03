using System.Collections.Generic;
using BR3.Domain;

namespace BR3.Presentation.DebugUi
{
    public static class TraitListFormatter
    {
        public static string Format(IReadOnlyList<TraitType> traits)
        {
            if (traits == null || traits.Count == 0)
            {
                return "Traits: None";
            }

            string[] parts = new string[traits.Count];
            for (int index = 0; index < traits.Count; index++)
            {
                parts[index] = traits[index].ToString();
            }

            return $"Traits: {string.Join(", ", parts)}";
        }
    }
}
