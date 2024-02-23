using CsvDataMapper.Core.Repositories;
using CsvDataMapper.Core.Services;
using CsvDataMapper.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core
{
    public class CsvDataMapper : ICsvDataMapper
    {
        private ICsvService _csvService;
        public string FilePath { get; set; }
        public Encoding Encoding { get; set; }
        public bool HasHeader { get; set; }
        public char Delimiter { get; set; }
        public CsvDataMapper()
        {
            _csvService = new CsvService(new CsvRepository(FilePath,Encoding), Delimiter, HasHeader);
        }

        private void SetCsvService()
        {
            _csvService = new CsvService(new CsvRepository(FilePath, Encoding), Delimiter, HasHeader);
        }
        
        public IList<TModel> MapCsvToListModel<TModel>() where TModel : new()
        {
            SetCsvService();
            return _csvService.MapCsvToListModel<TModel>();
        }

        public async Task<IList<TModel>> MapCsvToListModelAsync<TModel>() where TModel : new()
        {
            SetCsvService();
            return await _csvService.MapCsvToListModelAsync<TModel>();
        }

        public List<Dictionary<string, string>> ReadCsvAsDynamic()
        {
            SetCsvService();
            return _csvService.ReadCsvAsDynamic();
        }

        public async Task<List<Dictionary<string, string>>> ReadCsvAsDynamicAsync()
        {
            SetCsvService();
            return await _csvService.ReadCsvAsDynamicAsync();
        }
    }
}
