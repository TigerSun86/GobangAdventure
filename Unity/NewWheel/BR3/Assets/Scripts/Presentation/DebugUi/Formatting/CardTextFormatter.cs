using BR3.Config;
using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class CardTextFormatter
    {
        public static string FormatTitle(CardSpec cardSpec)
        {
            return cardSpec == null ? "-" : cardSpec.rpsType.ToString();
        }

        public static string FormatTitle(CardInstance cardInstance)
        {
            return cardInstance == null ? "-" : cardInstance.RpsType.ToString();
        }

        public static string FormatTraits(CardSpec cardSpec)
        {
            return cardSpec == null ? "Traits: -" : TraitListFormatter.Format(cardSpec.traits);
        }

        public static string FormatTraits(CardInstance cardInstance)
        {
            return cardInstance == null ? "Traits: -" : TraitListFormatter.Format(cardInstance.Traits);
        }

        public static string FormatStats(CardSpec cardSpec)
        {
            return cardSpec == null ? "Base: -" : $"Base: {cardSpec.basePower}";
        }

        public static string FormatStats(CardInstance cardInstance)
        {
            if (cardInstance == null)
            {
                return "Base: -";
            }

            return $"Base: {cardInstance.BasePower} | Perm: +{cardInstance.PermanentPowerBonus}";
        }
    }
}
