using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Utils
{
    public class CsvDataMapperException: Exception
    {
        public ErrorCode ErrorCode { get; }
        private static readonly ResourceManager _resourceManager = new ResourceManager("CsvDataMapper.Core.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);

        public CsvDataMapperException(ErrorCode errorCode) : base(GetErrorMessage(errorCode))
        {
            ErrorCode = errorCode;
        }

        public CsvDataMapperException(ErrorCode errorCode, Exception innerException) : base(GetErrorMessage(errorCode), innerException)
        {
            ErrorCode = errorCode;
        }

        private static string GetErrorMessage(ErrorCode errorCode)
        {
            return _resourceManager.GetString(errorCode.ToString(), CultureInfo.CurrentCulture) ?? "Unknown error.";
        }
    }
}
