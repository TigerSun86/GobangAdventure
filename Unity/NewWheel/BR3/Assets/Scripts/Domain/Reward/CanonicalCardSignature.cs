using System;
using System.Collections.Generic;
using BR3.Domain.Runtime;

namespace BR3.Domain.Reward
{
    public sealed class CanonicalCardSignature : IEquatable<CanonicalCardSignature>
    {
        public CanonicalCardSignature(
            RpsType rpsType,
            int basePower,
            int permanentPowerBonus,
            List<TraitType> traits,
            string signatureText)
        {
            RpsType = rpsType;
            BasePower = basePower;
            PermanentPowerBonus = permanentPowerBonus;
            Traits = traits;
            SignatureText = signatureText;
        }

        public RpsType RpsType { get; }

        public int BasePower { get; }

        public int PermanentPowerBonus { get; }

        public List<TraitType> Traits { get; }

        public string SignatureText { get; }

        public bool Equals(CanonicalCardSignature other)
        {
            return other != null && SignatureText == other.SignatureText;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CanonicalCardSignature);
        }

        public override int GetHashCode()
        {
            return SignatureText.GetHashCode();
        }

        public override string ToString()
        {
            return SignatureText;
        }
    }
}
