namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that translates all children relative to the specified origin.
  /// </summary>
  public class Expander3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="ExpandOrigin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpandOriginProperty = DependencyProperty.Register(
        "ExpandOrigin", typeof(Point3D?), typeof(Expander3D), new UIPropertyMetadata(null, ExpansionChanged));

    /// <summary>
    /// Identifies the <see cref="Expansion"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionProperty = DependencyProperty.Register(
        "Expansion", typeof(double), typeof(Expander3D), new UIPropertyMetadata(2.0, ExpansionChanged));

    /// <summary>
    /// The original transforms.
    /// </summary>
    private readonly Dictionary<Model3D, Transform3D> originalTransforms = new();

    /// <summary>
    /// The actual expand origin.
    /// </summary>
    private Point3D actualExpandOrigin;

    /// <summary>
    /// Gets or sets the origin of the expansion.
    /// </summary>
    /// <value>The expand origin.</value>
    public Point3D? ExpandOrigin
    {
      get => (Point3D?)GetValue(ExpandOriginProperty);

      set => SetValue(ExpandOriginProperty, value);
    }

    /// <summary>
    /// Gets or sets the expansion factor.
    /// </summary>
    /// <value>The expansion.</value>
    public double Expansion
    {
      get => (double)GetValue(ExpansionProperty);

      set => SetValue(ExpansionProperty, value);
    }

    /// <summary>
    /// Expands to the specified value.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    public void ExpandTo(double value, double animationTime)
    {
      var a = new DoubleAnimation(value, new Duration(TimeSpan.FromMilliseconds(animationTime)))
      {
        AccelerationRatio = 0.3,
        DecelerationRatio = 0.5
      };
      BeginAnimation(ExpansionProperty, a);
    }

    /// <summary>
    /// The expansion changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void ExpansionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Expander3D)d).OnExpansionChanged();
    }

    /// <summary>
    /// The expand.
    /// </summary>
    protected virtual void OnExpansionChanged()
    {
      if(!ExpandOrigin.HasValue)
      {
        if(Content != null)
        {
          actualExpandOrigin = Content.Bounds.Location;
        }
      }
      else
      {
        actualExpandOrigin = ExpandOrigin.Value;
      }

      Content.Traverse<GeometryModel3D>(Expand);
    }

    /// <summary>
    /// The expand.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <param name="transformation">
    /// The transformation.
    /// </param>
    private void Expand(GeometryModel3D model, Transform3D transformation)
    {
      Transform3D ot;
      if(originalTransforms.ContainsKey(model))
      {
        ot = originalTransforms[model];
      }
      else
      {
        ot = model.Transform;
        originalTransforms.Add(model, ot);
      }

      var totalTransform = Transform3DHelper.CombineTransform(transformation, ot);

      if(model.Geometry is not MeshGeometry3D mesh)
      {
        return;
      }

      var bounds = new Rect3D();
      foreach(var i in mesh.TriangleIndices)
      {
        bounds.Union(totalTransform.Transform(mesh.Positions[i]));
      }

      var p = bounds.Location;
      var d = p - actualExpandOrigin;
      d *= Expansion;
      var p2 = actualExpandOrigin + d;
      var t = new TranslateTransform3D(p2 - p);

      model.Transform = Transform3DHelper.CombineTransform(ot, t);
    }

  }
}