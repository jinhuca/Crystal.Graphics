namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a wireframe for the specified bounding box.
  /// </summary>
  public class BoundingBoxWireFrameVisual3D : LinesVisual3D
  {
    /// <summary>
    /// Gets or sets the bounding box.
    /// </summary>
    /// <value> The bounding box. </value>
    public Rect3D BoundingBox
    {
      get => (Rect3D)GetValue(BoundingBoxProperty);
      set => SetValue(BoundingBoxProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BoundingBox"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.Register(
      nameof(BoundingBox), typeof(Rect3D), typeof(BoundingBoxWireFrameVisual3D), new UIPropertyMetadata(new Rect3D(), BoxChanged));

    /// <summary>
    /// Updates the box.
    /// </summary>
    protected virtual void OnBoxChanged()
    {
      if(BoundingBox.IsEmpty)
      {
        Points = null;
        return;
      }

      var points = new Point3DCollection();

      var bb = BoundingBox;

      var p0 = new Point3D(bb.X, bb.Y, bb.Z);
      var p1 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z);
      var p2 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z);
      var p3 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z);
      var p4 = new Point3D(bb.X, bb.Y, bb.Z + bb.SizeZ);
      var p5 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
      var p6 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
      var p7 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z + bb.SizeZ);

      Action<Point3D, Point3D> addEdge = (p, q) =>
      {
        points.Add(p);
        points.Add(q);
      };

      addEdge(p0, p1);
      addEdge(p1, p2);
      addEdge(p2, p3);
      addEdge(p3, p0);

      addEdge(p4, p5);
      addEdge(p5, p6);
      addEdge(p6, p7);
      addEdge(p7, p4);

      addEdge(p0, p4);
      addEdge(p1, p5);
      addEdge(p2, p6);
      addEdge(p3, p7);

      Points = points;
    }

    /// <summary>
    /// Called when the box dimensions changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void BoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BoundingBoxWireFrameVisual3D)d).OnBoxChanged();
  }
}