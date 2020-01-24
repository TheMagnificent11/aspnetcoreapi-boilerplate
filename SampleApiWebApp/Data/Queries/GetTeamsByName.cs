using EntityManagement;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Data.Queries
{
    public sealed class GetTeamsByName : BaseQuerySpecification<Team>
    {
        public GetTeamsByName(string teamName)
#pragma warning disable CA1304 // Specify CultureInfo
            : base(i => i.Name.ToUpper() == teamName.ToUpper())
#pragma warning restore CA1304 // Specify CultureInfo
        {
        }
    }
}
