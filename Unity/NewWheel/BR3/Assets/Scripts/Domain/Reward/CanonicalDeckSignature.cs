using System;
using System.Collections.Generic;

namespace BR3.Domain.Reward
{
    public sealed class CanonicalDeckSignature : IEquatable<CanonicalDeckSignature>
    {
        public CanonicalDeckSignature(List<CanonicalCardSignature> cards, string signatureText)
        {
            Cards = cards;
            SignatureText = signatureText;
        }

        public List<CanonicalCardSignature> Cards { get; }

        public string SignatureText { get; }

        public bool Equals(CanonicalDeckSignature other)
        {
            return other != null && SignatureText == other.SignatureText;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CanonicalDeckSignature);
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
