using RequestManagement;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams.Put
{
    public sealed class PutTeamCommand : IPutCommand<long>, ITeam
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
