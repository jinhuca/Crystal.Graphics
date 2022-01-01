using Crystal.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ScatterPlot
{
  public class MainViewModel : INotifyPropertyChanged
  {
    public Point3D[] Data { get; set; }

    public double[] Values { get; set; }

    public Model3DGroup Lights
    {
      get
      {
        var group = new Model3DGroup();
        group.Children.Add(new AmbientLight(Colors.White));
        return group;
      }
    }

    public Brush SurfaceBrush => GradientBrushes.RainbowStripes;

    public MainViewModel() => UpdateModel();

    private void UpdateModel()
    {
      //Data = new Point3D[] { new Point3D(2, 1, 1), new Point3D(1, 1, 2), new Point3D(1, 2, 4) };
      Data = Enumerable.Range(0, 6 * 6 * 6).Select(i => new Point3D(i % 6, (i % 36) / 6, i / 36)).ToArray();

      var rnd = new Random();
      this.Values = Data.Select(d => rnd.NextDouble()).ToArray();

      RaisePropertyChanged(nameof(Data));
      RaisePropertyChanged(nameof(Values));
      RaisePropertyChanged(nameof(SurfaceBrush));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
