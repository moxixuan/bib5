using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Hosting.Console.Sessions;

public interface IBasicSessionManager
{
	/// <summary>
	/// 获取会话信息,如果回话接近过期将会被刷新(阈值:过期前1分钟)
	/// </summary>
	/// <param name="sessionKey">会话Key,用于区分会话属于哪个远程服务</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<Session?> GetAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 设置会话信息
	/// </summary>
	/// <param name="session"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<Session> SetAsync(Session session, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 是否有效回话
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<bool> IsVaildAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 获取会话状态
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<SessionStatus> GetSessionStatusAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 退出
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// 重新挂载会话数据到缓存
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task ReloadAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));
}
