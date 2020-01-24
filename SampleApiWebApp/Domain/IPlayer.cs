namespace SampleApiWebApp.Domain
{
    public interface IPlayer
    {
        string GivenName { get; }

        string Surname { get; }

        long TeamId { get; }

        int Number { get; }
    }
}
