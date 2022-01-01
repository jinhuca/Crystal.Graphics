﻿using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a panorama cube or a skybox.
  /// </summary>
  public class PanoramaCube3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="AutoCenter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoCenterProperty = DependencyProperty.Register(
        "AutoCenter", typeof(bool), typeof(PanoramaCube3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="ShowSeams"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowSeamsProperty = DependencyProperty.Register(
        "ShowSeams", typeof(bool), typeof(PanoramaCube3D), new UIPropertyMetadata(false, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Size"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        "Size", typeof(double), typeof(PanoramaCube3D), new UIPropertyMetadata(100.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        "Source", typeof(string), typeof(PanoramaCube3D), new UIPropertyMetadata(null, SourceChanged));

    /// <summary>
    /// The visual child.
    /// </summary>
    private readonly ModelVisual3D visualChild;

    /// <summary>
    /// Initializes a new instance of the <see cref = "PanoramaCube3D" /> class.
    /// </summary>
    public PanoramaCube3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [auto center].
    /// </summary>
    /// <value><c>true</c> if [auto center]; otherwise, <c>false</c>.</value>
    public bool AutoCenter
    {
      get => (bool)GetValue(AutoCenterProperty);

      set => SetValue(AutoCenterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show seams.
    /// </summary>
    public bool ShowSeams
    {
      get => (bool)GetValue(ShowSeamsProperty);

      set => SetValue(ShowSeamsProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the cube.
    /// </summary>
    /// <value>The size.</value>
    public double Size
    {
      get => (double)GetValue(SizeProperty);

      set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the panorama/skybox directory or file prefix.
    /// </summary>
    /// <remarks>
    /// If a directory is specified, the filename prefix will be set to "cube".
    /// If the filename prefix is "cube", the faces of the cube should be named
    /// cube_f.jpg
    /// cube_b.jpg
    /// cube_l.jpg
    /// cube_r.jpg
    /// cube_u.jpg
    /// cube_d.jpg
    /// </remarks>
    /// <value>The source.</value>
    public string Source
    {
      get => (string)GetValue(SourceProperty);

      set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// The source changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((PanoramaCube3D)obj).UpdateModel();
    }

    /// <summary>
    /// The geometry changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PanoramaCube3D)d).UpdateModel();
    }

    /// <summary>
    /// The add cube side.
    /// </summary>
    /// <param name="normal">
    /// The normal.
    /// </param>
    /// <param name="up">
    /// The up.
    /// </param>
    /// <param name="fileName">
    /// The file name.
    /// </param>
    /// <returns>
    /// </returns>
    private GeometryModel3D AddCubeSide(Vector3D normal, Vector3D up, string fileName)
    {
      string fullPath = Path.GetFullPath(fileName);

      if(!File.Exists(fullPath))
      {
        return null;
      }

      var image = new BitmapImage();
      image.BeginInit();
      image.UriSource = new Uri(fullPath);

      // image.CacheOption = BitmapCacheOption.Default;
      // image.CreateOptions = BitmapCreateOptions.None;
      image.EndInit();

      // image.DownloadCompleted += new EventHandler(image_DownloadCompleted);
      var brush = new ImageBrush(image);
      var material = new DiffuseMaterial(brush);

      var mesh = new MeshGeometry3D();
      var right = Vector3D.CrossProduct(normal, up);
      var origin = new Point3D(0, 0, 0);
      double f = ShowSeams ? 0.995 : 1;
      f *= Size;
      var n = normal * Size;

      right *= f;
      up *= f;
      Point3D p1 = origin + n - up - right;
      Point3D p2 = origin + n - up + right;
      Point3D p3 = origin + n + up + right;
      Point3D p4 = origin + n + up - right;
      mesh.Positions.Add(p1);
      mesh.Positions.Add(p2);
      mesh.Positions.Add(p3);
      mesh.Positions.Add(p4);
      mesh.TextureCoordinates.Add(new Point(0, 1));
      mesh.TextureCoordinates.Add(new Point(1, 1));
      mesh.TextureCoordinates.Add(new Point(1, 0));
      mesh.TextureCoordinates.Add(new Point(0, 0));
      mesh.TriangleIndices.Add(0);
      mesh.TriangleIndices.Add(1);
      mesh.TriangleIndices.Add(2);
      mesh.TriangleIndices.Add(2);
      mesh.TriangleIndices.Add(3);
      mesh.TriangleIndices.Add(0);

      return new GeometryModel3D(mesh, material);
    }

    /// <summary>
    /// The update model.
    /// </summary>
    private void UpdateModel()
    {
      string directory = Path.GetDirectoryName(Source);
      string prefix = Path.GetFileName(Source);

      if(string.IsNullOrEmpty(prefix))
      {
        prefix = "cube";
      }

      var front = Path.Combine(directory, prefix + "_f.jpg");
      var left = Path.Combine(directory, prefix + "_l.jpg");
      var right = Path.Combine(directory, prefix + "_r.jpg");
      var back = Path.Combine(directory, prefix + "_b.jpg");
      var up = Path.Combine(directory, prefix + "_u.jpg");
      var down = Path.Combine(directory, prefix + "_d.jpg");

      var group = new Model3DGroup();
      group.Children.Add(AddCubeSide(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), front));
      group.Children.Add(AddCubeSide(new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), left));
      group.Children.Add(AddCubeSide(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), right));
      group.Children.Add(AddCubeSide(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), back));
      group.Children.Add(AddCubeSide(new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), up));
      group.Children.Add(AddCubeSide(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), down));

      visualChild.Content = group;
    }

  }
}