using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    public class GeneratorDocument
    {
        public string Save(DatatableItem table)
        {
            string tableName = table.Name;
            string lastfix = "Entity";
            string className = tableName + lastfix;
            string saveFileDir = AppDomain.CurrentDomain.BaseDirectory + "Output\\";
            string fullPath = saveFileDir + className + ".cs";
            string nameSpace = System.Configuration.ConfigurationManager.AppSettings["nameSpace"];
            string assemblies = System.Configuration.ConfigurationManager.AppSettings["assemblies"];

            if (!System.IO.Directory.Exists(saveFileDir))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/**** gen by DataObject.Generator ****/");
            sb.AppendLine("using System;");//命名空间
            sb.AppendLine("namespace " + nameSpace);//命名空间
            sb.AppendLine("{");

            foreach (string ns in assemblies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.AppendLine(string.Format("    using {0};", ns));
            }
            sb.AppendLine();
            sb.AppendLine("\t[ActiveRecord(\"" + tableName + "\")]");
            sb.AppendLine("\tpublic class " + className + ":ActiveRecordBase");
            sb.AppendLine("\t{");

            foreach (DataCellItem item in table.Cells)
            {
                if (GetFieldTypeName(item.Type).ToLower() == "string")
                {
                    sb.AppendLine("\t\tprivate string m_" + item.Name + "= string.Empty;");
                }
                else
                {
                    sb.AppendLine("\t\tprivate " + GetFieldTypeName(item.Type) + " m_" + item.Name + ";");
                }
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + item.Description.Replace("\r\n", ""));
                sb.AppendLine("\t\t/// </summary>");
                if (item.IsPrimaryKey)
                {
                    if (item.IsIdentity)
                    {
                        sb.AppendLine("\t\t[PrimaryKey(Generator = PrimaryKeyType.Identity)]");

                    }
                    else
                    {
                        sb.AppendLine("\t\t[PrimaryKey(Generator = PrimaryKeyType.Assigned)]");
                    }
                }
                else
                {
                    sb.AppendLine("\t\t[Property(\"" + item.Name + "\")]");
                }
                sb.AppendLine("\t\t[JsonProperty(\"" + ParseToLower(item.Name) + "\")]");

                sb.AppendLine("\t\tpublic " + GetFieldTypeName(item.Type) + " " + item.Name);
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tget { return m_" + item.Name + "; }");
                sb.AppendLine("\t\t\tset { m_" + item.Name + " = value; }");
                sb.AppendLine("\t\t}");
                //sb.AppendLine("\t\tpublic " + GetFieldTypeName(item.Type) + " " + item.Name + " { set; get; }");
                sb.AppendLine();
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, false, Encoding.UTF8))
            {
                sw.Write(sb.ToString());
                sw.Flush();
                sw.Close();
            }

            return fullPath;
            
        }

        private string GetFieldTypeName(string s)
        {
            switch (s)
            {
                case "bigint":
                    return "long";
                case "int":
                    return "int";
                case "tinyint":
                    return "byte";
                case "decimal":
                    return "decimal";
                case "datetime":
                    return "DateTime";
                case "varchar":
                case "nvarchar":
                default:
                    return "string";
            }
        }

        private string ParseToLower(string s)
        {
            s = char.ToLower(s[0]) + s.Substring(1);
            string newStr = string.Empty;
            foreach (var c in s)
            {
                if (c >= 'a' && c <= 'z')
                {
                    newStr += c;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    newStr += "_" + char.ToLower(c);
                }
                else
                {
                    newStr += c;
                }
            }
            return newStr;
        }

    }

}
