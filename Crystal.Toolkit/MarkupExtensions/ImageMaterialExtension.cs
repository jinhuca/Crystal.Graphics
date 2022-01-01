namespace Crystal.Graphics
{
  /// <summary>
  /// Markupextension for Image Materials
  /// </summary>
  /// <example>
  /// <code>
  /// Material={crystal:ImageMaterial images\\myimage.png, Opacity=0.8}
  ///  </code>
  /// </example>
  public class ImageMaterialExtension : MarkupExtension
  {
    /// <summary>
    /// The path.
    /// </summary>
    private readonly string path;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageMaterialExtension"/> class.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    public ImageMaterialExtension(string path)
    {
      this.path = path;
      Opacity = 1;
      UriKind = UriKind.RelativeOrAbsolute;
    }

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    /// <value>The opacity.</value>
    public double Opacity { get; set; }

    /// <summary>
    /// Gets or sets the kind of the URI.
    /// </summary>
    /// <value>The kind of the URI.</value>
    public UriKind UriKind { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this material is emissive.
    /// </summary>
    /// <value>
    /// <c>true</c> if this material is emissive; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmissive { get; set; }

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
      if(IsEmissive)
      {
        return MaterialHelper.CreateEmissiveImageMaterial(path, Brushes.Black, UriKind);
      }

      return MaterialHelper.CreateImageMaterial(path, Opacity, UriKind);
    }

  }
}