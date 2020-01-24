using RequestManagement;

namespace SampleApiWebApp.Controllers.Players.GetOne
{
    public class GetPlayerRequest : IGetOneQuery<long, Player>
    {
        public long Id { get; set; }
    }
}
