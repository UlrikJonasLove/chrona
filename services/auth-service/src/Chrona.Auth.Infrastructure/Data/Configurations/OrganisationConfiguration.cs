using Chrona.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrona.Auth.Infrastructure.Data.Configurations;

public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
	public void Configure(EntityTypeBuilder<Organisation> builder) =>
		builder.ToTable("Organisation", "auth");
}
