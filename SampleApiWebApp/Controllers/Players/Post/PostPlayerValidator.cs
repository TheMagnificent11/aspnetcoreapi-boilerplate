using SampleApiWebApp.Domain.Validators;

namespace SampleApiWebApp.Controllers.Players.Post
{
    public sealed class PostPlayerValidator : BasePlayerValidator<PostPlayerCommand>
    {
        public PostPlayerValidator()
            : base()
        {
        }
    }
}
