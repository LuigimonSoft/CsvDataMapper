using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNameAttribute: Attribute
    {
        public string ColumnName { get; }
        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
