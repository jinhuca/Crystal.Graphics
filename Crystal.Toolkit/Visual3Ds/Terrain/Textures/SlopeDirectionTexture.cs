﻿namespace Crystal.Graphics
{
  /// <summary>
  /// Texture by the direction of the steepest gradient.
  /// </summary>
  public class SlopeDirectionTexture : TerrainTexture
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeDirectionTexture"/> class.
    /// </summary>
    /// <param name="gradientSteps">
    /// The gradient steps.
    /// </param>
    public SlopeDirectionTexture(int gradientSteps)
    {
      if(gradientSteps > 0)
      {
        Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.Hue, gradientSteps);
      }
      else
      {
        Brush = GradientBrushes.Hue;
      }
    }

    /// <summary>
    /// Gets or sets the brush.
    /// </summary>
    /// <value>The brush.</value>
    public Brush Brush { get; set; }

    /// <summary>
    /// Calculates the texture of the specified model.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
    {
      var normals = MeshGeometryHelper.CalculateNormals(mesh);
      var texcoords = new PointCollection();
      for(int i = 0; i < normals.Count; i++)
      {
        double slopedir = Math.Atan2(normals[i].Y, normals[i].X) * 180 / Math.PI;
        if(slopedir < 0)
        {
          slopedir += 360;
        }

        double u = slopedir / 360;
        texcoords.Add(new Point(u, u));
      }

      TextureCoordinates = texcoords;
      Material = MaterialHelper.CreateMaterial(Brush);
    }

  }
}