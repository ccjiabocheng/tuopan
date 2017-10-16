using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    using System.Data;
    using Collection;
    using Comm;
    public class DatatableItem: ISqlObject
    {
        private DatabaseItem _owner = null;

        public DatatableItem(DatabaseItem owner)
        {
            _owner = owner;
        }

        public long Id { set; get; }

        public string Name { set; get; }

        public DatabaseItem Owner
        {
            get
            {
                return _owner;
            }
        }

        public DateTime CreateTime { set; get; }

        public void LoadFrom(DataRow row)
        {
            if (row == null) return;

            if (row["id"] != null)
            {
                Id = Convert.ToInt64(row["id"]);
            }
            if (row["name"] != null)
            {
                Name = Convert.ToString(row["name"]);
            }
            if (row["crdate"] != null)
            {
                CreateTime = Convert.ToDateTime(row["crdate"]);
            }
        }

        public System.Collections.ICollection Children
        {
            get
            {
                return Cells.DefaultCollection;
            }
        }

        public SqlObjectCollection<DataCellItem> Cells
        {
            get
            {
                #region SQL
                string sql = @"
                USE [" + Owner.Name + @"]
                SELECT FieldName = Rtrim(b.name)
                       ,PK = CASE 
                               WHEN h.id IS NOT NULL THEN 'PK'
                               ELSE ''
                             END
                       ,FieldType = Type_name(b.xusertype)
	                   ,IsIdentity = ''
                                + CASE 
                                    WHEN b.colstat & 1 = 1 THEN ''
                                        + CONVERT(VARCHAR,Ident_seed(a.name))
                                        + ','
                                        + CONVERT(VARCHAR,Ident_incr(a.name))
                                        + ''
                                    ELSE ''
                                  END
                       ,MaxLength = b.length
                       ,AllowNull = CASE b.isnullable 
                                WHEN 0 THEN 'N'
                                ELSE 'Y'
                              END
                       ,DefaultValue = Isnull(e.TEXT,'')
                       ,Description = Isnull(c.VALUE,'')
                FROM     sysobjects a
                         INNER JOIN  sys.all_objects aa
                           ON a.id=aa.object_id 
                              AND  schema_name(schema_id)='dbo'
                         ,syscolumns b
                         LEFT OUTER JOIN sys.extended_properties c
                           ON b.id = c.major_id
                              AND b.colid = c.minor_id
                         LEFT OUTER JOIN syscomments e
                           ON b.cdefault = e.id
                         LEFT OUTER JOIN (SELECT g.id
                                                 ,g.colid
                                          FROM   sysindexes f
                                                 ,sysindexkeys g
                                          WHERE  (f.id = g.id)
                                                 AND (f.indid = g.indid)
                                                 AND (f.indid > 0)
                                                 AND (f.indid < 255)
                                                 AND (f.status & 2048) <> 0) h
                           ON (b.id = h.id)
                              AND (b.colid = h.colid)
                WHERE    (a.id = b.id)
                         AND (a.id = Object_id('" + this.Name + @"'))  --要查询的表改成你要查询表的名称
                ORDER BY b.colid
            ";
                #endregion

                DataTable results =
                    SqlHelper.ExecuteDataset(
                        SqlHelper.GetConnection(),
                        CommandType.Text, sql).Tables[0];

                SqlObjectCollection<DataCellItem> all = new SqlObjectCollection<DataCellItem>();
                if (results.Rows.Count > 0)
                {
                    foreach (DataRow row in results.Rows)
                    {
                        DataCellItem item = new DataCellItem(this);
                        item.LoadFrom(row);
                        all.Add(item);
                    }
                }

                return all;
            }
        }
    }
}
