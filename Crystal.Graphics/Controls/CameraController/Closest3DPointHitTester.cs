﻿namespace Crystal.Graphics
{
  internal class Closest3DPointHitTester
  {
    private readonly Viewport3D mViewPort3D;
    private readonly int mMaximumVerticesPerMesh;

    /// <summary>
    /// Create a new CloseObjectHitTest that returns a 3D point closest to the clicked point on the camera.
    /// </summary>
    /// <param name="viewPort3D"></param>
    /// <param name="maximumVerticesPerMesh">
    /// The maximum number of vertices a mesh may contain to exactly determine the closest vertex for it when there is no exact hit.
    /// Any mesh containing more vertices gets approximated by its bounds.
    /// Note: Will mostly save on computation time once the bounds are already calculated and cashed within the MeshGeometry3D.
    /// </param>
    public Closest3DPointHitTester(Viewport3D viewPort3D, int maximumVerticesPerMesh)
    {
      mViewPort3D = viewPort3D;
      mMaximumVerticesPerMesh = maximumVerticesPerMesh;
    }

    /// <summary>
    /// Returns the 3D point that the user most likely clicked on.
    /// Returns the closest intersection with a mesh, if possible.
    /// If useClosestMeshIfPossible is true it will otherwise returns the mesh vertex that is closest to the clicked 2D screen coordinate, if possible.
    /// Returns the unprojected position otherwise.
    /// Also returns the 2D point corresponding to the returned 3D point.
    /// </summary>
    /// <param name="position"> The point in screen coordinates to calculate the closest vertices for. </param>
    /// <param name="useClosestMeshIfPossible"> When true the closest vertex point to the clicked position will be used when no mesh is hit at the exact mouse cursor area.</param>
    public NearestPointInCamera CalculateMouseDownNearestPoint(Point position, bool useClosestMeshIfPossible)
    {
      if(mViewPort3D.FindNearest(position, out var nearestPoint, out _, out _))
      {
        return new NearestPointInCamera(position, nearestPoint);
      }

      if(useClosestMeshIfPossible)
      {
        var closestResult = ResultOfClosestPointHit2D(position);
        if(closestResult != null)
        {
          return new NearestPointInCamera(closestResult.ClosestPointIn2D, closestResult.ClosestPoint);
        }
      }

      var unprojectedPosition = mViewPort3D.UnProject(position);
      return new NearestPointInCamera(position, unprojectedPosition);
    }

    public ClosestVertexResult? ResultOfClosestPointHit2D(Point position)
    {
      double minX = 0;
      double minY = 0;
      var maxX = mViewPort3D.ActualWidth;
      var maxY = mViewPort3D.ActualHeight;
      var hitsInView = FindClosestHits(position).Where(result => result.ClosestPointIn2D.X >= minX && result.ClosestPointIn2D.X <= maxX &&
                                                                 result.ClosestPointIn2D.Y >= minY && result.ClosestPointIn2D.Y <= maxY);
      var closestHits = hitsInView.OrderBy(x => x.DistanceToPoint2D);

      //Return the closest valid result, or null in case no valid result was found.
      var closestHitInView = closestHits.FirstOrDefault();
      return closestHitInView;
    }

    /// <summary>
    /// For each mesh object in the viewport, this method calculates the vertex that when mapped to 2D is closest to the given 2D point on the screen
    /// </summary>
    /// <param name="pointToHitTest"> The point in screen coordinates to calculate the closest vertices for. </param>
    /// <returns> A ClosestVertexResult containing the distance to the 2D point and the closest 3D vertex point for each geometry/model </returns>
    public IEnumerable<ClosestVertexResult> FindClosestHits(Point pointToHitTest)
    {
      if(mViewPort3D.Camera is not ProjectionCamera camera)
      {
        throw new InvalidOperationException("No projection camera defined. Cannot find rectangle hits.");
      }

      var results = new List<ClosestVertexResult>();
      mViewPort3D.Children.Traverse<GeometryModel3D>((model, transform) =>
      {
        if(!(model.Geometry is MeshGeometry3D geometry) || geometry.Positions == null || geometry.TriangleIndices == null || geometry.Positions.Count == 0 || geometry.TriangleIndices.Count == 0)
        {
          return;
        }
        Point3D[] point3Ds;
        if(geometry.Positions.Count <= mMaximumVerticesPerMesh)
        {
          point3Ds = geometry.Positions.Select(transform.Transform).ToArray();
        }
        else
        {
          var bounds = geometry.Bounds;
          point3Ds = GetBoundaryPointsBoundingBox(bounds).Select(transform.Transform).ToArray();
        }

        // transform the positions of the mesh to screen coordinates
        var point2Ds = mViewPort3D.Point3DtoPoint2D(point3Ds).ToArray();
        var minSquaredDistanceToPoint = double.PositiveInfinity;
        var closestPoint = point3Ds[0];
        var closestPointIn2D = point2Ds[0];

        for(var i = 0; i < point2Ds.Length; i++)
        {
          var point = point2Ds[i];
          var xDiff = point.X - pointToHitTest.X;
          var yDiff = point.Y - pointToHitTest.Y;
          var squaredDistance = xDiff * xDiff + yDiff * yDiff;
          if(squaredDistance < minSquaredDistanceToPoint)
          {
            minSquaredDistanceToPoint = squaredDistance;
            closestPoint = point3Ds[i];
            closestPointIn2D = point;
          }
        }

        var hitTestResult = new ClosestVertexResult(model, geometry, closestPoint, closestPointIn2D, Math.Sqrt(minSquaredDistanceToPoint));
        results.Add(hitTestResult);
      });

      return results;
    }
    private IEnumerable<Point3D> GetBoundaryPointsBoundingBox(Rect3D boundingBox)
    {
      yield return new Point3D(boundingBox.X, boundingBox.Y, boundingBox.Z);
      yield return new Point3D(boundingBox.X, boundingBox.Y + boundingBox.SizeY, boundingBox.Z);
      yield return new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y + boundingBox.SizeY, boundingBox.Z);
      yield return new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y, boundingBox.Z);
      yield return new Point3D(boundingBox.X, boundingBox.Y, boundingBox.Z + boundingBox.SizeZ);
      yield return new Point3D(boundingBox.X, boundingBox.Y + boundingBox.SizeY, boundingBox.Z + boundingBox.SizeZ);
      yield return new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y + boundingBox.SizeY, boundingBox.Z + boundingBox.SizeZ);
      yield return new Point3D(boundingBox.X + boundingBox.SizeX, boundingBox.Y, boundingBox.Z + boundingBox.SizeZ);
    }
  }
}