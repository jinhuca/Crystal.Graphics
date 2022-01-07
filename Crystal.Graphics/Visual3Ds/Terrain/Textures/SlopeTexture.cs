namespace Crystal.Graphics
{
  /// <summary>
  /// Texture by the slope angle.
  /// </summary>
  public class SlopeTexture : TerrainTexture
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeTexture"/> class.
    /// </summary>
    /// <param name="gradientSteps">
    /// The gradient steps.
    /// </param>
    public SlopeTexture(int gradientSteps)
    {
      Brush = gradientSteps > 0 
        ? BrushHelper.CreateSteppedGradientBrush(GradientBrushes.BlueWhiteRed, gradientSteps) 
        : GradientBrushes.BlueWhiteRed;
    }

    /// <summary>
    /// Gets or sets the brush.
    /// </summary>
    /// <value>The brush.</value>
    public Brush Brush { get; set; }

    /// <summary>
    /// Calculates the texture for the specified model.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public override void Calculate(TerrainModel model, MeshGeometry3D? mesh)
    {
      var normals = mesh.CalculateNormals();
      var texcoords = new PointCollection();
      var up = new Vector3D(0, 0, 1);
      for(var i = 0; i < normals.Count; i++)
      {
        var slope = Math.Acos(Vector3D.DotProduct(normals[i], up)) * 180 / Math.PI;
        var u = slope / 40;
        if(u > 1)
        {
          u = 1;
        }

        if(u < 0)
        {
          u = 0;
        }

        texcoords.Add(new Point(u, u));
      }

      TextureCoordinates = texcoords;
      Material = MaterialHelper.CreateMaterial(Brush);
    }
  }
}