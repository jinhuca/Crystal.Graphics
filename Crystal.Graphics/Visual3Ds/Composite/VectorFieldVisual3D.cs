namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a vector field.
  /// </summary>
  public class VectorFieldVisual3D : ModelVisual3D
  {
    /// <summary>
    /// The model.
    /// </summary>
    private readonly ModelVisual3D model;

    /// <summary>
    /// The body geometry.
    /// </summary>
    private MeshGeometry3D? body;

    /// <summary>
    /// The head geometry.
    /// </summary>
    private MeshGeometry3D? head;

    /// <summary>
    /// Initializes a new instance of the <see cref = "VectorFieldVisual3D" /> class.
    /// </summary>
    public VectorFieldVisual3D()
    {
      Positions = new Point3DCollection();
      Directions = new Vector3DCollection();
      Fill = Brushes.Blue;
      ThetaDiv = 37;
      Diameter = 1;
      HeadLength = 2;

      model = new ModelVisual3D();
      Children.Add(model);
    }

    /// <summary>
    /// Gets or sets the diameter.
    /// </summary>
    /// <value>The diameter.</value>
    public double Diameter { get; set; }

    /// <summary>
    /// Gets or sets the directions.
    /// </summary>
    /// <value>The directions.</value>
    public Vector3DCollection Directions { get; set; }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    /// <value>The fill.</value>
    public Brush Fill { get; set; }

    /// <summary>
    /// Gets or sets the length of the head.
    /// </summary>
    /// <value>The length of the head.</value>
    public double HeadLength { get; set; }

    /// <summary>
    /// Gets or sets the positions.
    /// </summary>
    /// <value>The positions.</value>
    public Point3DCollection Positions { get; set; }

    /// <summary>
    /// Gets or sets the number of divisions of the arrows.
    /// </summary>
    /// <value>The theta div.</value>
    public int ThetaDiv { get; set; }

    /// <summary>
    /// Updates the model.
    /// </summary>
    public void UpdateModel()
    {
      CreateGeometry();
      var c = new Model3DGroup();
      var mat = MaterialHelper.CreateMaterial(Fill);
      var l = HeadLength * Diameter;

      for(var i = 0; i < Positions.Count; i++)
      {
        var p = Positions[i];
        var d = Directions[i];
        var headModel = new GeometryModel3D
        {
          Geometry = head,
          Material = mat,
          Transform = CreateHeadTransform(p + d, d)
        };
        c.Children.Add(headModel);

        var u = d;
        u.Normalize();
        var bodyModel = new GeometryModel3D
        {
          Geometry = body,
          Material = mat,
          Transform = CreateBodyTransform(p, u * (1.0 - l / d.Length))
        };
        c.Children.Add(bodyModel);
      }

      model.Content = c;
    }

    /// <summary>
    /// The create body transform.
    /// </summary>
    /// <param name="p">
    /// The p.
    /// </param>
    /// <param name="z">
    /// The z.
    /// </param>
    /// <returns>
    /// </returns>
    private static Transform3D CreateBodyTransform(Point3D p, Vector3D z)
    {
      var length = z.Length;
      z.Normalize();
      var x = z.FindAnyPerpendicular();
      x.Normalize();
      var y = Vector3D.CrossProduct(z, x);

      var mat = new Matrix3D(
          x.X, x.Y, x.Z, 0, y.X, y.Y, y.Z, 0, z.X * length, z.Y * length, z.Z * length, 0, p.X, p.Y, p.Z, 1);
      return new MatrixTransform3D(mat);
    }

    /// <summary>
    /// The create head transform.
    /// </summary>
    /// <param name="p">
    /// The p.
    /// </param>
    /// <param name="z">
    /// The z.
    /// </param>
    /// <returns>
    /// </returns>
    private static Transform3D CreateHeadTransform(Point3D p, Vector3D z)
    {
      z.Normalize();
      var x = z.FindAnyPerpendicular();
      x.Normalize();
      var y = Vector3D.CrossProduct(z, x);

      var mat = new Matrix3D(x.X, x.Y, x.Z, 0, y.X, y.Y, y.Z, 0, z.X, z.Y, z.Z, 0, p.X, p.Y, p.Z, 1);

      return new MatrixTransform3D(mat);
    }

    /// <summary>
    /// The create geometry.
    /// </summary>
    private void CreateGeometry()
    {
      var r = Diameter / 2;
      var l = HeadLength * Diameter;

      // arrowhead
      var pc = new PointCollection { new(-l, r), new(-l, r * 2), new(0, 0) };

      var headBuilder = new MeshBuilder(false, false);
      headBuilder.AddRevolvedGeometry(pc, null, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), ThetaDiv);
      head = headBuilder.ToMesh();
      head.Freeze();

      // body
      pc = new PointCollection { new(0, 0), new(0, r), new(1, r) };

      var bodyBuilder = new MeshBuilder(false, false);
      bodyBuilder.AddRevolvedGeometry(pc, null, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), ThetaDiv);
      body = bodyBuilder.ToMesh();
      body.Freeze();
    }

  }
}