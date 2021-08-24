﻿using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Backend.NLogSetup.DataDogHQ
{
    internal static class StringExtensions
    {
        public static object ToSystemType(this string field, Type type, IFormatProvider formatProvider,
            JsonSerializer jsonSerializer)
        {
            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(field, formatProvider);
                case "System.Double":
                    return Convert.ToDouble(field, formatProvider);
                case "System.DateTime":
                    return Convert.ToDateTime(field, formatProvider);
                case "System.Int32":
                    return Convert.ToInt32(field, formatProvider);
                case "System.Int64":
                    return Convert.ToInt64(field, formatProvider);
                case "System.Object":
                    using (var reader = new JsonTextReader(new StringReader(field)))
                    {
                        return ((ExpandoObject) jsonSerializer.Deserialize(reader, typeof(ExpandoObject)))
                            .ReplaceDotInKeys(false);
                    }
                default:
                    return field;
            }
        }
    }
}