﻿namespace Crystal.Graphics
{
  /// <summary>
  /// Terrain texture using a bitmap. Set the Left,Right,Bottom and Top coordinates to get the right alignment.
  /// </summary>
  public class MapTexture : TerrainTexture
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MapTexture"/> class.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    public MapTexture(string source)
    {
      Material = MaterialHelper.CreateImageMaterial(source);
    }

    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>The bottom.</value>
    public double Bottom { get; set; }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>The left.</value>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>The right.</value>
    public double Right { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>The top.</value>
    public double Top { get; set; }

    /// <summary>
    /// Calculates the texture of the specified model.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public override void Calculate(TerrainModel model, MeshGeometry3D? mesh)
    {
      var texcoords = new PointCollection();
      foreach(var p in mesh?.Positions)
      {
        var x = p.X + model.Offset.X;
        var y = p.Y + model.Offset.Y;
        var u = (x - Left) / (Right - Left);
        var v = (y - Top) / (Bottom - Top);
        texcoords.Add(new Point(u, v));
      }
      TextureCoordinates = texcoords;
    }
  }
}