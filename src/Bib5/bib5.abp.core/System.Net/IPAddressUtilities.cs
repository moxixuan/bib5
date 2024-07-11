using System.Collections.Generic;
using NUglify.Helpers;

namespace System.Net;

public static class IPAddressUtilities
{
	public static IPAddress[] ParseIPAddresses(this string[] ipStrings)
	{
		List<IPAddress> addresses = new List<IPAddress>();
		if (ipStrings.Length == 0)
		{
			addresses.Add(IPAddress.Any);
		}
		else
		{
			NUglifyExtensions.ForEach<string>((IEnumerable<string>)ipStrings, (Action<string>)delegate(string ip)
			{
				if (IPAddress.TryParse(ip, out IPAddress address))
				{
					addresses.Add(address);
					return;
				}
				throw new ArgumentException("\"{ip}\"不是有效的IP地址", ip);
			});
		}
		return addresses.ToArray();
	}
}
