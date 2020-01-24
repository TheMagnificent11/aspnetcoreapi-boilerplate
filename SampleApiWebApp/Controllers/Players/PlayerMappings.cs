using AutoMapper;

namespace SampleApiWebApp.Controllers.Players
{
    public sealed class PlayerMappings : Profile
    {
        public PlayerMappings()
        {
            this.CreateMap<Domain.Player, Player>();
        }
    }
}
