using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnPositionAttribute : Attribute
    {
        public int StartPosition { get; }
        public int Size { get; }
        public ColumnPositionAttribute(int startPosition, int size)
        {
            StartPosition = startPosition;
            Size = size;
        }
    }
}
