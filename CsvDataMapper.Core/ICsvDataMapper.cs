using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core
{
    public interface ICsvDataMapper
    {
        Task<IList<TModel>> MapCsvToListModelAsync<TModel>() where TModel : new();
        IList<TModel> MapCsvToListModel<TModel>() where TModel : new();
        Task<List<Dictionary<string, string>>> ReadCsvAsDynamicAsync();
        List<Dictionary<string, string>> ReadCsvAsDynamic();
    }
}
