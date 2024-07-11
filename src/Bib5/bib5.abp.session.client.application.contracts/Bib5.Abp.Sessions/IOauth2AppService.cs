using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Bib5.Abp.Sessions;

public interface IOauth2AppService : IApplicationService, IRemoteService
{
	Task<IActionResult> CallBackAsync(string code, string state, string iss);
}
