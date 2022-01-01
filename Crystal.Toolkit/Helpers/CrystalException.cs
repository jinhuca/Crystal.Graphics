namespace Crystal.Graphics
{
  /// <summary>
  /// Represents errors that occurs in the Toolkit.
  /// </summary>
  [Serializable]
  public class CrystalException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CrystalException"/> class.
    /// </summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="args">The args.</param>
    public CrystalException(string formatString, params object[] args)
        : base(string.Format(formatString, args))
    {
    }
  }
}