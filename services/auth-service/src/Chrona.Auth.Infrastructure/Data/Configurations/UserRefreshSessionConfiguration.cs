using Chrona.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrona.Auth.Infrastructure.Data.Configurations;

public class UserRefreshSessionConfiguration : IEntityTypeConfiguration<UserRefreshSession>
{
	public void Configure(EntityTypeBuilder<UserRefreshSession> builder) =>
		builder.ToTable("UserRefreshSession", "auth");
}
