using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    using Collection;
    public interface ISqlObject
    {
        string Name { get; }

        System.Collections.ICollection Children { get; }

    }
}
