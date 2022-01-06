﻿using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// Provides an Object File Format (OFF) reader.
  /// </summary>
  /// <remarks>
  /// The reader does not parse colors, normal vectors and texture coordinates.
  /// Only 3 dimensional vertices are supported.
  /// Homogeneous coordinates are not supported.
  /// </remarks>
  public class OffReader : ModelReader
    {
        private static readonly char[] Delimiters = new[] { ' ', '\t' };
        /// <summary>
        /// Initializes a new instance of the <see cref="OffReader" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public OffReader(Dispatcher dispatcher = null)
            : base(dispatcher)
        {
            //// http://www.geomview.org/
            //// http://people.sc.fsu.edu/~jburkardt/data/off/off.html
            //// http://people.sc.fsu.edu/~jburkardt/html/off_format.html
            //// http://segeval.cs.princeton.edu/public/off_format.html
            //// http://paulbourke.net/dataformats/off/

            Vertices = new List<Point3D>();

            // this.VertexColors = new List<Color>();
            // this.TexCoords = new PointCollection();
            // this.Normals = new Vector3DCollection();
            Faces = new List<int[]>();
        }

        /// <summary>
        /// Gets the faces.
        /// </summary>
        public IList<int[]> Faces { get; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public IList<Point3D> Vertices { get; }

        /// <summary>
        /// Creates a mesh from the loaded file.
        /// </summary>
        /// <returns>
        /// A <see cref="Mesh3D" />.
        /// </returns>
        public Mesh3D CreateMesh()
        {
            var mesh = new Mesh3D();
            foreach (var v in Vertices)
            {
                mesh.Vertices.Add(v);
            }

            foreach (var face in Faces)
            {
                mesh.Faces.Add((int[])face.Clone());
            }

            return mesh;
        }

        /// <summary>
        /// Creates a <see cref="MeshGeometry3D" /> object from the loaded file. Polygons are triangulated using triangle fans.
        /// </summary>
        /// <returns>
        /// A <see cref="MeshGeometry3D" />.
        /// </returns>
        public MeshGeometry3D? CreateMeshGeometry3D()
        {
            var mb = new MeshBuilder(false, false);
            foreach (var p in Vertices)
            {
                mb.Positions.Add(p);
            }

            foreach (var face in Faces)
            {
                mb.AddTriangleFan(face);
            }

            return mb.ToMesh();
        }

        /// <summary>
        /// Creates a <see cref="Model3DGroup" /> from the loaded file.
        /// </summary>
        /// <returns>A <see cref="Model3DGroup" />.</returns>
        public Model3DGroup CreateModel3D()
        {
            Model3DGroup modelGroup = null;
            Dispatch(
                () =>
                {
                    modelGroup = new Model3DGroup();
                    var g = CreateMeshGeometry3D();
                    var gm = new GeometryModel3D { Geometry = g, Material = DefaultMaterial };
                    gm.BackMaterial = gm.Material;
                    if (Freeze)
                    {
                        gm.Freeze();
                    }

                    modelGroup.Children.Add(gm);
                    if (Freeze)
                    {
                        modelGroup.Freeze();
                    }
                });

            return modelGroup;
        }

        /// <summary>
        /// Loads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        public void Load(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                var containsNormals = false;
                var containsTextureCoordinates = false;
                var containsColors = false;
                var containsHomogeneousCoordinates = false;
                var vertexDimension = 3;
                var nextLineContainsVertexDimension = false;
                var nextLineContainsNumberOfVertices = false;
                var numberOfVertices = 0;
                var numberOfFaces = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    if (nextLineContainsVertexDimension)
                    {
                        var values = GetIntValues(line);
                        vertexDimension = values[0];
                        nextLineContainsVertexDimension = false;
                        continue;
                    }

                    if (line.Contains("OFF"))
                    {
                        containsNormals = line.Contains("N");
                        containsColors = line.Contains("C");
                        containsTextureCoordinates = line.Contains("ST");
                        if (line.Contains("4"))
                        {
                            containsHomogeneousCoordinates = true;
                        }

                        if (line.Contains("n"))
                        {
                            nextLineContainsVertexDimension = true;
                        }

                        nextLineContainsNumberOfVertices = true;
                        continue;
                    }

                    if (nextLineContainsNumberOfVertices)
                    {
                        var values = GetIntValues(line);
                        numberOfVertices = values[0];
                        numberOfFaces = values[1];

                        /* numberOfEdges = values[2]; */
                        nextLineContainsNumberOfVertices = false;
                        continue;
                    }

                    if (Vertices.Count < numberOfVertices)
                    {
                        var x = new double[vertexDimension];
                        var values = GetValues(line);
                        var i = 0;
                        for (var j = 0; j < vertexDimension; j++)
                        {
                            x[j] = values[i++];
                        }

                        var n = new double[vertexDimension];
                        var uv = new double[2];

                        double w = 0;
                        if (containsHomogeneousCoordinates)
                        {
                            // ReSharper disable once RedundantAssignment
                            w = values[i++];
                        }

                        if (containsNormals)
                        {
                            for (var j = 0; j < vertexDimension; j++)
                            {
                                n[j] = values[i++];
                            }
                        }

                        if (containsColors)
                        {
                            // read color
                        }

                        if (containsTextureCoordinates)
                        {
                            for (var j = 0; j < 2; j++)
                            {
                                uv[j] = values[i++];
                            }
                        }

                        Vertices.Add(new Point3D(x[0], x[1], x[2]));

                        continue;
                    }

                    if (Faces.Count < numberOfFaces)
                    {
                        var values = GetIntValues(line);
                        var nv = values[0];
                        var vertices = new int[nv];
                        for (var i = 0; i < nv; i++)
                        {
                            vertices[i] = values[i + 1];
                        }

                        if (containsColors)
                        {
                            // read colorspec
                        }

                        Faces.Add(vertices);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns>A <see cref="Model3DGroup" />.</returns>
        public override Model3DGroup Read(Stream s)
        {
            Load(s);
            return CreateModel3D();
        }

        /// <summary>
        /// Parses integer values from a string.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// Array of integer values.
        /// </returns>
        private static int[] GetIntValues(string input)
        {
            var fields = RemoveComments(input).Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
            var result = new int[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                result[i] = (int)double.Parse(fields[i], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Parses double values from a string.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// Array of double values.
        /// </returns>
        private static double[] GetValues(string input)
        {
            var fields = RemoveComments(input).Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
            var result = new double[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                result[i] = double.Parse(fields[i], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Removes comments from the line.
        /// </summary>
        /// <param name="input">
        /// The line.
        /// </param>
        /// <returns>
        /// A line without comments.
        /// </returns>
        private static string RemoveComments(string input)
        {
            var commentIndex = input.IndexOf('#');
            if (commentIndex >= 0)
            {
                return input.Substring(0, commentIndex);
            }

            return input;
        }
    }
}