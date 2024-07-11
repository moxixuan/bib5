using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO;

public static class StreamExpansions
{
	private static ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();

	public static async Task<byte> ReadByteAsync(this Stream stream, CancellationToken cancellationToken)
	{
		byte[] buffer = _arrayPool.Rent(1);
		await stream.ReadExactlyAsync(buffer.AsMemory(0, 1), cancellationToken);
		byte result = buffer[0];
		_arrayPool.Return(buffer);
		return result;
	}
}
