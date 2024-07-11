using System;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(AbpAutofacModule) })]
[DependsOn(new Type[] { typeof(AbpDataModule) })]
[DependsOn(new Type[] { typeof(AbpMultiTenancyModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingSimplifiedModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingDotNetSharedModule) })]
public class Bib5HostingDotNetSimplifiedModule : AbpModule
{
}
