﻿using System.IO;
using System.IO.Compression;

namespace Crystal.Graphics
{
  /// <summary>
  /// A Wavefront .obj file reader.
  /// </summary>
  public class ObjReader : ModelReader
  {
    private static readonly char[] Delimiters = new[] { ' ' };
    /// <summary>
    /// The smoothing group maps.
    /// </summary>
    /// <remarks>
    /// The outer dictionary maps from a smoothing group number to a dictionary.
    /// The inner dictionary maps from an obj file (vertex, texture coordinates, normal) index to a vertex index in the current group.
    /// </remarks>
    private readonly Dictionary<long, Dictionary<Tuple<int, int, int>, int>> smoothingGroupMaps;

    /// <summary>
    /// The current smoothing group.
    /// </summary>
    private long currentSmoothingGroup;

    /// <summary>
    /// The line number of the line being parsed.
    /// </summary>
    private int currentLineNo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjReader" /> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    public ObjReader(Dispatcher dispatcher = null)
        : base(dispatcher)
    {
      IgnoreErrors = false;
      SwitchYZ = false;

      IsSmoothingDefault = true;
      SkipTransparencyValues = true;

      Points = new List<Point3D>();
      TextureCoordinates = new List<Point>();
      Normals = new List<Vector3D>();

      Groups = new List<Group>();
      Materials = new Dictionary<string, MaterialDefinition>();

      smoothingGroupMaps = new Dictionary<long, Dictionary<Tuple<int, int, int>, int>>();

      // File format specifications
      // http://en.wikipedia.org/wiki/Obj
      // http://en.wikipedia.org/wiki/Material_Template_Library
      // http://www.martinreddy.net/gfx/3d/OBJ.spec
      // http://www.eg-models.de/formats/Format_Obj.html
    }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore errors.
    /// </summary>
    /// <value><c>true</c> if errors should be ignored; <c>false</c> if errors should throw an exception.</value>
    /// <remarks>
    /// The default value is on (true).
    /// </remarks>
    public bool IgnoreErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to switch Y and Z coordinates.
    /// </summary>
    public bool SwitchYZ { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip transparency values in the material files.
    /// </summary>
    /// <value>
    /// <c>true</c> if transparency values should be skipped; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This option is added to allow disabling the <code>Tr</code> values in files where it has been defined incorrectly.
    /// The transparency values (<code>Tr</code>) are interpreted as 0 = transparent, 1 = opaque.
    /// The dissolve values (<code>d</code>) are interpreted as 0 = transparent, 1 = opaque.
    /// </remarks>
    public bool SkipTransparencyValues { get; set; }

    /// <summary>
    /// Sets a value indicating whether smoothing is default.
    /// </summary>
    /// <remarks>
    /// The default value is smoothing=on (true).
    /// </remarks>
    public bool IsSmoothingDefault
    {
      set => currentSmoothingGroup = value ? 1 : 0;
    }

    /// <summary>
    /// Gets the groups of the file.
    /// </summary>
    /// <value>The groups.</value>
    public IList<Group> Groups { get; }

    /// <summary>
    /// Gets the materials in the imported material files.
    /// </summary>
    /// <value>The materials.</value>
    public Dictionary<string, MaterialDefinition> Materials { get; }

    /// <summary>
    /// Gets or sets the current material.
    /// </summary>
    private Material CurrentMaterial { get; set; }

    /// <summary>
    /// Gets the current group.
    /// </summary>
    private Group CurrentGroup
    {
      get
      {
        if(Groups.Count == 0)
        {
          AddGroup("default");
        }

        return Groups[Groups.Count - 1];
      }
    }

    /// <summary>
    /// Gets or sets the normal vectors.
    /// </summary>
    private IList<Vector3D> Normals { get; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    private IList<Point3D> Points { get; }

    /// <summary>
    /// Gets or sets the stream reader.
    /// </summary>
    private StreamReader Reader { get; set; }

    /// <summary>
    /// Gets or sets the texture coordinates.
    /// </summary>
    private IList<Point> TextureCoordinates { get; }

    /// <summary>
    /// Reads the model and any associated materials from streams
    /// </summary>
    /// <param name="objStream">A model stream from the obj file</param>
    /// <param name="mtlStreams">Array of Material streams referenced in the obj file</param>
    /// <returns></returns>
    public Model3DGroup Read(Stream objStream, Stream[] mtlStreams)
    {
      foreach(var mtlStream in mtlStreams)
      {
        using(var mtlStreamReader = new StreamReader(mtlStream))
        {
          ReadMaterial(mtlStreamReader);
        }
      }

      return Read(objStream);
    }

    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The model.</returns>
    public override Model3DGroup Read(string path)
    {
      TexturePath = Path.GetDirectoryName(path);
      using(var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        return Read(s);
      }
    }

    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">The stream.</param>
    /// <returns>The model.</returns>
    public override Model3DGroup Read(Stream s)
    {
      using(Reader = new StreamReader(s))
      {
        currentLineNo = 0;
        while(!Reader.EndOfStream)
        {
          currentLineNo++;
          var line = Reader.ReadLine();
          if(line == null)
          {
            break;
          }

          line = line.Trim();
          while(line.EndsWith("\\"))
          {
            var nextLine = Reader.ReadLine();
            while(nextLine.Length == 0)
            {
              nextLine = Reader.ReadLine();
            }

            line = line.TrimEnd('\\') + nextLine;
          }

          if(line.StartsWith("#") || line.Length == 0)
          {
            continue;
          }

          SplitLine(line, out var keyword, out var values);

          switch(keyword.ToLower())
          {
            // Vertex data
            case "v": // geometric vertices
              AddVertex(values);
              break;
            case "vt": // texture vertices
              AddTexCoord(values);
              break;
            case "vn": // vertex normals
              AddNormal(values);
              break;
            case "vp": // parameter space vertices
            case "cstype": // rational or non-rational forms of curve or surface type: basis matrix, Bezier, B-spline, Cardinal, Taylor
            case "degree": // degree
            case "bmat": // basis matrix
            case "step": // step size
                         // not supported
              break;

            // Elements
            case "f": // face
              AddFace(values);
              break;
            case "p": // point
            case "l": // line
            case "curv": // curve
            case "curv2": // 2D curve
            case "surf": // surface
                         // not supported
              break;

            // Free-form curve/surface body statements
            case "parm": // parameter name
            case "trim": // outer trimming loop (trim)
            case "hole": // inner trimming loop (hole)
            case "scrv": // special curve (scrv)
            case "sp":  // special point (sp)
            case "end": // end statement (end)
                        // not supported
              break;

            // Connectivity between free-form surfaces
            case "con": // connect
                        // not supported
              break;

            // Grouping
            case "g": // group name
              AddGroup(values);
              break;
            case "s": // smoothing group
              SetSmoothingGroup(values);
              break;
            case "mg": // merging group
              break;
            case "o": // object name
                      // not supported
              break;

            // Display/render attributes
            case "mtllib": // material library
              LoadMaterialLib(values);
              break;
            case "usemtl": // material name
              EnsureNewMesh();

              SetMaterial(values);
              break;
            case "usemap": // texture map name
              EnsureNewMesh();

              break;
            case "bevel": // bevel interpolation
            case "c_interp": // color interpolation
            case "d_interp": // dissolve interpolation
            case "lod": // level of detail
            case "shadow_obj": // shadow casting
            case "trace_obj": // ray tracing
            case "ctech": // curve approximation technique
            case "stech": // surface approximation technique
                          // not supported
              break;
          }
        }
      }

      return BuildModel();
    }

    /// <summary>
    /// Reads a GZipStream compressed OBJ file.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A Model3D object containing the model.</returns>
    /// <remarks>This is a file format used by Crystal.Graphics only.
    /// Use the GZipHelper class to compress an .obj file.</remarks>
    public Model3DGroup ReadZ(string path)
    {
      TexturePath = Path.GetDirectoryName(path);
      using(var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var deflateStream = new GZipStream(s, CompressionMode.Decompress, true);
        return Read(deflateStream);
      }
    }

    /// <summary>
    /// Parses a color string.
    /// </summary>
    /// <param name="values">
    /// The input.
    /// </param>
    /// <returns>
    /// The parsed color.
    /// </returns>
    private static Color ColorParse(string values)
    {
      var fields = Split(values);
      return Color.FromRgb((byte)(fields[0] * 255), (byte)(fields[1] * 255), (byte)(fields[2] * 255));
    }

    /// <summary>
    /// Parse a string containing a double value.
    /// </summary>
    /// <param name="input">
    /// The input string.
    /// </param>
    /// <returns>
    /// The value.
    /// </returns>
    private static double DoubleParse(string input)
    {
      return double.Parse(input, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Splits the specified string using whitespace(input) as separators.
    /// </summary>
    /// <param name="input">
    /// The input string.
    /// </param>
    /// <returns>
    /// List of input.
    /// </returns>
    private static IList<double> Split(string input)
    {
      input = input.Trim();
      var fields = input.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
      var result = new double[fields.Length];
      for(var i = 0; i < fields.Length; i++)
      {
        result[i] = DoubleParse(fields[i]);
      }

      return result;
    }

    /// <summary>
    /// Splits a line in keyword and arguments.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="keyword">
    /// The keyword.
    /// </param>
    /// <param name="arguments">
    /// The arguments.
    /// </param>
    private static void SplitLine(string line, out string keyword, out string arguments)
    {
      var idx = line.IndexOf(' ');
      if(idx < 0)
      {
        keyword = line;
        arguments = null;
        return;
      }

      keyword = line.Substring(0, idx);
      arguments = line.Substring(idx + 1);
    }

    /// <summary>
    /// Adds a group with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    private void AddGroup(string name)
    {
      Groups.Add(new Group(name, CurrentMaterial));
      smoothingGroupMaps.Clear();
    }

    /// <summary>
    /// Ensures that a new mesh is created.
    /// </summary>
    private void EnsureNewMesh()
    {
      if(CurrentGroup.MeshBuilder.TriangleIndices.Count != 0)
      {
        CurrentGroup.AddMesh(CurrentMaterial);
        smoothingGroupMaps.Clear();
      }
    }

    /// <summary>
    /// Sets the smoothing group number.
    /// </summary>
    /// <param name="values">The group number.</param>
    private void SetSmoothingGroup(string values)
    {
      if(values == "off")
      {
        currentSmoothingGroup = 0;
      }
      else
      {
        if(long.TryParse(values, out var smoothingGroup))
        {
          currentSmoothingGroup = smoothingGroup;
        }
        else
        {
          // invalid parameter
          if(IgnoreErrors)
          {
            return;
          }

          throw new FileFormatException($"Invalid smoothing group ({values}) at line {currentLineNo}.");
        }
      }
    }

    /// <summary>
    /// Adds a face.
    /// </summary>
    /// <param name="values">
    /// The input values.
    /// </param>
    /// <remarks>
    /// Adds a polygonal face. The numbers are indexes into the arrays of vertex positions,
    /// texture coordinates, and normal vectors respectively. A number may be omitted if,
    /// for example, texture coordinates are not being defined in the model.
    /// There is no maximum number of vertices that a single polygon may contain.
    /// The .obj file specification says that each face must be flat and convex.
    /// </remarks>
    private void AddFace(string values)
    {
      var currentGroup = CurrentGroup;
      var builder = currentGroup.MeshBuilder;
      var positions = builder.Positions;
      var textureCoordinates = builder.TextureCoordinates;
      var normals = builder.Normals;

      Dictionary<Tuple<int, int, int>, int> smoothingGroupMap = null;

      // If a smoothing group is defined, get the map from obj-file-index to current-group-vertex-index.
      if(currentSmoothingGroup != 0)
      {
        if(!smoothingGroupMaps.TryGetValue(currentSmoothingGroup, out smoothingGroupMap))
        {
          smoothingGroupMap = new Dictionary<Tuple<int, int, int>, int>();
          smoothingGroupMaps.Add(currentSmoothingGroup, smoothingGroupMap);
        }
      }

      var fields = values.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
      var faceIndices = new List<int>();
      foreach(var field in fields)
      {
        if(string.IsNullOrEmpty(field))
        {
          continue;
        }

        var ff = field.Split('/');
        var vi = int.Parse(ff[0]);
        var vti = ff.Length > 1 && ff[1].Length > 0 ? int.Parse(ff[1]) : int.MaxValue;
        var vni = ff.Length > 2 && ff[2].Length > 0 ? int.Parse(ff[2]) : int.MaxValue;

        // Handle relative indices (negative numbers)
        if(vi < 0)
        {
          vi = Points.Count + vi + 1;
        }

        if(vti < 0)
        {
          vti = TextureCoordinates.Count + vti + 1;
        }

        if(vni < 0)
        {
          vni = Normals.Count + vni + 1;
        }

        // Check if the indices are valid
        if(vi - 1 >= Points.Count)
        {
          if(IgnoreErrors)
          {
            return;
          }

          throw new FileFormatException($"Invalid vertex index ({vi}) on line {currentLineNo}.");
        }

        if(vti == int.MaxValue)
        {
          // turn off texture coordinates in the builder
          builder.CreateTextureCoordinates = false;
        }

        if(vni == int.MaxValue)
        {
          // turn off normals in the builder
          builder.CreateNormals = false;
        }

        // check if the texture coordinate index is valid
        if(builder.CreateTextureCoordinates && vti - 1 >= TextureCoordinates.Count)
        {
          if(IgnoreErrors)
          {
            return;
          }

          throw new FileFormatException(
            $"Invalid texture coordinate index ({vti}) on line {currentLineNo}.");
        }

        // check if the normal index is valid
        if(builder.CreateNormals && vni - 1 >= Normals.Count)
        {
          if(IgnoreErrors)
          {
            return;
          }

          throw new FileFormatException(
            $"Invalid normal index ({vni}) on line {currentLineNo}.");
        }

        var addVertex = true;

        if(smoothingGroupMap != null)
        {
          var key = Tuple.Create(vi, vti, vni);

          if(smoothingGroupMap.TryGetValue(key, out var vix))
          {
            // use the index of a previously defined vertex
            addVertex = false;
          }
          else
          {
            // add a new vertex
            vix = positions.Count;
            smoothingGroupMap.Add(key, vix);
          }

          faceIndices.Add(vix);
        }
        else
        {
          // if smoothing is off, always add a new vertex
          faceIndices.Add(positions.Count);
        }

        if(addVertex)
        {
          // add vertex
          positions.Add(Points[vi - 1]);

          // add texture coordinate (if enabled)
          if(builder.CreateTextureCoordinates)
          {
            textureCoordinates.Add(TextureCoordinates[vti - 1]);
          }

          // add normal (if enabled)
          if(builder.CreateNormals)
          {
            normals.Add(Normals[vni - 1]);
          }
        }
      }

      if(faceIndices.Count <= 4)
      {
        // add triangles or quads
        builder.AddPolygon(faceIndices);
      }
      else
      {
        // add triangles by cutting ears algorithm
        // this algorithm is quite expensive...
        builder.AddPolygonByTriangulation(faceIndices);
      }
    }

    /// <summary>
    /// Adds a normal.
    /// </summary>
    /// <param name="values">
    /// The input values.
    /// </param>
    private void AddNormal(string values)
    {
      var fields = Split(values);
      if(SwitchYZ)
      {
        Normals.Add(new Vector3D(fields[0], -fields[2], fields[1]));
      }
      else
      {
        Normals.Add(new Vector3D(fields[0], fields[1], fields[2]));
      }
    }

    /// <summary>
    /// Adds a texture coordinate.
    /// </summary>
    /// <param name="values">
    /// The input values.
    /// </param>
    private void AddTexCoord(string values)
    {
      var fields = Split(values);
      TextureCoordinates.Add(new Point(fields[0], 1 - fields[1]));
    }

    /// <summary>
    /// Adds a vertex.
    /// </summary>
    /// <param name="values">
    /// The input values.
    /// </param>
    private void AddVertex(string values)
    {
      var fields = Split(values);
      if(SwitchYZ)
      {
        Points.Add(new Point3D(fields[0], -fields[2], fields[1]));
      }
      else
      {
        Points.Add(new Point3D(fields[0], fields[1], fields[2]));
      }
    }

    /// <summary>
    /// Builds the model.
    /// </summary>
    /// <returns>A Model3D object.</returns>
    private Model3DGroup BuildModel()
    {
      Model3DGroup modelGroup = null;
      Dispatch(() =>
          {
            modelGroup = new Model3DGroup();
            foreach(var g in Groups)
            {
              foreach(var gm in g.CreateModels())
              {
                gm.SetName(g.Name);

                if(Freeze)
                {
                  gm.Freeze();
                }

                modelGroup.Children.Add(gm);
              }
            }

            if(Freeze)
            {
              modelGroup.Freeze();
            }
          });
      return modelGroup;
    }

    /// <summary>
    /// Gets the material with the specified name.
    /// </summary>
    /// <param name="materialName">
    /// The material name.
    /// </param>
    /// <returns>
    /// The material.
    /// </returns>
    private Material GetMaterial(string materialName)
    {
      if(!string.IsNullOrEmpty(materialName) && Materials.TryGetValue(materialName, out var mat))
      {
        Material m = null;
        Dispatch(() =>
            {
              m = mat.GetMaterial(TexturePath);
            });
        return m;
      }

      return DefaultMaterial;
    }

    /// <summary>
    /// Loads a material library.
    /// </summary>
    /// <param name="mtlFile">
    /// The material file name.
    /// </param>
    private void LoadMaterialLib(string mtlFile)
    {
      var path = PathHelpers.GetFullPath(TexturePath, mtlFile);

      if(!File.Exists(path))
      {
        return;
      }

      using(var materialReader = new StreamReader(path))
      {
        ReadMaterial(materialReader);
      }
    }

    /// <summary>
    /// Loads the material library from a streamreader
    /// </summary>
    /// <param name="materialReader"></param>
    private void ReadMaterial(StreamReader materialReader)
    {
      MaterialDefinition currentMaterial = null;

      while(!materialReader.EndOfStream)
      {
        var line = materialReader.ReadLine();
        if(line == null)
        {
          break;
        }

        line = line.Trim();

        if(line.StartsWith("#") || line.Length == 0)
        {
          continue;
        }

        SplitLine(line, out var keyword, out var value);

        switch(keyword.ToLower())
        {
          case "newmtl":
            if(value != null)
            {
              if(Materials.ContainsKey(value))
              {
                currentMaterial = null;
              }
              else
              {
                currentMaterial = new MaterialDefinition(value);
                Materials.Add(value, currentMaterial);
              }
            }

            break;
          case "ka":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.Ambient = ColorParse(value);
            }

            break;
          case "kd":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.Diffuse = ColorParse(value);
            }

            break;
          case "ks":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.Specular = ColorParse(value);
            }

            break;
          case "ns":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.SpecularCoefficient = DoubleParse(value);
            }

            break;
          case "d":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.Dissolved = DoubleParse(value);
            }

            break;
          case "tr":
            if(!SkipTransparencyValues && currentMaterial != null && value != null)
            {
              currentMaterial.Dissolved = DoubleParse(value);
            }

            break;
          case "illum":
            if(currentMaterial != null && value != null)
            {
              currentMaterial.Illumination = int.Parse(value);
            }

            break;
          case "map_ka":
            if(currentMaterial != null)
            {
              currentMaterial.AmbientMap = value;
            }

            break;
          case "map_kd":
            if(currentMaterial != null)
            {
              currentMaterial.DiffuseMap = value;
            }

            break;
          case "map_ks":
            if(currentMaterial != null)
            {
              currentMaterial.SpecularMap = value;
            }

            break;
          case "map_d":
            if(currentMaterial != null)
            {
              currentMaterial.AlphaMap = value;
            }

            break;
          case "map_bump":
          case "bump":
            if(currentMaterial != null)
            {
              currentMaterial.BumpMap = value;
            }

            break;
        }
      }
    }

    /// <summary>
    /// Sets the material for the current group.
    /// </summary>
    /// <param name="materialName">
    /// The material name.
    /// </param>
    private void SetMaterial(string materialName)
    {
      CurrentGroup.Material = CurrentMaterial = GetMaterial(materialName);
    }

    /// <summary>
    /// Represents a group in the obj file.
    /// </summary>
    public class Group
    {
      /// <summary>
      /// List of mesh builders.
      /// </summary>
      private readonly List<MeshBuilder> meshBuilders;

      /// <summary>
      /// List of materials.
      /// </summary>
      private readonly List<Material> materials;

      /// <summary>
      /// Initializes a new instance of the <see cref="Group"/> class.
      /// </summary>
      /// <param name="name">
      /// The name of the group.
      /// </param>
      /// <param name="material">The material of the group.</param>
      public Group(string name, Material material)
      {
        Name = name;
        meshBuilders = new List<MeshBuilder>();
        materials = new List<Material>();
        AddMesh(material);
      }

      /// <summary>
      /// Sets the material.
      /// </summary>
      /// <value>The material.</value>
      public Material Material
      {
        set => materials[materials.Count - 1] = value;
      }

      /// <summary>
      /// Gets the mesh builder for the current mesh.
      /// </summary>
      /// <value>The mesh builder.</value>
      public MeshBuilder MeshBuilder => meshBuilders[meshBuilders.Count - 1];

      /// <summary>
      /// Gets or sets the group name.
      /// </summary>
      /// <value>The name.</value>
      public string Name { get; set; }

      /// <summary>
      /// Adds a mesh.
      /// </summary>
      /// <param name="material">The material of the group.</param>
      public void AddMesh(Material material)
      {
        var meshBuilder = new MeshBuilder(true);
        meshBuilders.Add(meshBuilder);
        materials.Add(material);
      }

      /// <summary>
      /// Creates the models of the group.
      /// </summary>
      /// <returns>The models.</returns>
      public IEnumerable<Model3D> CreateModels()
      {
        for(var i = 0; i < meshBuilders.Count; i++)
        {
          var material = materials[i];
          var mesh = meshBuilders[i].ToMesh();
          var model = new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material };
          yield return model;
        }
      }
    }

    /// <summary>
    /// A material definition.
    /// </summary>
    /// <remarks>
    /// The file format is documented in http://en.wikipedia.org/wiki/Material_Template_Library.
    /// </remarks>
    public class MaterialDefinition
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="MaterialDefinition" /> class.
      /// </summary>
      /// <param name="name">The name.</param>
      public MaterialDefinition(string name)
      {
        Name = name;
        Dissolved = 1.0;
      }

      /// <summary>
      /// Gets or sets the alpha map.
      /// </summary>
      /// <value>The alpha map.</value>
      public string AlphaMap { get; set; }

      /// <summary>
      /// Gets or sets the ambient color.
      /// </summary>
      /// <value>The ambient.</value>
      public Color Ambient { get; set; }

      /// <summary>
      /// Gets or sets the ambient map.
      /// </summary>
      /// <value>The ambient map.</value>
      public string AmbientMap { get; set; }

      /// <summary>
      /// Gets or sets the bump map.
      /// </summary>
      /// <value>The bump map.</value>
      public string BumpMap { get; set; }

      /// <summary>
      /// Gets or sets the diffuse color.
      /// </summary>
      /// <value>The diffuse.</value>
      public Color Diffuse { get; set; }

      /// <summary>
      /// Gets or sets the diffuse map.
      /// </summary>
      /// <value>The diffuse map.</value>
      public string DiffuseMap { get; set; }

      /// <summary>
      /// Gets or sets the opacity value.
      /// </summary>
      /// <value>The opacity.</value>
      /// <remarks>
      /// 0.0 is transparent, 1.0 is opaque.
      /// </remarks>
      public double Dissolved { get; set; }

      /// <summary>
      /// Gets or sets the illumination.
      /// </summary>
      /// <value>The illumination.</value>
      public int Illumination { get; set; }

      /// <summary>
      /// Gets or sets the name of the material.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      public string Name { get; set; }

      /// <summary>
      /// Gets or sets the specular color.
      /// </summary>
      /// <value>The specular color.</value>
      public Color Specular { get; set; }

      /// <summary>
      /// Gets or sets the specular coefficient.
      /// </summary>
      /// <value>The specular coefficient.</value>
      public double SpecularCoefficient { get; set; }

      /// <summary>
      /// Gets or sets the specular map.
      /// </summary>
      /// <value>The specular map.</value>
      public string SpecularMap { get; set; }

      /// <summary>
      /// Gets or sets the material.
      /// </summary>
      /// <value>The material.</value>
      public Material Material { get; set; }

      /// <summary>
      /// Gets the material from the specified path.
      /// </summary>
      /// <param name="texturePath">
      /// The texture path.
      /// </param>
      /// <returns>
      /// The material.
      /// </returns>
      public Material GetMaterial(string texturePath)
      {
        if(Material == null)
        {
          Material = CreateMaterial(texturePath);
        }

        return Material;
      }

      /// <summary>
      /// Creates the material.
      /// </summary>
      /// <param name="texturePath">The texture path.</param>
      /// <returns>A WPF material.</returns>
      private Material CreateMaterial(string texturePath)
      {
        var mg = new MaterialGroup();
        mg.SetName(Name);

        // add the diffuse component
        if(DiffuseMap == null)
        {
          var diffuseBrush = new SolidColorBrush(Diffuse) { Opacity = Dissolved };
          mg.Children.Add(new DiffuseMaterial(diffuseBrush));
        }
        else
        {
          var path = PathHelpers.GetFullPath(texturePath, DiffuseMap);
          if(File.Exists(path))
          {
            mg.Children.Add(new DiffuseMaterial(CreateTextureBrush(path)));
          }
        }

        // add the ambient components
        if(AmbientMap == null)
        {
          // ambient material is not supported by WPF?
        }
        else
        {
          var path = PathHelpers.GetFullPath(texturePath, AmbientMap);
          if(File.Exists(path))
          {
            mg.Children.Add(new EmissiveMaterial(CreateTextureBrush(path)));
          }
        }

        // add the specular component
        if(Specular.R > 0 || Specular.G > 0 || Specular.B > 0)
        {
          mg.Children.Add(new SpecularMaterial(new SolidColorBrush(Specular), SpecularCoefficient));
        }

        return mg.Children.Count != 1 ? mg : mg.Children[0];
      }

      /// <summary>
      /// Creates a texture brush.
      /// </summary>
      /// <param name="path">The path.</param>
      /// <returns>The brush.</returns>
      private ImageBrush CreateTextureBrush(string path)
      {
        var img = new BitmapImage(new Uri(path, UriKind.Relative));
        var textureBrush = new ImageBrush(img) { Opacity = Dissolved, ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.Tile };
        return textureBrush;
      }
    }

    /// <summary>
    /// Path helpers.
    /// </summary>
    private static class PathHelpers
    {
      /// <summary>
      /// Gets a full path.
      /// </summary>
      /// <param name="basePath">
      /// The base path.
      /// </param>
      /// <param name="path">
      /// The path.
      /// </param>
      public static string GetFullPath(string basePath, string path)
      {
        if(path.Length > 1
            && (path[0] == Path.DirectorySeparatorChar || path[0] == Path.AltDirectorySeparatorChar)
            && (path[1] != Path.DirectorySeparatorChar && path[1] != Path.AltDirectorySeparatorChar))
        {
          path = path.Substring(1);
        }

        return !string.IsNullOrWhiteSpace(basePath) ? Path.GetFullPath(Path.Combine(basePath, path)) : "";
      }
    }
  }
}
