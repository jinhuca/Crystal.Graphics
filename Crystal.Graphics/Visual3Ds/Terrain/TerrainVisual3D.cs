namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that shows a terrain model.
  /// </summary>
  /// <remarks>
  /// The following terrrain model file formats are supported:
  /// .bt
  /// .btz (gzip compressed .bt)
  ///  <para>
  /// The origin of model will be at the midpoint of the terrain.
  /// A compression method to convert from ".bt" to ".btz" can be found in the GZipHelper.
  /// Note that no LOD algorithm is implemented - this is for small terrains only...
  ///  </para>
  /// </remarks>
  public class TerrainVisual3D : ModelVisual3D
  {
    /// <summary>
    /// Gets or sets the source terrain file.
    /// </summary>
    /// <value>The source.</value>
    public string Source
    {
      get => (string)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      nameof(Source), typeof(string), typeof(TerrainVisual3D), new UIPropertyMetadata(null, SourceChanged));

    /// <summary>
    /// The visual child.
    /// </summary>
    private readonly ModelVisual3D visualChild;

    /// <summary>
    /// Initializes a new instance of the <see cref = "TerrainVisual3D" /> class.
    /// </summary>
    public TerrainVisual3D()
    {
      visualChild = new ModelVisual3D();
      Children.Add(visualChild);
    }

    /// <summary>
    /// The source changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((TerrainVisual3D)obj).UpdateModel();
    }

    /// <summary>
    /// Updates the model.
    /// </summary>
    private void UpdateModel()
    {
      var r = new TerrainModel();
      r.Load(Source);

      // r.Texture = new SlopeDirectionTexture(0);
      r.Texture = new SlopeTexture(8);

      // r.Texture = new MapTexture(@"D:\tmp\CraterLake.png") { Left = r.Left, Right = r.Right, Top = r.Top, Bottom = r.Bottom };
      visualChild.Content = r.CreateModel(2);
    }
  }
}