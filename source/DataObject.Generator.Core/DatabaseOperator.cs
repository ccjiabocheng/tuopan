using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core
{
    public class DatabaseOperator
    {
        private ICollection<DatabaseItem> all = null;
        public ICollection<DatabaseItem> All
        {
            get
            {
                if(all == null)
                {
                    all = new List<DatabaseItem>();
                }
                return all;
            }
        }
    }
}
