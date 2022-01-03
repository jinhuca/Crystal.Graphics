using System.Windows;

namespace ScatterPlotDemo
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel(); 
    }
  }
}
