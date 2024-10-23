using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Services.Interfaces
{
    public interface ICsvService
    {
        Task<IList<TModel>> MapCsvToListModelAsync<TModel>() where TModel : new();
        IList<TModel> MapCsvToListModel<TModel>() where TModel : new();
        Task<List<Dictionary<string, string>>> ReadCsvAsDynamicAsync();
        List<Dictionary<string, string>> ReadCsvAsDynamic();

        Task<bool> WriteModelToCsvAsync<TModel>(IEnumerable<TModel> models) where TModel : new();
        bool WriteModelToCsv<TModel>(IEnumerable<TModel> models) where TModel : new();
    }
}
