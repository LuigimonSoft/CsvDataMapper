using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumNameAttribute: Attribute
    {
        public string ColumnName { get; }
        public ColumNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
