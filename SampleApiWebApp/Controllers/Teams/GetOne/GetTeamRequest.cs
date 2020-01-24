using RequestManagement;

namespace SampleApiWebApp.Controllers.Teams.GetOne
{
    public class GetTeamRequest : IGetOneQuery<long, Team>
    {
        public long Id { get; set; }
    }
}
