using System;
using System.Collections.Generic;
using System.Linq;
using EntityManagement.Core;
using FluentValidation.Extensions;
using SampleApiWebApp.Domain.Validators;

namespace SampleApiWebApp.Domain
{
    public class Team : BaseEntity<long>, ITeam
    {
        public string Name { get; protected set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<Player> Players { get; protected set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public static Team CreateTeam(string teamName)
        {
            var team = new Team { Name = teamName };

            team.ApplyTrackingData();

            var errors = ValidateTeam(team);
            if (errors.Any()) throw new InvalidOperationException(errors.GetMultiLineErrorMessage());

            return team;
        }

        public static IDictionary<string, IEnumerable<string>> ValidateTeam(Team team)
        {
            var validator = new TeamValidator();

            return validator.ValidateEntity(team);
        }

        public void ChangeName(string newName)
        {
            this.Name = newName;

            this.ApplyTrackingData();

            var errors = ValidateTeam(this);
            if (errors.Any()) throw new InvalidOperationException(errors.GetMultiLineErrorMessage());
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
