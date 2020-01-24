using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams
{
    public class Team : ITeam
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
