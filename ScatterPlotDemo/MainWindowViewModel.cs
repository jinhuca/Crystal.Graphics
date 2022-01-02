using Crystal.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ScatterPlotDemo
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private int totalPoints = 1_000;
    private Random random = new Random();

    public Point3D[] Data { get; set; }

    public double[] Values { get; set; }

    public Model3DGroup Lights { get; set; }

    public Brush SurfaceBrush => GradientBrushes.RainbowStripes;

    private System.Threading.Timer timer;

    public MainWindowViewModel()
    {
      GenerateModel();
      //timer = new Timer(UpdateData, null, 1000, 750);
    }

    private void UpdateData(object? state)
    {
      for(int i = 0; i < 30; i++)
      {
        var temp = random.Next(0, totalPoints - 1);
        Data[temp] = new Point3D(random.NextDouble() * 6, random.NextDouble() * 6, random.NextDouble() * 6);
      }

      Values = Data.Select(d => random.NextDouble()).ToArray();

      RaisePropertyChanged(nameof(Data));
      RaisePropertyChanged(nameof(Values));
      RaisePropertyChanged(nameof(SurfaceBrush));
    }

    private void CreateData(object? state)
    {
      Data = new Point3D[totalPoints];
      for(int i = 0; i < totalPoints; i++)
      {
        Data[i] = new Point3D(random.NextDouble() * 6, random.NextDouble() * 6, random.NextDouble() * 6);
      }
      RaisePropertyChanged(nameof(Data));
      RaisePropertyChanged(nameof(Values));
      RaisePropertyChanged(nameof(SurfaceBrush));
    }

    private void GenerateModel()
    {
      CreateData(null);
      //Data = new[] { new Point3D(1, 1, 2), new Point3D(2, 1, 3), new Point3D(0, 1, 2), new Point3D(0, 2, 1), new Point3D(0,0,0) };
      //Data = Enumerable.Range(0, 7 * 7 * 7).Select(i => new Point3D(i % 7, (i % 49) / 7, i / 49)).ToArray();
      Lights = new Model3DGroup();
      Lights.Children.Add(new AmbientLight(Colors.White));

      var rnd = new Random();
      Values = Data.Select(d => rnd.NextDouble()).ToArray();

      RaisePropertyChanged(nameof(Data));
      RaisePropertyChanged(nameof(Values));
      RaisePropertyChanged(nameof(SurfaceBrush));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
