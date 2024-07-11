using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace System.IO.StreamTransfers;

public interface IStreamTransferService : IScopedDependency
{
	Guid Id { get; }

	long BytesTransferred1To2 { get; }

	long BytesTransferred2To1 { get; }

	event EventHandler<TransferredEventArgs> Transferred1To2;

	event EventHandler<TransferredEventArgs> Transferred2To1;

	Task<(long BytesTransferred1To2, long BytesTransferred2To1)> TransferAsync(Stream stream1, Stream stream2, int? bufferSize = null, bool autoClose = true, CancellationToken cancellationToken = default(CancellationToken));
}
