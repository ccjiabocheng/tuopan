using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core.Collection
{
    public class DatabaseCollection: SqlObjectCollection<DatabaseItem>, IDbService<DatabaseItem> 
    {
        public DatabaseItem Select(string dbName)
        {
            if (!Exists(dbName))
            {
                return DatabaseItem.Empty;
            }
            return this.Find(x => { return x.Name.Equals(dbName); });
        }

        public bool Exists(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                return false;
            }
            if (this.Count == 0)
            {
                return false;
            }
            return this.Find(x => { return x.Name.Equals(dbName); }) != null;
        }
    }
}
