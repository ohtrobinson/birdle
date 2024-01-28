using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace birdle.Data;

public static class QuickConfig
{
    public static Dictionary<string, string> Parse(string text)
    {
        using StringReader reader = new StringReader(text);

        Dictionary<string, string> result = new Dictionary<string, string>();

        string line = "";
        int currentLine = 0;
        while ((line = reader.ReadLine()) != null)
        {
            currentLine++;
            
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            int equalsLocation = line.IndexOf('=');

            if (equalsLocation == -1)
                throw new Exception($"Malformed text at line {currentLine}. Expected '=', was not found.");

            string key = line[..equalsLocation].Trim().ToLower();
            string value = line[(equalsLocation + 1)..].Trim();
            
            result.Add(key, value);
        }

        return result;
    }

    public static string ToQuickConfig(Dictionary<string, string> values)
    {
        StringBuilder builder = new StringBuilder();

        foreach ((string key, string value) in values)
        {
            builder.AppendLine($"{key.ToLower()}={value}");
        }

        return builder.ToString();
    }
}