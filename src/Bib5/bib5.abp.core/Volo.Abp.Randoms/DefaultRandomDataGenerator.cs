using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Randoms;

public class DefaultRandomDataGenerator : IRandomDataGenerator, ISingletonDependency
{
	private readonly string[] _urlSymbol = new string[9] { "/", "+", "?", ":", " ", "%", "#", "&", "=" };

	public void FillBytes(byte[] data)
	{
		RandomNumberGenerator.Fill(data);
	}

	public void FillBytes(byte[] data, int offset, int count)
	{
		RandomNumberGenerator.Fill(new Span<byte>(data, offset, count));
	}

	public void FillBytes(Span<byte> data)
	{
		RandomNumberGenerator.Fill(data);
	}

	public byte[] GetBytes(int count)
	{
		byte[] array = new byte[count];
		RandomNumberGenerator.Fill(array);
		return array;
	}

	public string GetString(int count, string[]? filteredStrings)
	{
		byte[] array = new byte[count];
		byte[] array2 = new byte[count * 2];
		RandomNumberGenerator.Fill(array);
		Base64.EncodeToUtf8(array, array2, out var _, out var bytesWritten);
		string text = Encoding.UTF8.GetString(array2, 0, bytesWritten);
		if (filteredStrings != null)
		{
			foreach (string oldValue in filteredStrings)
			{
				text = text.Replace(oldValue, string.Empty);
			}
		}
		text = text.Substring(0, count);
		if (text.Length < count)
		{
			return GetString(count, filteredStrings);
		}
		return text;
	}

	public string GetString(int count, bool filtrateUrlSymbol = true)
	{
		return GetString(count, _urlSymbol);
	}
}
