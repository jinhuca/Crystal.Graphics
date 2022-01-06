namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows Mesh3D meshes.
  /// </summary>
  public class MeshVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Identifies the <see cref="EdgeDiameter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EdgeDiameterProperty = DependencyProperty.Register(
        "EdgeDiameter", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.03, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="EdgeMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EdgeMaterialProperty = DependencyProperty.Register(
        "EdgeMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gray));

    /// <summary>
    /// Identifies the <see cref="FaceBackMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FaceBackMaterialProperty =
        DependencyProperty.Register(
            "FaceBackMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gray));

    /// <summary>
    /// Identifies the <see cref="FaceMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FaceMaterialProperty = DependencyProperty.Register(
        "FaceMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Blue));

    /// <summary>
    /// Identifies the <see cref="Mesh"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MeshProperty = DependencyProperty.Register(
        "Mesh", typeof(Mesh3D), typeof(MeshVisual3D), new UIPropertyMetadata(null, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="SharedVertices"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SharedVerticesProperty = DependencyProperty.Register(
        "SharedVertices", typeof(bool), typeof(MeshVisual3D), new UIPropertyMetadata(false, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="ShrinkFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShrinkFactorProperty = DependencyProperty.Register(
        "ShrinkFactor", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.0, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="VertexMaterial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VertexMaterialProperty = DependencyProperty.Register(
        "VertexMaterial", typeof(Material), typeof(MeshVisual3D), new UIPropertyMetadata(Materials.Gold));

    /// <summary>
    /// Identifies the <see cref="VertexRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VertexRadiusProperty = DependencyProperty.Register(
        "VertexRadius", typeof(double), typeof(MeshVisual3D), new UIPropertyMetadata(0.05, MeshChanged));

    /// <summary>
    /// Identifies the <see cref="VertexResolution"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VertexResolutionProperty =
        DependencyProperty.Register(
            "VertexResolution", typeof(int), typeof(MeshVisual3D), new UIPropertyMetadata(2));

    /// <summary>
    /// Gets or sets the edge diameter.
    /// </summary>
    /// <value> The edge diameter. </value>
    public double EdgeDiameter
    {
      get => (double)GetValue(EdgeDiameterProperty);

      set => SetValue(EdgeDiameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the edge material.
    /// </summary>
    /// <value> The edge material. </value>
    public Material EdgeMaterial
    {
      get => (Material)GetValue(EdgeMaterialProperty);

      set => SetValue(EdgeMaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the face back material.
    /// </summary>
    /// <value> The face back material. </value>
    public Material FaceBackMaterial
    {
      get => (Material)GetValue(FaceBackMaterialProperty);

      set => SetValue(FaceBackMaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the face material.
    /// </summary>
    /// <value> The face material. </value>
    public Material FaceMaterial
    {
      get => (Material)GetValue(FaceMaterialProperty);

      set => SetValue(FaceMaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the mesh.
    /// </summary>
    /// <value> The mesh. </value>
    public Mesh3D Mesh
    {
      get => (Mesh3D)GetValue(MeshProperty);

      set => SetValue(MeshProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to share vertices (smooth shading).
    /// </summary>
    /// <value> <c>true</c> if vertices are shared; otherwise, <c>false</c> . </value>
    public bool SharedVertices
    {
      get => (bool)GetValue(SharedVerticesProperty);

      set => SetValue(SharedVerticesProperty, value);
    }

    /// <summary>
    /// Gets or sets the shrink factor.
    /// </summary>
    /// <value> The shrink factor. </value>
    public double ShrinkFactor
    {
      get => (double)GetValue(ShrinkFactorProperty);

      set => SetValue(ShrinkFactorProperty, value);
    }

    /// <summary>
    /// Gets or sets the mapping from triangle index to face index.
    /// </summary>
    /// <value> The index mapping. </value>
    public List<int> TriangleIndexToFaceIndex { get; set; }

    /// <summary>
    /// Gets or sets the vertex material.
    /// </summary>
    /// <value> The vertex material. </value>
    public Material VertexMaterial
    {
      get => (Material)GetValue(VertexMaterialProperty);

      set => SetValue(VertexMaterialProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertex radius.
    /// </summary>
    /// <value> The vertex radius. </value>
    public double VertexRadius
    {
      get => (double)GetValue(VertexRadiusProperty);

      set => SetValue(VertexRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertex resolution (number of subdivisions).
    /// </summary>
    /// <value> The vertex resolution. </value>
    public int VertexResolution
    {
      get => (int)GetValue(VertexResolutionProperty);

      set => SetValue(VertexResolutionProperty, value);
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
      ((MeshVisual3D)obj).UpdateVisuals();
    }

    /// <summary>
    /// Updates the visuals.
    /// </summary>
    protected void UpdateVisuals()
    {
      if(Mesh == null)
      {
        Content = null;
        return;
      }

      var m = new Model3DGroup();

      TriangleIndexToFaceIndex = new List<int>();
      var faceGeometry = Mesh.ToMeshGeometry3D(
          SharedVertices, ShrinkFactor, TriangleIndexToFaceIndex);
      m.Children.Add(
          new GeometryModel3D(faceGeometry, FaceMaterial) { BackMaterial = FaceBackMaterial });

      // Add the nodes
      if(VertexRadius > 0)
      {
        var gm = new MeshBuilder(false, false);
        foreach(var p in Mesh.Vertices)
        {
          gm.AddSubdivisionSphere(p, VertexRadius, VertexResolution);

          // gm.AddBox(p, VertexRadius, VertexRadius, VertexRadius);
        }

        m.Children.Add(new GeometryModel3D(gm.ToMesh(), VertexMaterial));
      }

      // Add the edges
      if(EdgeDiameter > 0)
      {
        var em = new MeshBuilder(false, false);
        //// int fi = 0;
        foreach(var p in Mesh.Faces)
        {
          //// var n = this.Mesh.GetFaceNormal(fi++);
          for(var i = 0; i < p.Length; i += 1)
          {
            var p0 = Mesh.Vertices[p[i]];
            var p1 = Mesh.Vertices[p[(i + 1) % p.Length]];
            em.AddCylinder(p0, p1, EdgeDiameter, 4);
          }
        }

        m.Children.Add(new GeometryModel3D(em.ToMesh(), EdgeMaterial));
      }

      Content = m;
    }

  }
}