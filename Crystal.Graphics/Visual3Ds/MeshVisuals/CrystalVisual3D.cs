namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a space curve.
  /// </summary>
  public class CrystalVisual3D : ParametricModelVisual3D
  {
    /// <summary>
    /// Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Point3D Origin
    {
      get => (Point3D)GetValue(OriginProperty);
      set => SetValue(OriginProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Origin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
      nameof(Origin),
      typeof(Point3D),
      typeof(CrystalVisual3D),
      new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Diameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
      nameof(Diameter), 
      typeof(double), 
      typeof(CrystalVisual3D), 
      new UIPropertyMetadata(0.5, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Length"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
      nameof(Length), 
      typeof(double), 
      typeof(CrystalVisual3D), 
      new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Phase"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(
      nameof(Phase), 
      typeof(double), 
      typeof(CrystalVisual3D), 
      new UIPropertyMetadata(0.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Radius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
      nameof(Radius),
      typeof(double), 
      typeof(CrystalVisual3D), 
      new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Turns"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TurnsProperty = DependencyProperty.Register(
      nameof(Turns),
      typeof(double), 
      typeof(CrystalVisual3D), 
      new UIPropertyMetadata(1.0, GeometryChanged));

    /// <summary>
    /// Gets or sets the diameter.
    /// </summary>
    /// <value>The diameter.</value>
    public double Diameter
    {
      get => (double)GetValue(DiameterProperty);
      set => SetValue(DiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length.</value>
    public double Length
    {
      get => (double)GetValue(LengthProperty);
      set => SetValue(LengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the phase.
    /// </summary>
    /// <value>The phase.</value>
    public double Phase
    {
      get => (double)GetValue(PhaseProperty);
      set => SetValue(PhaseProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    public double Radius
    {
      get => (double)GetValue(RadiusProperty);
      set => SetValue(RadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of turns.
    /// </summary>
    /// <value>The turns.</value>
    public double Turns
    {
      get => (double)GetValue(TurnsProperty);
      set => SetValue(TurnsProperty, value);
    }

    /// <summary>
    /// Evaluates the surface.
    /// </summary>
    /// <param name="u">
    /// The u parameter.
    /// </param>
    /// <param name="v">
    /// The v parameter.
    /// </param>
    /// <param name="texCoord">
    /// The texture coordinate.
    /// </param>
    /// <returns>
    /// The evaluated <see cref="Point3D"/>.
    /// </returns>
    protected override Point3D Evaluate(double u, double v, out Point texCoord)
    {
      double color = u;
      v *= 2 * Math.PI;

      double b = Turns * 2 * Math.PI;
      double r = Radius / 2;
      double d = Diameter;
      double dr = Diameter / r;
      double p = Phase / 180 * Math.PI;

      double x = r * Math.Cos((b * u) + p) * (2 + (dr * Math.Cos(v)));
      double y = r * Math.Sin((b * u) + p) * (2 + (dr * Math.Cos(v)));
      double z = (u * Length) + (d * Math.Sin(v));

      texCoord = new Point(color, 0);
      return Origin + new Vector3D(x, y, z);
    }
  }
}