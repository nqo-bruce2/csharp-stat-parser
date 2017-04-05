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

            Run();         
        }

        public static void Run()
        {
            string[] args = Environment.GetCommandLineArgs();

            //TODO: at some point need to add logic to handle any existing files as this is only looking for new files

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
            
            // Only watch files ending in .log.
            // TODO: make this config driven. Not a priority anytime soon.
            watcher.Filter = "*.log";

            // Add event handlers.
            // Watches for newly created files or files that are renamed. 
            //Currently, KT creates the file as *.tmp and once all data is written, renames it to *.log. 
            // This triggers the Renamed event
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private static void OnRenamed(object sender, FileSystemEventArgs e)
        {
            logger.Info("\n\n-------------OnRenamed: Found file: " + e.FullPath + "--------------\n\n");
            DoWork(e);
        }


        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            logger.Info("\n\n-------------OnCreated: Found file: " + e.FullPath + "--------------\n\n");
            DoWork(e);
        }

        private static void DoWork(FileSystemEventArgs e)
        {
            // TODO: the type of stat parser should be config driven in case we want to parse different mods. 
            // All one needs to do is extend StatProcessorBase and implement their mod parser. 
            StatProcessorBase processor = new CrmodStatParser();
            processor.StartParsing(e.Name, e.FullPath);
        }
    }
}
