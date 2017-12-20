using System.Collections.Generic;

namespace StockManager.Models
{
    class IdentityEqualityComparer<TIdentity> : IEqualityComparer<Identity> where TIdentity : Identity
    {
        public bool Equals(Identity x, Identity y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Identity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
