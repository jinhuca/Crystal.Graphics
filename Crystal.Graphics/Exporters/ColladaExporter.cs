﻿using System.IO;

namespace Crystal.Graphics
{
  /// <summary>
  /// Exports the 3D visual tree to a Collada 1.5.0 file.
  /// </summary>
  public class ColladaExporter : Exporter<XmlWriter>
  {
    /// <summary>
    /// The effect dictionary.
    /// </summary>
    private readonly Dictionary<Material, string> effects = new();

    /// <summary>
    /// The geometry dictionary.
    /// </summary>
    private readonly Dictionary<MeshGeometry3D, string> geometries = new();

    /// <summary>
    /// The light dictionary.
    /// </summary>
    private readonly Dictionary<Light, string> lights = new();

    /// <summary>
    /// The material dictionary.
    /// </summary>
    private readonly Dictionary<Material, string> materials = new();

    /// <summary>
    /// The node dictionary.
    /// </summary>
    private readonly Dictionary<Model3D, string> nodes = new();

    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>The author.</value>
    public string Author { get; set; } = null!;

    /// <summary>
    /// Gets or sets the comments.
    /// </summary>
    /// <value>The comments.</value>
    public string Comments { get; set; } = null!;

    /// <summary>
    /// Gets or sets the copyright.
    /// </summary>
    /// <value>The copyright.</value>
    public string Copyright { get; set; } = null!;

    /// <summary>
    /// Exports the specified viewport.
    /// Exports model, camera and lights.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="stream">The output stream.</param>
    public override void Export(Viewport3D viewport, Stream stream)
    {
      //// http://en.wikipedia.org/wiki/COLLADA
      //// http://www.khronos.org/collada/
      //// https://collada.org/mediawiki/index.php/COLLADA_-_Digital_Asset_and_FX_Exchange_Schema
      //// https://collada.org/mediawiki/index.php/COLLADA.net
      //// http://www.mogware.com/index.php?page=collada.NET
      //// http://www.okino.com/conv/exp_collada.htm

      var writer = Create(stream);
      WriteAssets(writer, viewport);

      // Export camera

      // Export lights
      viewport.Children.Traverse<Light>((l, t) => base.ExportLight(writer, l, t));

      writer.WriteStartElement("library_materials");
      viewport.Children.Traverse<GeometryModel3D>((gm, _) => ExportMaterial(writer, gm));
      writer.WriteEndElement();

      writer.WriteStartElement("library_effects");
      viewport.Children.Traverse<GeometryModel3D>((gm, _) => ExportEffect(writer, gm));
      writer.WriteEndElement();

      // writer.WriteStartElement("library_cameras");
      // this.ExportCamera(viewport.Camera);
      // writer.WriteEndElement();

      // writer.WriteStartElement("library_lights");
      // Visual3DHelper.Traverse<Light>(viewport.Children, this.ExportLight);
      // writer.WriteEndElement();
      writer.WriteStartElement("library_geometries");
      viewport.Children.Traverse<GeometryModel3D>((gm, t) => ExportGeometry(writer, gm, t));
      writer.WriteEndElement();

      writer.WriteStartElement("library_nodes");
      viewport.Children.Traverse<GeometryModel3D>((gm, t) => ExportNode(writer, gm, t));
      writer.WriteEndElement();

      writer.WriteStartElement("library_visual_scenes");
      writer.WriteStartElement("visual_scene");
      writer.WriteAttributeString("id", "RootNode");
      writer.WriteAttributeString("name", "RootNode");

      // this.ExportCameraNode(viewport.Camera);
      viewport.Children.Traverse<GeometryModel3D>((gm, t) => ExportSceneNode(writer, gm, t));
      writer.WriteEndElement();
      writer.WriteEndElement();

      writer.WriteStartElement("scene");
      writer.WriteStartElement("instance_visual_scene");
      writer.WriteAttributeString("url", "#RootNode");
      writer.WriteEndElement();
      writer.WriteEndElement();

      Close(writer);
    }

    /// <summary>
    /// Creates a new <see cref="XmlWriter" /> on the specified stream.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    /// <returns>A <see cref="XmlWriter"/>.</returns>
    protected override XmlWriter Create(Stream stream)
    {
      var writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented };
      writer.WriteStartDocument(false);
      writer.WriteStartElement("COLLADA");
      writer.WriteAttributeString("xmlns", "http://www.collada.org/2008/03/COLLADASchema");
      writer.WriteAttributeString("version", "1.5.0");
      return writer;
    }

    /// <summary>
    /// Closes this exporter.
    /// </summary>
    /// <param name="writer">The writer.</param>
    protected override void Close(XmlWriter writer)
    {
      writer.WriteEndElement(); // COLLADA
      writer.WriteEndDocument();
      writer.Close();
    }

    /// <summary>
    /// Exports the model.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="model">The model.</param>
    /// <param name="inheritedTransform">The inherited transform.</param>
    protected override void ExportModel(XmlWriter writer, GeometryModel3D model, Transform3D inheritedTransform)
    {
      // var mesh = model.Geometry as MeshGeometry3D;
      // var indices = new StringBuilder();
      // foreach (int i in mesh.TriangleIndices)
      // {
      // indices.Append(i + " ");
      // }

      // var points = new StringBuilder();
      // foreach (var pt in mesh.Positions)
      // {
      // points.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", pt.X, pt.Y, pt.Z);
      // }

      // writer.WriteStartElement("Transform");
      // writer.WriteStartElement("Shape");
      // writer.WriteStartElement("IndexedFaceSet");
      // writer.WriteAttributeString("coordIndex", indices.ToString());
      // writer.WriteStartElement("Coordinate");
      // writer.WriteAttributeString("point", points.ToString());
      // writer.WriteEndElement();
      // writer.WriteEndElement(); // IndexedFaceSet
      // writer.WriteStartElement("Appearance");

      // writer.WriteStartElement("Material");
      // writer.WriteAttributeString("diffuseColor", "0.8 0.8 0.2");
      // writer.WriteAttributeString("specularColor", "0.5 0.5 0.5");
      // writer.WriteEndElement();
      // writer.WriteEndElement(); // Appearance

      // writer.WriteEndElement(); // Shape
      // writer.WriteEndElement(); // Transform
    }

    /// <summary>
    /// Writes the file assets.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="viewport">The viewport.</param>
    private void WriteAssets(XmlWriter writer, Viewport3D viewport)
    {
      var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

      var assemblyName = assembly.GetName();
      var authoringTool = $"{assemblyName.Name} {assemblyName.Version?.ToString(3)}";

      var created = DateTime.Now;
      var createdString = created.ToString("u").Replace(' ', 'T');
      var projectionCamera = viewport.Camera as ProjectionCamera;
      var upAxis = "Z_UP";
      if(projectionCamera != null && projectionCamera.UpDirection.Y > projectionCamera.UpDirection.Z)
      {
        upAxis = "Y_UP";
      }

      writer.WriteStartElement("asset");

      writer.WriteStartElement("contributor");
      if(Author != null)
      {
        writer.WriteElementString("author", Author);
      }

      if(Copyright != null)
      {
        writer.WriteElementString("copyright", Copyright);
      }

      if(Comments != null)
      {
        writer.WriteElementString("comments", Comments);
      }

      writer.WriteElementString("authoring_tool", authoringTool);
      writer.WriteEndElement(); // contributor

      writer.WriteElementString("created", createdString);
      writer.WriteElementString("modified", createdString);

      // writer.WriteStartElement("unit");
      // writer.WriteAttributeString("meter", "1.0");
      // writer.WriteAttributeString("name", "meter");
      // writer.WriteEndElement(); // unit
      writer.WriteElementString("up_axis", upAxis);
      writer.WriteEndElement(); // asset
    }

    /// <summary>
    /// Binds the specified geometry and material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="geometryId">The geometry identifier.</param>
    /// <param name="materialId">The material identifier.</param>
    private void BindMaterial(XmlWriter writer, string geometryId, string materialId)
    {
      writer.WriteStartElement("instance_geometry");
      writer.WriteAttributeString("url", "#" + geometryId);
      writer.WriteStartElement("bind_material");
      writer.WriteStartElement("technique_common");
      writer.WriteStartElement("instance_material");
      writer.WriteAttributeString("symbol", "Material2");
      writer.WriteAttributeString("target", "#" + materialId);
      writer.WriteStartElement("bind_vertex_input");
      writer.WriteAttributeString("semantic", "UVSET0");
      writer.WriteAttributeString("input_semantic", "TEXCOORD");
      writer.WriteAttributeString("input_set", "0");
      writer.WriteEndElement(); // bind_vertex_input
      writer.WriteEndElement(); // instance_material
      writer.WriteEndElement(); // technique_common
      writer.WriteEndElement(); // bind_material
      writer.WriteEndElement(); // instance_geometry
    }

    /// <summary>
    /// Exports the effect of the specified model.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="model">The model.</param>
    private void ExportEffect(XmlWriter writer, GeometryModel3D model)
    {
      ExportEffect(writer, model.Material);
      ExportEffect(writer, model.BackMaterial);
    }

    /// <summary>
    /// Exports the effect of the specified material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="m">The material.</param>
    private void ExportEffect(XmlWriter writer, Material m)
    {
      if(m == null)
      {
        return;
      }

      var id = effects[m];
      writer.WriteStartElement("effect");
      writer.WriteAttributeString("id", id);
      writer.WriteAttributeString("name", id);
      writer.WriteStartElement("profile_COMMON");
      writer.WriteStartElement("technique");
      writer.WriteAttributeString("sid", "common");
      writer.WriteStartElement("phong");

      var emissiveMaterial = MaterialHelper.GetFirst<EmissiveMaterial>(m);
      if(emissiveMaterial != null)
      {
        WritePhongMaterial(writer, "emission", emissiveMaterial.Color);
      }

      var diffuseMaterial = MaterialHelper.GetFirst<DiffuseMaterial>(m);
      if(diffuseMaterial != null)
      {
        WritePhongMaterial(writer, "diffuse", diffuseMaterial.Color);
      }

      var specularMaterial = MaterialHelper.GetFirst<SpecularMaterial>(m);
      if(specularMaterial != null)
      {
        WritePhongMaterial(writer, "specular", specularMaterial.Color);
      }

      writer.WriteEndElement(); // phong
      writer.WriteEndElement(); // technique
      writer.WriteEndElement(); // profile_COMMON
      writer.WriteEndElement(); // effect
    }

    /// <summary>
    /// Exports the geometry of the specified model.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="model">The model.</param>
    /// <param name="transform">The transform.</param>
    /// <exception cref="System.InvalidOperationException">Model is not a MeshGeometry3D.</exception>
    // ReSharper disable once UnusedParameter.Local
    private void ExportGeometry(XmlWriter writer, GeometryModel3D model, Transform3D transform)
    {
      if(model.Geometry is not MeshGeometry3D mg)
      {
        throw new InvalidOperationException("Model is not a MeshGeometry3D.");
      }

      writer.WriteStartElement("geometry");
      writer.WriteStartElement("mesh");

      // write positions
      var id = geometries.Count;
      var meshId = "mesh" + id;
      geometries.Add(mg, meshId);

      writer.WriteStartElement("source");
      var positionsId = "p" + id;
      writer.WriteAttributeString("id", positionsId);
      writer.WriteStartElement("float_array");
      var positionsArrayId = positionsId + "-array";
      writer.WriteAttributeString("id", positionsArrayId);
      writer.WriteAttributeString("count", (mg.Positions.Count * 3).ToString(CultureInfo.InvariantCulture));
      var psb = new StringBuilder();
      foreach(var p in mg.Positions)
      {
        psb.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", p.X, p.Y, p.Z);
      }

      writer.WriteRaw(psb.ToString());
      writer.WriteEndElement(); // float array

      writer.WriteStartElement("technique_common");
      writer.WriteStartElement("accessor");
      writer.WriteAttributeString("source", "#" + positionsArrayId);
      writer.WriteAttributeString("count", mg.Positions.Count.ToString(CultureInfo.InvariantCulture));
      writer.WriteAttributeString("stride", "3");
      writer.WriteStartElement("param");
      writer.WriteAttributeString("name", "X");
      writer.WriteAttributeString("type", "float");
      writer.WriteEndElement(); // param
      writer.WriteStartElement("param");
      writer.WriteAttributeString("name", "Y");
      writer.WriteAttributeString("type", "float");
      writer.WriteEndElement(); // param
      writer.WriteStartElement("param");
      writer.WriteAttributeString("name", "Z");
      writer.WriteAttributeString("type", "float");
      writer.WriteEndElement(); // param
      writer.WriteEndElement(); // accessor
      writer.WriteEndElement(); // technique_common

      writer.WriteEndElement(); // source

      writer.WriteStartElement("vertices");
      var verticesId = "v" + id;
      writer.WriteAttributeString("id", verticesId);

      writer.WriteStartElement("input");
      writer.WriteAttributeString("semantic", "POSITION");
      writer.WriteAttributeString("source", "#" + positionsId);
      writer.WriteEndElement(); // input

      // writer.WriteStartElement("input");
      // writer.WriteAttributeString("semantic", "NORMAL");
      // writer.WriteAttributeString("source", normalsId);
      // writer.WriteEndElement(); // input
      writer.WriteEndElement(); // vertices

      writer.WriteStartElement("triangles");
      writer.WriteAttributeString("count", mg.TriangleIndices.Count.ToString(CultureInfo.InvariantCulture));
      writer.WriteAttributeString("material", "xx");
      writer.WriteStartElement("input");
      writer.WriteAttributeString("offset", "0");
      writer.WriteAttributeString("semantic", "VERTEX");
      writer.WriteAttributeString("source", "#" + verticesId);
      writer.WriteEndElement(); // input
      var sb = new StringBuilder();
      foreach(var i in mg.TriangleIndices)
      {
        sb.Append(i + " ");
      }

      writer.WriteElementString("p", sb.ToString());
      writer.WriteEndElement(); // triangles

      writer.WriteEndElement(); // mesh
      writer.WriteEndElement(); // geometry
    }

    /// <summary>
    /// Exports the specified light.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="light">The light.</param>
    private void ExportLight(XmlWriter writer, Light light)
    {
      if(light == null || lights.ContainsKey(light))
      {
        return;
      }

      var id = "light_" + lights.Count;
      lights.Add(light, id);
      writer.WriteStartElement("light");
      writer.WriteAttributeString("id", id);
      writer.WriteAttributeString("name", id);
      writer.WriteStartElement("technique_common");

      if(light is AmbientLight al)
      {
        writer.WriteStartElement("ambient");
        WriteColor(writer, al.Color);
        writer.WriteEndElement();
      }

      if(light is DirectionalLight dl)
      {
        writer.WriteStartElement("directional");
        WriteColor(writer, dl.Color);
        writer.WriteEndElement();
      }

      if(light is PointLight pl)
      {
        writer.WriteStartElement("point");
        WriteColor(writer, pl.Color);
        WriteDouble(writer, "constant_attenuation", pl.ConstantAttenuation);
        WriteDouble(writer, "linear_attenuation", pl.LinearAttenuation);
        WriteDouble(writer, "quadratic_attenuation", pl.QuadraticAttenuation);
        writer.WriteEndElement();
      }

      if(light is SpotLight sl)
      {
        writer.WriteStartElement("spot");
        WriteColor(writer, sl.Color);
        WriteDouble(writer, "constant_attenuation", sl.ConstantAttenuation);
        WriteDouble(writer, "linear_attenuation", sl.LinearAttenuation);
        WriteDouble(writer, "quadratic_attenuation", sl.QuadraticAttenuation);
        WriteDouble(writer, "falloff_angle", sl.InnerConeAngle);

        // writer.WriteElementString("falloff_exponent",sl.xxx.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();
      }

      writer.WriteEndElement(); // technique_common
      writer.WriteEndElement(); // light
    }

    /// <summary>
    /// Exports the material in the specified model.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="model">The model.</param>
    private void ExportMaterial(XmlWriter writer, GeometryModel3D model)
    {
      ExportMaterial(writer, model.Material);
      ExportMaterial(writer, model.BackMaterial);
    }

    /// <summary>
    /// Exports the specified material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="m">The material.</param>
    private void ExportMaterial(XmlWriter writer, Material m)
    {
      if(m == null || materials.ContainsKey(m))
      {
        return;
      }

      var id = "material_" + materials.Count;
      var effectid = "effect_" + materials.Count;
      materials.Add(m, id);
      effects.Add(m, effectid);
      writer.WriteStartElement("material");
      writer.WriteAttributeString("id", id);
      writer.WriteAttributeString("name", id);
      writer.WriteStartElement("instance_effect");
      writer.WriteAttributeString("url", "#" + effectid);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    /// <summary>
    /// Exports the specified model as a node.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="gm">The model.</param>
    /// <param name="transform">The transform.</param>
    /// <exception cref="System.InvalidOperationException">Model is not a MeshGeometry3D.</exception>
    // ReSharper disable once UnusedParameter.Local
    private void ExportNode(XmlWriter writer, GeometryModel3D gm, Transform3D transform)
    {
      if(gm.Geometry is not MeshGeometry3D mg)
      {
        throw new InvalidOperationException("Model is not a MeshGeometry3D.");
      }

      var geometryId = geometries[mg];
      var nodeId = geometryId + "-node";
      nodes.Add(gm, nodeId);
      writer.WriteStartElement("node");
      writer.WriteAttributeString("id", nodeId);
      writer.WriteAttributeString("name", nodeId);
      if(gm.Material != null && materials.TryGetValue(gm.Material, out var frontMaterialId))
      {
        BindMaterial(writer, geometryId, frontMaterialId);
      }

      if(gm.BackMaterial != null && materials.TryGetValue(gm.BackMaterial, out var backMaterialId))
      {
        BindMaterial(writer, geometryId, backMaterialId);
      }

      writer.WriteEndElement(); // node
    }

    /// <summary>
    /// Exports the specified model as a scene node.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="gm">The model.</param>
    /// <param name="transform">The transform.</param>
    private void ExportSceneNode(XmlWriter writer, Model3D gm, Transform3D transform)
    {
      var nodeId = nodes[gm];
      var instanceId = nodeId + "-instance";
      writer.WriteStartElement("node");
      writer.WriteAttributeString("id", instanceId);
      writer.WriteAttributeString("name", instanceId);
      var totalTransform = Transform3DHelper.CombineTransform(transform, gm.Transform);
      WriteMatrix(writer, "matrix", totalTransform.Value);
      writer.WriteStartElement("instance_node");
      writer.WriteAttributeString("url", "#" + nodeId);
      writer.WriteEndElement(); // instance node
      writer.WriteEndElement(); // node
    }

    /// <summary>
    /// Writes the specified color.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="color">The color.</param>
    private void WriteColor(XmlWriter writer, Color color)
    {
      writer.WriteElementString("color", string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0));
    }

    /// <summary>
    /// Writes the specified element value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The value.</param>
    private void WriteDouble(XmlWriter writer, string name, double value)
    {
      writer.WriteElementString(name, value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Writes the specified element matrix.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="m">The matrix.</param>
    private void WriteMatrix(XmlWriter writer, string name, Matrix3D m)
    {
      var value = string.Format(
          CultureInfo.InvariantCulture,
          "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
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
          m.OffsetZ,
          0,
          0,
          0,
          1);

      writer.WriteElementString(name, value);
    }

    /// <summary>
    /// Writes a phong material.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="name">The name.</param>
    /// <param name="color">The color.</param>
    private void WritePhongMaterial(XmlWriter writer, string name, Color color)
    {
      writer.WriteStartElement(name);
      WriteColor(writer, color);
      writer.WriteEndElement();
    }
  }
}