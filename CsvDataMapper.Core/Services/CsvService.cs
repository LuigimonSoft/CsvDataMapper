using CsvDataMapper.Core.Attributes;
using CsvDataMapper.Core.Repositories.Interfaces;
using CsvDataMapper.Core.Services.Interfaces;
using CsvDataMapper.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections;

namespace CsvDataMapper.Core.Services
{
    internal class CsvService : ICsvService
    {
        private readonly ICsvRepository _csvRepository;
        private char _delimiter = ',';
        private bool _hasHeader = true;

        public CsvService(ICsvRepository csvRepository, char delimiter, bool hasHeader)
        {
            _csvRepository = csvRepository;
            _delimiter = delimiter;
            _hasHeader = hasHeader;
        }
        public IList<TModel> MapCsvToListModel<TModel>() where TModel : new()
        {
            var models = new List<TModel>();
            try
            {
                string? line = string.Empty;
                var properties = typeof(TModel).GetProperties();
                var columns = new List<CsvColumn>();

                if (_hasHeader)
                {
                    line = _csvRepository.ReadCsvFileLine();
                    if (line == null)
                        throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);

                    columns = CreateColumnsList(line, properties);
                }

                while ((line = _csvRepository.ReadCsvFileLine()) != null)
                {
                    models.Add(LineToModel<TModel>(line, columns));
                }
            }
            catch (CsvDataMapperException ex)
            {
                throw;
            }
            catch (FileNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileNotFound);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied);
            }
            catch (Exception ex)
            {
                throw new CsvDataMapperException(ErrorCode.GeneralError);
            }

            return models;
        }

        public async Task<IList<TModel>> MapCsvToListModelAsync<TModel>() where TModel : new()
        {
            var models = new List<TModel>();
            try
            {
                string? line = string.Empty;

                var properties = typeof(TModel).GetProperties();
                var columns = new List<CsvColumn>();

                if (_hasHeader)
                {
                    line = await _csvRepository.ReadCsvFileLineAsync();
                    if (line == null)
                        throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);

                    columns = CreateColumnsList(line, properties);
                }

                while ((line = await _csvRepository.ReadCsvFileLineAsync()) != null)
                {
                    models.Add(LineToModel<TModel>(line, columns));
                }
            }
            catch(CsvDataMapperException ex)
            {
                throw;
            }
            catch (FileNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileNotFound);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied);
            }
            catch (Exception ex)
            {
                throw new CsvDataMapperException(ErrorCode.GeneralError);
            }

            return models;
        }

        public List<Dictionary<string, string>> ReadCsvAsDynamic()
        {
            var result = new List<Dictionary<string, string>>();
            var headers = new List<string>();
            bool isFirstRow = true;
            string? line = string.Empty;

            if (_hasHeader)
            {
                line = _csvRepository.ReadCsvFileLine();
                if (String.IsNullOrEmpty(line))
                    throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);
                headers.AddRange(line.Split(_delimiter));
            }

            while ((line = _csvRepository.ReadCsvFileLine()) != null)
            {
                var columns = line.Split(_delimiter);

                var row = new Dictionary<string, string>();
                for (int i = 0; i < columns.Length; i++)
                {
                    var key = _hasHeader ? headers[i] : $"Column{i}";
                    row[key] = columns[i];
                }
                result.Add(row);
            }

            return result;
        }

        public async Task<List<Dictionary<string, string>>> ReadCsvAsDynamicAsync()
        {
            var result = new List<Dictionary<string, string>>();
            var headers = new List<string>();
            bool isFirstRow = true;
            string? line = string.Empty;

            if (_hasHeader)
            {
                line = await _csvRepository.ReadCsvFileLineAsync();
                if (String.IsNullOrEmpty(line))
                    throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);
                headers.AddRange(line.Split(_delimiter));
            }

            while ((line = await _csvRepository.ReadCsvFileLineAsync()) != null)
            {
                var columns = line.Split(_delimiter);

                var row = new Dictionary<string, string>();
                for (int i = 0; i < columns.Length; i++)
                {
                    var key = _hasHeader ? headers[i] : $"Column{i}";
                    row[key] = columns[i];
                }
                result.Add(row);
            }

            return result;
        }

        public bool WriteModelToCsv<TModel>(IEnumerable<TModel> models) where TModel : new()
        {
            return _csvRepository.WriteLines(WriteModel(models));
        }

        public async Task<bool> WriteModelToCsvAsync<TModel>(IEnumerable<TModel> models) where TModel : new()
        {
            return await _csvRepository.WriteLinesAsync(WriteModel(models));
        }

        private List<string> WriteModel<TModel>(IEnumerable<TModel> models) where TModel : new ()
        {
            var lines = new List<string>();

            var properties = typeof(TModel).GetProperties()
                .Select(p => new
                {
                    Property = p,
                    Order = p.GetCustomAttribute<ColumnAttribute>()?.Order ?? int.MaxValue,
                    ColumnName = p.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? p.Name
                })
                .OrderBy(p => p.Order)
                .ToList();

            var header = string.Join(",", properties.Select(p => p.ColumnName.Replace("\"", "\"\"")));
            lines.Add(header);


            foreach (var model in models)
            {
                var lineValues = properties.Select(prop =>
                {
                    var rawValue = prop.Property.GetValue(model)?.ToString() ?? "";
                    var escapedValue = rawValue.Replace("\"", "\"\"");
                    return (rawValue.Contains(",") || rawValue.Contains("\"")) ? $"\"{escapedValue}\"" : escapedValue;
                });

                lines.Add(string.Join(",", lineValues));
            }

            return lines;
        }
    
        private List<CsvColumn> CreateColumnsList(string line, PropertyInfo[] properties)
        {
            var columns = new List<CsvColumn>();
            string[] columnsHeaders = Array.Empty<string>();

            columnsHeaders = line.Split(_delimiter);

            foreach (var property in properties)
            {
                CsvColumn csvColumn = new CsvColumn();
                csvColumn.Name = property.Name;

                if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnAttribute)))
                {
                    var csvColumnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                    csvColumn.Name = csvColumnAttribute.ColumnName;

                    csvColumn.Index = csvColumnAttribute.Position ?? -1;
                    csvColumn.size = csvColumnAttribute.Size ?? -1;
                    csvColumn.Order = csvColumnAttribute.Order ?? -1;
                }

                csvColumn.PropertyInfo = property;
                columns.Add(csvColumn);
            }

            return columns;
        }

        private TModel LineToModel<TModel>(string line, List<CsvColumn> columns) where TModel : new()
        {
            var model = new TModel();
            string[] Datacolumns = new string[] { };
            if (line.Contains("[") && line.Contains("]"))
            {
                int iniIndex = 0;
                while (line.Contains("[") && line.Contains("]") && line.IndexOf("[", iniIndex) > 0)
                {
                    int ini = line.IndexOf("[", iniIndex);
                    int end = line.IndexOf("]", iniIndex);
                    string substring = line.Substring(ini, end - ini + 1);
                    substring = substring.Replace(",", "\uFFFF");
                    line = line.Replace(line.Substring(ini, end - ini + 1), substring);
                    iniIndex = end + 1;
                }

            }

            line = Regex.Replace(line, @"(?<!\\)" + _delimiter, "\uFFFE");
            Datacolumns = line.Split("\uFFFE");

            if (_hasHeader)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    bool isArray = false;
                    string value = string.Empty;

                    var column = columns[i];
                    if (column.Index != -1)
                    {
                        if (column.size > 0)
                            value = Datacolumns[i].Substring(column.Index, column.size);
                        else
                            value = Datacolumns[i];
                    }
                    else
                    {
                        if (Datacolumns[i].Contains("[") && Datacolumns[i].Contains("]"))
                        {
                            isArray = true;
                            Datacolumns[i] = Datacolumns[i].Replace("[", "");
                            Datacolumns[i] = Datacolumns[i].Replace("]", "");
                            value = Datacolumns[i];
                        }
                        else
                            value = Datacolumns[i];
                    }

                    value = Regex.Replace(value, @"(?<!\\)""", string.Empty);
                    value = value.Replace("\\,", ",").Replace("\\\\", "\\");

                    if (!isArray)
                        column.PropertyInfo.SetValue(model, Convert.ChangeType(value, column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                    else
                    {
                        Type typeArray = typeof(List<>).MakeGenericType(column.PropertyInfo.PropertyType.GenericTypeArguments);
                        IList array = (IList)Activator.CreateInstance(typeArray);

                        foreach (string item in value.Split("\uFFFF"))
                            array.Add(Convert.ChangeType(item, column.PropertyInfo.PropertyType.GenericTypeArguments[0]));

                        column.PropertyInfo.SetValue(model, array);
                    }
                }
            }
            else
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    column.PropertyInfo.SetValue(model, Convert.ChangeType(Datacolumns[i], column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                }
            }
            return model;
        }
    }
}
