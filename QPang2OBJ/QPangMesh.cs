using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibEveryFileExplorer.Collections;
using System.IO;

namespace QPang2OBJ
{
	public class QPangMesh
	{
		public QPangMesh(byte[] Data)
		{
			EndianBinaryReader er = new EndianBinaryReader(new MemoryStream(Data), Endianness.LittleEndian);
			try
			{
				Header = new QPangMeshHeader(er);
				Models = new QPangMeshModel[Header.NrModels];
				for (int i = 0; i < Header.NrModels; i++)
				{
					Models[i] = new QPangMeshModel(er);
				}
			}
			catch (NotSupportedException e) { }
			finally
			{
				er.Close();
			}
		}

		public QPangMeshHeader Header;
		public class QPangMeshHeader
		{
			public QPangMeshHeader(EndianBinaryReader er)
			{
				Unknown1 = er.ReadUInt32();
				Version = er.ReadUInt32();
				Unknown2 = er.ReadUInt32();
				NrModels = er.ReadUInt32();
				FileSize = er.ReadUInt32();
			}
			public UInt32 Unknown1;
			public UInt32 Version;//?
			public UInt32 Unknown2;
			public UInt32 NrModels;
			public UInt32 FileSize;//without header
		}

		public QPangMeshModel[] Models;
		public class QPangMeshModel
		{
			public QPangMeshModel(EndianBinaryReader er)
			{
				Name = er.ReadStringNT(Encoding.ASCII);
				Unknown1 = er.ReadBytes(0x84 - (Name.Length + 1));
				CoordType = er.ReadUInt32();
				Unknown2 = er.ReadUInt32();
				Unknown3 = er.ReadUInt32();
				Unknown4 = er.ReadUInt16();
				NrMeshes = er.ReadUInt16();
				BBMin = er.ReadVector3();
				BBMax = er.ReadVector3();
				Unknown6 = er.ReadUInt32();
				if (Unknown6 != 0)
				{
					Unknown6Unk = er.ReadBytes(0x10);
					Unknown6NumSepInd = er.ReadUInt32();
				}
				int count = 1;
				if (Unknown6 != 0) count = (int)Unknown6NumSepInd;
				Meshes = new QPangMeshModelMesh[NrMeshes];
				for (int i = 0; i < NrMeshes; i++)
				{
					Meshes[i] = new QPangMeshModelMesh(er, count);
				}
			}
			public String Name;
			public byte[] Unknown1;
			public UInt32 CoordType;//0 = global, 1 = local
			public UInt32 Unknown2;
			public UInt32 Unknown3;
			public UInt16 Unknown4;
			public UInt16 NrMeshes;
			public Vector3 BBMin;
			public Vector3 BBMax;
			public UInt32 Unknown6;

			public byte[] Unknown6Unk;
			public UInt32 Unknown6NumSepInd;

			public QPangMeshModelMesh[] Meshes;
			public class QPangMeshModelMesh
			{
				public QPangMeshModelMesh(EndianBinaryReader er, int NumSeps)
				{
					Name = er.ReadStringNT(Encoding.ASCII);
					Unknown1 = er.ReadBytes(0x40 - (Name.Length + 1));
					Unknown2 = er.ReadUInt32();
					VertexLength = er.ReadUInt32();
					VertexNrAttributes = er.ReadUInt32();
					VertexAttributes = new QPangMeshVertexAttribute[VertexNrAttributes];
					for(int i = 0; i < VertexNrAttributes; i++)
					{
						VertexAttributes[i] = new QPangMeshVertexAttribute(er);
					}

					Seps = new QPangMeshSep[NumSeps];
					for (int i = 0; i < NumSeps; i++)
					{
						Seps[i] = new QPangMeshSep(er, VertexLength);
					}
					NrIndieces = er.ReadUInt32();
					Indieces = er.ReadUInt16s((int)NrIndieces);
				}
				public String Name;
				public byte[] Unknown1;

				public UInt32 Unknown2;
				public UInt32 VertexLength;
				public UInt32 VertexNrAttributes;
				public QPangMeshVertexAttribute[] VertexAttributes;
				public class QPangMeshVertexAttribute
				{
					public QPangMeshVertexAttribute(EndianBinaryReader er)
					{
						Offset = er.ReadUInt32();
						Unknown1 = er.ReadUInt32();
						Unknown2 = er.ReadUInt32();
						Unknown3 = er.ReadUInt32();
					}
					public UInt32 Offset;
					public UInt32 Unknown1;
					public UInt32 Unknown2;
					public UInt32 Unknown3;
				}

				public QPangMeshSep[] Seps;
				public class QPangMeshSep
				{
					public QPangMeshSep(EndianBinaryReader er, uint EntryLength)
					{
						NrVertices = er.ReadUInt32();
						VertexData = er.ReadBytes((int)(NrVertices * EntryLength));
					}
					public UInt32 NrVertices;
					public byte[] VertexData;

					public Vector3[] GetPositions()
					{
						int entlength = (int)(VertexData.Length / NrVertices);
						List<Vector3> result = new List<Vector3>();
						int offs = 0;
						for (int i = 0; i < NrVertices; i++)
						{
							result.Add(new Vector3(BitConverter.ToSingle(VertexData, offs), BitConverter.ToSingle(VertexData, offs + 4), BitConverter.ToSingle(VertexData, offs + 8)));
							offs += entlength;
						}
						return result.ToArray();
					}
				}
				public UInt32 NrIndieces;
				public UInt16[] Indieces;
			}			
		}
	}
}
