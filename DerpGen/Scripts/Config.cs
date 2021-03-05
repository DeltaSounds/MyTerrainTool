using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DerpGen
{
	[System.Serializable]
	public class Config
	{
		private float _positionX = 0;
		private float _positionY = 0;

		private float _width = 1020;
		private float _height = 768;

		private int _mapSize = 300;
		private int _seed = 121;
		private float _scale = 32;
		private int _octaves = 3;
		private float _persistence = 0.5f;
		private float _lacunarity = 2;
		private float _radius = 60;
		private float _offsetX = 0;
		private float _offsetY = 0;

		private bool _randomizeSeedOnGenerated = false;



		public event PropertyChangedEventHandler PropertyChanged;

		public float Width
		{
			get { return _width; }
			set
			{
				if (value != _width)
				{
					_width = value;
					OnValueChanged();
				}
			}
		}

		public float Height
		{
			get { return _height; }
			set
			{
				if (value != _height)
				{
					_height = value;
					OnValueChanged();
				}
			}
		}

		public float PositionX
		{
			get { return _positionX; }
			set
			{
				if (value != _positionX)
				{
					_positionX = value;
					OnValueChanged();
				}
			}
		}

		public float PositionY
		{
			get { return _positionY; }
			set
			{
				if (value != _positionY)
				{
					_positionY= value;
					OnValueChanged();
				}
			}
		}



		public int MapSize
		{
			get { return _mapSize; }
			set
			{
				if (value != _mapSize)
				{
					_mapSize = value;
					OnValueChanged();
				}
			}
		}

		public int Seed
		{
			get { return _seed; }
			set
			{
				if (value != _seed)
				{
					_seed = value;
					OnValueChanged();
				}
			}
		}

		public float Scale
		{
			get { return _scale; }
			set
			{
				if (value != _scale)
				{
					_scale = value;
					OnValueChanged();
				}
			}
		}

		public int Octaves
		{
			get { return _octaves; }
			set
			{
				if (value != _octaves)
				{
					_octaves = value;
					OnValueChanged();
				}
			}
		}

		public float Persistence
		{
			get { return _persistence; }
			set
			{
				if (value != _persistence)
				{
					_persistence = value;
					OnValueChanged();
				}
			}
		}

		public float Lacunarity
		{
			get { return _lacunarity; }
			set
			{
				if (value != _lacunarity)
				{
					_lacunarity = value;
					OnValueChanged();
				}
			}
		}

		public float Radius
		{
			get { return _radius; }
			set
			{
				if (value != _radius)
				{
					_radius = value;
					OnValueChanged();
				}
			}
		}

		public float OffsetX
		{
			get { return _offsetX; }
			set
			{
				if (value != _offsetX)
				{
					_offsetX = value;
					OnValueChanged();
				}
			}
		}

		public float OffsetY
		{
			get { return _offsetY; }
			set
			{
				if (value != _offsetY)
				{
					_offsetY = value;
					OnValueChanged();
				}
			}
		}

		public bool RandomizeSeedOnGenerate
		{
			get { return _randomizeSeedOnGenerated; }
			set
			{
				if (value != _randomizeSeedOnGenerated)
				{
					_randomizeSeedOnGenerated = value;
					OnValueChanged();
				}
			}
		}

		public List<string> recentFilePaths = new List<string>();

		// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification?view=netframeworkdesktop-4.8
		public void OnValueChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
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
