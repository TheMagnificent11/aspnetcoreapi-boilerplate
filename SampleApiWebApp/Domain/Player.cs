using EntityManagement.Core;

namespace SampleApiWebApp.Domain
{
    public class Player : BaseEntity<long>, IPlayer
    {
        public string GivenName { get; protected set; }

        public string Surname { get; protected set; }

        public long TeamId { get; protected set; }

        public Team Team { get; protected set; }

        public int Number { get; protected set; }

        public static class FieldNameMaxLengths
        {
            public const int Name = 50;
        }
    }
}
