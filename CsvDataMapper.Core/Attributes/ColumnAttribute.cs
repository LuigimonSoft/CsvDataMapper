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

        public ColumnAttribute(string columnName, int order )
        {
            ColumnName = columnName;
            Order = order;
        }


        public ColumnAttribute(string columnName,  int position, int size )
        {
            ColumnName = columnName;
            Position = position;
            Size = size;
        }

        public ColumnAttribute(string columnName, int order ,int position, int size)
        {
            ColumnName = columnName;
            Position = position;
            Size = size;
            Order = order;
        }
    }
}
