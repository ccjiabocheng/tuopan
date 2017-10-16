using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Generator.Core.Collection
{
    public class DataCellCollection : SqlObjectCollection<DataCellItem>, ITableService<DataCellItem>
    {
        public SqlObjectCollection<DataCellItem> Where(Predicate<DataCellItem> predicate)
        {
            SqlObjectCollection<DataCellItem> items = new SqlObjectCollection<DataCellItem>();
            foreach (var item in this)
            {
                if (predicate(item))
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
