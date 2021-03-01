using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DerpGen
{
	/// <summary>
	/// Interaction logic for PropertiesWindow.xaml
	/// </summary>
	public partial class PropertiesWindow : Window
	{
		private MainWindow _mainWindow = new MainWindow();

		public PropertiesWindow(Config config)
		{
			InitializeComponent();
			DataContext = config;

			config.PropertyChanged += OnValueChanged;
		}

		private void OnValueChanged(object sender, PropertyChangedEventArgs e)
		{
			applyButton.IsEnabled = true;
		}

		private void CancelButton(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ApplyButton(object sender, RoutedEventArgs e)
		{
			applyButton.IsEnabled = false;
		}
	}
}
