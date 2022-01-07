﻿namespace Crystal.Graphics
{
  /// <summary>
  /// A visual element that can be bound to a <see cref="MeshGeometry3D"/>.
  /// </summary>
  public class MeshGeometryVisual3D : MeshModelVisual3D
  {
    /// <summary>
    /// Gets or sets the <see cref="MeshGeometry3D"/> defining the shape of the visual.
    /// </summary>
    public MeshGeometry3D? MeshGeometry
    {
      get => (MeshGeometry3D)GetValue(GeometryProperty);
      set => SetValue(GeometryProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MeshGeometry"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GeometryProperty =
      DependencyProperty.Register("MeshGeometry", typeof(MeshGeometry3D), typeof(MeshGeometryVisual3D), new PropertyMetadata(null, GeometryChanged));

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
      return MeshGeometry;
    }
  }
}