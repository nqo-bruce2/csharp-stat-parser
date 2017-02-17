using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApplication3.PlayerStatObjects;
using ConsoleApplication3.StatGroups;
using Newtonsoft.Json;

namespace ConsoleApplication3
{
    class Program
    {
        public static Statistics MStats;
        static void Main(string[] args)
        {
            Run();         
        }

        public static void Run()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            if (args.Length != 2)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = args[1];
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Created += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                MStats = new Statistics();
                MStats.MatchId = new Guid().ToString();
                //var text = File.ReadAllText("C:\\Users\\user\\Downloads\\match_stats_example.txt");
                var text = File.ReadAllText(e.FullPath);
                string pattern = "\r\n\r\n";
                string[] substrings = Regex.Split(text, pattern, RegexOptions.Singleline);
                ProcessStats(substrings[2]);
                ProcessQStats(substrings[3]);
                ProcessBadStats(substrings[4]);
                ProcessEfficiencyStats(substrings[5]);
                ProcessKStats(substrings[6]);
                var jsonString = JsonConvert.SerializeObject(MStats);
                Console.WriteLine(jsonString);
                //Do persistence, file temporarily
                var CurrentDir = Environment.CurrentDirectory;
                var ParentDir = Directory.GetParent(CurrentDir);
                File.WriteAllText(Environment.CurrentDirectory + "/" + MStats.MatchId + ".json", jsonString);
    
            }
            catch (Exception ex)
            {
                var CurrentDir = Environment.CurrentDirectory;
                var ParentDir = Directory.GetParent(CurrentDir);
                //File.WriteAllText(ParentDir + "/error/" + MStats.MatchId + ".json", jsonString);
                File.Move(e.FullPath, ParentDir + "/error/" + e.Name);
            }


        }
    
        public static void ProcessStats(String s)
        {
            MStats.Stats = new Stats();
            var statsData = s.Split(new string[] {"\r\n"}, StringSplitOptions.None);
            // Check if player count is uneven and throw exception if so.
            if (statsData.Length % 2 != 0)
                throw new Exception("Odd number of players. Rage quit must have occured. Is omi, bib, ck1, dave/pete playing?");
            // Determine match type by counting num of players. Work around until this can be done server side. 
            if (statsData.Length == 4)
                MStats.MatchType = "1v1";
            if (statsData.Length == 6)
                MStats.MatchType = "2v2";
            if (statsData.Length == 8)
                MStats.MatchType = "3v3";
            if (statsData.Length == 10)
                MStats.MatchType = "4v4";
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerStats p = new PlayerStats();
                p.Name = pdata[0].Trim();
                p.Team = pdata[1].Trim();
                p.K_Eff = pdata[2].Trim();
                p.W_Eff = pdata[3].Trim();
                MStats.Stats.Players.Add(p);
            }
        }

        public static void ProcessQStats(String s)
        {
            MStats.QStats = new QStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerQuadStats p = new PlayerQuadStats();
                p.Name = pdata[0].Trim();
                p.Quads = Convert.ToInt32(pdata[1].Trim());
                p.Q_eff = pdata[2].Trim();
                p.Q_E_K = Convert.ToInt32(pdata[3].Trim());
                p.Q_Self_K = Convert.ToInt32(pdata[4].Trim());
                p.Q_Team_K = Convert.ToInt32(pdata[5].Trim());
                MStats.QStats.Players.Add(p);
            }
        }

        private static void ProcessBadStats(string s)
        {
            MStats.BadStats = new BadStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerBadStats p = new PlayerBadStats();
                p.Name = pdata[0].Trim();
                p.Dropped_Paks = Convert.ToInt32(pdata[1].Trim());
                p.Self_Damage = pdata[2].Trim();
                p.Team_Damage = pdata[3].Trim();
                MStats.BadStats.Players.Add(p);
            }
        }

        private static void ProcessEfficiencyStats(string s)
        {
            MStats.EfficiencyStats = new EfficiencyStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerEfficiencyStats p = new PlayerEfficiencyStats();
                p.Name = pdata[0].Trim();
                p.Bull_Eff = pdata[1].Trim();
                p.Nails_Eff = pdata[2].Trim();
                p.Rocket_Eff = pdata[3].Trim();
                p.Lightning_Eff = pdata[4].Trim();
                p.Total_Eff = pdata[5].Trim();
                MStats.EfficiencyStats.Players.Add(p);
            }
        }

        private static void ProcessKStats(string s)
        {
            MStats.KStats = new KStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerKStats p = new PlayerKStats();
                p.Name = pdata[0].Trim();
                p.Frag_Count = Convert.ToInt32(pdata[1].Trim());
                p.E_Count = Convert.ToInt32(pdata[2].Trim());
                p.Self_Count = Convert.ToInt32(pdata[3].Trim());
                p.Team_count = Convert.ToInt32(pdata[4].Trim());
                p.K_count = Convert.ToInt32(pdata[5].Trim());
                MStats.KStats.Players.Add(p);
            }
        }
    }
}
