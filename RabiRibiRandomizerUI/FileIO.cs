using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RabiRibiRandomizerUI
{
    class FileIO
    {
        /// <summary>
        /// Calls the Randomizer program with the given parameters.
        /// Returns the output of the randomizer program.
        /// </summary>
        /// <param name="parameters">
        /// A dictionary of named key-value parameters. e.g. "seed": 124 (converts to argument "-seed 124")
        /// </param>
        /// <param name="settings">
        /// A set of flags. e.g. "no-write" (converts to argument "--no-write")
        /// </param>
        /// <returns>
        /// output of the randomizer program.
        /// </returns>
        public static string CallRandomizer(Dictionary<string, object> parameters, HashSet<string> settings, String extraParams="")
        {
            string arguments =
                string.Join(" ", parameters.Select(pair => $"-{pair.Key} \"{pair.Value}\"")) + " " +
                string.Join(" ", settings.Select(setting => $"--{setting}")) + " " + extraParams;

            Process p = new Process();
            p.StartInfo.FileName = @"bin\randomizer.exe";
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                output = "An unexpected error has occured.\r\n\r\nOutput:\r\n";
                output += p.StandardError.ReadToEnd();
            }

            return output;
        }

        /// <summary>
        /// Reads a config file into a ConfigData object.
        /// </summary>
        public static ConfigData ReadConfig(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("File does not exist!");
            }

            string[] lines = File.ReadAllLines(configFilePath);

            // Remove file comments
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                int commentPosition = line.IndexOf("//");
                if (commentPosition == -1) continue;
                lines[i] = line.Substring(0, commentPosition);
            }

            // fix up the JSON file by removing the extra commas
            string data = string.Join(" ", lines);
            data = Regex.Replace(data, @",\s*]", "]"); // remove comma after last entry in each array
            data = Regex.Replace(data, @",\s*}", "}"); // remove comma after last entry in each dict
            data = "{" + data + "}"; // add start, end braces { }
            
            ConfigData parsedData = new JavaScriptSerializer().Deserialize<ConfigData>(data);
            return parsedData;
        }

        /// <summary>
        /// Writes to a config file using a ConfigData object.
        /// </summary>
        public static void WriteConfig(string configFilePath, ConfigData configData)
        {
            string data = new JavaScriptSerializer().Serialize(configData);
            data = data.Substring(1, data.Length - 2); // remove start, end braces { }
            data = FormatPrettyJson(data);
            data = data.Replace("}", ",\r\n}"); // add commas after the last entry in each dict
            data = data.Replace("]", ",\r\n]"); // add commas after the last entry in each array
            data = data.Replace("},", "},\r\n"); // add extra new line after each dict
            data = data.Replace("],", "],\r\n"); // add extra new line after each array

            File.WriteAllText(configFilePath, data);
        }

        // Code taken from here and modified:
        // https://stackoverflow.com/questions/5881204/how-to-set-formatting-with-javascriptserializer-when-json-serializing
        public static string FormatPrettyJson(string jsonString)
        {
            var stringBuilder = new StringBuilder();

            bool escaping = false;
            bool inQuotes = false;
            int indentation = 0;

            foreach (char character in jsonString)
            {
                if (escaping)
                {
                    escaping = false;
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character == '\\')
                    {
                        escaping = true;
                        stringBuilder.Append(character);
                    }
                    else if (character == '\"')
                    {
                        inQuotes = !inQuotes;
                        stringBuilder.Append(character);
                    }
                    else if (!inQuotes)
                    {
                        if (character == ',')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append(' ', 4*indentation);
                        }
                        else if (character == '[' || character == '{')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append(' ', 4*(++indentation));
                        }
                        else if (character == ']' || character == '}')
                        {
                            //stringBuilder.Append("\r\n");
                            stringBuilder.Append(' ', 4*(--indentation));
                            stringBuilder.Append(character);
                        }
                        else if (character == ':')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append(' ');
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// Representation of the data in a config file.
    /// </summary>
    public class ConfigData
    {
        public Dictionary<string, bool> settings;
        public string[] to_shuffle;
        public string[] must_be_reachable;
    }
}
