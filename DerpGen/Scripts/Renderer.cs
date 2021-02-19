using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DerpGen
{
	public class Renderer
	{
		public void DrawHeightMap(MainWindow mainWindow, Parameters Params)
		{
			// https://nerdparadise.com/programming/csharpimageediting

			float[,] noiseMap = Noise.GenerateNoiseMap(Params.MapSize, Params.MapSize, Params.Seed, Params.Scale, Params.Octaves, Params.Persistence, Params.Lacunarity, Params.Offset, Params.Radius);

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
			mainWindow.MainImage.Source = bitmap;

		}
	}
}
