using System;
using Bib5.Abp.Cli;
using Hangfire;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace Volo.Abp.BackgroundWorkers.Hangfire;

public abstract class Bib5HangfireBackgroundWorker : HangfireBackgroundWorkerBase, IBib5BackgroundWorker
{
	protected IClock Clock => ((BackgroundWorkerBase)this).LazyServiceProvider.LazyGetRequiredService<IClock>();

	protected IGuidGenerator GuidGenerator => ((BackgroundWorkerBase)this).LazyServiceProvider.LazyGetService<IGuidGenerator>((IGuidGenerator)(object)SimpleGuidGenerator.Instance);

	protected ICurrentTenant CurrentTenant => ((BackgroundWorkerBase)this).LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

	protected IAsyncQueryableExecuter AsyncExecuter => ((BackgroundWorkerBase)this).LazyServiceProvider.LazyGetRequiredService<IAsyncQueryableExecuter>();

	public bool EnableDataMigrationEnvironment { get; protected set; } = true;


	public bool EnableCliEnvironment { get; protected set; } = true;


	public virtual string CronExpression
	{
		get
		{
			if (AbpDataMigrationEnvironmentExtensions.IsDataMigrationEnvironment((IServiceProvider)((BackgroundWorkerBase)this).LazyServiceProvider) && !EnableDataMigrationEnvironment)
			{
				return Cron.Never();
			}
			if (CliEnvironmentExtensions.IsCliEnvironment((IServiceProvider)((BackgroundWorkerBase)this).LazyServiceProvider) && !EnableCliEnvironment)
			{
				return Cron.Never();
			}
			return ((HangfireBackgroundWorkerBase)this).CronExpression;
		}
		set
		{
			((HangfireBackgroundWorkerBase)this).CronExpression = value;
		}
	}
}
