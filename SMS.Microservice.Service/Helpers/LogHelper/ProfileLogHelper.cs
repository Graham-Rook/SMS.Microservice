using SMS.Microservice.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Microservice.Service.Helpers.LogHelper
{
    public class ProfileLogHelper : ILogHelper
    {
        protected List<Profile> Profiles;
        public static object FileLocker = new object();

        public ProfileLogHelper(List<Profile> profiles)
        {
            try
            {

                Profiles = profiles;
                if (Profiles == null)
                    Profiles = new List<Profile>();

                // Validate profiles
                foreach (var profile in profiles)
                {
                    if (string.IsNullOrWhiteSpace(profile.Name))
                        throw new Exception("Invalid profile name.");

                    if (profiles.Count(x => x.Name.ToLower() == profile.Name.ToLower()) > 1)
                        throw new Exception("Profile names must be unique.");

                    foreach (var destination in profile.Destinations)
                    {
                        try
                        {
                            Path.GetFullPath(destination.Path);
                        }
                        catch
                        {
                            throw new Exception("Invalid Path for File destination.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogDesperate(ex);
                throw;
            }
        }

        public void LogMessage(params string[] messages)
        {
            LogMessage(new Options(), messages);
        }

        public void LogException(Exception ex, Options options = null)
        {
            LogMessage(options, ex.Message, "; ", ex.StackTrace);
        }

        public void LogMessage(Options options, params string[] messages)
        {
            Task.Run(() =>
            {
                // Defaults
                if (options == null)
                    options = new Options();

                // Get the profile
                var profile = Profiles.FirstOrDefault(x => x.Name.ToLower() == options.ProfileName.ToLower());

                if (profile == null)
                    return;

                // Check which type
                foreach (var destination in profile.Destinations)
                {
                    switch (destination.Type.ToLower())
                    {
                        case "file":
                            LogMessageFile(destination, options, messages);
                            break;
                    }
                }
            });
        }

        public static void LogDesperate(Exception ex)
        {
            try
            {
                if (!Directory.Exists("c:\\temp"))
                    Directory.CreateDirectory("c:\\temp");

                var filename = "c:\\temp\\" + Guid.NewGuid() + ".txt";

                File.AppendAllText(filename, ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch
            {
                // End of the road
            }
        }

        private void LogMessageFile(Destination destination, Options options, params string[] messages)
        {
            var now = DateTime.Now;

            if (options.Filename == null)
                options.Filename = now.ToString("yyyy-MM-dd") + ".txt";

            var path = destination.Path;
            if (!path.EndsWith(@"\"))
                path += @"\";

            path += options.Filename;

            var directoryPart = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPart))
                Directory.CreateDirectory(directoryPart);

            lock (FileLocker)
            {
                using (var writer = new StreamWriter(path, true))
                {
                    if (options.PrefixMessageWithTimestamp)
                        writer.Write(now.ToString("HH:mm:ss.fff") + "> ");

                    foreach (var message in messages)
                        writer.Write(message);
                    writer.Write(Environment.NewLine);
                }
            }
        }
    }
}
