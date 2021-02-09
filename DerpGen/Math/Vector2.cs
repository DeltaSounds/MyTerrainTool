namespace DerpGen
{
	public struct Vector2
	{
		public float x { get; set; }
		public float y { get; set; }

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2 Normalized { get => Normalize(this); }

		public float Magnitude { get => (float)System.Math.Sqrt(x * x + y * y); }

		public static Vector2 operator +(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vector2 operator *(Vector2 v, float s)
		{
			return new Vector2(v.x * s, v.y * s);
		}

		public static Vector2 operator *(float s, Vector2 v)
		{
			return v * s;
		}

		public static Vector2 operator /(Vector2 v, float s)
		{
			return new Vector2(v.x / s, v.y / s);
		}

		public static Vector2 Normalize(Vector2 v)
		{
			var m = 1 / v.Magnitude;
			return new Vector2(v.x * m, v.y * m);
		}

		public static float Dot(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.x + v1.y * v2.y;
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			float diff_x = a.x - b.x;
			float diff_y = a.y - b.y;
			return (float)System.Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
		}
	}
}
