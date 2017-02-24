using System;
using System.IO;
using NLog;
using System.Configuration;
using Integration;
using System.Linq;


namespace stat_parser
{
    class Program
    {
        private static readonly string DirectoryToWatch = ConfigurationManager.AppSettings["UnprocessedDirectory"];
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("starting stat parser - looking for file...");
            //using (var db = new StatsDbContext())
            //{
            //    //var match = db.Match
            //    //    .Where(b => b.MatchId == "matchid")
            //    //    .FirstOrDefault();
            //    db.Match.Add(new Models.Match() { MatchId = "matchid", MatchType = "1v1", MapName = "dm3", MatchText = "lots of data", Date = DateTime.Now });
            //    db.SaveChanges();
            //}
            //logger.Info("starting stat parser - looking for file...");

            Run();         
        }

        public static void Run()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            /*if (args.Length != 2)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }*/
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            if (!Directory.Exists(DirectoryToWatch))
            {
                logger.Error("Directory not found: " + DirectoryToWatch);
                throw new DirectoryNotFoundException("Directory not found: " + DirectoryToWatch);
            }
            watcher.Path = DirectoryToWatch;

            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.log";

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
            logger.Info("Found file: " + e.FullPath);
            StatProcessor processor = new StatProcessor();
            processor.StartParsing(e.Name, e.FullPath);

        }
    }
}
