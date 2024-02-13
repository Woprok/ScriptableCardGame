using System;

namespace Shared.Networking.Common.Entities
{
    [Serializable]
    public class UniqueEntity
    {
        public UniqueEntity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public override int GetHashCode()
        {
            return 17^Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is UniqueEntity ue)
                return Equals(ue);
            return false;
        }

        public bool Equals(UniqueEntity obj)
        {
            return obj.Id == Id;
        }
    }
}