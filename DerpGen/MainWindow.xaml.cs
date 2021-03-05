using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
		private static string _configCompanyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), COMPANY_NAME);
		private static string _configApplicationPath = Path.Combine(_configCompanyPath, APPLICATION_NAME);
		private static string _configPath = Path.Combine(_configApplicationPath, "config.json");
		private Config _config;

		private readonly BackgroundWorker bgWorker = new BackgroundWorker();
		private delegate void myDel(WriteableBitmap bitmap);

		public Parameters Parameter = new Parameters();
		public Random random = new Random();
		public List<string> RecentList = new List<string>();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = Parameter;

			bgWorker.WorkerReportsProgress = true;
			bgWorker.WorkerSupportsCancellation = true;

			bgWorker.DoWork += new DoWorkEventHandler(RenderImage);
			bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateCompleted);
			bgWorker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);


			LoadConfig();
		}

		#region Configurations
		private void LoadConfig()
		{
			var loadConfig = ConfigLoader.Load(_configPath);

			if (loadConfig != null)
			{
				outpotLogBox.Items.Add($">> Loaded config file from {_configPath}");
				_config = loadConfig;

				//Check if the configs dimensions are less than min
				if (_config.Width < MinWidth)
				{
					_config.Width = (float)MinWidth;
				}
				else if (_config.Height < MinHeight)
				{
					_config.Height = (float)MinHeight;
				}
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
				if (File.Exists(_config.recentFilePaths[i]))
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

		public void ApplyProperties()
		{
			Width = _config.Width;
			Height = _config.Height;

			Parameter.RandomizeSeedOnGenerate = _config.RandomizeSeedOnGenerate;
		}
		#endregion

		#region Generate HeightMap (BackgroudWorker)
		private void RenderImage(object sender, DoWorkEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				btnGen.IsEnabled = false;
				btnAbort.IsEnabled = true;

				if (Parameter.RandomizeSeedOnGenerate)
				{
					Parameter.Seed = random.Next(1, int.MaxValue);
					inpSeed.Text = Parameter.Seed.ToString();
				}
			});



			// https://nerdparadise.com/programming/csharpimageediting
			float[,] noiseMap = Noise.GenerateNoiseMap(Parameter, bgWorker);

			if (bgWorker.CancellationPending)
			{
				e.Cancel = true;
				return;
			}

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
				bgWorker.ReportProgress((int)((float)x / h * 100));
			}

			// apply pixels to bitmap
			bitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w * 4, 0);
			var bmp = bitmap;
			bmp.Freeze();
			e.Result = bmp;
		}

		private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pBar.Value = e.ProgressPercentage;
		}

		private void GenerateCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				MainImage.Source = (WriteableBitmap)e.Result;
				pBar.Value = 0;

				btnGen.IsEnabled = true;
				btnAbort.IsEnabled = false;

				outpotLogBox.Items.Add(">> Heightmap generated successfully!");
			}
			else
			{
				MessageBox.Show("Generation has been terminated!", "Warning!", MessageBoxButton.OK);
				outpotLogBox.Items.Add(">> Heightmap generation has been terminated!");

				pBar.Value = 0;

				btnGen.IsEnabled = true;
				btnAbort.IsEnabled = false;
			}
		}
		#endregion

		#region Open Windows
		private void OpenPropertiesWindow(object sender, RoutedEventArgs e)
		{
			PropertiesWindow PropWindow = new PropertiesWindow(_config, this);
			PropWindow.ShowDialog();
		}

		private void OpenDocumentation(object sender, RoutedEventArgs e)
		{
			DocWindow docWindow = new DocWindow();
			docWindow.ShowDialog();
		}
		#endregion

		#region RoutedEvents
		private void Abort(object sender, RoutedEventArgs e)
		{
			bgWorker.CancelAsync();
		}

		private void ClearOutputLog(object sender, RoutedEventArgs e)
		{
			outpotLogBox.Items.Clear();
		}

		private void OpenNew(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				string savePath = GetSavePath();

				if (savePath != null)
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


					outpotLogBox.Items.Add(">> Loaded new file");
					SaveManager.Save(Parameter, savePath);
					DataContext = SaveManager.Load(savePath);
					bgWorker.RunWorkerAsync();
				}
			}
		}

		private void UpdateHeightMap(object sender, RoutedEventArgs e)
		{
			if (!bgWorker.IsBusy)
			{
				outpotLogBox.Items.Add(">> Generating Terrain...");
				bgWorker.RunWorkerAsync();
			}
		}

		private void RandomizeSeed(object sender, RoutedEventArgs e)
		{
			Parameter.Seed = random.Next(1, int.MaxValue);
			inpSeed.Text = Parameter.Seed.ToString();
		}
		#endregion

		#region Recent List
		private void UpdateRecentFiles()
		{
			recentMenuItem.Items.Clear();

			for (int i = 0; i < RecentList.Count; i++)
			{

				// Create a new menu item & set name
				MenuItem menuItem = new MenuItem();
				menuItem.Header = RecentList[i];
				menuItem.Template = (ControlTemplate)FindResource("SingleDropDownMenuItem");
				menuItem.Click += OpenRecent;

				recentMenuItem.Items.Add(menuItem);
			}

			Separator sep = new Separator();
			sep.Style = (Style)FindResource("sepDark");
			recentMenuItem.Items.Add(sep);

			MenuItem clear = new MenuItem();
			clear.Header = "Clear List";
			clear.Template = (ControlTemplate)FindResource("SingleDropDownMenuItem");
			clear.Click += ClearList;

			recentMenuItem.Items.Add(clear);
		}

		private void OpenRecent(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)e.Source;

			MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				if (File.Exists(item.Header.ToString()))
				{
					Parameter = SaveManager.Load(item.Header.ToString());
					DataContext = Parameter;
					bgWorker.RunWorkerAsync();
				}
				else
				{
					MessageBox.Show("The file you are trying to open no longer exists!", "Error!", MessageBoxButton.OK);
				}
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
		#endregion

		#region Save
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
		#endregion

		#region Load
		private void OnLoad(object sender, RoutedEventArgs e)
		{
			string loadPath = GetLoadPath();

			if (loadPath != null)
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
		#endregion

		#region Export
		private void ExportAsPNG(object sender, RoutedEventArgs e)
		{
			string path = GetExportPath();

			if (path != null)
			{
				using (FileStream fileStream = File.Create(path))
				{
					PngBitmapEncoder encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(MainImage.Source as BitmapSource));
					encoder.Save(fileStream);
					MessageBox.Show("Successfully exported heightmap!", "Success!", MessageBoxButton.OK);
					outpotLogBox.Items.Add($">> File Exported to {path}");
				}
			}

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
		#endregion

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to close? Any unsaved changes will be lost!", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				e.Cancel = false;
				return;
			}
			else
			{
				if (_config != null)
				{
					ConfigLoader.Save(_config, _configPath);
					e.Cancel = true;
				}
			}

		}
	}
}
