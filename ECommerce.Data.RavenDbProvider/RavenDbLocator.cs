using System;
using System.IO;
using System.Linq;
using Raven.TestDriver;

namespace ECommerce.Data.RavenDbProvider
{
    internal class RavenDbLocator : RavenServerLocator
    {
        private const string RavenServerName = "Raven.Server";

        private string _serverPath;
        private string _command = "dotnet";
        private string _arguments;

        public override string ServerPath
        {
            get
            {
                if (string.IsNullOrEmpty(_serverPath) == false)
                {
                    return _serverPath;
                }

                var path = Environment.GetEnvironmentVariable("Raven_Server_Test_Path");

                if (!string.IsNullOrEmpty(path) && InitializeFromPath(path)) return _serverPath;

                //If we got here we didn't have ENV:RavenServerTestPath setup for us maybe this is a CI enviroement
                path = Environment.GetEnvironmentVariable("RavenServerCIPath");

                if (!string.IsNullOrEmpty(path) && InitializeFromPath(path)) return _serverPath;

                //We couldn't find Raven.Server in either enviroment variables lets look for it in the current directory
                if (Directory.GetFiles(Environment.CurrentDirectory, $"{RavenServerName}.exe; {RavenServerName}.dll").Any(InitializeFromPath))
                {
                    return _serverPath;
                }

                //Lets try some brut force
                foreach (var file in Directory.GetFiles(Directory.GetDirectoryRoot(Environment.CurrentDirectory), $"{RavenServerName}.exe; {RavenServerName}.dll", SearchOption.AllDirectories))
                {
                    if (!InitializeFromPath(file)) continue;

                    try
                    {
                        //We don't want to override the variable if defined
                        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RavenServerTestPath")))
                            Environment.SetEnvironmentVariable("RavenServerTestPath", file);
                    }
                    //We might not have permissions to set the enviroment variable
                    catch
                    {
                        //
                    }

                    return _serverPath;
                }

                throw new FileNotFoundException($"Could not find {RavenServerName} anywhere on the device.");
            }
        }

        private bool InitializeFromPath(string path)
        {
            if (Path.GetFileNameWithoutExtension(path) != RavenServerName)
                return false;

            var ext = Path.GetExtension(path);

            switch (ext)
            {
                case ".dll":
                    _serverPath = path;
                    _arguments = _serverPath;
                    return true;
                case ".exe":
                    _serverPath = path;
                    _command = _serverPath;
                    _arguments = string.Empty;
                    return true;
            }

            return false;
        }

        public override string Command => _command;
        public override string CommandArguments => _arguments;
    }
}
