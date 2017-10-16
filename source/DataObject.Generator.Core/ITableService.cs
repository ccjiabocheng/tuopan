using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    using Collection;
    public interface ITableService<T> where T:ISqlObject
    {
        SqlObjectCollection<T> Where(Predicate<T> predicate);
    }

}
