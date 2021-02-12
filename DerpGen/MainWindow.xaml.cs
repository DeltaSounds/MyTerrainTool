using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Math;
using Microsoft.Win32;

namespace DerpGen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _actualFilename;
		
		private int width = 128;
		private int height = 128;
		private int seed = 349;
		private float scale = 32;
		private int octaves = 4;
		private float persistence = 0.5f;
		private float lacunarity = 2;
		private Vector2 offset = new Vector2(16, 8);
		private float radius = 35;

		public MainWindow()
		{
			InitializeComponent();
			
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
			this.MainImage.Source = bitmap;
		}

		private void OnSaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if(_actualFilename == null)
			{
				_actualFilename = GetFilename();
			}

			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = File.OpenWrite(_actualFilename);
			bf.Serialize(fs, seed);
		}

		private string GetFilename()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			
			// File format filters
			sfd.Filter = "*.dat | Data Files | *.txt | Text Files";
			sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			bool ? result = sfd.ShowDialog();

			if (result.HasValue && result.Value) return sfd.FileName;

			return null;
		}
	}
}
