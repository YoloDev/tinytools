namespace yolodev.tinytools
{
    using System;
    using System.IO;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 2:
                    RunFileWatcher(args[0], TimeSpan.FromSeconds(uint.Parse(args[1])));
                    break;

                case 1:
                    RunFileWatcher(args[0]);
                    break;

                default:
                    ShowInfo();
                    break;
            }
        }

        private static TimeSpan timeout = TimeSpan.FromSeconds(5);
        private static DateTime? LastEvent;

        private static void ChangeFound()
        {
            LastEvent = DateTime.Now;
        }

        private static void Tick()
        {
            if (LastEvent.HasValue)
            {
                if (LastEvent < DateTime.Now - timeout)
                {
                    System.Environment.Exit(0);
                }
            }
        }

        private static void RunFileWatcher(string path, TimeSpan? timeout = null)
        {
            if (timeout.HasValue)
            {
                Program.timeout = timeout.Value;
            }

            var fileSystemWatcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            var timer = new Timer(a => Tick());
            timer.Change(0, 100);

            fileSystemWatcher.Renamed += (a, b) => ChangeFound();
            fileSystemWatcher.Deleted += (a, b) => ChangeFound();
            fileSystemWatcher.Changed += (a, b) => ChangeFound();
            fileSystemWatcher.Created += (a, b) => ChangeFound();

            Console.ReadKey(true);
        }

        private static void ShowInfo()
        {
            Console.WriteLine("Usage: WaitForFileChanges.exe {path to watch} {optional timeout}");
        }
    }
}