namespace Crystal.Graphics
{
  /// <summary>
  /// Imports a model from a file.
  /// </summary>
  public class ModelImporter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelImporter"/> class.
    /// </summary>
    public ModelImporter()
    {
      DefaultMaterial = Materials.Blue;
    }

    /// <summary>
    /// Gets or sets the default material.
    /// </summary>
    /// <value>
    /// The default material.
    /// </value>
    public Material DefaultMaterial { get; set; }

    /// <summary>
    /// Loads a model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="dispatcher">The dispatcher used to create the model.</param>
    /// <param name="freeze">Freeze the model if set to <c>true</c>.</param>
    /// <returns>A model.</returns>
    /// <exception cref="System.InvalidOperationException">File format not supported.</exception>
    public Model3DGroup Load(string path, Dispatcher dispatcher = null, bool freeze = false)
    {
      if(path == null)
      {
        return null;
      }

      dispatcher ??= Dispatcher.CurrentDispatcher;

      Model3DGroup model;
      var ext = System.IO.Path.GetExtension(path);
      ext = ext?.ToLower();

      switch(ext)
      {
        case ".3ds":
          {
            var r = new StudioReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);
            break;
          }

        case ".lwo":
          {
            var r = new LwoReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);

            break;
          }

        case ".obj":
          {
            var r = new ObjReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);
            break;
          }

        case ".objz":
          {
            var r = new ObjReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.ReadZ(path);
            break;
          }

        case ".stl":
          {
            var r = new StLReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);
            break;
          }

        case ".off":
          {
            var r = new OffReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);
            break;
          }
        case ".ply":
          {
            var r = new PlyReader(dispatcher) { DefaultMaterial = DefaultMaterial, Freeze = freeze };
            model = r.Read(path);
            break;
          }
        default:
          throw new InvalidOperationException("File format not supported.");
      }

      //if (!freeze)
      //{
      //    dispatcher.Invoke(new Action(() => model.SetName(Path.GetFileName(path))));
      //}

      return model;
    }
  }
}