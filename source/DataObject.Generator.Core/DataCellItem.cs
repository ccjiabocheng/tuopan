using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    using System.Data;

    public class DataCellItem: ISqlObject
    {
        public DataCellItem(DatatableItem owner)
        {
            _owner = owner;
            id = Guid.NewGuid().ToString();
        }

        private DatatableItem _owner = null;
        private string id = string.Empty;
        private bool isPrimaryKey = false;
        private string name = string.Empty;
        private bool isIdentity = false;
        private int maxLength = 0;
        private bool allowNull = true;
        private object defaultValue = string.Empty;
        private string description = string.Empty;
        private string type = string.Empty;


        public void LoadFrom(DataRow row)
        {
            if (row == null) return;

            if (row["FieldName"] != null)
            {
                name = Convert.ToString(row["FieldName"]);
            }
            if (row["MaxLength"] != null)
            {
                maxLength = Convert.ToInt32(row["MaxLength"]);
            }
            if (row["DefaultValue"] != null)
            {
                defaultValue = Convert.ToString(row["DefaultValue"]);
            }
            if (row["Description"] != null)
            {
                description = Convert.ToString(row["Description"]);
            }
            if (row["PK"] != null && "PK".Equals(row["PK"].ToString()))
            {
                isPrimaryKey = true;
            }
            if (row["IsIdentity"] != null && !string.IsNullOrEmpty(row["IsIdentity"].ToString()))
            {
                isIdentity = true;
            }
            if (row["AllowNull"] != null && "N".Equals(row["AllowNull"].ToString()))
            {
                allowNull = false;
            }
            if (row["FieldType"] != null)
            {
                type = row["FieldType"].ToString();
            }

        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public bool IsPrimaryKey
        {
            get
            {
                return isPrimaryKey;
            }
        }

        public bool IsIdentity
        {
            get
            {
                return isIdentity;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public int MaxLength
        {
            get
            {
                return maxLength;
            }
        }

        public bool AllowNull
        {
            get
            {
                return allowNull;
            }
        }

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public DatatableItem Owner
        {
            get
            {
                return _owner;
            }
        }

        public System.Collections.ICollection Children
        {
            get
            {
                return new List<DataCellItem>();
            }
        }


    }
}
