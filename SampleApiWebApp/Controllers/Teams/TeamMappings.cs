using AutoMapper;

namespace SampleApiWebApp.Controllers.Teams
{
    public sealed class TeamMappings : Profile
    {
        public TeamMappings()
        {
            this.CreateMap<Domain.Team, Team>();
        }
    }
}
