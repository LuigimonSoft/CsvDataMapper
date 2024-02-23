using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvDataMapper.Core.Repositories.Interfaces;
using CsvDataMapper.Core.Utils;

namespace CsvDataMapper.Core.Repositories
{
    internal  class CsvRepository: ICsvRepository
    {
        private string _csvFilePath;
        private Encoding _encoding = Encoding.UTF8;

        public CsvRepository(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }
        public CsvRepository(string csvFilePath, Encoding? encoding) 
        {
            _csvFilePath = csvFilePath;
            _encoding = encoding?? Encoding.UTF8;
        }

        #region ReadCsvFileAsString
        public string ReadCsvFileAsString()
        {
            try
            {
                return File.ReadAllText(_csvFilePath, _encoding);
            }
            catch (FileNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileNotFound, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (Exception ex)
            {

                throw new CsvDataMapperException(ErrorCode.GeneralError, ex);
            }
        }

        public async Task<string> ReadCsvFileAsStringAsync()
        {
            try
            {
                return await File.ReadAllTextAsync(_csvFilePath, _encoding);
            }
            catch(FileNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileNotFound, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (Exception ex)
            {

                throw new CsvDataMapperException(ErrorCode.GeneralError,ex);
            }
        }
        #endregion

        #region ReadCsvFileInChunks
        public IEnumerable<string> ReadCsvFileInChunks(int chunkSize, long startPosition = 0)
        {
            var buffer = new char[chunkSize];

            using (var fileStream = File.OpenRead(_csvFilePath))
            {
                fileStream.Seek(startPosition, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(fileStream, _encoding))
                {
                    while (streamReader.Read(buffer, 0, buffer.Length) > 0)
                    {
                        yield return new string(buffer);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public async IAsyncEnumerable<string> ReadCsvFileInChunksAsync(int chunkSize, long startPosition = 0)
        {
            var buffer = new char[chunkSize];

            using (var fileStream = File.OpenRead(_csvFilePath))
            {
                fileStream.Seek(startPosition, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(fileStream, _encoding))
                {
                    while (await streamReader.ReadAsync(buffer, 0, buffer.Length) > 0)
                    {
                        yield return new string(buffer);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
            }
        }
        #endregion
    }
}
