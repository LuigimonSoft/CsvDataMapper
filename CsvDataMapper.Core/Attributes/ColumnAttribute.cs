using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute: Attribute
    {
        public string? ColumnName { get; }
        public int? Position { get; }
        public int? Size { get; }
        public int? Order { get; }

        public ColumnAttribute() { }
        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
