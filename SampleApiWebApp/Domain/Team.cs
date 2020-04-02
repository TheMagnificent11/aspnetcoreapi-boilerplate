using System.Collections.Generic;
using EntityManagement.Core;
using SampleApiWebApp.Domain.Validators;

namespace SampleApiWebApp.Domain
{
    public class Team : BaseEntityWithValidation<Team, long, TeamValidator>, ITeam
    {
        public string Name { get; protected set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<Player> Players { get; protected set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public static Team CreateTeam(string teamName)
        {
            var team = new Team { Name = teamName };

            team.ApplyTrackingData();

            Validate(team);

            return team;
        }

        public void ChangeName(string newName)
        {
            this.Name = newName;

            this.ApplyTrackingData();

            Validate(this);
        }

        public static class FieldMaxLenghts
        {
            public const int Name = 50;
        }

        public static class ErrorMessages
        {
            public const string NameNotUniqueFormat = "Team Name '{0}' is not unqiue";
        }
    }
}
