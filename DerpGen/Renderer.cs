using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DerpGen
{
	class Renderer
	{
		public void DrawHeightMap(MainWindow image)
		{
			Params parameter = new Params();

			int width = parameter.Width;
			int height = parameter.Height;
			int seed = parameter.Seed;
			float scale = parameter.Scale;
			int octaves = parameter.Octaves;
			float persistence = parameter.Persistence;
			float lacunarity = parameter.Lacunarity;
			Vector2 offset = parameter.Offset;
			float radius = parameter.Radius;

			
			float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset, radius);

			
			int w = noiseMap.GetLength(0);
			int h = noiseMap.GetLength(1);


			WriteableBitmap bitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);


			uint[] pixels = new uint[w * h];

			byte r;
			byte g;
			byte b;
			byte a;


			for (int x = 0; x < h; x++)
			{
				for (int y = 0; y < w; y++)
				{
					int i = y * w + x;

					r = (byte)(noiseMap[x, y] * 255);
					g = (byte)(noiseMap[x, y] * 255);
					b = (byte)(noiseMap[x, y] * 255);
					a = 255;

					pixels[i] = (uint)((a << 24) + (r << 16) + (g << 8) + b);
				}
			}

			// apply pixels to bitmap
			bitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w * 4, 0);

			// set image source to the new bitmap
			image.MainImage.Source = bitmap;
		}
	}
}
