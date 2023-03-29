using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class Settings
    {
        public static Dictionary<string, string> DefaultUserSettings { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> UserSettings { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> LanguagePack { get; set; } = new Dictionary<string, string>();
        public static int[] CollorMills { get; set; } = new int[4];
        public static int LanguageCode { get; set; } = 0;

        public static void LoadSettings()
        {
            string[][] data = LoadData("UserSettings.txt");
            for (int i = 0; i < data[0].Length; i++)
            {
                UserSettings.Add(data[0][i], data[1][i]);
            }

            switch (UserSettings["Language"])
            {
                case "English":
                    {
                        LanguageCode = 0;
                        LoadLanguagePack("LanguagePackEN.txt"); 
                    } 
                    break;
                case "Русский":
                    {
                        LanguageCode = 1;
                        LoadLanguagePack("LanguagePackRU.txt");
                    } 
                    break;
            }

            SetCollorMilles();
            SetPriorities();
            SetServiceMessages();

            DatabaseConnector.SetConnectionConfiguration();

            data = LoadData("DefaultUserSettings.txt");
            for (int i = 0; i < data[0].Length; i++)
            {
                DefaultUserSettings.Add(data[0][i], data[1][i]);
            }
        }

        public static void SaveSettings()
        {
            string[] keys = UserSettings.Keys.ToArray();
            string[] values = UserSettings.Values.ToArray();

            string data = "";
            for (int i = 0; i < UserSettings.Count; i++)
            {
                data += keys[i] + ": '" + values[i] + "';\n";
            }

            string filePath = Path.Combine(Path.GetTempPath() + "\\RarSFX2\\", "UserSettings.txt");
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(data);
            writer.Close();
        }

        private static string[][] LoadData(string fileName)
        {
            string filePath = Path.Combine(Path.GetTempPath() + "\\RarSFX2\\", fileName);
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            string[] keys = ExtractKeys(content);
            string[] values = ExtractValues(content);

            string[][] data = { keys, values };
            return data;
        }

        private static void LoadLanguagePack(string langFileName)
        {
            string[][] data = LoadData(langFileName);
            for (int i = 0; i < data[0].Length; i++)
            {
                LanguagePack.Add(data[0][i], data[1][i]);
            }
        }

        private static void SetCollorMilles()
        {
            CollorMills[0] = int.Parse(UserSettings["Yellow"]) * 60000;
            CollorMills[1] = int.Parse(UserSettings["Orange"]) * 60000;
            CollorMills[2] = int.Parse(UserSettings["Red"]) * 60000;
            CollorMills[3] = int.Parse(UserSettings["Gray"]) * 60000;
        }

        private static void SetPriorities()
        {
            string[] langCodes = { "EN", "RU" };
            List<string[]> langPriorities = new List<string[]>(langCodes.Length);

            for (int i = 0; i < langCodes.Length; i++)
            {
                string[][] data = LoadData("Priorities" + langCodes[i] + ".txt");
                langPriorities.Add(data[1]);
            }

            string[] temp = { langPriorities[0][0], langPriorities[1][0] };
            Priority.Little = temp;

            string[] temp1 = { langPriorities[0][1], langPriorities[1][1] };
            Priority.Low = temp1;

            string[] temp2 = { langPriorities[0][2], langPriorities[1][2] };
            Priority.Medium = temp2;

            string[] temp3 = { langPriorities[0][3], langPriorities[1][3] };
            Priority.High = temp3;

            string[] temp4 = { langPriorities[0][4], langPriorities[1][4] };
            Priority.Critical = temp4;

            string[][] priorities = { Priority.Little, Priority.Low, Priority.Medium, Priority.High, Priority.Critical };
            Priority.All = priorities;
        }

        private static void SetServiceMessages()
        {
            ServiceMessage.ApplicationLaunched = LanguagePack["ServiceApplicationLaunched"];
            ServiceMessage.TasksLoaded = LanguagePack["ServiceTasksLoaded"];
            ServiceMessage.TaskSaving = LanguagePack["ServiceTaskSaving"];
            ServiceMessage.TaskAdded = LanguagePack["ServiceTaskAdded"];
            ServiceMessage.TaskNotAdded = LanguagePack["ServiceTaskNotAdded"];
            ServiceMessage.TaskUpdated = LanguagePack["ServiceTaskUpdated"];
            ServiceMessage.TaskRollback = LanguagePack["ServiceTaskRollback"];
            ServiceMessage.TaskFinished = LanguagePack["ServiceTaskFinished"];
            ServiceMessage.TaskCancelled = LanguagePack["ServiceTaskCancelled"];
        }

        private static string[] ExtractKeys(string content)
        {
            List<string> keys = new List<string>();

            Regex regex1 = new Regex(@"\w+:");
            Regex regex2 = new Regex(@"\w+[^:]");

            MatchCollection matches = regex1.Matches(content);

            foreach (Match match in matches)
            {
                Match innerMatch = regex2.Match(match.Value);
                keys.Add(innerMatch.Value);
            }

            return keys.ToArray();
        }

        private static string[] ExtractValues(string content)
        {
            List<string> values = new List<string>();

            Regex regex1 = new Regex(@"\'.+");
            Regex regex2 = new Regex(@"[^\';\r\n]");

            MatchCollection matches = regex1.Matches(content);

            foreach (Match match in matches)
            {
                MatchCollection innerMatch = regex2.Matches(match.Value);

                string value = "";
                foreach (Match m in innerMatch)
                {
                    value += m.Value;
                }

                values.Add(value.ToString());
            }

            return values.ToArray();
        }
    }
}
