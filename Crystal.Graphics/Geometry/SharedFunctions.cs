using DoubleOrSingle = System.Double;

namespace Crystal.Graphics
{
  /// <summary>
  /// Functions for the Shared Projects to simplify the Code
  /// </summary>
  internal static class SharedFunctions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D CrossProduct(ref Vector3D first, ref Vector3D second) => Vector3D.CrossProduct(first, second);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D CrossProduct(Vector3D first, Vector3D second) => Vector3D.CrossProduct(first, second);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoubleOrSingle DotProduct(ref Vector3D first, ref Vector3D second) => first.X * second.X + first.Y * second.Y + first.Z * second.Z;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoubleOrSingle DotProduct(ref Vector first, ref Vector second) => first.X * second.X + first.Y * second.Y;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoubleOrSingle LengthSquared(ref Vector3D vector) => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;

    /// <summary>
    /// Lengthes the squared.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoubleOrSingle LengthSquared(ref Vector vector) => vector.X * vector.X + vector.Y * vector.Y;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoubleOrSingle Length(ref Vector3D vector) => Math.Sqrt(LengthSquared(ref vector));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point3D ToPoint3D(ref Vector3D vector)
    {
      return new Point3D(vector.X, vector.Y, vector.Z);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D ToVector3D(ref Vector3D vector)
    {
      return new Vector3D(vector.X, vector.Y, vector.Z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D ToVector3D(Point3D vector)
    {
      return new Vector3D(vector.X, vector.Y, vector.Z);
    }
  }
}
