namespace Crystal.Graphics
{
  /// <summary>
  /// Provides event data for the VisualsSelected event of the <see cref="SelectionCommand" />.
  /// </summary>
  public class VisualsSelectedEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VisualsSelectedEventArgs" /> class.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="areSortedByDistanceAscending">if set to <c>true</c> the selected visuals are sorted by distance in ascending order.</param>
    public VisualsSelectedEventArgs(IList<Visual3D> selected, bool areSortedByDistanceAscending)
    {
      SelectedVisuals = selected;
      AreSortedByDistanceAscending = areSortedByDistanceAscending;
    }

    /// <summary>
    /// Gets the selected visuals.
    /// </summary>
    public IList<Visual3D> SelectedVisuals { get; }

    /// <summary>
    /// Gets a value indicating whether the selected visuals are sorted by distance in ascending order.
    /// </summary>
    /// <value>
    /// <c>true</c> if the selected visuals are sorted by distance in ascending order; otherwise, <c>false</c>.
    /// </value>
    public bool AreSortedByDistanceAscending { get; }
  }
}