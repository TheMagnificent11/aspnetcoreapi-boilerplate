using System;
using EntityManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Data.Configuration
{
    public class PlayerConfiguration : BaseEntityConfiguration<Player, long>
    {
        public override void Configure(EntityTypeBuilder<Player> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            builder.Property(i => i.GivenName)
                .IsRequired()
                .HasMaxLength(Player.FieldNameMaxLengths.Name);

            builder.Property(i => i.Surname)
                .IsRequired()
                .HasMaxLength(Player.FieldNameMaxLengths.Name);

            builder.HasOne(i => i.Team)
                .WithMany(i => i.Players)
                .HasForeignKey(i => i.TeamId);

            builder.HasIndex(i => new { i.TeamId, i.Number })
                .IsUnique();
        }
    }
}
