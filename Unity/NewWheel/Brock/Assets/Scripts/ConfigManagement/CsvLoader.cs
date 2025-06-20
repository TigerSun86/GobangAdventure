using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class CsvLoader
{
    public static List<T> LoadFromCSV<T>(TextAsset csvFile, ICsvRowParser<T> parser)
    {
        List<T> result = new List<T>();

        using StringReader reader = new StringReader(csvFile.text);
        string headerLine = reader.ReadLine();
        if (headerLine == null) return result;

        string[] headers = headerLine.Split(',');

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] values = line.Split(',');
            T item = parser.ParseRow(values, headers);
            result.Add(item);
        }

        return result;
    }
}
