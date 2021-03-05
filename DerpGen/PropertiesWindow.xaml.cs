using System.ComponentModel;
using System.Windows;

namespace DerpGen
{
	/// <summary>
	/// Interaction logic for PropertiesWindow.xaml
	/// </summary>
	public partial class PropertiesWindow : Window
	{
		private MainWindow _mainWindow = new MainWindow();

		public PropertiesWindow(Config config, MainWindow mw)
		{
			InitializeComponent();
			DataContext = config;

			config.PropertyChanged += OnValueChanged;
			_mainWindow = mw;
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
			_mainWindow.ApplyProperties();
		}
	}
}
