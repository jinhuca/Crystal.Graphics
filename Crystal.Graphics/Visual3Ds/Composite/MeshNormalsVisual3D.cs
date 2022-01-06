namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows the normals of the specified mesh geometry.
  /// </summary>
  public class MeshNormalsVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="Color"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        "Color", typeof(Color), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(Colors.Blue, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        "Diameter", typeof(double), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(0.1, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="Mesh"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MeshProperty = DependencyProperty.Register(
        "Mesh", typeof(MeshGeometry3D), typeof(MeshNormalsVisual3D), new UIPropertyMetadata(null, MeshChanged));

    /// <summary>
    /// Gets or sets the color of the normals.
    /// </summary>
    /// <value> The color. </value>
    public Color Color
    {
      get => (Color)GetValue(ColorProperty);

      set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the diameter of the normal arrows.
    /// </summary>
    /// <value> The diameter. </value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);

      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the mesh.
    /// </summary>
    /// <value> The mesh. </value>
    public MeshGeometry3D Mesh
    {
      get => (MeshGeometry3D)GetValue(MeshProperty);

      set => SetValue(MeshProperty, value);
    }

    /// <summary>
    /// The mesh changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void MeshChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((MeshNormalsVisual3D)obj).OnMeshChanged();
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected virtual void OnMeshChanged()
    {
      Children.Clear();

      var builder = new MeshBuilder();
      for(var i = 0; i < Mesh.Positions.Count; i++)
      {
        builder.AddArrow(
            Mesh.Positions[i], Mesh.Positions[i] + Mesh.Normals[i], Diameter, 3, 10);
      }

      Content = new GeometryModel3D
      {
        Geometry = builder.ToMesh(true),
        Material = MaterialHelper.CreateMaterial(Color)
      };
    }

  }
}