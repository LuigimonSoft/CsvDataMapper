using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Repositories.Interfaces
{
    public interface ICsvRepository
    {
        Task<string> ReadCsvFileAsStringAsync();
        string ReadCsvFileAsString();
        IAsyncEnumerable<string> ReadCsvFileInChunksAsync(int chunkSize, long startPosition = 0);
        IEnumerable<string> ReadCsvFileInChunks(int chunkSize, long startPosition = 0);
        Task<string> ReadAllCsvFileLineByLineAsync();
        string ReadAllCsvFileLineByLine();
        Task<string?> ReadCsvFileLineAsync();
        string? ReadCsvFileLine();
        Task<bool> WriteLinesAsync(IEnumerable<string> lines);
        bool WriteLines(IEnumerable<string> lines);
    }
}
