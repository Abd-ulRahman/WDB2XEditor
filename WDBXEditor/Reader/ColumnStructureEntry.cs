using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WDBXEditor.Common.Constants;
using WDBXEditor.Commons;

namespace WDBXEditor.Reader
{
	public class ColumnStructureEntry
	{
		public ushort RecordOffset { get; set; }
		public ushort Size { get; set; }
		public uint AdditionalDataSize { get; set; }

		private CompressionType compressionType;
		public CompressionType CompressionType
		{
			get { return compressionType; }
			set
			{
				if (value >= CompressionType.None && value <= CompressionType.SignedImmediate)
				{
					compressionType = value;
				}
				else // Never happens just in case
					try
					{
						if (value <= CompressionType.SignedImmediate && value >= CompressionType.None)
						{
							compressionType = value;
						}
					}
					catch (Exception) { }
			}
		}

		public int BitOffset { get; set; }  // used as common data column for Sparse
		public int BitWidth { get; set; }
		public int Cardinality { get; set; } // flags for Immediate, &1: Signed


		public List<byte[]> PalletValues { get; set; }
		public Dictionary<int, byte[]> SparseValues { get; set; }
		public int ArraySize { get; set; } = 1;
	}
}
