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
		private static string _configPath = System.IO.Path.Combine(_configApplicationPath, "Config.json");
		private Config _config;

		public Parameters Parameter = new Parameters();
		public Renderer Render = new Renderer();
		public Random random = new Random();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = Parameter;
			LoadConfig();

			Parameter.PropertyChanged += UpdateAll;

			Render.DrawHeightMap(this, Parameter);
		}

		private void UpdateAll(object sender, PropertyChangedEventArgs e)
		{
			if(Parameter.UpdateOnValueChanged)
			{
				Render.DrawHeightMap(this, Parameter);
			}
		}

		private void LoadConfig()
		{
			var loadConfig = ConfigLoader.Load(_configPath);

			if(loadConfig != null)
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

			Parameter.UpdateOnValueChanged = _config.UpdateOnValueChanged;
			Parameter.RandomizeSeedOnStart = _config.RandomizeSeedOnStart;
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
			string savePath = GetSavePath();

			if(_currentFilePath == null && savePath != null)
			{
				_currentFilePath = savePath;
			}

			SaveManager.Save(Parameter, _currentFilePath);
		}

		private void OnSaveAs(object sender, RoutedEventArgs e)
		{
			SaveManager.Save(Parameter, GetSavePath());
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
				return saveFileDialog.FileName;
			}

			return null;
		}
	}
}
