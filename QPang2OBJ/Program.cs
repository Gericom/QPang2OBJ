using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonFiles;
using LibEveryFileExplorer.Collections;

namespace QPang2OBJ
{
	class Program
	{
		static void Main(string[] args)
		{
			QPangMesh q = new QPangMesh(File.ReadAllBytes(args[0]));
			OBJ o = new OBJ();
			int nr = 0;
			for (int model = 0; model < q.Models.Length; model++)
			{
				for (int mesh = 0; mesh < q.Models[model].Meshes.Length; mesh++)
				{
					Vector3[] Vertices = q.Models[model].Meshes[mesh].Seps[0].GetPositions();
					for (int i = 0; i < q.Models[model].Meshes[mesh].NrIndieces; i++)
					{
						o.Vertices.Add(Vertices[q.Models[model].Meshes[mesh].Indieces[i]]);
					}
					for (int i = 0; i + 2 < q.Models[model].Meshes[mesh].NrIndieces; i += 2)
					{
						var f = new OBJ.OBJFace();
						f.VertexIndieces.Add(nr + i);
						f.VertexIndieces.Add(nr + i + 1);
						f.VertexIndieces.Add(nr + i + 2);
						if (!(o.Vertices[nr + i] == o.Vertices[nr + i + 1] || o.Vertices[nr + i] == o.Vertices[nr + i + 2] || o.Vertices[nr + i + 1] == o.Vertices[nr + i + 2]))
							o.Faces.Add(f);

						if (i + 3 < q.Models[model].Meshes[mesh].NrIndieces)
						{
							var f2 = new OBJ.OBJFace();
							f2.VertexIndieces.Add(nr + i + 1);
							f2.VertexIndieces.Add(nr + i + 3);
							f2.VertexIndieces.Add(nr + i + 2);
							if (!(o.Vertices[nr + i + 1] == o.Vertices[nr + i + 2] || o.Vertices[nr + i + 1] == o.Vertices[nr + i + 3] || o.Vertices[nr + i + 2] == o.Vertices[nr + i + 3]))
								o.Faces.Add(f2);
						}
					}
					nr += (int)q.Models[model].Meshes[mesh].NrIndieces;
				}
			}
			File.Create("out.obj").Close();
			File.WriteAllBytes("out.obj", o.Write());
		}
	}
}
