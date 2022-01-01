﻿namespace Crystal.Graphics
{
  /// <summary>
  /// Markupextension for LinearGradientBrush
  /// </summary>
  /// <example>
  /// <code>
  /// Background={crystal:LinearGradientBrush Black,White}
  ///  </code>
  /// </example>
  public class LinearGradientBrushExtension : MarkupExtension
  {
    /// <summary>
    /// The brush.
    /// </summary>
    private readonly LinearGradientBrush brush;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientBrushExtension"/> class.
    /// </summary>
    /// <param name="startColor">
    /// The start color.
    /// </param>
    /// <param name="endColor">
    /// The end color.
    /// </param>
    /// <param name="angle">
    /// The angle.
    /// </param>
    public LinearGradientBrushExtension(Color startColor, Color endColor, double angle)
    {
      brush = new LinearGradientBrush(startColor, endColor, angle);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientBrushExtension"/> class.
    /// </summary>
    /// <param name="startColor">
    /// The start color.
    /// </param>
    /// <param name="endColor">
    /// The end color.
    /// </param>
    public LinearGradientBrushExtension(Color startColor, Color endColor)
        : this(startColor, endColor, 90)
    {
    }

    /// <summary>
    /// Returns the linear gradient brush.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// The brush to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return brush;
    }

  }
}