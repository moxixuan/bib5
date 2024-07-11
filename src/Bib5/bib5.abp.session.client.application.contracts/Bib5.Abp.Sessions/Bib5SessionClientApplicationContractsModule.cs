using System;
using Bib5.Abp.Session;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(AbpDddApplicationContractsModule) })]
[DependsOn(new Type[] { typeof(AbpObjectExtendingModule) })]
[DependsOn(new Type[] { typeof(AbpAuthorizationModule) })]
[DependsOn(new Type[] { typeof(Bib5SessionClientDomainSharedModule) })]
public class Bib5SessionClientApplicationContractsModule : AbpModule
{
}
