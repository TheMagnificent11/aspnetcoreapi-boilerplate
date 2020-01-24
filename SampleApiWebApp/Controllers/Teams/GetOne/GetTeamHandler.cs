using System;
using AutoMapper;
using EntityManagement;
using RequestManagement;
using Serilog;

namespace SampleApiWebApp.Controllers.Teams.GetOne
{
    public sealed class GetTeamHandler : GetOneQueryHandler<long, Domain.Team, Team, GetTeamRequest>
    {
        public GetTeamHandler(IEntityRepository<Domain.Team, long> repository, ILogger logger, IMapper mapper)
            : base(repository, logger)
        {
            this.Mapper = mapper;
        }

        private IMapper Mapper { get; }

        protected override Team MapEntity(Domain.Team entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return this.Mapper.Map<Team>(entity);
        }
    }
}
