namespace Crystal.Graphics
{
  /// <summary>
  /// Linear (mx+b) converter.
  /// </summary>
  public class LinearConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the B.
    /// </summary>
    /// <value>The B.</value>
    public double B { get; set; }

    /// <summary>
    /// Gets or sets the M.
    /// </summary>
    /// <value>The M.</value>
    public double M { get; set; }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value produced by the binding source.
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var m = GetDoubleValue(parameter, M);
      var v = GetDoubleValue(value, 0.0);
      return (m * v) + B;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value that is produced by the binding target.
    /// </param>
    /// <param name="targetType">
    /// The type to convert to.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var m = GetDoubleValue(parameter, M);
      var v = GetDoubleValue(value, 0.0);
      return (v - B) / m;
    }

    /// <summary>
    /// Gets the double value.
    /// </summary>
    /// <param name="parameter">
    /// The parameter.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get double value.
    /// </returns>
    private double GetDoubleValue(object parameter, double defaultValue)
    {
      double a;
      if(parameter != null)
      {
        try
        {
          a = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
        }
        catch
        {
          a = defaultValue;
        }
      }
      else
      {
        a = defaultValue;
      }

      return a;
    }
  }
}