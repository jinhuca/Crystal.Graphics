﻿namespace Crystal.Graphics
{
  /// <summary>
  /// Represents a billboard text item.
  /// </summary>
  public class BillboardTextItem : TextItem
  {
    /// <summary>
    /// Gets or sets the depth offset.
    /// </summary>
    /// <value>The depth offset.</value>
    public double DepthOffset { get; set; }

    /// <summary>
    /// Gets or sets the depth offset in world coordinates.
    /// </summary>
    /// <value>The depth offset.</value>
    public double WorldDepthOffset { get; set; }
  }
}