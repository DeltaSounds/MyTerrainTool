using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace DerpGen
{
	public static class SaveManager
	{
		public static Parameters Load(string path)
		{
			try
			{
				using (FileStream fileStream = File.OpenRead(path))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					Parameters result = (Parameters)binaryFormatter.Deserialize(fileStream);

					return result;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error: {e.Message}", "Error");
				return null;
			}
		}

		public static void Save(Parameters parameters, string path)
		{
			try
			{
				using (FileStream fileStream = File.OpenWrite(path))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(fileStream, parameters);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error: {e.Message}", "Error");
				return;
			}
		}
	}
}
