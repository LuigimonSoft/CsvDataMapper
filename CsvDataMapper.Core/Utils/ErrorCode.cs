﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataMapper.Core.Utils
{
    public enum ErrorCode
    {
        GeneralError = 1,
        FileNotFound = 100,
        FileAccessDenied = 101,
        InvalidCsvFormat = 200,
        ParsingError = 201,
    }
}
