using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace System.IO.StreamTransfers;

public class DefaultStreamTransferService : IStreamTransferService, IScopedDependency
{
	private long _bytesTransferred1To2;

	private long _bytesTransferred2To1;

	private bool _autoClose = true;

	protected IOptionsMonitor<StreamTransferConfig> StreamTransferConfig;

	protected static ArrayPool<byte> ArrayPool { get; } = ArrayPool<byte>.Create();


	protected ILogger Logger { get; }

	public Guid Id { get; }

	public long BytesTransferred1To2 => _bytesTransferred1To2;

	public long BytesTransferred2To1 => _bytesTransferred2To1;

	public event EventHandler<TransferredEventArgs> Transferred1To2;

	public event EventHandler<TransferredEventArgs> Transferred2To1;

	public DefaultStreamTransferService(IOptionsMonitor<StreamTransferConfig> streamTransferConfig, IGuidGenerator guidGenerator, ILogger<DefaultStreamTransferService> logger)
	{
		Id = guidGenerator.Create();
		StreamTransferConfig = streamTransferConfig;
		Logger = logger;
	}

	public async Task<(long BytesTransferred1To2, long BytesTransferred2To1)> TransferAsync(Stream stream1, Stream stream2, int? bufferSize = null, bool autoClose = true, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (bufferSize.HasValue)
		{
			Check.Range(bufferSize.Value, "bufferSize", 0, int.MaxValue);
		}
		else
		{
			bufferSize = StreamTransferConfig.CurrentValue.BufferSize;
		}
		_autoClose = autoClose;
		Task<long> task1to2 = SingleDirectionalTransferAsync(stream1, stream2, bufferSize.Value, delegate(long bytesTransferred)
		{
			_bytesTransferred1To2 += bytesTransferred;
		}, this.Transferred1To2, cancellationToken, "request").ContinueWith((Task<long> t) => t.IsCompletedSuccessfully ? t.Result : 0);
		Task<long> task2to1 = SingleDirectionalTransferAsync(stream2, stream1, bufferSize.Value, delegate(long bytesTransferred)
		{
			_bytesTransferred2To1 += bytesTransferred;
		}, this.Transferred2To1, cancellationToken, "response").ContinueWith((Task<long> t) => t.IsCompletedSuccessfully ? t.Result : 0);
		await Task.WhenAll<long>(task1to2, task2to1);
		return (BytesTransferred1To2: task1to2.Result, BytesTransferred2To1: task2to1.Result);
	}

	private async Task<long> SingleDirectionalTransferAsync(Stream streamSrc, Stream streamDest, int bufferSize, Action<long> setBytesTransferred, EventHandler<TransferredEventArgs>? eventHandler, CancellationToken cancellationToken, string name)
	{
		byte[] buffer = ArrayPool.Rent(bufferSize);
		long bytesTransferred = 0L;
		try
		{
			while (true)
			{
				int num;
				int bytesRead = (num = await streamSrc.ReadAsync(buffer, cancellationToken));
				if (num <= 0)
				{
					break;
				}
				await streamDest.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
				await streamDest.FlushAsync(cancellationToken);
				Interlocked.Add(ref bytesTransferred, bytesRead);
				if (Logger.IsEnabled(LogLevel.Trace))
				{
					Logger.LogTrace("StreamTransfer {0} {1} {2}", Id, name, bytesRead);
					if (name == "request")
					{
						Logger.LogTrace("StreamTransfer {0} {1} {2}", Id, name, Encoding.UTF8.GetString(buffer.AsSpan(0, bytesRead)));
					}
				}
				setBytesTransferred(bytesTransferred);
				eventHandler?.Invoke(this, new TransferredEventArgs(new ArraySegment<byte>(buffer, 0, bytesRead)));
			}
			if (_autoClose)
			{
				streamDest.Close();
			}
			Logger.LogDebug("StreamTransfer {0} 正常关闭", name);
		}
		catch (IOException ex) when (!cancellationToken.IsCancellationRequested)
		{
			if (!(ex.InnerException is SocketException ex2))
			{
				Logger.LogDebug("StreamTransfer {0} 异常关闭 stackTrace: {1}", name, ex.StackTrace);
				return bytesTransferred;
			}
			if (ex2.SocketErrorCode != SocketError.OperationAborted && ex2.SocketErrorCode != SocketError.ConnectionReset)
			{
				Logger.LogDebug("StreamTransfer {0} 异常关闭 stackTrace: {1}", name, ex2.StackTrace);
			}
			else
			{
				Logger.LogDebug("StreamTransfer {0} 正常关闭 code: {1}", name, ex2.SocketErrorCode);
			}
		}
		catch (ObjectDisposedException) when (!cancellationToken.IsCancellationRequested)
		{
			Logger.LogDebug("StreamTransfer {0} 正常关闭", name);
		}
		catch (Exception ex4) when (!cancellationToken.IsCancellationRequested)
		{
			Logger.LogDebug("StreamTransfer {0} 异常关闭 stackTrace: {1}", name, ex4.StackTrace);
			AbpLoggerExtensions.LogException(Logger, ex4, (LogLevel?)null);
		}
		finally
		{
			ArrayPool.Return(buffer);
		}
		return bytesTransferred;
	}
}
