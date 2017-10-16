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
    public class DBEngine
    {
        public static SqlObjectCollection<DatabaseItem> All()
        {
            return new DatabaseItem().All();
        }
    }
}
