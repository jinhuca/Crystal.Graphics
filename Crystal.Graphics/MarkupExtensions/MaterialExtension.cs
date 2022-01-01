namespace Crystal.Graphics
{
  /// <summary>
  /// Markupextension for Materials
  /// </summary>
  /// <example>
  /// <code>
  /// Material={crystal:Material Blue, Opacity=0.5}
  ///  </code>
  /// </example>
  public class MaterialExtension : MarkupExtension
  {
    /// <summary>
    /// The color.
    /// </summary>
    private readonly Color color;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialExtension"/> class.
    /// </summary>
    /// <param name="color">
    /// The color.
    /// </param>
    public MaterialExtension(Color color)
    {
      this.color = color;
      SpecularPower = 100;
      SpecularIntensity = 1;
      Opacity = 1;
    }

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    /// <value>The opacity.</value>
    public double Opacity { get; set; }

    /// <summary>
    /// Gets or sets the specular intensity.
    /// </summary>
    /// <value>The specular intensity.</value>
    public double SpecularIntensity { get; set; }

    /// <summary>
    /// Gets or sets the specular power.
    /// </summary>
    /// <value>The specular power.</value>
    public double SpecularPower { get; set; }

    /// <summary>
    /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var diffuse = new SolidColorBrush(color);
      var specular = BrushHelper.CreateGrayBrush(SpecularIntensity);
      return MaterialHelper.CreateMaterial(diffuse, null, specular, Opacity, SpecularPower);
    }

  }
}