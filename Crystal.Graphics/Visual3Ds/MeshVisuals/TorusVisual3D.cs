namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a torus defined by two diameters (torus and it's tube).
  /// </summary>
  public class TorusVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the (torus) diameter.
    /// </summary>
    /// <value>The diameter. The default value is <c>3</c>.</value>
    public double TorusDiameter
    {
      get => (double)GetValue(TorusDiameterProperty);
      set
      {
        if(value >= 0.0)
          SetValue(TorusDiameterProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="TorusDiameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TorusDiameterProperty = DependencyProperty.Register(
      nameof(TorusDiameter), typeof(double), typeof(TorusVisual3D), new UIPropertyMetadata(3.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the tube diameter.
    /// </summary>
    /// <value>The tube diameter. The default value is <c>1</c>.</value>
    public double TubeDiameter
    {
      get => (double)GetValue(TubeDiameterProperty);
      set
      {
        if(value >= 0.0)
          SetValue(TubeDiameterProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="TubeDiameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TubeDiameterProperty = DependencyProperty.Register(
      nameof(TubeDiameter), typeof(double), typeof(TorusVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the UDiv.
    /// </summary>
    /// <value>The UDiv. The default value is <c>36</c>.</value>
    public int ThetaDiv
    {
      get => (int)GetValue(ThetaDivProperty);
      set
      {
        if(value >= 3)
          SetValue(ThetaDivProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="ThetaDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
      nameof(ThetaDiv), typeof(int), typeof(TorusVisual3D), new UIPropertyMetadata(36, GeometryChanged));

    /// <summary>
    /// Gets or sets the PhiDiv.
    /// </summary>
    /// <value>The PhiDiv. The default value is <c>24</c>.</value>
    public int PhiDiv
    {
      get => (int)GetValue(PhiDivProperty);
      set
      {
        if(value >= 3)
          SetValue(PhiDivProperty, value);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PhiDiv"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PhiDivProperty = DependencyProperty.Register(
      nameof(PhiDiv), typeof(int), typeof(TorusVisual3D), new UIPropertyMetadata(24, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D" />.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
      var builder = new MeshBuilder(false);
      builder.AddTorus(TorusDiameter, TubeDiameter, ThetaDiv, PhiDiv);
      return builder.ToMesh();
    }
  }
}