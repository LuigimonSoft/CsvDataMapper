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
                string[] columnsHeaders = Array.Empty<string>();
                var model = new TModel();
                var properties = typeof(TModel).GetProperties();
                var columns = new List<CsvColumn>();

                if (_hasHeader)
                {
                    line = _csvRepository.ReadCsvFileLine();
                    if (line == null)
                        throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);

                    columnsHeaders = line.Split(_delimiter);

                    foreach (var property in properties)
                    {
                        CsvColumn csvColumn = new CsvColumn();
                        csvColumn.Name = property.Name;

                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnNameAttribute)))
                            csvColumn.Name = property.GetCustomAttribute<ColumnNameAttribute>().ColumnName;
                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnPositionAttribute)))
                        {
                            csvColumn.Index = property.GetCustomAttribute<ColumnPositionAttribute>().StartPosition;
                            csvColumn.size = property.GetCustomAttribute<ColumnPositionAttribute>().Size;
                        }

                        csvColumn.PropertyInfo = property;
                        columns.Add(csvColumn);
                    }
                }

                while ((line = _csvRepository.ReadCsvFileLine()) != null)
                {
                    if (line.Contains("[") && line.Contains("]") )
                    {
                        int ini = line.IndexOf("[");
                        int end = line.IndexOf("]");
                        string substring = line.Substring(ini, end - ini + 1);
                        substring = substring.Replace(",", "|");
                        line = line.Replace(line.Substring(ini, end - ini + 1), substring);
                    }
                    string[] Datacolumns = line.Split(_delimiter);
                    if(Datacolumns.Contains("[") && Datacolumns.Contains("]"))
                    {
                        for (int i = 0; i < Datacolumns.Length; i++)
                        {
                            if (Datacolumns[i].Contains("[") && Datacolumns[i].Contains("]"))
                            {
                                int ini = Datacolumns[i].IndexOf("[");
                                int end = Datacolumns[i].IndexOf("]");
                                string substring = Datacolumns[i].Substring(ini, end - ini + 1);
                                substring = substring.Replace("|", ",");
                                Datacolumns[i] = Datacolumns[i].Replace(Datacolumns[i].Substring(ini, end - ini + 1), substring);
                            }
                        }
                    }

                    if (_hasHeader)
                    {
                        for (int i = 0; i < columns.Count; i++)
                        {
                            var column = columns[i];
                            if (column.Index != -1)
                            {
                                if (column.size > 0)
                                {
                                    string value = line.Substring(column.Index, column.size);
                                    column.PropertyInfo.SetValue(model, Convert.ChangeType(value, column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    column.PropertyInfo.SetValue(model, Convert.ChangeType(Datacolumns[i], column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                                }
                            }
                            else
                            {
                                column.PropertyInfo.SetValue(model, Convert.ChangeType(Datacolumns[i], column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
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



                    models.Add(model);
                }
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
                string[] columnsHeaders = Array.Empty<string>();
                
                var properties = typeof(TModel).GetProperties();
                var columns = new List<CsvColumn>();

                if (_hasHeader)
                {
                    line = await _csvRepository.ReadCsvFileLineAsync();
                    if(line == null)
                        throw new CsvDataMapperException(ErrorCode.InvalidCsvFormat);

                    columnsHeaders = line.Split(_delimiter);

                    foreach (var property in properties)
                    {
                        CsvColumn csvColumn = new CsvColumn();
                        csvColumn.Name = property.Name;

                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnNameAttribute)))
                            csvColumn.Name = property.GetCustomAttribute<ColumnNameAttribute>().ColumnName;
                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnPositionAttribute)))
                        {
                            csvColumn.Index = property.GetCustomAttribute<ColumnPositionAttribute>().StartPosition;
                            csvColumn.size = property.GetCustomAttribute<ColumnPositionAttribute>().Size;
                        }

                        csvColumn.PropertyInfo = property;
                        columns.Add(csvColumn);
                    }
                }

                while ((line = await _csvRepository.ReadCsvFileLineAsync()) != null)
                {
                    var model = new TModel();
                    string[] Datacolumns =new string[] { };
                    if (line.Contains("[") && line.Contains("]"))
                    {
                        int ini = line.IndexOf("[");
                        int end = line.IndexOf("]");
                        string substring = line.Substring(ini, end - ini + 1);
                        substring = substring.Replace(",", "|");
                        line = line.Replace(line.Substring(ini, end - ini + 1), substring);

                    }
                    
                    Datacolumns = line.Split(_delimiter);

                    if (_hasHeader)
                    {
                        for (int i = 0; i < columns.Count; i++)
                        {
                            var column = columns[i];
                            if (column.Index != -1)
                            {
                                if (column.size > 0)
                                {
                                    string value = Datacolumns[i].Substring(column.Index, column.size);
                                    column.PropertyInfo.SetValue(model, Convert.ChangeType(value, column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    column.PropertyInfo.SetValue(model, Convert.ChangeType(Datacolumns[i], column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
                                }
                            }
                            else
                            {
                                if (Datacolumns[i].Contains("[") && Datacolumns[i].Contains("]"))
                                {
                                    Datacolumns[i] = Datacolumns[i].Replace("[", "");
                                    Datacolumns[i] = Datacolumns[i].Replace("]", "").Replace("\"","");
                                    var dataArray = Datacolumns[i].Trim().Count() > 0 ? Datacolumns[i].Split("|").ToList() : new List<string>();
                                    column.PropertyInfo.SetValue(model, dataArray);
                                }
                                else
                                    column.PropertyInfo.SetValue(model, Convert.ChangeType(Datacolumns[i], column.PropertyInfo.PropertyType, CultureInfo.InvariantCulture));
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
                        
                    
                    
                    models.Add(model);
                }
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
                if(String.IsNullOrEmpty(line))
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
    }
}
