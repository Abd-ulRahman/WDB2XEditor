﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static WDBXEditor.Common.Constants;

namespace WDBXEditor.Storage
{
	[Serializable]
	public class Definition
	{
		[XmlElement("Table")]
		public HashSet<Table> Tables { get; set; } = new HashSet<Table>();
		[XmlIgnore]
		public int Build { get; set; }
		[XmlIgnore]
		private bool _loading = false;

		public bool LoadDefinition(string path)
		{
			if (_loading) return true;

			try
			{
				XmlSerializer deser = new XmlSerializer(typeof(Definition));
				using (var fs = new FileStream(path, FileMode.Open))
				{
					Definition def = (Definition)deser.Deserialize(fs);
					var newtables = def.Tables.Where(x => Tables.Count(y => x.Build == y.Build && x.Name == y.Name) == 0).ToList();
					newtables.ForEach(x => x.Load());
					Tables.UnionWith(newtables.Where(x => x.Key != null));
					return true;
				}
			}
			catch { return false; }
		}

		public bool SaveDefinitions()
		{
			Func<string, string> ValidFilename = b =>
			{
				return string.Join("_", b.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.') + ".xml";
			};

			try
			{
				_loading = true;

				var builds = Tables.OrderBy(x => x.Name).GroupBy(x => x.Build).ToList();
				Tables.Clear();
				foreach (var build in builds)
				{
					Definition _def = new Definition();
					_def.Build = build.Key;
					_def.Tables = new HashSet<Table>(build);

					XmlSerializer ser = new XmlSerializer(typeof(Definition));
					using (var fs = new FileStream(Path.Combine(DEFINITION_DIR, ValidFilename(BuildText(build.Key))), FileMode.Create))
						ser.Serialize(fs, _def);
				}

				_loading = false;
				return true;
			}
			catch (Exception ex)
			{
				_loading = false;
				return false;
			}
		}



		public string DBDTypeToWDBXType(string type, int size)
		{
			if (type == "uint")
			{
				switch (size)
				{
					case 8:
						return "byte";
					case 16:
						return "ushort";
					case 32:
						return "uint";
					case 64:
						return "ulong";
					default:
						return "uint";
				}
			}
			else if (type == "int")
			{
				switch (size)
				{
					case 8:
						return "sbyte";
					case 16:
						return "short";
					case 32:
						return "int";
					case 64:
						return "long";
					default:
						return "int";
				}
			}
			else if (type == "locstring")
			{
				return "string";
			}
			else
			{
				// string, float
				return type;
			}
		}


		private Table BuildTable(DBDefsLib.Structs.VersionDefinitions dbdversion,
									DBDefsLib.Structs.DBDefinition dbdef,
									string dbName,
									string buildtext,
									uint build)
		{
			var table = new Table();
			table.Build = (int)build;
			table.BuildText = buildtext;
			table.Fields = new List<Field>();
			table.Name = dbName;

			Func<string, string> formatFieldName = (s) =>
			{
				string[] parts = s.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < parts.Length; i++)
					parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);

				return string.Join("_", parts);
			};

			Field relation = null;
			foreach (var dbdfield in dbdversion.definitions)
			{
				var field = new Field();
				if (dbdfield.arrLength > 0)
				{
					field.ArraySize = dbdfield.arrLength;
                }

                if (dbdfield.isID || dbdfield.name == "ID")
                {
                    field.IsIndex = true;
                    field.NonInline = dbdfield.isNonInline;
                }

                field.Name = formatFieldName(dbdfield.name);
				field.Type = DBDTypeToWDBXType(dbdef.columnDefinitions[dbdfield.name].type, dbdfield.size);

                if (field.AutoGenerate && !field.IsIndex)
                    continue; // skip relationship data columns but keep parent columns

                if (dbdfield.isNonInline && dbdfield.isRelation)
				{
					field.Relationship = true; // append relations to the end
					relation = field;
					continue;
                }

                if (dbdfield.isRelation && dbdfield.name == "Ui_order")
                {
                    field.IsIndex = false;
                    relation = field.Clone() as Field;
                    relation.Relationship = true;
                    relation.Relationship = dbdfield.isRelation;
                    relation.Name = field.Name + "_RelationShip"; // append parents to the end
                    continue;
                }

                table.Fields.Add(field);
			}

			// WDBX requires an ID column
			if (!table.Fields.Any(x => x.IsIndex))
			{
				Field autoGenerate = new Field()
				{
				//	Name = "ID",
					AutoGenerate = true,
					IsIndex = true,
					Type = "int"
				};

				table.Fields.Insert(0, autoGenerate);
			}

			if (relation != null) // force to the end
				table.Fields.Add(relation);

			return table;
		}

		public bool LoadDBDefinition(string path/*, out List<string> errors*/)
		{
			//errors = new List<string>();

			if (_loading) return true;

			var reader = new DBDefsLib.DBDReader();
			var dbdef = reader.Read(path);
			var dbName = Path.GetFileNameWithoutExtension(path);

			var newtables = new List<Table>();

			// going to need some form of build lookup or summat for ranges...
			foreach (var dbdversion in dbdef.versionDefinitions)
			{
				foreach (var dbdbuild in dbdversion.buildRanges)
				{
					newtables.Add(BuildTable(dbdversion, dbdef, dbName, DBDefsLib.Utils.BuildToString(dbdbuild.minBuild), dbdbuild.minBuild.build));
					newtables.Add(BuildTable(dbdversion, dbdef, dbName, DBDefsLib.Utils.BuildToString(dbdbuild.maxBuild), dbdbuild.maxBuild.build));
				}

				foreach (var dbdbuild in dbdversion.builds)
				{
					newtables.Add(BuildTable(dbdversion, dbdef, dbName, DBDefsLib.Utils.BuildToString(dbdbuild), dbdbuild.build));
				}
			}

			newtables.ForEach(x => x.Load());
			//errors.AddRange(newtables.Where(x => x.Key == null).Select(x => $"{x.Name} {x.BuildText} missing ID."));
			Tables.UnionWith(newtables.Where(x => x.Key != null));

			return true;
		}
	}

	[Serializable]
	public class Table
	{
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public int Build { get; set; }
		[XmlElement("Field")]
		public List<Field> Fields { get; set; }
		[XmlIgnore]
		public Field Key { get; private set; }
		[XmlIgnore]
		public bool Changed { get; set; } = false;
		[XmlIgnore]
		public string BuildText { get; set; }

		public void Load()
		{
			Key = Fields.FirstOrDefault(x => x.IsIndex);
			BuildText = BuildText(Build);
		}
	}

	[Serializable]
	public class Field : ICloneable
	{
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public string Type { get; set; }
		[XmlAttribute, DefaultValue(1)]
		public int ArraySize { get; set; } = 1;
		[XmlAttribute, DefaultValue(false)]
		public bool IsIndex { get; set; } = false;
		[XmlAttribute, DefaultValue(false)]
		public bool AutoGenerate { get; set; } = false;
		[XmlAttribute, DefaultValue("")]
		public string DefaultValue { get; set; } = "";
		[XmlAttribute, DefaultValue("")]
		public string ColumnNames { get; set; } = "";
		[XmlAnyAttribute, DefaultValue(false)]
		public bool Relationship { get; set; } = false;
		[XmlAnyAttribute, DefaultValue(false)]
		public bool NonInline { get; set; } = false;
		[XmlIgnore]
		public string InternalName { get; set; }

		public object Clone() => this.MemberwiseClone();
	}
}
