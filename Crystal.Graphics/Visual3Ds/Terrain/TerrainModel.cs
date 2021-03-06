using System.IO;
using System.IO.Compression;

namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a terrain model.
  /// </summary>
  /// <remarks>
  /// Supports the following terrain file types
  /// .bt
  /// .btz
  ///  <para>
  /// Read .bt files from disk, keeps the model data and creates the Model3D.
  /// The .btz format is a gzip compressed version of the .bt format.
  ///  </para>
  /// </remarks>
  public class TerrainModel
  {
    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>The bottom.</value>
    public double Bottom { get; set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    public double[] Data { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>The left.</value>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the maximum Z.
    /// </summary>
    /// <value>The maximum Z.</value>
    public double MaximumZ { get; set; }

    /// <summary>
    /// Gets or sets the minimum Z.
    /// </summary>
    /// <value>The minimum Z.</value>
    public double MinimumZ { get; set; }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public Point3D Offset { get; set; }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>The right.</value>
    public double Right { get; set; }

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>The texture.</value>
    public TerrainTexture? Texture { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>The top.</value>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Creates the 3D model of the terrain.
    /// </summary>
    /// <param name="lod">
    /// The level of detail.
    /// </param>
    /// <returns>
    /// The Model3D.
    /// </returns>
    public GeometryModel3D CreateModel(int lod)
    {
      var ni = Height / lod;
      var nj = Width / lod;
      var pts = new List<Point3D>(ni * nj);

      var mx = (Left + Right) / 2;
      var my = (Top + Bottom) / 2;
      var mz = (MinimumZ + MaximumZ) / 2;

      Offset = new Point3D(mx, my, mz);

      for(var i = 0; i < ni; i++)
      {
        for(var j = 0; j < nj; j++)
        {
          var x = Left + (Right - Left) * j / (nj - 1);
          var y = Top + (Bottom - Top) * i / (ni - 1);
          var z = Data[i * lod * Width + j * lod];

          x -= Offset.X;
          y -= Offset.Y;
          z -= Offset.Z;
          pts.Add(new Point3D(x, y, z));
        }
      }

      var mb = new MeshBuilder(false, false);
      mb.AddRectangularMesh(pts, nj);
      var mesh = mb.ToMesh();

      var material = Materials.Green;

      if(Texture != null)
      {
        Texture.Calculate(this, mesh);
        material = Texture.Material;
        mesh!.TextureCoordinates = Texture.TextureCoordinates;
      }

      return new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material };
    }

    /// <summary>
    /// Loads the specified file.
    /// </summary>
    /// <param name="source">
    /// The file name.
    /// </param>
    public void Load(string source)
    {
      if(source == null)
      {
        throw new ArgumentNullException(nameof(source));
      }

      var ext = Path.GetExtension(source);
      if(ext != null)
      {
        ext = ext.ToLower();
      }

      switch(ext)
      {
        case ".btz":
          ReadZippedFile(source);
          break;
        case ".bt":
          ReadTerrainFile(source);
          break;
      }
    }

    /// <summary>
    /// Reads a .bt (Binary terrain) file.
    /// http://www.vterrain.org/Implementation/Formats/BT.html
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    public void ReadTerrainFile(Stream stream)
    {
      using(var reader = new BinaryReader(stream))
      {
        var buffer = reader.ReadBytes(10);
        var enc = new ASCIIEncoding();
        var marker = enc.GetString(buffer);
        if(!marker.StartsWith("binterr"))
        {
          throw new FileFormatException("Invalid marker.");
        }

        var version = marker.Substring(7);

        Width = reader.ReadInt32();
        Height = reader.ReadInt32();
        var dataSize = reader.ReadInt16();
        var isFloatingPoint = reader.ReadInt16() == 1;
        var horizontalUnits = reader.ReadInt16();
        var utmZone = reader.ReadInt16();
        var datum = reader.ReadInt16();
        Left = reader.ReadDouble();
        Right = reader.ReadDouble();
        Bottom = reader.ReadDouble();
        Top = reader.ReadDouble();
        var proj = reader.ReadInt16();
        var scale = reader.ReadSingle();
        var padding = reader.ReadBytes(190);

        var index = 0;
        Data = new double[Width * Height];
        MinimumZ = double.MaxValue;
        MaximumZ = double.MinValue;

        for(var y = 0; y < Height; y++)
        {
          for(var x = 0; x < Width; x++)
          {
            double z;

            if(dataSize == 2)
            {
              z = reader.ReadUInt16();
            }
            else
            {
              z = isFloatingPoint ? reader.ReadSingle() : reader.ReadUInt32();
            }

            Data[index++] = z;
            if(z < MinimumZ)
            {
              MinimumZ = z;
            }

            if(z > MaximumZ)
            {
              MaximumZ = z;
            }
          }
        }
      }
    }

    /// <summary>
    /// Reads the specified .bt terrain file.
    /// </summary>
    /// <param name="path">
    /// The file name.
    /// </param>
    private void ReadTerrainFile(string path)
    {
      using(var infile = File.OpenRead(path))
      {
        ReadTerrainFile(infile);
      }
    }

    /// <summary>
    /// Read a gzipped .bt file.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    private void ReadZippedFile(string source)
    {
      using(var infile = File.OpenRead(source))
      {
        var deflateStream = new GZipStream(infile, CompressionMode.Decompress, true);
        ReadTerrainFile(deflateStream);
      }
    }
  }
}