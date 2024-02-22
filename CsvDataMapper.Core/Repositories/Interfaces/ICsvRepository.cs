using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Repositories.Interfaces
{
    internal interface ICsvRepository
    {
        Task<string> ReadCsvFileAsStringAsync();
        string ReadCsvFileAsString();
    }
}
