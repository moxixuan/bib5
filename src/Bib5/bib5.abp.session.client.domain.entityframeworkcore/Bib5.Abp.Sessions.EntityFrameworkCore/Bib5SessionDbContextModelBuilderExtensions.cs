using System;
using System.Linq.Expressions;
using Bib5.Abp.Hosting.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Bib5.Abp.Sessions.EntityFrameworkCore;

public static class Bib5SessionDbContextModelBuilderExtensions
{
	public static void ConfigureBib5Session(this ModelBuilder builder)
	{
		Check.NotNull<ModelBuilder>(builder, "builder");
		builder.Entity<Session>((Action<EntityTypeBuilder<Session>>)delegate(EntityTypeBuilder<Session> b)
		{
			RelationalEntityTypeBuilderExtensions.ToTable<Session>(b, Bib5SessionDbProperties.DbTablePrefix + "Sessions", Bib5SessionDbProperties.DbSchema);
			AbpEntityTypeBuilderExtensions.ConfigureByConvention((EntityTypeBuilder)(object)b);
			b.HasIndex((Expression<Func<Session, object>>)((Session x) => x.Authority));
		});
	}
}
