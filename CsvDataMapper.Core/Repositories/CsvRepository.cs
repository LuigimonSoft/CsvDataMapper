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
        private StreamReader _streamReader;

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

        #region ReadCsvFileLine
        public string ReadAllCsvFileLineByLine()
        {
            try
            {
                using (var fileStream = File.OpenRead(_csvFilePath))
                using (var streamReader = new StreamReader(fileStream, _encoding))
                {
                    string? line;
                    StringBuilder lines = new StringBuilder();
                    while ((line = streamReader.ReadLine()) != null)
                        lines.AppendLine(line);
                    return lines.ToString();
                }
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

        public async Task<string> ReadAllCsvFileLineByLineAsync()
        {
            try
            {
                using (var fileStream = File.OpenRead(_csvFilePath))
                using (var streamReader = new StreamReader(fileStream, _encoding))
                {
                    string? line;
                    StringBuilder lines = new StringBuilder();
                    while ((line = await streamReader.ReadLineAsync()) != null)
                        lines.AppendLine(line);
                    return lines.ToString();
                }
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

        public async Task<string?> ReadCsvFileLineAsync()
        {
            try
            {
                if (_streamReader == null)
                {
                    _streamReader?.Dispose(); 
                    _streamReader = new StreamReader(_csvFilePath, _encoding);
                }

                if (_streamReader.EndOfStream)
                {
                    DisposeStreamReader();
                    return null;
                }

                return await _streamReader.ReadLineAsync();
            }
            catch (FileNotFoundException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.FileNotFound, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (IOException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.IOError, ex);
            }
            catch (Exception ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.GeneralError, ex);
            }
        }

        public string? ReadCsvFileLine()
        {
            try
            {
                if (_streamReader == null)
                {
                    _streamReader?.Dispose();
                    _streamReader = new StreamReader(_csvFilePath, _encoding);
                }

                if (_streamReader.EndOfStream)
                {
                    DisposeStreamReader();
                    return null;
                }

                return _streamReader.ReadLine();
            }
            catch (FileNotFoundException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.FileNotFound, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (IOException ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.IOError, ex);
            }
            catch (Exception ex)
            {
                DisposeStreamReader();
                throw new CsvDataMapperException(ErrorCode.GeneralError, ex);
            }
        }

        private void DisposeStreamReader()
        {
            _streamReader?.Dispose();
            _streamReader = null;
        }

        #endregion

        #region WriteCsvFile
        public async Task<bool> WriteLinesAsync(IEnumerable<string> lines)
        {
            try
            {
                using (var streamWriter = new StreamWriter(_csvFilePath, append: false, _encoding))
                {
                    foreach (var line in lines)
                        await streamWriter.WriteLineAsync(line);
                    return true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.DirectoryNotFound, ex);
            }
            catch (IOException ex)
            {
                throw new CsvDataMapperException(ErrorCode.IOErrorWriter, ex);
            }
            catch (Exception ex)
            {
                throw new CsvDataMapperException(ErrorCode.GeneralError, ex);
            }

        }

        public bool WriteLines(IEnumerable<string> lines)
        {
            try
            {
                using (var streamWriter = new StreamWriter(_csvFilePath, append: false, _encoding))
                {
                    foreach (var line in lines)
                        streamWriter.WriteLine(line);
                    return true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CsvDataMapperException(ErrorCode.FileAccessDenied, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new CsvDataMapperException(ErrorCode.DirectoryNotFound, ex);
            }
            catch (IOException ex)
            {
                throw new CsvDataMapperException(ErrorCode.IOErrorWriter, ex);
            }
            catch (Exception ex)
            {
                throw new CsvDataMapperException(ErrorCode.GeneralError, ex);
            }
        }
        #endregion
    }
}
