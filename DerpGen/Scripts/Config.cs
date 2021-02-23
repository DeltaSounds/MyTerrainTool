using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DerpGen
{
	[System.Serializable]
	public class Config
	{
		private float _positionX = 0;
		private float _positionY = 0;

		private float _width = 784;
		private float _height = 522;

		private int _mapWidth = 300;
		private int _mapHeight = 300;
		private int _seed = 121;
		private float _scale = 32;
		private int _octaves = 3;
		private float _persistence = 0.5f;
		private float _lacunarity = 2;
		private float _radius = 60;
		private float _offsetX = 0;
		private float OffsetY = 0;

		private bool RandomizeSeedOnGenerated = false;

		public List<string> recentFilePaths = new List<string>();
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
			string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
			File.WriteAllText(path, configJson);
		}
	}
}
