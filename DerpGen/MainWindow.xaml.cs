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
using Microsoft.Win32;

namespace DerpGen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _actualFilename;
		


		public MainWindow()
		{
			InitializeComponent();
			
			//Render Heightmap on the image
			Renderer renderer = new Renderer();
			renderer.DrawHeightMap(this);
		}

		private void OnSaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if(_actualFilename == null)
			{
				_actualFilename = GetFilename();
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.OpenWrite(_actualFilename);
			Params parameters = new Params();
			binaryFormatter.Serialize(fileStream, parameters);
		}

		private string GetFilename()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			
			// File format filters
			saveFileDialog.Filter = "*.dat | Data Files | *.txt | Text Files";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			bool ? result = saveFileDialog.ShowDialog();

			if (result.HasValue && result.Value) return saveFileDialog.FileName;

			return null;
		}
	}
}
