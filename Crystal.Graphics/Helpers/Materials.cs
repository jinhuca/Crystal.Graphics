namespace Crystal.Graphics
{
  /// <summary>
  /// Contains a set of predefined materials.
  /// </summary>
  public static class Materials
  {
    /// <summary>
    /// The gray material.
    /// </summary>
    private static readonly Material GrayMaterial = MaterialHelper.CreateMaterial(Brushes.Gray);

    /// <summary>
    /// The light gray material.
    /// </summary>
    private static readonly Material LightGrayMaterial = MaterialHelper.CreateMaterial(Brushes.LightGray);

    /// <summary>
    /// The white material.
    /// </summary>
    private static readonly Material WhiteMaterial = MaterialHelper.CreateMaterial(Brushes.White);

    /// <summary>
    /// The hue material.
    /// </summary>
    private static readonly Material HueMaterial = MaterialHelper.CreateMaterial(BrushHelper.CreateHsvBrush());

    /// <summary>
    /// The rainbow material.
    /// </summary>
    private static readonly Material RainbowMaterial = MaterialHelper.CreateMaterial(BrushHelper.CreateRainbowBrush());

    /// <summary>
    /// The red material.
    /// </summary>
    private static readonly Material RedMaterial = MaterialHelper.CreateMaterial(Brushes.Red);

    /// <summary>
    /// The orange material.
    /// </summary>
    private static readonly Material OrangeMaterial = MaterialHelper.CreateMaterial(Brushes.Orange);

    /// <summary>
    /// The yellow material.
    /// </summary>
    private static readonly Material YellowMaterial = MaterialHelper.CreateMaterial(Brushes.Yellow);

    /// <summary>
    /// The green material.
    /// </summary>
    private static readonly Material GreenMaterial = MaterialHelper.CreateMaterial(Brushes.Green);

    /// <summary>
    /// The blue material.
    /// </summary>
    private static readonly Material BlueMaterial = MaterialHelper.CreateMaterial(Brushes.Blue);

    /// <summary>
    /// The indigo material.
    /// </summary>
    private static readonly Material IndigoMaterial = MaterialHelper.CreateMaterial(Brushes.Indigo);

    /// <summary>
    /// The violet material.
    /// </summary>
    private static readonly Material VioletMaterial = MaterialHelper.CreateMaterial(Brushes.Violet);

    /// <summary>
    /// The brown material.
    /// </summary>
    private static readonly Material BrownMaterial = MaterialHelper.CreateMaterial(Brushes.Brown);

    /// <summary>
    /// The gold material.
    /// </summary>
    private static readonly Material GoldMaterial = MaterialHelper.CreateMaterial(Brushes.Gold);

    /// <summary>
    /// Gets the black material.
    /// </summary>
    public static Material Black { get; } = MaterialHelper.CreateMaterial(Brushes.Black);

    /// <summary>
    /// Gets the dark GrayMaterial material.
    /// </summary>
    public static Material DarkGray { get; } = MaterialHelper.CreateMaterial(Brushes.DarkGray);

    /// <summary>
    /// Gets the GrayMaterial material.
    /// </summary>
    public static Material Gray => GrayMaterial;

    /// <summary>
    /// Gets the light GrayMaterial material.
    /// </summary>
    public static Material LightGray => LightGrayMaterial;

    /// <summary>
    /// Gets the white material.
    /// </summary>
    public static Material White => WhiteMaterial;

    /// <summary>
    /// Gets the hue material.
    /// </summary>
    public static Material Hue => HueMaterial;

    /// <summary>
    /// Gets the rainbow material.
    /// </summary>
    public static Material Rainbow => RainbowMaterial;

    /// <summary>
    /// Gets the red material.
    /// </summary>
    public static Material Red => RedMaterial;

    /// <summary>
    /// Gets the orange material.
    /// </summary>
    public static Material Orange => OrangeMaterial;

    /// <summary>
    /// Gets the yellow material.
    /// </summary>
    public static Material Yellow => YellowMaterial;

    /// <summary>
    /// Gets the green material.
    /// </summary>
    public static Material Green => GreenMaterial;

    /// <summary>
    /// Gets the blue material.
    /// </summary>
    public static Material Blue => BlueMaterial;

    /// <summary>
    /// Gets the indigo material.
    /// </summary>
    public static Material Indigo => IndigoMaterial;

    /// <summary>
    /// Gets the violet material.
    /// </summary>
    public static Material Violet => VioletMaterial;

    /// <summary>
    /// Gets the brown material.
    /// </summary>
    public static Material Brown => BrownMaterial;

    /// <summary>
    /// Gets the gold material.
    /// </summary>
    public static Material Gold => GoldMaterial;
  }
}