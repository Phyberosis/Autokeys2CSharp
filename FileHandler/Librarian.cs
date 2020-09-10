using Data;
using Recordings;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace FileHandler
{
    public class Librarian
    {
        public struct SaveData
        {
            public Recording Recording;
            public Settings Settings;
            public string FileName;
        }

        public static Librarian I { get; private set; } = new Librarian();

        public string Path { get; private set; }

        private const string EXT = ".aks";
        private const string PREFS = "Preferences\\settings.txt";

        private Librarian() 
        {
            Path = AppDomain.CurrentDomain.BaseDirectory+"Saves\\";
            Directory.CreateDirectory(Path);
        }

        public bool IsPrefsExist()
        {
            return File.Exists(Path + PREFS);
        }

        public void SavePrefs()
        {
            File.WriteAllText(Path + PREFS, "");
        }

        public string CompileName(string name)
        {
            return name + EXT;
        }

        public void Save(Recording r, Settings s, Stream fs)
        {
            var sw = new StreamWriter(fs);
            sw.WriteLine(s.ToString());
            sw.WriteLine(r.ToString());
            sw.Close();
        }

        public SaveData Load(string name, Stream fs)
        {
            var data = new SaveData();
            name = name.Replace(".aks", "");
            data.FileName = name;

            var sr = new StreamReader(fs);
            data.Settings = new Settings(sr);
            data.Recording = new Recording(sr);

            return data;
        }
    }
}
