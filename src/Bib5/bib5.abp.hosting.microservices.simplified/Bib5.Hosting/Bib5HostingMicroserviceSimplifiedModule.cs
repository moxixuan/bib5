using System;
using Volo.Abp.Hosting.AspNetCore;
using Volo.Abp.Modularity;

namespace Bib5.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingAspNetCoreSimplifiedModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingMicroserviceSharedModule) })]
public class Bib5HostingMicroserviceSimplifiedModule : AbpModule
{
}
