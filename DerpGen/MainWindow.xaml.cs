using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		private static readonly string COMPANY_NAME = "SAE";
		private static readonly string APPLICATION_NAME = "DerpGen";

		private string _currentFilePath;
		private static string _configCompanyPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), COMPANY_NAME);
		private static string _configApplicationPath = System.IO.Path.Combine(_configCompanyPath, APPLICATION_NAME);
		private static string _configPath = System.IO.Path.Combine(_configApplicationPath, "config.json");	
		private Config _config;

		public Parameters Parameter = new Parameters();
		public Renderer Render = new Renderer();
		public Random random = new Random();
		public List<string> RecentList = new List<string>();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = Parameter;
			LoadConfig();

			Closed += OnExitApplication;

			Render.DrawHeightMap(this, Parameter);
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
				_config = loadConfig;
			}
			else
			{
				_config = new Config();

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

			// Set Parameters
			Parameter.Seed = _config.Seed;
			Parameter.Scale = _config.Scale;
			Parameter.Octaves = _config.Octaves;
			Parameter.Persistence = _config.Persistence;
			Parameter.Lacunarity = _config.Lacunarity;
			Parameter.Radius = _config.Radius;
			Parameter.OffsetX = _config.OffsetX;
			Parameter.OffsetY = _config.OffsetY;

			Parameter.RandomizeSeedOnStart = _config.RandomizeSeedOnGenerated;

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

		}

		private void UpdateRecentFiles()
		{
			recentMenuItem.Items.Clear();

			for (int i = 0; i < RecentList.Count; i++)
			{

				// Create a new menu item & set name
				MenuItem menuItem = new MenuItem();
				menuItem.Header = RecentList[i];

				recentMenuItem.Items.Add(menuItem);
			}

			Separator sep = new Separator();
			recentMenuItem.Items.Add(sep);

			MenuItem clear = new MenuItem();
			clear.Header = "Clear List";
			clear.Click += ClearList;

			recentMenuItem.Items.Add(clear);
		}

		private void ClearList(object sender, RoutedEventArgs e)
		{
			RecentList.Clear();
			_config.recentFilePaths.Clear();
			UpdateRecentFiles();
		}

		private void AddRecentItem(string fileName)
		{
			Console.WriteLine("========================** function has started! **========================");

			if (RecentList.Count > 0)
			{
				Console.WriteLine("RecentList.Count is more than 0");
				for (int i = 0; i < RecentList.Count; i++)
				{
					Console.WriteLine($"for loop running at index: {i}");

					if (RecentList[i] == fileName)
					{
						Console.WriteLine("FilePath already exists!! ");
						Console.WriteLine("Updating Config \n Updating Visuals");
						Console.WriteLine("========================** Terminating function **========================");
						_config.recentFilePaths = RecentList;
						UpdateRecentFiles();
						return;
					}

					Console.WriteLine($"Recent List has added path: {fileName}");
					RecentList.Add(fileName);
				}
			}
			else
			{
				Console.WriteLine($"Recent list is below 0, Adding file path: {fileName} to List");
				RecentList.Add(fileName);
			}

			Console.WriteLine("Updating Config \n Updating Visuals");
			_config.recentFilePaths = RecentList;
			UpdateRecentFiles();
			Console.WriteLine("========================** Terminating function **========================");
		}

		private void OpenPropertiesWindow(object sender, RoutedEventArgs e)
		{
			PropertiesWindow PropWindow = new PropertiesWindow(Parameter);
			PropWindow.ShowDialog();
		}

		private void ResetParameters(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Any unsaved changes will be lost! do you wish to proceed?", "Warning!", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				ApplyConfig();
				Render.DrawHeightMap(this, Parameter);
			}
		}

		private void ExportAsPNG(object sender, RoutedEventArgs e)
		{
			using (FileStream fileStream = File.Create("DrawHeightMap.png"))
			{
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(MainImage.Source as BitmapSource));
				encoder.Save(fileStream);
			}
		}

		private void UpdateHeightMap(object sender, RoutedEventArgs e)
		{
			Render.DrawHeightMap(this, Parameter);
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
		}

		private void OnSaveAs(object sender, RoutedEventArgs e)
		{
			string savePath = GetSavePath();

			if (savePath == null)
				return;

			SaveManager.Save(Parameter, savePath);
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			var loadData = SaveManager.Load(GetLoadPath());

			if (loadData != null)
			{
				Parameter = loadData;

				DataContext = Parameter;
				Render.DrawHeightMap(this, Parameter);
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
				return openFileDialog.FileName;
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
