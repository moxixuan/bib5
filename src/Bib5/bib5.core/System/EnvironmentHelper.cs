namespace System;

public static class EnvironmentHelper
{
	public static void SetEnvironmentVariableInProcess(string dll_path)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
		if (!environmentVariable.ToUpper().Contains(dll_path.ToUpper()))
		{
			string value = environmentVariable.TrimEnd(';') + ";" + dll_path + ";";
			Environment.SetEnvironmentVariable("PATH", value, EnvironmentVariableTarget.Process);
		}
	}
}
