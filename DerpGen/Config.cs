using Newtonsoft.Json;
using System.IO;

namespace DerpGen
{
	[System.Serializable]
	public class Config
	{
		public float PositionX = 0;
		public float PositionY = 0;

		public float Width = 442;
		public float Height = 784;

		public int MapWidth = 300;
		public int MapHeight = 300;
		public int Seed = 121;
		public float Scale = 32;
		public int Octaves = 3;
		public float Persistence = 0.5f;
		public float Lacunarity = 2;
		public float Radius = 60;
		public float OffsetX = 0;
		public float OffsetY = 0;

		public bool UpdateOnValueChanged = true;
		public bool RandomizeSeedOnStart = false;
	}

	public static class ConfigLoader
	{
		public static Config Load(string path)
		{
			if(File.Exists(path))
			{
				string configJson = File.ReadAllText(path);

				var config = JsonConvert.DeserializeObject<Config>(configJson);

				return config;
			}
			else
			{
				return null;
			}

			
		}

		public static void Save(Config config, string path)
		{
			string configJson = JsonConvert.SerializeObject(config);
			File.WriteAllText(path, configJson);
		}
	}
}
