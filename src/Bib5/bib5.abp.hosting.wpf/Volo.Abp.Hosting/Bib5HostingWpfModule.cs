using System;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingWindowsDesktopModule) })]
public class Bib5HostingWpfModule : AbpModule
{
}
