using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Services.Interfaces
{
    internal interface ICsvService
    {
        Task<IList<TModel>> MapCsvToListModelAsync<TModel>() where TModel : new();
        IList<TModel> MapCsvToListModel<TModel>() where TModel : new();
    }
}
