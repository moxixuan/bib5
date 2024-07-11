namespace System.IO.StreamTransfers;

public class TransferredEventArgs
{
	public ArraySegment<byte> DataTransferred;

	public TransferredEventArgs(ArraySegment<byte> dataTransferred)
	{
		DataTransferred = dataTransferred;
	}
}
