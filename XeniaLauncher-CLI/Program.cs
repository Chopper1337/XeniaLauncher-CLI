using System.Net;

namespace XeniaLauncher_CLI
{
    internal class Program
    {

        static void Main(string[] args)
        {
            XeniaBuild master = new XeniaBuild("Master",
                "https://github.com/xenia-project/release-builds-windows/releases/latest/download/xenia_master.zip",
                 "XeniaMaster",
                 "xenia", "xenia_master");
            XeniaBuild canary = new XeniaBuild("Canary",
                "https://github.com/xenia-canary/xenia-canary/releases/latest/download/xenia_canary.zip",
                 "XeniaCanary",
                 "xenia_canary", "xenia_canary");

            List<XeniaBuild> Builds = new List<XeniaBuild>
            {
                master,
                canary
            };

            List<string> validVerbs = new List<string>
            {
                "update",
                "u",
                "run",
                "r",
                "help",
                "h"

            };

            XeniaBuild selectedBuild = null;

            if (args.Length < 2)
            {
                insufficientArgs();
                help();
                return;
            }

            if (!validVerbs.Contains(args[0]))
            {
                Console.WriteLine($"Invalid argument: {args[0]}.");
                return;
            }

            foreach (XeniaBuild xeniaBuild in Builds)
            {
                if (xeniaBuild.Name.ToLower().Contains(args[1]))
                {
                    selectedBuild = xeniaBuild;
                    break;
                }
            }

            if (selectedBuild == null)
            {
                Console.WriteLine($"Invalid argument: {args[1]}.");
                return;
            }

            if (args[0].StartsWith('u'))
            {
                update(selectedBuild);
            }
            else if (args[0].StartsWith('r'))
            {
                run(selectedBuild);
            }
            else if (args[0].StartsWith('h'))
            {
                help();
            }
            else
            {
                insufficientArgs();
                help();
            }



            void insufficientArgs()
            {
                Console.WriteLine("Insufficient number of arguments.");
            }

            void help()
            {
                Console.WriteLine("XeniaUpdater-CLI requires two arguments.\n");
                Console.WriteLine("Valid first arguments:");
                validVerbs.ForEach(x => Console.WriteLine("\t" + x));
                Console.WriteLine("\nValid second arguments:");
                Builds.ForEach(x => Console.WriteLine("\t" + x.Name.ToLower()));

                Console.WriteLine("\nExample usage:\n");
                Console.WriteLine("\tXeniaUpdater-CLI.exe update master\n");
                Console.WriteLine("\tXeniaUpdater-CLI.exe run canary\n");
                Console.WriteLine("\tXeniaUpdater-CLI.exe u canary\n");
                Console.WriteLine("\tXeniaUpdater-CLI.exe r master\n");

            }


            void update(XeniaBuild build)
            {
                var helper = new Helper();
                if (helper.InternetAvailable()) //Checks if there is a working internet connection
                {
                    helper.CreateFolderStructure(build); // Create the folder which the build will be downloaded to
                    Console.WriteLine($"Updating {build.Name}");
                    DownloadXenia(build); // Download the build
                }
                else
                    Console.WriteLine("Could not connected to server.\nPlease check you internet connection.", "Error");

            }

            void run(XeniaBuild build)
            {
                Helper helper = new Helper();
                Console.WriteLine($"Running {build.Name}");
                helper.StartProcess(build);
            }

            void DownloadXenia(XeniaBuild build)
            {
                var helper = new Helper();

                using (var wc = new WebClient())
                {
                    //Download from URL to location
                    wc.DownloadFile(new Uri(build.URL), $"{build.FolderName}/{build.ZipName}.zip");

                    helper.ExtractBuild(build); // Go to the next step, extracting the downloaded build
                }
            }

        }
    }
}