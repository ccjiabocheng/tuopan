using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    using System.Data;
    using Comm;
    using Collection;
    public class DatabaseItem: ISqlObject
    {
        public long Id { set; get; }
        
        public string Name { set; get; }
        
        public DateTime CreateTime { set; get; }
        
        public string FileName { set; get; }

        public void LoadFrom(DataRow row)
        {
            if (row == null) return;

            if (row["dbid"] != null)
            {
                Id = Convert.ToInt64(row["dbid"]);
            }
            if (row["name"] != null)
            {
                Name = Convert.ToString(row["name"]);
            }
            if (row["crdate"] != null)
            {
                CreateTime = Convert.ToDateTime(row["crdate"]);
            }
            if (row["filename"] != null)
            {
                FileName = Convert.ToString(row["filename"]);
            }
        }

        public SqlObjectCollection<DatabaseItem> All()
        {
            DataTable results =
                   SqlHelper.ExecuteDataset(
                       SqlHelper.GetConnection(),
                       CommandType.Text,
                       @"SELECT * FROM master..sysdatabases ORDER BY name").Tables[0];

            SqlObjectCollection<DatabaseItem> all = new SqlObjectCollection<DatabaseItem>();
            if (results.Rows.Count > 0)
            {
                foreach (DataRow row in results.Rows)
                {
                    DatabaseItem item = new DatabaseItem();
                    item.LoadFrom(row);
                    all.Add(item);
                }
            }

            return all;
        }

        public System.Collections.ICollection Children
        {
            get
            {
                return Tables.DefaultCollection;
            }
        }

        public SqlObjectCollection<DatatableItem> Tables
        {
            get
            {
                DataTable results =
                    SqlHelper.ExecuteDataset(
                        SqlHelper.GetConnection(),
                        CommandType.Text,
                        @"SELECT Id,Name,CrDate FROM [" + this.Name + @"]..sysobjects WHERE    xtype = 'U' ORDER BY name
                    ").Tables[0];

                SqlObjectCollection<DatatableItem> all = new SqlObjectCollection<DatatableItem>();
                if (results.Rows.Count > 0)
                {
                    foreach (DataRow row in results.Rows)
                    {
                        DatatableItem item = new DatatableItem(this);
                        item.LoadFrom(row);
                        all.Add(item);
                    }
                }

                return all;
            }
        }

        public static DatabaseItem Empty
        {
            get
            {
                return new DatabaseItem();
            }
        }

        public DatatableItem CreateTable()
        {
            return new DatatableItem(this);
        }
    }
}
