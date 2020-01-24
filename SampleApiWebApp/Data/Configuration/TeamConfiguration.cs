using System;
using EntityManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Data.Configuration
{
    public class TeamConfiguration : BaseEntityConfiguration<Team, long>
    {
        public override void Configure(EntityTypeBuilder<Team> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(Team.FieldMaxLenghts.Name);

            builder.HasIndex(i => i.Name)
                .IsUnique();

            builder.HasMany(i => i.Players)
                .WithOne(i => i.Team)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(i => i.TeamId);
        }
    }
}
