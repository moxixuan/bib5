using System;
using System.Net.Http;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;

namespace Bib5.Abp.Sessions;

public class SessionHubHostedService : Bib5BackgroundService
{
	protected HubConnection HubConnection { get; private set; }

	protected AbpRemoteServiceOptions RemoteServiceOptions => base.LazyServiceProvider.LazyGetRequiredService<IOptions<AbpRemoteServiceOptions>>().Value;

	protected IDesktopSessionManager SessionManager => base.LazyServiceProvider.LazyGetRequiredService<IDesktopSessionManager>();

	public bool IsConnected { get; private set; }

	protected IOptionsMonitor<SessionOptions> SessionOptions => base.LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<SessionOptions>>();

	public event EventHandler<EventArgs>? Connected;

	public event EventHandler<EventArgs>? Disconnected;

	public SessionHubHostedService(IAbpLazyServiceProvider lazyServiceProvider)
		: base(lazyServiceProvider)
	{
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_ = RemoteServiceOptions.RemoteServices.GetConfigurationOrDefault("Bib5Session").BaseUrl + "/hub/session";
		HubConnection = HubConnectionBuilderExtensions.WithAutomaticReconnect(HubConnectionBuilderHttpExtensions.WithUrl((IHubConnectionBuilder)new HubConnectionBuilder(), RemoteServiceOptions.RemoteServices.GetConfigurationOrDefault("Bib5Session").BaseUrl + "/hub/session", (Action<HttpConnectionOptions>)delegate(HttpConnectionOptions options)
		{
			options.HttpMessageHandlerFactory = delegate(HttpMessageHandler h)
			{
				((HttpClientHandler)h).ServerCertificateCustomValidationCallback = (HttpRequestMessage _, X509Certificate2? _, X509Chain? _, SslPolicyErrors _) => true;
				return h;
			};
			options.WebSocketConfiguration = delegate(ClientWebSocketOptions options)
			{
				options.RemoteCertificateValidationCallback = (object _, X509Certificate? _, X509Chain? _, SslPolicyErrors _) => true;
			};
			options.AccessTokenProvider = async () => (await SessionManager.GetAsync(SessionOptions.CurrentValue.Session.SessionKey, stoppingToken))?.AccessToken;
		}), (IRetryPolicy)(object)new RepeatedlyRetryPolicy()).Build();
		HubConnection.Closed += async delegate
		{
			await Task.Delay(2000);
			await HubConnection.StartAsync(stoppingToken);
			base.Logger.LogDebug("SessionHub 断开 {0}", HubConnection.ConnectionId);
		};
		HubConnection.Reconnecting += async delegate
		{
			IsConnected = false;
			await Task.Run(delegate
			{
				this.Disconnected?.Invoke(this, EventArgs.Empty);
			});
		};
		HubConnection.Reconnected += async delegate
		{
			IsConnected = true;
			await Task.Run(delegate
			{
				this.Connected?.Invoke(this, EventArgs.Empty);
			});
		};
		HubConnectionExtensions.On(HubConnection, "login-notify", (Func<Task>)async delegate
		{
			await SessionManager.ReloadAsync(SessionOptions.CurrentValue.Session.SessionKey);
			base.Logger.LogDebug("SessionHub 刷新会话(用户登录)");
		});
		HubConnectionExtensions.On(HubConnection, "logout-notify", (Func<Task>)async delegate
		{
			await SessionManager.ReloadAsync(SessionOptions.CurrentValue.Session.SessionKey);
			base.Logger.LogDebug("SessionHub 刷新会话(用户登出)");
		});
		await HubConnection.StartAsync((IRetryPolicy)(object)new RepeatedlyRetryPolicy(), stoppingToken);
		IsConnected = true;
		this.Connected?.Invoke(this, EventArgs.Empty);
		base.Logger.LogDebug("SessionHub 连接成功 {0}", HubConnection.ConnectionId);
	}

	private async Task InvokeAsync(string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
	{
		try
		{
			await HubConnectionExtensions.InvokeCoreAsync(HubConnection, methodName, args, cancellationToken);
		}
		catch (InvalidOperationException)
		{
			await HubConnection.StopAsync(cancellationToken);
			await HubConnection.StartAsync((IRetryPolicy)(object)new RepeatedlyRetryPolicy(), cancellationToken);
			base.Logger.LogDebug("SessionHub 操作失败,主动重新连接 {0}", HubConnection.ConnectionId);
			await Task.Delay(500, cancellationToken);
			await InvokeAsync(methodName, args, cancellationToken);
		}
	}
}
