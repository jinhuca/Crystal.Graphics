﻿using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// Exports the 3D visual tree to a <a href="http://www.kerkythea.net/joomla">Kerkythea</a> input file.
  /// </summary>
  public class KerkytheaExporter : Exporter<KerkytheaExporter.KerkytheaWriter>
  {
    /// <summary>
    /// Dictionary of registered materials.
    /// </summary>
    private readonly Dictionary<Material, XmlDocument> registeredMaterials = new Dictionary<Material, XmlDocument>();

    /// <summary>
    /// Initializes a new instance of the <see cref="KerkytheaExporter"/> class.
    /// </summary>
    public KerkytheaExporter()
    {
      Name = "My Scene";
      BackgroundColor = Colors.Black;
      ReflectionColor = Colors.Gray;
      Reflections = true;
      Shadows = true;
      SoftShadows = true;
      LightMultiplier = 3.0;
      Threads = 2;

      ShadowColor = Color.FromArgb(255, 100, 100, 100);
      RenderSetting = RenderSettings.RayTracer;
      Aperture = "Pinhole";
      FocusDistance = 1.0;
      LensSamples = 3;

      Width = 500;
      Height = 500;

      TextureWidth = 1024;
      TextureHeight = 1024;
      FileCreator = File.Create;
    }

    /// <summary>
    /// Render settings.
    /// </summary>
    public enum RenderSettings
    {
      /// <summary>
      /// Use RayTracer.
      /// </summary>
      RayTracer,

      /// <summary>
      /// Use PhotonMap.
      /// </summary>
      PhotonMap,

      /// <summary>
      /// Use MetropolisLightTransport.
      /// </summary>
      MetropolisLightTransport
    }

    /// <summary>
    /// Gets or sets the aperture.
    /// </summary>
    /// <value>The aperture.</value>
    public string Aperture { get; set; }

    /// <summary>
    /// Gets or sets the color of the background.
    /// </summary>
    /// <value>The color of the background.</value>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the texture file creator.
    /// </summary>
    /// <value>The file creator.</value>
    public Func<string, Stream> FileCreator { get; set; }

    /// <summary>
    /// Gets or sets the length of the focal.
    /// </summary>
    /// <value>The length of the focal.</value>
    public double FocalLength { get; set; }

    /// <summary>
    /// Gets or sets the focus distance.
    /// </summary>
    /// <value>The focus distance.</value>
    public double FocusDistance { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the lens samples.
    /// </summary>
    /// <value>The lens samples.</value>
    public int LensSamples { get; set; }

    /// <summary>
    /// Gets or sets the light multiplier.
    /// </summary>
    /// <value>The light multiplier.</value>
    public double LightMultiplier { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the color of the reflection.
    /// </summary>
    /// <value>The color of the reflection.</value>
    public Color ReflectionColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref = "KerkytheaExporter" /> is reflections.
    /// </summary>
    /// <value><c>true</c> if reflections; otherwise, <c>false</c>.</value>
    public bool Reflections { get; set; }

    /// <summary>
    /// Gets or sets the render setting.
    /// </summary>
    /// <value>The render setting.</value>
    public RenderSettings RenderSetting { get; set; }

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>The color of the shadow.</value>
    public Color ShadowColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref = "KerkytheaExporter" /> is shadows.
    /// </summary>
    /// <value><c>true</c> if shadows; otherwise, <c>false</c>.</value>
    public bool Shadows { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [soft shadows].
    /// </summary>
    /// <value><c>true</c> if [soft shadows]; otherwise, <c>false</c>.</value>
    public bool SoftShadows { get; set; }

    /// <summary>
    /// Gets or sets the height of the texture.
    /// </summary>
    /// <value>The height of the texture.</value>
    public int TextureHeight { get; set; }

    /// <summary>
    /// Gets or sets the texture path.
    /// </summary>
    /// <value>The texture path.</value>
    public string TexturePath { get; set; }

    /// <summary>
    /// Gets or sets the width of the texture.
    /// </summary>
    /// <value>The width of the texture.</value>
    public int TextureWidth { get; set; }

    /// <summary>
    /// Gets or sets the threads.
    /// </summary>
    /// <value>The threads.</value>
    public int Threads { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Exports the mesh.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="m">The mesh.</param>
    public void ExportMesh(KerkytheaWriter writer, MeshGeometry3D m)
    {
      writer.WriteStartObject("Triangular Mesh", "Triangular Mesh", string.Empty, "Surface");

      writer.WriteStartElement("Parameter");
      {
        writer.WriteAttributeString("Name", "Vertex List");
        writer.WriteAttributeString("Type", "Point3D List");
        writer.WriteAttributeString("Value", m.Positions.Count.ToString());
        foreach(var p in m.Positions)
        {
          writer.WriteStartElement("P");
          writer.WriteAttributeString("xyz", ToKerkytheaString(p));
          writer.WriteEndElement();
        }
      }

      writer.WriteFullEndElement();

      int triangles = m.TriangleIndices.Count / 3;

      // NORMALS
      // todo: write normal list per vertex instead of per triangle index
      if(m.Normals != null && m.Normals.Count > 0)
      {
        writer.WriteStartElement("Parameter");
        {
          writer.WriteAttributeString("Name", "Normal List");
          writer.WriteAttributeString("Type", "Point3D List");
          writer.WriteAttributeString("Value", m.TriangleIndices.Count.ToString());
          foreach(int index in m.TriangleIndices)
          {
            if(index >= m.Normals.Count)
            {
              continue;
            }

            var n = m.Normals[index];
            writer.WriteStartElement("P");
            writer.WriteAttributeString("xyz", ToKerkytheaString(n));
            writer.WriteEndElement();
          }
        }

        writer.WriteFullEndElement();
      }

      // TRIANGLE INDICES
      writer.WriteStartElement("Parameter");
      {
        writer.WriteAttributeString("Name", "Index List");
        writer.WriteAttributeString("Type", "Triangle Index List");
        writer.WriteAttributeString("Value", triangles.ToString());
        for(int a = 0; a < triangles; a++)
        {
          int a3 = a * 3;
          int i = m.TriangleIndices[a3];
          int j = m.TriangleIndices[a3 + 1];
          int k = m.TriangleIndices[a3 + 2];
          writer.WriteStartElement("F");
          writer.WriteAttributeString("ijk", string.Format("{0} {1} {2}", i, j, k));
          writer.WriteEndElement();
        }
      }

      writer.WriteFullEndElement();

      writer.WriteParameter("Smooth", true);
      writer.WriteParameter("AA Tolerance", 15.0);

      writer.WriteEndObject();
    }

    /// <summary>
    /// Registers a material.
    /// </summary>
    /// <param name="m">The material to register.</param>
    /// <param name="stream">The material stream.</param>
    public void RegisterMaterial(Material m, Stream stream)
    {
      var doc = new XmlDocument();
      doc.Load(stream);
      registeredMaterials.Add(m, doc);
    }

    /// <summary>
    /// Writes the Metropolis Light Transport properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name of the MLP ray tracer.</param>
    public void WriteMetropolisLightTransport(KerkytheaWriter writer, string name)
    {
      writer.WriteStartObject("./Ray Tracers/" + name, "Metropolis Light Transport", name, "Ray Tracer");
      writer.WriteParameter("Max Ray Tracing Depth", 100);
      writer.WriteParameter("Max Iterations", 10000);
      writer.WriteParameter("Linear Lightflow", true);
      writer.WriteParameter("Seed Paths", 50000);
      writer.WriteParameter("Large Step Probability", 0.2);
      writer.WriteParameter("Max Mutation Distance", 0.02);
      writer.WriteParameter("Live Probability", 0.7);
      writer.WriteParameter("Max Consecutive Rejections", 200);
      writer.WriteParameter("Bidirectional", true);
      writer.WriteParameter("Super Sampling", "3x3");
      writer.WriteParameter("Image Filename", "temp.jpg");
      writer.WriteParameter("Random Seed", "Automatic");
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes the standard ray tracer properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name of the ray tracer.</param>
    public void WriteStandardRayTracer(KerkytheaWriter writer, string name)
    {
      writer.WriteStartObject("./Ray Tracers/" + name, "Standard Ray Tracer", name, "Ray Tracer");
      writer.WriteParameter("Rasterization", "Auto");

      // WriteParameter("Antialiasing", "Extra Pass 3x3");
      writer.WriteParameter("Antialiasing", "Production AA");
      writer.WriteParameter("Antialiasing Filter", "Mitchell-Netravali 0.5 0.8");
      writer.WriteParameter("Antialiasing Threshold", 0.3);
      writer.WriteParameter("Texture Filtering", true);
      writer.WriteParameter("Ambient Lighting", true);
      writer.WriteParameter("Direct Lighting", true);
      writer.WriteParameter("Sky Lighting", true);
      writer.WriteParameter("Brightness Threshold", 0.002);
      writer.WriteParameter("Max Ray Tracing Depth", 5);
      writer.WriteParameter("Max Scatter Bounces", 5);
      writer.WriteParameter("Max Dirac Bounces", 5);
      writer.WriteParameter("Irradiance Precomputation", 4);
      writer.WriteParameter("Irradiance Scale", Colors.White);
      writer.WriteParameter("Linear Lightflow", true);
      writer.WriteParameter("Max Iterations", 5);
      writer.WriteParameter("Super Sampling", "None");
      writer.WriteParameter("Image Filename", "temp.jpg");
      writer.WriteParameter("./Sampling Criteria/Diffuse Samples", 1024);
      writer.WriteParameter("./Sampling Criteria/Specular Samples", 32);
      writer.WriteParameter("./Sampling Criteria/Dispersion Samples", true);
      writer.WriteParameter("./Sampling Criteria/Trace Diffusers", false);
      writer.WriteParameter("./Sampling Criteria/Trace Translucencies", false);
      writer.WriteParameter("./Sampling Criteria/Trace Fuzzy Reflections", true);
      writer.WriteParameter("./Sampling Criteria/Trace Fuzzy Refractions", true);
      writer.WriteParameter("./Sampling Criteria/Trace Reflections", true);
      writer.WriteParameter("./Sampling Criteria/Trace Refractions", true);
      writer.WriteParameter("./Sampling Criteria/Random Generator", "Pure");
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes the threaded ray tracer properties.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="threads">The number of threads.</param>
    public void WriteThreadedRaytracer(KerkytheaWriter writer, int threads)
    {
      writer.WriteStartObject("./Ray Tracers/Threaded Ray Tracer", "Threaded Ray Tracer", "Threaded Ray Tracer", "Ray Tracer");
      for(int i = 0; i < threads; i++)
      {
        writer.WriteParameter("Thread #" + i, "#" + i);
      }

      writer.WriteParameter("Network Mode", "None");
      writer.WriteParameter("Listening Port", 6200);
      writer.WriteParameter("Host", "127.0.0.1");
      writer.WriteEndObject();
    }

    /// <summary>
    /// Creates the writer for the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The writer.</returns>
    protected override KerkytheaWriter Create(Stream stream)
    {
      return new KerkytheaWriter(stream);
    }

    /// <summary>
    /// Exports the camera.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="c">The camera.</param>
    /// <exception cref="System.InvalidOperationException">Only perspective cameras are supported.</exception>
    protected override void ExportCamera(KerkytheaWriter writer, Camera c)
    {
      var pc = c as PerspectiveCamera;
      if(pc == null)
      {
        throw new InvalidOperationException("Only perspective cameras are supported.");
      }

      const string name = "Camera #1";
      writer.WriteStartObject("./Cameras/" + name, "Pinhole Camera", name, "Camera");

      // FOV = 2 arctan (x / (2 f)), x is diagonal, f is focal length
      // f = x / 2 / Tan(FOV/2)
      // http://en.wikipedia.org/wiki/Angle_of_view
      // http://kmp.bdimitrov.de/technology/fov.html

      // PerspectiveCamera.FieldOfView: Horizontal field of view
      // Must multiply by ratio of Viewport Width/Height
      double ratio = Width / (double)Height;
      const double x = 40;
      double f = 0.5 * ratio * x / Math.Tan(0.5 * pc.FieldOfView / 180.0 * Math.PI);

      writer.WriteParameter("Focal Length (mm)", f);
      writer.WriteParameter("Film Height (mm)", x);
      writer.WriteParameter("Resolution", string.Format(CultureInfo.InvariantCulture, "{0}x{1}", Width, Height));

      var t = CreateTransform(pc.Position, pc.LookDirection, pc.UpDirection);
      writer.WriteTransform("Frame", t);

      writer.WriteParameter("Focus Distance", FocusDistance);
      writer.WriteParameter("f-number", Aperture);
      writer.WriteParameter("Lens Samples", LensSamples);
      writer.WriteParameter("Blades", 6);
      writer.WriteParameter("Diaphragm", "Circular");
      writer.WriteParameter("Projection", "Planar");

      writer.WriteEndObject();
    }

    /// <summary>
    /// Exports the document header.
    /// </summary>
    /// <param name="writer">The writer.</param>
    protected override void ExportHeader(KerkytheaWriter writer)
    {
      writer.WriteStartDocument();

      writer.WriteStartElement("Root");
      writer.WriteAttributeString("Label", "Default Kernel");
      writer.WriteAttributeString("Name", string.Empty);
      writer.WriteAttributeString("Type", "Kernel");

      writer.WriteStartObject("./Modellers/XML Modeller", "XML Modeller", "XML Modeller", "Modeller");
      writer.WriteEndObject();

      writer.WriteStartObject("./Image Handlers/Free Image Support", "Free Image Support", "Free Image Support", "Image Handler");
      writer.WriteParameter("Tone Mapping", "External");
      writer.WriteParameter("Jpeg Quality", "Higher");
      writer.WriteEndObject();

      writer.WriteStartObject("./Direct Light Estimators/Refraction Enhanced", "Refraction Enhanced", "Refraction Enhanced", "Direct Light Estimator");
      writer.WriteParameter("Enabled", "Boolean", "1");
      writer.WriteParameter("PseudoCaustics", "Boolean", "0");
      writer.WriteParameter("PseudoTranslucencies", "Boolean", "0");
      writer.WriteParameter("Area Light Evaluation", "Boolean", "1");
      writer.WriteParameter("Optimized Area Lights", "Boolean", "1");
      writer.WriteParameter("Accurate Soft Shadows", "Boolean", "0");
      writer.WriteParameter("Antialiasing", "String", "High");
      writer.WriteParameter("./Evaluation/Diffuse", "Boolean", "1");
      writer.WriteParameter("./Evaluation/Specular", "Boolean", "1");
      writer.WriteParameter("./Evaluation/Translucent", "Boolean", "1");
      writer.WriteParameter("./Evaluation/Transmitted", "Boolean", "1");
      writer.WriteEndObject();

      // add ray tracer module.
      for(int i = 0; i < Threads; i++)
      {
        WriteStandardRayTracer(writer, "#" + i);
      }

      WriteThreadedRaytracer(writer, Threads);

      // add spatial subdivision module.
      writer.WriteStartObject("./Environments/Octree Environment", "Octree Environment", "Octree Environment", "Environment");
      writer.WriteParameter("Max Objects per Cell", 20);
      writer.WriteParameter("Instancing Switch", 1000000);
      writer.WriteParameter("Caching Switch", 6000000);
      writer.WriteEndObject();

      // add basic post filtering / tone mapping.
      writer.WriteStartObject("./Filters/Simple Tone Mapping", "Simple Tone Mapping", string.Empty, "Filter");
      writer.WriteParameter("Enabled", true);
      writer.WriteParameter("Method", "Simple");
      writer.WriteParameter("Exposure", 1.0);
      writer.WriteParameter("Gamma", 1.0);
      writer.WriteParameter("Dark Multiplier", 1.0);
      writer.WriteParameter("Bright Multiplier", 1.0);
      writer.WriteParameter("Reverse Correction", true);
      writer.WriteParameter("Reverse Gamma", 2.2);
      writer.WriteEndObject();

      // start of scene description.
      writer.WriteStartObject("./Scenes/" + Name, "Default Scene", Name, "Scene");
    }

    /// <summary>
    /// Exports the light.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="l">The light.</param>
    /// <param name="t">The transform.</param>
    protected override void ExportLight(KerkytheaWriter writer, Light l, Transform3D t)
    {
      if(l is AmbientLight)
      {
        return;
      }

      string name = GetUniqueName(writer, l, l.GetType().Name);

      var d = l as DirectionalLight;
      var s = l as SpotLight;
      var p = l as PointLight;

      writer.WriteStartObject("./Lights/" + name, "Default Light", name, "Light");
      {
        string stype = "Projector Light";
        if(s != null)
        {
          stype = "Spot Light";
        }

        if(p != null)
        {
          stype = "Omni Light";
        }

        writer.WriteStartObject(stype, stype, string.Empty, "Emittance");

        // emitter Radiance
        writer.WriteStartObject("./Radiance/Constant Texture", "Constant Texture", string.Empty, "Texture");
        var c = Colors.White;
        writer.WriteParameter("Color", c);
        writer.WriteEndObject();

        // var v = new Vector3D(l.Color.R, l.Color.G, l.Color.B);
        // double lum = v.Length;
        writer.WriteParameter("Attenuation", "None");

        // SpotLight (Spot Light)
        if(s != null)
        {
          // todo : export the specular parameters
          // s.ConstantAttenuation
          // s.LinearAttenuation
          // s.QuadraticAttenuation
          writer.WriteParameter("Fall Off", s.OuterConeAngle);
          writer.WriteParameter("Hot Spot", s.InnerConeAngle);
        }

        // DirectionalLight (Projector Light)
        if(d != null)
        {
          writer.WriteParameter("Width", 2.0);
          writer.WriteParameter("Height", 2.0);
        }

        // PointLight (Omni light)
        if(p != null)
        {
          // todo: export pointlight parameters
          // name.ConstantAttenuation
          // name.LinearAttenuation
          // name.QuadraticAttenuation
          // name.Range // distance beyond which the light has no effect
        }

        writer.WriteParameter("Focal Length", 1.0);

        writer.WriteEndObject(); // stype

        writer.WriteParameter("Enabled", true);
        writer.WriteParameter("Shadow", Shadows);
        writer.WriteParameter("Soft Shadow", SoftShadows);

        writer.WriteParameter("Negative Light", false);
        writer.WriteParameter("Global Photons", true);
        writer.WriteParameter("Caustic Photons", true);
        writer.WriteParameter("Multiplier", LightMultiplier);

        Matrix3D transform;
        var upVector = new Vector3D(0, 0, 1);
        if(s != null)
        {
          transform = CreateTransform(s.Position, s.Direction, upVector);
          writer.WriteTransform("Frame", transform);
        }

        if(d != null)
        {
          var origin = new Point3D(-1000 * d.Direction.X, -1000 * d.Direction.Y, -1000 * d.Direction.Z);
          transform = CreateTransform(origin, d.Direction, upVector);
          writer.WriteTransform("Frame", transform);
        }

        if(p != null)
        {
          var direction = new Vector3D(-p.Position.X, -p.Position.Y, -p.Position.Z);
          transform = CreateTransform(p.Position, direction, upVector);
          writer.WriteTransform("Frame", transform);
        }

        writer.WriteParameter("Focus Distance", 4.0);
        writer.WriteParameter("Radius", 0.2);
        writer.WriteParameter("Shadow Color", ShadowColor);
      }

      writer.WriteEndObject();
    }

    /// <summary>
    /// Exports the model.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="g">The model geometry.</param>
    /// <param name="transform">The transform.</param>
    protected override void ExportModel(KerkytheaWriter writer, GeometryModel3D g, Transform3D transform)
    {
      var mesh = g.Geometry as MeshGeometry3D;
      if(mesh == null)
      {
        return;
      }

      string name = GetUniqueName(writer, g, g.GetType().Name);
      writer.WriteStartObject("./Models/" + name, "Default Model", name, "Model");

      ExportMesh(writer, mesh);

      if(g.Material != null)
      {
        ExportMaterial(writer, g.Material);
      }

      var tg = new Transform3DGroup();
      tg.Children.Add(g.Transform);
      tg.Children.Add(transform);

      if(mesh.TextureCoordinates != null)
      {
        ExportMapChannel(writer, mesh);
      }

      writer.WriteTransform("Frame", tg.Value);

      writer.WriteParameter("Enabled", true);
      writer.WriteParameter("Visible", true);
      writer.WriteParameter("Shadow Caster", true);
      writer.WriteParameter("Shadow Receiver", true);
      writer.WriteParameter("Caustics Transmitter", true);
      writer.WriteParameter("Caustics Receiver", true);
      writer.WriteParameter("Exit Blocker", false);

      writer.WriteEndObject();
    }

    /// <summary>
    /// Exports the specified viewport.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="v">The viewport to export.</param>
    protected override void ExportViewport(KerkytheaWriter writer, Viewport3D v)
    {
      var ambient = Visual3DHelper.Find<AmbientLight>(v);

      // default global settings
      writer.WriteStartObject("Default Global Settings", "Default Global Settings", string.Empty, "Global Settings");
      if(ambient != null)
      {
        writer.WriteParameter("Ambient Light", ambient.Color);
      }

      writer.WriteParameter("Background Color", BackgroundColor);
      writer.WriteParameter("Compute Volume Transfer", false);
      writer.WriteParameter("Transfer Recursion Depth", 1);
      writer.WriteParameter("Background Type", "Sky Color");
      writer.WriteParameter("Sky Intensity", 1.0);
      writer.WriteParameter("Sky Frame", "Transform", "1 0 0 0 0 1 0 0 0 0 1 0 ");
      writer.WriteParameter("Sun Direction", "0 0 1");
      writer.WriteParameter("Sky Turbidity", 2.0);
      writer.WriteParameter("Sky Luminance Gamma", 1.2);
      writer.WriteParameter("Sky Chromaticity Gamma", 1.8);
      writer.WriteParameter("Linear Lightflow", true);
      writer.WriteParameter("Index of Refraction", 1.0);
      writer.WriteParameter("Scatter Density", 0.1);
      writer.WriteParameter("./Location/Latitude", 0.0);
      writer.WriteParameter("./Location/Longitude", 0.0);
      writer.WriteParameter("./Location/Timezone", 0);
      writer.WriteParameter("./Location/Date", "0/0/2007");
      writer.WriteParameter("./Location/Time", "12:0:0");
      writer.WriteParameter("./Background Image/Filename", "[No Bitmap]");
      writer.WriteParameter("./Background Image/Projection", "UV");
      writer.WriteParameter("./Background Image/Offset X", 0.0);
      writer.WriteParameter("./Background Image/Offset Y", 0.0);
      writer.WriteParameter("./Background Image/Scale X", 1.0);
      writer.WriteParameter("./Background Image/Scale Y", 1.0);
      writer.WriteParameter("./Background Image/Rotation", 0.0);
      writer.WriteParameter("./Background Image/Smooth", true);
      writer.WriteParameter("./Background Image/Inverted", false);
      writer.WriteParameter("./Background Image/Alpha Channel", false);
      writer.WriteEndObject();

      // Visual3DHelper.Traverse<Light>(v.Children, ExportLight);
      // Visual3DHelper.Traverse<GeometryModel3D>(v.Children, ExportGeometryModel3D);
    }

    /// <summary>
    /// Closes this exporter.
    /// </summary>
    /// <param name="writer">The writer.</param>
    protected override void Close(KerkytheaWriter writer)
    {
      // end of scene description.
      writer.WriteFullEndElement();

      // it is necessary to describe the primary/active modules as there might exist more than one!
      writer.WriteParameter("Mip Mapping", true);
      writer.WriteParameter("./Interfaces/Active", "Null Interface");
      writer.WriteParameter("./Modellers/Active", "XML Modeller");
      writer.WriteParameter("./Image Handlers/Active", "Free Image Support");

      writer.WriteParameter("./Ray Tracers/Active", "Threaded Ray Tracer");
      writer.WriteParameter("./Irradiance Estimators/Active", "Null Irradiance Estimator");
      writer.WriteParameter("./Direct Light Estimators/Active", "Refraction Enhanced");
      writer.WriteParameter("./Environments/Active", "Octree Environment");
      writer.WriteParameter("./Filters/Active", "Simple Tone Mapping");
      writer.WriteParameter("./Scenes/Active", Name);
      writer.WriteParameter("./Libraries/Active", "Material Librarian");

      // end of root element
      writer.WriteFullEndElement();

      writer.WriteEndDocument();
      writer.Close();
    }

    /// <summary>
    /// Creates a transform from the original coordinate system to the system defined by translation origin
    /// </summary>
    /// <param name="origin">The origin.</param>
    /// <param name="direction">The direction vector.</param>
    /// <param name="up">The up vector.</param>
    /// <returns>A transformation matrix.</returns>
    private static Matrix3D CreateTransform(Point3D origin, Vector3D direction, Vector3D up)
    {
      var z = direction;
      var x = Vector3D.CrossProduct(direction, up);
      var y = up;

      x.Normalize();
      y.Normalize();
      z.Normalize();

      var m = new Matrix3D(x.X, y.X, z.X, 0, x.Y, y.Y, z.Y, 0, x.Z, y.Z, z.Z, 0, origin.X, origin.Y, origin.Z, 1);

      return m;
    }

    /// <summary>
    /// Converts a <see cref="Point"/> to a string formatted for Kerkythea.
    /// </summary>
    /// <param name="p">
    /// The point.
    /// </param>
    /// <returns>
    /// A string representing the point.
    /// </returns>
    private static string ToKerkytheaString(Point p)
    {
      return string.Format(CultureInfo.InvariantCulture, "{0} {1}", p.X, p.Y);
    }

    /// <summary>
    /// Converts a <see cref="Point3D"/> to a string formatted for Kerkythea.
    /// </summary>
    /// <param name="point">
    /// The vector.
    /// </param>
    /// <returns>
    /// A string representing the point.
    /// </returns>
    private static string ToKerkytheaString(Point3D point)
    {
      return string.Format(
          CultureInfo.InvariantCulture,
          "{0:0.######} {1:0.######} {2:0.######}",
          ValueOrDefault(point.X, 1),
          ValueOrDefault(point.Y, 0),
          ValueOrDefault(point.Z, 0));
    }

    /// <summary>
    /// Converts a <see cref="Vector3D"/> to a string formatted for Kerkythea.
    /// </summary>
    /// <param name="vector">
    /// The vector.
    /// </param>
    /// <returns>
    /// A string representing the vector.
    /// </returns>
    private static string ToKerkytheaString(Vector3D vector)
    {
      return string.Format(
          CultureInfo.InvariantCulture,
          "{0:0.######} {1:0.######} {2:0.######}",
          ValueOrDefault(vector.X, 1),
          ValueOrDefault(vector.Y, 0),
          ValueOrDefault(vector.Z, 0));
    }

    /// <summary>
    /// Converts a <see cref="Color"/> to a string formatted for Kerkythea.
    /// </summary>
    /// <param name="c">
    /// The color.
    /// </param>
    /// <returns>
    /// A string representing the color.
    /// </returns>
    private static string ToKerkytheaString(Color c)
    {
      return string.Format(
          CultureInfo.InvariantCulture,
          "{0:0.######} {1:0.######} {2:0.######}",
          c.R / 255.0,
          c.G / 255.0,
          c.B / 255.0);
    }

    /// <summary>
    /// Exports the map channel (texture coordinates) from the specified mesh.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="m">The mesh.</param>
    private void ExportMapChannel(KerkytheaWriter writer, MeshGeometry3D m)
    {
      writer.WriteStartElement("Parameter");
      {
        writer.WriteAttributeString("Name", "Map Channel");
        writer.WriteAttributeString("Type", "Point2D List");
        int n = m.TriangleIndices.Count;
        writer.WriteAttributeString("Value", n.ToString());
        foreach(int index in m.TriangleIndices)
        {
          if(index >= m.TextureCoordinates.Count)
          {
            continue;
          }

          var uv = m.TextureCoordinates[index];
          writer.WriteStartElement("P");
          writer.WriteAttributeString("xy", ToKerkytheaString(uv));
          writer.WriteEndElement();
        }
      }

      writer.WriteFullEndElement();
    }

    /// <summary>
    /// Exports a material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name of the material.</param>
    /// <param name="material">The material.</param>
    /// <param name="weights">The weights.</param>
    private void ExportMaterial(KerkytheaWriter writer, string name, Material material, IList<double> weights)
    {
      var g = material as MaterialGroup;
      if(g != null)
      {
        foreach(var m in g.Children)
        {
          ExportMaterial(writer, name, m, weights);
        }
      }

      var d = material as DiffuseMaterial;
      if(d != null)
      {
        string texture = null;
        Color? color = null;
        double alpha = 1.0;
        if(d.Brush is SolidColorBrush)
        {
          color = GetSolidColor(d.Brush, d.Color);
          alpha = color.Value.A / 255.0;
        }
        else
        {
          texture = GetTexture(writer, d.Brush, name);
        }

        if(alpha > 0)
        {
          WriteWhittedMaterial(writer, string.Format("#{0}", weights.Count), texture, color, null, null);
          weights.Add(alpha);
        }

        // The refractive part
        if(alpha < 1)
        {
          WriteWhittedMaterial(writer, string.Format("#{0}", weights.Count), null, null, null, Colors.White);
          weights.Add(1 - alpha);
        }
      }

      var s = material as SpecularMaterial;
      if(s != null)
      {
        var color = GetSolidColor(s.Brush, s.Color);

        // color = Color.FromArgb((byte)(color.A * factor), (byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
        WriteWhittedMaterial(writer, string.Format("#{0}", weights.Count), null, null, color, null, s.SpecularPower * 0.5);
        double weight = color.A / 255.0;
        weight *= 0.01;
        weights.Add(weight);
      }

      var e = material as EmissiveMaterial;
      if(e != null)
      {
        // TODO
        System.Diagnostics.Debug.WriteLine("KerkytheaExporter: Emissive materials are not yet supported.");

        // Color color = GetSolidColor(e.Brush, d.Color);
        // WriteWhittedMaterial(string.Format("#{0}", weights.Count + 1), color, null, null);
        // WriteStartObject("./Translucent/Constant Texture", "Constant Texture", "", "Texture");
        // WriteParameter("Color", e.Color);
        // WriteEndObject();
      }
    }

    /// <summary>
    /// Exports the specified material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="material">The material.</param>
    private void ExportMaterial(KerkytheaWriter writer, Material material)
    {
      // If the material is registered, simply output the xml
      if(registeredMaterials.ContainsKey(material))
      {
        var doc = registeredMaterials[material];
        if(doc != null && doc.DocumentElement != null)
        {
          foreach(XmlNode e in doc.DocumentElement.ChildNodes)
          {
            writer.Write(e);
          }
        }

        return;
      }

      string name = GetUniqueName(writer, material, "Material");
      writer.WriteStartObject(name, "Layered Material", name, "Material");

      var weights = new List<double>();

      ExportMaterial(writer, name, material, weights);

      // if (Reflections)
      // {
      // WriteConstantTexture("Reflection", ReflectionColor);
      // }
      for(int i = 0; i < weights.Count; i++)
      {
        WriteWeight(writer, "Weight #" + i, weights[i]);
      }

      /*
       switch (MaterialType)
       {
           case MaterialTypes.Ashikhmin:
               this.WriteParameter("Rotation", 0.0);
               this.WriteParameter("Attenuation", "Schlick");
               this.WriteParameter("Index of Refraction", 1.0);
               this.WriteParameter("N-K File", "");
               break;
           case MaterialTypes.Diffusive: // Whitted material
               this.WriteParameter("Shininess", 60.0);
               this.WriteParameter("Transmitted Shininess", 128.0);
               this.WriteParameter("Index of Refraction", 1.0);
               this.WriteParameter("Specular Sampling", true);
               this.WriteParameter("Transmitted Sampling", false);
               this.WriteParameter("Specular Attenuation", "Cosine");
               this.WriteParameter("Transmitted Attenuation", "Cosine");
               break;
       }
       */
      writer.WriteEndObject();
    }

    /// <summary>
    /// Gets the solid color from a brush.
    /// </summary>
    /// <param name="brush">
    /// The brush.
    /// </param>
    /// <param name="defaultColor">
    /// The default color (used if the specified brush is not a <see cref="SolidColorBrush"/>).
    /// </param>
    /// <returns>
    /// The color.
    /// </returns>
    private Color GetSolidColor(Brush brush, Color defaultColor)
    {
      var scb = brush as SolidColorBrush;
      if(scb != null)
      {
        return scb.Color;
      }

      return defaultColor;
    }

    /// <summary>
    /// Gets the texture for a brush.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="brush">The brush.</param>
    /// <param name="name">The name of the material.</param>
    /// <returns>
    /// The texture filename.
    /// </returns>
    private string GetTexture(KerkytheaWriter writer, Brush brush, string name)
    {
      // reuse textures
      string textureFile;
      if(writer.TryGetTexture(brush, out textureFile))
      {
        return textureFile;
      }

      string filename = name + ".png";
      string path = Path.Combine(TexturePath, filename);
      using(var s = FileCreator(path))
      {
        RenderBrush(s, brush, TextureWidth, TextureHeight);
      }

      writer.AddTexture(brush, filename);
      return filename;
    }

    /// <summary>
    /// Gets a unique name.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="o">The object.</param>
    /// <param name="defaultName">The default name.</param>
    /// <returns>A unique name.</returns>
    private string GetUniqueName(KerkytheaWriter writer, DependencyObject o, string defaultName)
    {
      var name = o.GetValue(FrameworkElement.NameProperty) as string;
      return writer.GetUniqueName(name, defaultName);
    }

    /// <summary>
    /// Writes a ashikhmin material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="identifier">The identifier.</param>
    /// <param name="diffuse">The diffuse.</param>
    /// <param name="specular">The specular.</param>
    /// <param name="shininessXMap">The shininess x map.</param>
    /// <param name="shininessYMap">The shininess y map.</param>
    /// <param name="rotationMap">The rotation map.</param>
    /// <param name="shininessX">The shininess x.</param>
    /// <param name="shininessY">The shininess y.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="indexOfRefraction">The index of refraction.</param>
    /// <param name="nkfile">The nkfile.</param>
    private void WriteAshikhminMaterial(KerkytheaWriter writer,
        string identifier,
        Color? diffuse,
        Color? specular,
        Color? shininessXMap,
        Color? shininessYMap,
        Color? rotationMap,
        double shininessX = 100,
        double shininessY = 100,
        double rotation = 0,
        double indexOfRefraction = 1.0,
        string nkfile = null)
    {
      writer.WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

      if(diffuse.HasValue)
      {
        WriteConstantTexture(writer, "Diffuse", diffuse.Value);
      }

      if(specular.HasValue)
      {
        WriteConstantTexture(writer, "Specular", specular.Value);
      }

      if(shininessXMap.HasValue)
      {
        WriteConstantTexture(writer, "Shininess X Map", shininessXMap.Value);
      }

      if(shininessYMap.HasValue)
      {
        WriteConstantTexture(writer, "Shininess Y Map", shininessYMap.Value);
      }

      if(rotationMap.HasValue)
      {
        WriteConstantTexture(writer, "RotationMap", rotationMap.Value);
      }

      writer.WriteParameter("Shininess X", shininessX);
      writer.WriteParameter("Shininess Y", shininessY);
      writer.WriteParameter("Rotation", rotation);
      writer.WriteParameter("Attenuation", "Schlick");
      writer.WriteParameter("Index of Refraction", indexOfRefraction);
      writer.WriteParameter("N-K File", nkfile);
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes a bitmap texture.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name.</param>
    /// <param name="filename">The filename.</param>
    private void WriteBitmapTexture(KerkytheaWriter writer, string name, string filename)
    {
      if(!string.IsNullOrEmpty(filename))
      {
        writer.WriteStartObject("./" + name + "/Bitmap Texture", "Bitmap Texture", string.Empty, "Texture");
        writer.WriteParameter("Filename", filename);
        writer.WriteParameter("Projection", "UV");
        writer.WriteParameter("Offset X", 0.0);
        writer.WriteParameter("Offset Y", 0.0);
        writer.WriteParameter("Scale X", 1.0);
        writer.WriteParameter("Scale Y", 1.0);
        writer.WriteParameter("Rotation", 0.0);
        writer.WriteParameter("Smooth", true);
        writer.WriteParameter("Inverted", false);
        writer.WriteParameter("Alpha Channel", false);
        writer.WriteEndObject();
      }
    }

    /// <summary>
    /// Writes a constant texture.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name.</param>
    /// <param name="color">The color.</param>
    private void WriteConstantTexture(KerkytheaWriter writer, string name, Color color)
    {
      writer.WriteStartObject("./" + name + "/Constant Texture", "Constant Texture", string.Empty, "Texture");
      writer.WriteParameter("Color", color);
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes a dielectric material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="identifier">The identifier.</param>
    /// <param name="reflection">The reflection.</param>
    /// <param name="refraction">The refraction.</param>
    /// <param name="indexOfRefraction">The index of refraction.</param>
    /// <param name="dispersion">The dispersion.</param>
    /// <param name="nkfile">The nkfile.</param>
    private void WriteDielectricMaterial(KerkytheaWriter writer,
        string identifier,
        Color? reflection,
        Color? refraction,
        double indexOfRefraction = 1.0,
        double dispersion = 0.0,
        string nkfile = null)
    {
      writer.WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

      if(reflection.HasValue)
      {
        WriteConstantTexture(writer, "Reflection", reflection.Value);
      }

      if(refraction.HasValue)
      {
        WriteConstantTexture(writer, "Refraction", refraction.Value);
      }

      writer.WriteParameter("Index of Refraction", indexOfRefraction);
      writer.WriteParameter("Dispersion", dispersion);
      writer.WriteParameter("N-K File", string.Empty);
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes a weight.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="identifier">The identifier.</param>
    /// <param name="weight">The weight.</param>
    private void WriteWeight(KerkytheaWriter writer, string identifier, double weight)
    {
      writer.WriteStartObject(identifier, "Weighted Texture", identifier, "Texture");
      writer.WriteStartObject("Constant Texture", "Constant Texture", string.Empty, "Texture");
      writer.WriteParameter("Color", Colors.White);
      writer.WriteEndObject();
      writer.WriteParameter("Weight #0", weight);
      writer.WriteEndObject();
    }

    /// <summary>
    /// Writes a whitted material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="identifier">The identifier.</param>
    /// <param name="texture">The texture.</param>
    /// <param name="diffuse">The diffuse.</param>
    /// <param name="specular">The specular.</param>
    /// <param name="refraction">The refraction.</param>
    /// <param name="shininess">The shininess.</param>
    /// <param name="indexOfRefraction">The index of refraction.</param>
    private void WriteWhittedMaterial(KerkytheaWriter writer,
        string identifier,
        string texture,
        Color? diffuse,
        Color? specular,
        Color? refraction,
        double shininess = 128.0,
        double indexOfRefraction = 1.0)
    {
      writer.WriteStartObject(identifier, "Whitted Material", identifier, "Material");

      if(texture != null)
      {
        WriteBitmapTexture(writer, "Diffuse", texture);
      }

      if(diffuse.HasValue)
      {
        WriteConstantTexture(writer, "Diffuse", diffuse.Value);
      }

      if(specular.HasValue)
      {
        WriteConstantTexture(writer, "Specular", specular.Value);
      }

      if(refraction.HasValue)
      {
        WriteConstantTexture(writer, "Refraction", refraction.Value);
      }

      writer.WriteParameter("Shininess", shininess);
      writer.WriteParameter("Transmitted Shininess", 128.0);
      writer.WriteParameter("Index of Refraction", indexOfRefraction);
      writer.WriteParameter("Specular Sampling", false);
      writer.WriteParameter("Transmitted Sampling", false);
      writer.WriteParameter("Specular Attenuation", "Cosine");
      writer.WriteParameter("Transmitted Attenuation", "Cosine");

      writer.WriteEndObject();
    }

    /// <summary>
    /// Returns the <paramref name="value"/> or the <paramref name="defaultValue"/> if the <paramref name="value"/> is NaN.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The value.
    /// </returns>
    public static double ValueOrDefault(double value, double defaultValue)
    {
      if(double.IsNaN(value))
      {
        return defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Represents the output writer for the <see cref="KerkytheaExporter"/>.
    /// </summary>
    public class KerkytheaWriter
    {
      /// <summary>
      /// The writer
      /// </summary>
      private readonly XmlWriter writer;

      /// <summary>
      /// The names.
      /// </summary>
      private readonly HashSet<string> names = new HashSet<string>();

      /// <summary>
      /// Texture bitmaps are reused. This dictionary contains a map from brush to filename
      /// </summary>
      private readonly Dictionary<Brush, string> textureFiles = new Dictionary<Brush, string>();

      /// <summary>
      /// Initializes a new instance of the <see cref="KerkytheaWriter"/> class.
      /// </summary>
      /// <param name="stream">The stream.</param>
      public KerkytheaWriter(Stream stream)
      {
        var settings = new XmlWriterSettings { Indent = true };
        writer = XmlWriter.Create(stream, settings);
      }

      /// <summary>
      /// Writes the start element.
      /// </summary>
      /// <param name="localName">Name of the element.</param>
      public void WriteStartElement(string localName)
      {
        writer.WriteStartElement(localName);
      }

      /// <summary>
      /// Writes the attribute string.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <param name="value">The value.</param>
      public void WriteAttributeString(string name, string value)
      {
        writer.WriteAttributeString(name, value);
      }

      /// <summary>
      /// Writes the end element.
      /// </summary>
      public void WriteEndElement()
      {
        writer.WriteEndElement();
      }

      /// <summary>
      /// Writes the full end element.
      /// </summary>
      public void WriteFullEndElement()
      {
        writer.WriteFullEndElement();
      }

      /// <summary>
      /// Writes the start document.
      /// </summary>
      public void WriteStartDocument()
      {
        writer.WriteStartDocument();
      }

      /// <summary>
      /// Closes this instance.
      /// </summary>
      public void Close()
      {
        writer.Close();
      }

      /// <summary>
      /// Writes a parameter.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="type">
      /// The type.
      /// </param>
      /// <param name="value">
      /// The value.
      /// </param>
      public void WriteParameter(string name, string type, string value)
      {
        writer.WriteStartElement("Parameter");
        writer.WriteAttributeString("Name", name);
        writer.WriteAttributeString("Type", type);
        writer.WriteAttributeString("Value", value);
        writer.WriteEndElement();
      }

      /// <summary>
      /// Writes a string parameter.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="value">
      /// The value.
      /// </param>
      public void WriteParameter(string name, string value)
      {
        WriteParameter(name, "String", value);
      }

      /// <summary>
      /// Writes a color parameter.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="color">
      /// The color.
      /// </param>
      public void WriteParameter(string name, Color color)
      {
        WriteParameter(name, "RGB", ToKerkytheaString(color));
      }

      /// <summary>
      /// Writes a boolean parameter.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="flag">
      /// The flag.
      /// </param>
      public void WriteParameter(string name, bool flag)
      {
        WriteParameter(name, "Boolean", flag ? "1" : "0");
      }

      /// <summary>
      /// Writes a double parameter.
      /// </summary>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="value">
      /// The value.
      /// </param>
      public void WriteParameter(string name, double value)
      {
        WriteParameter(name, "Real", value.ToString(CultureInfo.InvariantCulture));
      }

      /// <summary>
      /// Writes an integer parameter.
      /// </summary>
      /// <param name="name">
      /// The parameter name.
      /// </param>
      /// <param name="value">
      /// The value.
      /// </param>
      public void WriteParameter(string name, int value)
      {
        WriteParameter(name, "Integer", value.ToString(CultureInfo.InvariantCulture));
      }

      /// <summary>
      /// Writes a transformation matrix.
      /// </summary>
      /// <param name="name">
      /// The name of the matrix.
      /// </param>
      /// <param name="m">
      /// The matrix.
      /// </param>
      public void WriteTransform(string name, Matrix3D m)
      {
        string value = string.Format(
            CultureInfo.InvariantCulture,
            "{0:0.######} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######} {9:0.######} {10:0.######} {11:0.######}",
            m.M11,
            m.M12,
            m.M13,
            m.OffsetX,
            m.M21,
            m.M22,
            m.M23,
            m.OffsetY,
            m.M31,
            m.M32,
            m.M33,
            m.OffsetZ);

        WriteParameter(name, "Transform", value);
      }

      /// <summary>
      /// Writes the end object.
      /// </summary>
      public void WriteEndObject()
      {
        writer.WriteFullEndElement();
      }

      /// <summary>
      /// Writes the object.
      /// </summary>
      /// <param name="identifier">
      /// The identifier.
      /// </param>
      /// <param name="label">
      /// The label.
      /// </param>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="type">
      /// The type.
      /// </param>
      public void WriteObject(string identifier, string label, string name, string type)
      {
        WriteStartObject(identifier, label, name, type);
        WriteEndObject();
      }

      /// <summary>
      /// Writes the start object.
      /// </summary>
      /// <param name="identifier">
      /// The identifier.
      /// </param>
      /// <param name="label">
      /// The label.
      /// </param>
      /// <param name="name">
      /// The name.
      /// </param>
      /// <param name="type">
      /// The type.
      /// </param>
      public void WriteStartObject(string identifier, string label, string name, string type)
      {
        writer.WriteStartElement("Object");
        writer.WriteAttributeString("Identifier", identifier);
        writer.WriteAttributeString("Label", label);
        writer.WriteAttributeString("Name", name);
        writer.WriteAttributeString("Type", type);
      }

      /// <summary>
      /// Writes the end document.
      /// </summary>
      public void WriteEndDocument()
      {
        writer.WriteEndDocument();
      }

      /// <summary>
      /// Writes the specified XML node.
      /// </summary>
      /// <param name="xmlNode">The XML node.</param>
      public void Write(XmlNode xmlNode)
      {
        xmlNode.WriteTo(writer);
      }

      /// <summary>
      /// Tries to get the texture for the specified brush.
      /// </summary>
      /// <param name="brush">The brush.</param>
      /// <param name="textureFile">The texture file.</param>
      /// <returns><c>true</c> if the texture was found.</returns>
      public bool TryGetTexture(Brush brush, out string textureFile)
      {
        return textureFiles.TryGetValue(brush, out textureFile);
      }

      /// <summary>
      /// Adds the specified texture.
      /// </summary>
      /// <param name="brush">The brush.</param>
      /// <param name="filename">The filename.</param>
      public void AddTexture(Brush brush, string filename)
      {
        textureFiles.Add(brush, filename);
      }

      /// <summary>
      /// Gets a unique name.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <param name="defaultName">The default name.</param>
      /// <returns></returns>
      public string GetUniqueName(string name, string defaultName)
      {
        if(string.IsNullOrEmpty(name))
        {
          int n = 1;
          while(true)
          {
            // name = defaultName + " #" + n;
            name = defaultName + n;
            if(!names.Contains(name))
            {
              break;
            }

            n++;
          }
        }

        names.Add(name);
        return name;
      }
    }
  }
}