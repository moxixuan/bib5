using System;
using Volo.Abp.Hosting.AspNetCore;
using Volo.Abp.Modularity;

namespace Bib5.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingAspNetCoreSharedModule) })]
public class Bib5HostingMicroserviceSharedModule : AbpModule
{
}
