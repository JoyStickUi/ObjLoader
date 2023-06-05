using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CGRGZ
{
    class ObjLoader
    {
        List<Point3D> verticies;
        List<Poly> pl;
        List<Point3D> normals;
        List<int> normal_refs;

        public bool isQuads = false;

        public ObjLoader(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File doesn't exist");
            }

            verticies = new List<Point3D>();
            pl = new List<Poly>();
            normals = new List<Point3D>();
            normal_refs = new List<int>();

            using (StreamReader sr = File.OpenText(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] s_line = line.Replace('.', ',').Replace("  ", " ").Split(' ');
                    s_line = s_line.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (s_line.Length == 0) continue;
                    if (s_line[0] == "v")
                    {
                        verticies.Add(new Point3D(
                            float.Parse(s_line[1]),
                            float.Parse(s_line[2]),
                            float.Parse(s_line[3])
                            ));
                    }
                    if (s_line[0] == "vn")
                    {
                        normals.Add(new Point3D(
                            float.Parse(s_line[1]),
                            float.Parse(s_line[2]),
                            float.Parse(s_line[3])
                            ));
                    }
                    if (s_line[0] == "f")
                    {
                        Poly tr = new Poly();

                        string[] refs = null;

                        for (int i = 1; i < s_line.Length; ++i)
                        {
                            refs = s_line[i].Split('/');
                            tr.verts.Add(verticies[int.Parse(refs[0]) - 1]);
                        }

                        Point3D normal = null;

                        if (refs.Length > 2)
                        {
                            normal = normals[int.Parse(refs[2]) - 1];                          
                        }
                        else
                        {
                            Point3D p11 = tr.verts[0], p22 = tr.verts[1], p33 = tr.verts[2];
                            normal = CrossProduct(
                                 new Point3D(p22._x - p11._x, p22._y - p11._y, p22._z - p11._z)
                                , new Point3D(p33._x - p11._x, p33._y - p11._y, p33._z - p11._z)
                            );
                            normals.Add(normal);
                        }

                        foreach (var vert in tr.verts)
                        {
                            vert.adjacentN.Add(normal);
                        }

                        pl.Add(tr);
                    }
                }
            }
        }

        public void CalcNormalsForEachVert()
        {
            for (int i = 0; i < pl.Count; i++)
            {
                for (int j = 0; j < pl[i].verts.Count; ++j)
                {
                    Point3D p = pl[i].verts[j];

                    foreach (Point3D pn in p.adjacentN)
                    {
                        p.xn += pn._x;
                        p.yn += pn._y;
                        p.zn += pn._z;
                    }
                }
            }
        }

        private Point3D CrossProduct(Point3D a, Point3D b)
        {
            Point3D c = new Point3D();
            c._x = a._y * b._z - a._z * b._y;
            c._y = a._z * b._x - a._x * b._z;
            c._z = a._x * b._y - a._y * b._x;
            return c;
        }

        public List<Point3D> Verticies()
        {
            return verticies;
        }

        public List<Poly> Pol()
        {
            return pl;
        }

        public List<Point3D> Normals()
        {
            return normals;
        }

        public List<int> Nrefs()
        {
            return normal_refs;
        }
    }
}
