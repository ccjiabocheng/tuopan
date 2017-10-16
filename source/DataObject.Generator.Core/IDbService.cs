using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    public interface IDbService<T>
    {

        bool Exists(string name);
        T Select(string name);
    }
}
