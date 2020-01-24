using SampleApiWebApp.Domain.Validators;

namespace SampleApiWebApp.Controllers.Teams.Put
{
    public sealed class PutTeamValidator : BaseTeamValidator<PutTeamCommand>
    {
        public PutTeamValidator()
            : base()
        {
        }
    }
}
