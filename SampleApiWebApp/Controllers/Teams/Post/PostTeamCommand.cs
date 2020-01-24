using RequestManagement;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams.Post
{
    public class PostTeamCommand : IPostCommand<long, Team>, ITeam
    {
        public string Name { get; set; }
    }
}
