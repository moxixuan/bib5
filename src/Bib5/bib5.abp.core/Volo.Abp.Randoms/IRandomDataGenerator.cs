using System;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Randoms;

public interface IRandomDataGenerator : ISingletonDependency
{
	byte[] GetBytes(int count);

	void FillBytes(byte[] data);

	void FillBytes(byte[] data, int offset, int count);

	void FillBytes(Span<byte> data);

	string GetString(int count, string[]? filteredStrings);

	string GetString(int count, bool filtrateUrlSymbol = true);
}
