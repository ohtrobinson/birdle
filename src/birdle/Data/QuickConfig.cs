using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace birdle.Data;

public static class QuickConfig
{
    public static Dictionary<string, object> Parse(string text)
    {
        using StringReader reader = new StringReader(text);

        Dictionary<string, object> result = new Dictionary<string, object>();

        List<object> objectList = new List<object>();
        StringBuilder builderString = new StringBuilder();

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
            string valueStr = line[(equalsLocation + 1)..].Trim();

            bool inString = false;
            for (int i = 0; i < valueStr.Length; i++)
            {
                char c = valueStr[i];
                
                switch (c)
                {
                    case '"':
                        inString = !inString;
                        break;
                    
                    case ',' when !inString:
                        objectList.Add(builderString.ToString());
                        builderString.Clear();
                        break;
                    
                    default:
                        builderString.Append(c);
                        break;
                }
            }
            
            objectList.Add(builderString.ToString());
            builderString.Clear();

            for (int i = 0; i < objectList.Count; i++)
            {
                string value = (string) objectList[i];
                object v = null;

                if (double.TryParse(value, out double number))
                    v = number;
                else if (bool.TryParse(value, out bool boolean))
                    v = boolean;
                else
                    v = value;

                objectList[i] = v;
            }
            
            if (objectList.Count == 1)
                result.Add(key, objectList[0]);
            else
                result.Add(key, objectList.ToArray());
            
            objectList.Clear();
        }

        return result;
    }

    public static string ToQuickConfig(Dictionary<string, object> values)
    {
        StringBuilder builder = new StringBuilder();

        foreach ((string key, object value) in values)
        {
            builder.Append($"{key.ToLower()}=");
            
            AppendToStringBuilder(builder, value);

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static void AppendToStringBuilder(StringBuilder builder, object value)
    {
        switch (value)
        {
            case string sValue:
                builder.Append('"');
                builder.Append(sValue);
                builder.Append('"');
                break;
                
            case Enum eValue:
                builder.Append(eValue.ToString().ToLower());
                break;
                
            case byte:
            case sbyte:
            case short:
            case ushort:
            case int:
            case uint:
            case long:
            case ulong:
            case float:
            case double:
                builder.Append(value);
                break;
                
            case bool bValue:
                builder.Append(bValue ? "true" : "false");
                break;
                
            case Array aValue:
                for (int i = 0; i < aValue.Length; i++)
                {
                    AppendToStringBuilder(builder, aValue.GetValue(i));

                    if (i < aValue.Length - 1)
                        builder.Append(',');
                }

                break;
            
            default:
                throw new Exception($"Unsupported type \"{value.GetType()}\".");
        }
    }
}