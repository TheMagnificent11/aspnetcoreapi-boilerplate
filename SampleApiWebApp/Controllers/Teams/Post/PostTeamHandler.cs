using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityManagement;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using RequestManagement;
using SampleApiWebApp.Data.Queries;
using Serilog;

namespace SampleApiWebApp.Controllers.Teams.Post
{
    public sealed class PostTeamHandler : PostCommandHandler<long, Domain.Team, Team, PostTeamCommand>
    {
        public PostTeamHandler(IEntityRepository<Domain.Team, long> repository, ILogger logger)
            : base(repository, logger)
        {
        }

        protected override async Task<Domain.Team> GenerateAndValidateDomainEntity(
            [NotNull] PostTeamCommand request,
            [NotNull] ILogger logger,
            [NotNull] CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var teamName = request.Name.Trim();

            var query = new GetTeamsByName(teamName);
            var teamsWithSameName = await this.Repository
                .Query(query)
                .ToListAsync(cancellationToken);

            if (teamsWithSameName.Any())
            {
                var error = new ValidationFailure(nameof(request.Name), string.Format(Domain.Team.ErrorMessages.NameNotUniqueFormat, teamName));
#pragma warning disable CA1062 // Validate arguments of public methods
                logger.Information("Validation failed: a Team with the name {TeamName} already exists", request.Name);
#pragma warning restore CA1062 // Validate arguments of public methods
                throw new ValidationException(new ValidationFailure[] { error });
            }

            return Domain.Team.CreateTeam(teamName);
        }
    }
}
