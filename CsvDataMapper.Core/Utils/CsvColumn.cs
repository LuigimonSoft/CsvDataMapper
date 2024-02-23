using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Utils
{
    internal class CsvColumn
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public int size { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public CsvColumn()
        {
            Index = -1;
        }

    }
}
