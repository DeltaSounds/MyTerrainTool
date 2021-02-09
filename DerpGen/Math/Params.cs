namespace DerpGen
{
	public class Params
	{
		private int _width = 96;
		private int _height = 96;
		private int _seed = 232;
		private float _scale = 20;
		private int _octaves = 5;
		private float _persistence = 1;
		private float _lacunarity = 1;
		private float _radius = 50;

		static private float _offsetX = 0;
		static private float _offsetY = 0;
		private Vector2 _offset = new Vector2(_offsetX, _offsetY);

		public int Width { get => _width; set => _width = value; }
		public int Height { get => _height; set => _height = value; }
		public int Seed { get => _seed; set => _seed = value; }
		public float Scale { get => _scale; set => _scale = value; }
		public int Octaves { get => _octaves; set => _octaves = value; }
		public float Persistence { get => _persistence; set => _persistence = value; }
		public float Lacunarity { get => _lacunarity; set => _lacunarity = value; }
		public Vector2 Offset { get => _offset; set => _offset = value; }
		public float Radius { get => _radius; set => _radius = value; }
	}
}
