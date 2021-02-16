using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DerpGen
{
	[Serializable]
	public class Parameters
	{
		private int _mapSize = 300;

		private int _seed = 121;
		private float _scale = 32;
		private int _octaves = 3;
		private float _persistence = 0.5f;
		private float _lacunarity = 2;
		private float _radius = 60;
		private float _offsetX = 0;
		private float _offsetY = 0;
		
		public event PropertyChangedEventHandler PropertyChanged;

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

		public Vector2 Offset { get => new Vector2(_offsetX, _offsetY); }

		public bool UpdateOnValueChanged { get; set; }
		public bool RandomizeSeedOnStart { get; set; }


		// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification?view=netframeworkdesktop-4.8
		public void OnValueChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
