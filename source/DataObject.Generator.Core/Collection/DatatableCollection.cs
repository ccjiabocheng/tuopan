using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core.Collection
{
    public class DatatableCollection: SqlObjectCollection<DatatableItem>, IDbService<DatatableItem>
    {
        public DatatableItem Select(string tbName)
        {
            if (!Exists(tbName))
            {
                return null;
            }
            return this.Find(x => { return x.Name.Equals(tbName); });
        }

        public bool Exists(string tbName)
        {
            if (string.IsNullOrEmpty(tbName))
            {
                return false;
            }
            if (this.Count == 0)
            {
                return false;
            }
            return this.Find(x => { return x.Name.Equals(tbName); }) != null;
        }
    }
}
