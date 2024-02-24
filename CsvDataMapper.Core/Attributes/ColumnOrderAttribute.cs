using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnOrderAttribute : Attribute
    {
        public int Order { get; }
        public ColumnOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
