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

namespace SampleApiWebApp.Controllers.Teams.Put
{
    public sealed class PutTeamHandler : PutCommandHandler<long, Domain.Team, PutTeamCommand>
    {
        public PutTeamHandler(IEntityRepository<Domain.Team, long> repository, ILogger logger)
            : base(repository, logger)
        {
        }

        protected override async Task BindToDomainEntityAndValidate(
            [NotNull] Domain.Team domainEntity,
            [NotNull] PutTeamCommand request,
            [NotNull] ILogger logger,
            [NotNull] CancellationToken cancellationToken)
        {
            if (domainEntity == null) throw new ArgumentNullException(nameof(domainEntity));
            if (request == null) throw new ArgumentNullException(nameof(request));

            var query = new GetTeamsByName(request.Name);
            var teamsWithSameName = await this.Repository
                .Query(query)
                .ToListAsync(cancellationToken);

            if (teamsWithSameName.Any(i => i.Id != domainEntity.Id))
            {
                var error = new ValidationFailure(nameof(request.Name), string.Format(Domain.Team.ErrorMessages.NameNotUniqueFormat, request.Name));
#pragma warning disable CA1062 // Validate arguments of public methods
                logger.Information("Validation failed: a Team with the name {TeamName} already exists", request.Name);
#pragma warning restore CA1062 // Validate arguments of public methods
                throw new ValidationException(new ValidationFailure[] { error });
            }

            domainEntity.ChangeName(request.Name);
        }
    }
}
