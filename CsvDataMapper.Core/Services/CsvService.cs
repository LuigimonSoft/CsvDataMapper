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

                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumNameAttribute)))
                            csvColumn.Name = property.GetCustomAttribute<ColumNameAttribute>().ColumnName;
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
                    string[] Datacolumns = line.Split(_delimiter);

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
                var model = new TModel();
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

                        if (property.CustomAttributes.Any(x => x.AttributeType == typeof(ColumNameAttribute)))
                            csvColumn.Name = property.GetCustomAttribute<ColumNameAttribute>().ColumnName;
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
                    string[] Datacolumns = line.Split(_delimiter);

                    if(_hasHeader)
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

        
    }
}
