# ObjLoader

Simple obj file loader.

## Usage example
```
ObjLoader ol;

void Init(){
  ol = new ObjLoader("Owl.obj");
  ol.CalcNormalsForEachVert();
}

void Draw(){

  List<Poly> pl = ol.Pol();

  for (int i = 0; i < pl.Count; i++)
  {
      if(pl[i].verts.Count == 3)
      {
          gl.Begin(OpenGL.GL_TRIANGLES);
      }
      else
      {
          gl.Begin(OpenGL.GL_QUADS);
      }
      gl.Color(1f, 1f, 1f);

      foreach (var vert in pl[i].verts)
      {
          gl.Normal(vert.xn, vert.yn, vert.zn);
          gl.Vertex(vert._x, vert._y, vert._z);
      }

      gl.End();
  }

}
```

**CalcNormalsForEachVert** method calculates normal for each vertex by summary adjacent surfaces normals
