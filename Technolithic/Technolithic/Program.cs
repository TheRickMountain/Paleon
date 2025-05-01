using System;
using System.IO;
using System.Threading;
namespace Technolithic
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
#if RELEASE
            try
            {
#endif
            using (var game = new Engine())
                    game.Run();
#if RELEASE
        }
            catch(Exception exception)
            {
                WriteExceptionLog(exception);
            }
#endif
        }

        public static void WriteExceptionLog(Exception exception)
        {
            SignalShutdown();
            DirectoryInfo worldDirectory = Directory.CreateDirectory(Engine.GetGameDirectory() + Path.DirectorySeparatorChar + "Logging");
            StreamWriter file =
                new StreamWriter(worldDirectory.FullName + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + "Crashlog.txt", true);

            file.WriteLine(exception.ToString());
            file.Close();
        }

        public static void WriteWarningLog(string warning)
        {
            DirectoryInfo worldDirectory = Directory.CreateDirectory(Engine.GetGameDirectory() + Path.DirectorySeparatorChar + "Warning");
            StreamWriter file =
                new StreamWriter(worldDirectory.FullName + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + "Warninglog.txt", true);

            file.WriteLine(warning);
            file.Close();
        }

        public static ManualResetEvent ShutdownEvent = new ManualResetEvent(false);

        public static void SignalShutdown()
        {
            ShutdownEvent.Set();
        }
    }
}
