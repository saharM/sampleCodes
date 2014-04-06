using System;
using System.Windows;
using WebSearcher.ViewModels;

namespace WebSearcher.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = new MainWindowViewModel();
		}
	}
}
