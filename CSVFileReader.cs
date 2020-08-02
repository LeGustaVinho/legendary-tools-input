using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LegendaryTools.Input
{
    public class CSVFileReader
    {
        private static readonly string LINE_BREAK = "\n";
        private static readonly string ESCAPED_LINE_BREAK = "\\n";

        private static readonly string QUOTE = "\"";
        private static readonly string DOUBLE_QUOTE = "\"\"";

        private readonly byte[] buffer;

        private readonly char fieldDelimiter = ',';
        private readonly char textDelimiter = '"';
        private int offset;

        public CSVFileReader(byte[] bytes)
        {
            buffer = bytes;
        }

        public CSVFileReader(byte[] bytes, char fieldDelimiter, char textDelimiter) : this(bytes)
        {
            this.fieldDelimiter = fieldDelimiter;
            this.textDelimiter = textDelimiter;
        }

        public CSVFileReader(TextAsset asset) : this(asset.bytes)
        {
        }

        public CSVFileReader(TextAsset asset, char fieldDelimiter, char textDelimiter) : this(asset.bytes,
            fieldDelimiter, textDelimiter)
        {
        }

        /// <summary>
        /// Whether the buffer is readable.
        /// </summary>

        private bool canRead => buffer != null && offset < buffer.Length;

        /// <summary>
        /// Read a single line from the buffer.
        /// </summary>
        private static string ReadLine(byte[] buffer, int start, int count)
        {
            return Encoding.UTF8.GetString(buffer, start, count);
        }

        /// <summary>
        /// Read a single line from the buffer.
        /// </summary>
        public string ReadLine(bool skipEmptyLines = true)
        {
            int max = buffer.Length;

            // Skip empty characters
            if (skipEmptyLines)
            {
                while (offset < max && buffer[offset] < 32)
                {
                    ++offset;
                }
            }

            int end = offset;

            if (end < max)
            {
                for (;;)
                {
                    if (end < max)
                    {
                        int ch = buffer[end++];
                        if (ch != '\n' && ch != '\r')
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ++end;
                    }

                    string line = ReadLine(buffer, offset, end - offset - 1);
                    offset = end;
                    return line;
                }
            }

            offset = max;
            return null;
        }

        /// <summary>
        /// Read a single line of Comma-Separated Values from the file.
        /// </summary>
        public string[] ReadLineCSV()
        {
            List<string> temp = new List<string>();
            string line = string.Empty;
            bool insideQuotes = false;
            int wordStart = 0;

            while (canRead)
            {
                if (insideQuotes)
                {
                    string s = ReadLine(false);
                    if (s == null)
                    {
                        return null;
                    }

                    s = s.Replace(ESCAPED_LINE_BREAK, LINE_BREAK);
                    line += LINE_BREAK + s;
                }
                else
                {
                    line = ReadLine(true);
                    if (line == null)
                    {
                        return null;
                    }

                    line = line.Replace(ESCAPED_LINE_BREAK, LINE_BREAK);
                    wordStart = 0;
                }

                for (int i = wordStart, imax = line.Length; i < imax; ++i)
                {
                    char ch = line[i];

                    if (ch == ',')
                    {
                        if (!insideQuotes)
                        {
                            temp.Add(line.Substring(wordStart, i - wordStart));
                            wordStart = i + 1;
                        }
                    }
                    else if (ch == textDelimiter)
                    {
                        if (insideQuotes)
                        {
                            if (i + 1 >= imax)
                            {
                                temp.Add(line.Substring(wordStart, i - wordStart).Replace(DOUBLE_QUOTE, QUOTE));
                                return temp.ToArray();
                            }

                            if (line[i + 1] != textDelimiter)
                            {
                                temp.Add(line.Substring(wordStart, i - wordStart).Replace(DOUBLE_QUOTE, QUOTE));
                                insideQuotes = false;

                                if (line[i + 1] == fieldDelimiter)
                                {
                                    ++i;
                                    wordStart = i + 1;
                                }
                            }
                            else
                            {
                                ++i;
                            }
                        }
                        else
                        {
                            wordStart = i + 1;
                            insideQuotes = true;
                        }
                    }
                }

                if (wordStart < line.Length)
                {
                    if (insideQuotes)
                    {
                        continue;
                    }

                    temp.Add(line.Substring(wordStart, line.Length - wordStart));
                }

                return temp.ToArray();
            }

            return null;
        }

        public Dictionary<string, Dictionary<string, string>> ReadAllCSV()
        {
            Dictionary<string, Dictionary<string, string>>
                result = new Dictionary<string, Dictionary<string, string>>();
            string[] currentLine = null;
            string[] languages = null;
            int i = 0;
            for (;;)
            {
                currentLine = ReadLineCSV();
                if (currentLine == null || currentLine.Length == 0)
                {
                    break;
                }

                for (int j = 1; j < currentLine.Length; j++)
                {
                    if (i == 0)
                    {
                        languages = currentLine;
                        result.Add(languages[j], new Dictionary<string, string>());
                    }
                    else
                    {
                        result[languages[j]].Add(currentLine[0], currentLine[j]);
                    }
                }

                i++;
            }

            return result;
        }
    }
}