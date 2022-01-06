using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides helper methods to generate xaml.
  /// </summary>
  public class XamlHelper
  {
    /// <summary>
    /// Gets the xaml for the specified viewport.
    /// </summary>
    /// <param name="view">
    /// The viewport.
    /// </param>
    /// <returns>
    /// The get xaml.
    /// </returns>
    public static string GetXaml(Viewport3D view)
    {
      var sb = new StringBuilder();
      using(var tw = new StringWriter(sb))
      {
        var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
        XamlWriter.Save(view, xw);

        var xaml = sb.ToString();
        xaml =
            xaml.Replace(
              $"<Viewport3D Height=\"{view.ActualHeight}\" Width=\"{view.ActualWidth}\" ",
                "<Viewport3D ");

        return xaml;
      }
    }

    /// <summary>
    /// Gets the xaml for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The object.
    /// </param>
    /// <returns>
    /// The get xaml.
    /// </returns>
    public static string GetXaml(object obj)
    {
      var sb = new StringBuilder();
      using(var tw = new StringWriter(sb))
      {
        var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
        XamlWriter.Save(obj, xw);
      }

      return sb.ToString();
    }

  }
}