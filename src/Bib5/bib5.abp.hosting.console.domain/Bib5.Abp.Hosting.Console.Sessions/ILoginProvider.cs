using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Hosting.Console.Sessions;

/// <summary>
/// 登陆提供程序
/// </summary>
public interface ILoginProvider
{
	Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session> LoginAsync(string sessionKey, string refreshToken, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 授权码方式登陆(开始)
	/// <para>生成状态,并返回</para>
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken);

	/// <summary>
	/// 授权码方式登陆(结束)
	/// <para>使用授权码获取token,并返回Session</para>
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="code"></param>
	/// <param name="state"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken = default(CancellationToken));
}
