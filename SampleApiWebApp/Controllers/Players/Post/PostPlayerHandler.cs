using System;
using System.Threading;
using System.Threading.Tasks;
using EntityManagement;
using RequestManagement;
using Serilog;

namespace SampleApiWebApp.Controllers.Players.Post
{
    public sealed class PostPlayerHandler : PostCommandHandler<long, Domain.Player, Player, PostPlayerCommand>
    {
        public PostPlayerHandler(IEntityRepository<Domain.Player, long> repository, ILogger logger)
            : base(repository, logger)
        {
        }

        protected override Task<Domain.Player> GenerateAndValidateDomainEntity(
            PostPlayerCommand request,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
