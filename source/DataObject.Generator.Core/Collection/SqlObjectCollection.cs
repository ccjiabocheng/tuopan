using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core.Collection
{
    public class SqlObjectCollection<T>: List<T>, ICollection<T> where T:ISqlObject
    {
        public System.Collections.ICollection DefaultCollection
        {
            get
            {
                return this ;
            }
        }
    }
}
