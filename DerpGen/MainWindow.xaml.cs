using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Accord.Math;
using Microsoft.Win32;

namespace DerpGen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly string COMPANY_NAME = "SAE";
		private static readonly string APPLICATION_NAME = "DerpGen";

		private string _currentFilePath;
		private static string _configCompanyPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), COMPANY_NAME);
		private static string _configApplicationPath = System.IO.Path.Combine(_configCompanyPath, APPLICATION_NAME);
		private static string _configPath = System.IO.Path.Combine(_configApplicationPath, "config.json");	
		private Config _config;

		private readonly BackgroundWorker bgWorker = new BackgroundWorker();
		private delegate void myDel(WriteableBitmap bitmap);

		public Parameters Parameter = new Parameters();
		public Renderer Render = new Renderer();
		public Random random = new Random();
		public List<string> RecentList = new List<string>();
		public event ProgressChangedEventHandler ProgressChanged;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = Parameter;
			LoadConfig();

			bgWorker.WorkerReportsProgress = true;

			bgWorker.DoWork += new DoWorkEventHandler(RenderImage);
			bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateCompleted);
			bgWorker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
		}

		private void GenerateCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if(!e.Cancelled)
			{
				MainImage.Source = (WriteableBitmap)e.Result;
				pBar.Value = 0;

				outpotLogBox.Items.Add(">> Heightmap generated successfully!");
			}
			else
			{
				MessageBox.Show("Generation has been terminated!", "Warning!", MessageBoxButton.OK);
				outpotLogBox.Items.Add(">> Heightmap generation has been terminated!");
			}
		}

		private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pBar.Value = e.ProgressPercentage;
		}

		private void RenderImage(object sender, DoWorkEventArgs e)
		{

			// https://nerdparadise.com/programming/csharpimageediting

			float[,] noiseMap = Noise.GenerateNoiseMap(Parameter.MapSize, Parameter.MapSize, Parameter.Seed, Parameter.Scale, Parameter.Octaves, Parameter.Persistence, Parameter.Lacunarity, Parameter.Offset, Parameter.Radius);
			
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
					this.Dispatcher.Invoke(() =>
					{
						pBar.Value = (float)x / w * 100;
					});
				}
				
			}

			// apply pixels to bitmap
			bitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w * 4, 0);
			var bmp = bitmap;
			bmp.Freeze();
			e.Result = bmp;

			//On Aborted
			if (bgWorker.CancellationPending)
			{
				e.Cancel = true;
				return;
			}
		}

		private void OnExitApplication(object sender, EventArgs e)
		{
			if (_config != null)
			{
				ConfigLoader.Save(_config, _configPath);
			}
		}

		private void OpenDocumentation(object sender, RoutedEventArgs e)
		{
			DocWindow docWindow = new DocWindow();
			docWindow.ShowDialog();
		}

		private void LoadConfig()
		{
			var loadConfig = ConfigLoader.Load(_configPath);

			if (loadConfig != null)
			{
				outpotLogBox.Items.Add($">> Loaded config file from {_configPath}");
				_config = loadConfig;
			}
			else
			{
				_config = new Config();

				outpotLogBox.Items.Add(">> Config file not found! Creating new config file...");
				Directory.CreateDirectory(_configCompanyPath);
				Directory.CreateDirectory(_configApplicationPath);
			}

			ApplyConfig();
		}

		private void ApplyConfig()
		{
			// Set Window Size
			this.Width = _config.Width;
			this.Height = _config.Height;
			
			Parameters param = new Parameters();
			
			// Set Parameters
			param.Seed = _config.Seed;
			param.Scale = _config.Scale;
			param.Octaves = _config.Octaves;
			param.Persistence = _config.Persistence;
			param.Lacunarity = _config.Lacunarity;
			param.Radius = _config.Radius;
			param.OffsetX = _config.OffsetX;
			param.OffsetY = _config.OffsetY;

			param.RandomizeSeedOnGenerate = _config.RandomizeSeedOnGenerate;

			Parameter = param;
			
			DataContext = Parameter;
			// Load Recent List
			for (int i = 0; i < _config.recentFilePaths.Count; i++)
			{
				if(File.Exists(_config.recentFilePaths[i]))
				{
					RecentList.Add(_config.recentFilePaths[i]);
				}
				else
				{
					_config.recentFilePaths.RemoveAt(i);
				}
			}

			UpdateRecentFiles();
			outpotLogBox.Items.Add(">> Generating heightmap...");
			bgWorker.RunWorkerAsync();

			outpotLogBox.Items.Add(">> Config has been applied!");
		}

		private void UpdateRecentFiles()
		{
			recentMenuItem.Items.Clear();

			for (int i = 0; i < RecentList.Count; i++)
			{

				// Create a new menu item & set name
				MenuItem menuItem = new MenuItem();
				menuItem.Header = RecentList[i];
				menuItem.Click += OpenRecent;

				recentMenuItem.Items.Add(menuItem);
			}

			Separator sep = new Separator();
			recentMenuItem.Items.Add(sep);

			MenuItem clear = new MenuItem();
			clear.Header = "Clear List";
			clear.Click += ClearList;

			recentMenuItem.Items.Add(clear);
		}

		private void OpenRecent(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;

			MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				Parameter = SaveManager.Load(item.Header.ToString());
				DataContext = Parameter;
				bgWorker.RunWorkerAsync();
			}
		}

		private void ClearList(object sender, RoutedEventArgs e)
		{
			RecentList.Clear();
			_config.recentFilePaths.Clear();
			UpdateRecentFiles();
		}

		private void AddRecentItem(string fileName)
		{
			if (!RecentList.Contains(fileName))
			{
				RecentList.Add(fileName);
				_config.recentFilePaths = RecentList;
				UpdateRecentFiles();
			}
		}

		private void OpenPropertiesWindow(object sender, RoutedEventArgs e)
		{
			PropertiesWindow PropWindow = new PropertiesWindow(_config);
			PropWindow.ShowDialog();
		}

		private void OpenNew(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				string savePath = GetSavePath();

				if(savePath != null)
				{
					Parameter.MapSize = 256;
					Parameter.Seed = 100;
					Parameter.Scale = 80;
					Parameter.Octaves = 5;
					Parameter.Persistence = 0.5f;
					Parameter.Lacunarity = 2;
					Parameter.Radius = 90;
					Parameter.OffsetX = 0;
					Parameter.OffsetY = 0;

					
					
					SaveManager.Save(Parameter, savePath);
					DataContext = SaveManager.Load(savePath);
					bgWorker.RunWorkerAsync();
				}
			}
		}

		private void ExportAsPNG(object sender, RoutedEventArgs e)
		{
			string path = GetExportPath();

			if(path != null)
			{
				using (FileStream fileStream = File.Create(path))
				{
					PngBitmapEncoder encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(MainImage.Source as BitmapSource));
					encoder.Save(fileStream);
					MessageBox.Show("Successfully exported heightmap!", "Success!", MessageBoxButton.OK);
				}
			}

		}

		private void UpdateHeightMap(object sender, RoutedEventArgs e)
		{
			if(!bgWorker.IsBusy)
			{
				outpotLogBox.Items.Add(">> Generating Terrain..");
				bgWorker.RunWorkerAsync();
			}
		}

		private void RandomizeSeed(object sender, RoutedEventArgs e)
		{
			Parameter.Seed = random.Next(1, int.MaxValue);
			inpSeed.Text = Parameter.Seed.ToString();
		}

		private void OnSave(object sender, RoutedEventArgs e)
		{
			if (_currentFilePath == null)
			{
				string savePath = GetSavePath();

				if (savePath != null)
				{
					_currentFilePath = savePath;
				}
			}

			SaveManager.Save(Parameter, _currentFilePath);
			outpotLogBox.Items.Add($">> Saved file at {_currentFilePath}");
		}

		private void OnSaveAs(object sender, RoutedEventArgs e)
		{
			string savePath = GetSavePath();

			if (savePath != null)
			{
				SaveManager.Save(Parameter, savePath);
				outpotLogBox.Items.Add($">> Saved file at {savePath}");
			}

		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			string loadPath = GetLoadPath();

			if(loadPath != null)
			{
				var loadData = SaveManager.Load(loadPath);

				if (loadData != null)
				{
					Parameter = loadData;
					DataContext = Parameter;
					bgWorker.RunWorkerAsync();
					outpotLogBox.Items.Add($">> Opened file at {loadPath}");
				}
			}

		}

		private void ExitApplication(object sender, CancelEventArgs e)
		{
			//MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);
			//
			//if (result == MessageBoxResult.No)
			//{
			//	e.Cancel = true;
			//	return;
			//}
		}

		private string GetLoadPath()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = "Data File | *.dat";
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			bool? result = openFileDialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				AddRecentItem(openFileDialog.FileName);
				return openFileDialog.FileName;
			}

			return null;
		}

		private string GetExportPath()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.Filter = "PNG file | .png";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			bool? result = saveFileDialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				return saveFileDialog.FileName;
			}

			return null;
		}

		private string GetSavePath()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.DefaultExt = "dat";

			// File format filters
			saveFileDialog.Filter = "Data File | *.dat";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			bool? result = saveFileDialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				AddRecentItem(saveFileDialog.FileName);
				return saveFileDialog.FileName;
			}

			return null;
		}
	}
}
