using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutomaticUDGB
{
    public static class Program
    {
        static void Main(string[] args)
        {

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/UDGB/UDGB.exe"))
            {
                ColorConsole.Msg("Extracting UDGB...");
                try
                {
                    Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("AutomaticUDGB.Resources.UDGB.zip");
                    FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/UDGB.zip", FileMode.Create, FileAccess.Write);
                    resource.CopyTo(file);
                    resource.Close();
                    file.Close();

                    ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "/UDGB.zip", AppDomain.CurrentDomain.BaseDirectory + "/UDGB");

                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "/UDGB.zip");

                    ColorConsole.Msg("Successfully extracted UDGB.");
                }
                catch (Exception e)
                {
                    ColorConsole.Error(e.Message);
                }
            }

            ColorConsole.Msg("");

            UnityVersionManager unityVersionManager = new UnityVersionManager("https://symbolserver.unity3d.com/000Admin/history.txt");
            List<string> unityVersions = unityVersionManager.FetchVersions();

            GitHubVersionManager gitHubVersionManager = new GitHubVersionManager("https://api.github.com/repos/LavaGang/Unity-Runtime-Libraries/git/trees/master");
            List<string> githubVersions = gitHubVersionManager.FetchVersions();

            ColorConsole.Msg("\nComparing version lists...");

            List<string> missingVersions = new List<string>();

            foreach (string unityVersion in unityVersions)
            {
                if (!githubVersions.Contains(unityVersion))
                    missingVersions.Add(unityVersion);
            }

            ColorConsole.Msg("");

            if (missingVersions.Count == 0)
            {
                ColorConsole.Success("No version is missing on GitHub!");
            }
            else
            {
                ColorConsole.Msg("The following versions are missing on GitHub:");
                foreach (string version in missingVersions)
                {
                    ColorConsole.Msg("Unity " + version);
                }

                ColorConsole.Msg("\nDo you want to automatically generate the zip files? (y/n): ", true);

                string input = Console.ReadLine();

                if (input.ToLower().Replace(" ", "").Equals("y"))
                {
                    List<string> failed = new List<string>();
                    foreach (string version in missingVersions)
                    {
                        ColorConsole.Msg("Generating dependencies for Unity " + version + "...\n");
                        Process process = new Process();
                        process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/UDGB/UDGB.exe";
                        process.StartInfo.Arguments = version;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.OutputDataReceived += ProcessOutputStream;
                        process.ErrorDataReceived += ProcessErrorStream;
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            ColorConsole.Success("\nSuccessfully generated dependencies for Unity " + version);
                            string outputDir = AppDomain.CurrentDomain.BaseDirectory + "/Output Dependencies";
                            string outputFile = outputDir + "/" + version + ".zip";
                            if (!Directory.Exists(outputDir))
                                Directory.CreateDirectory(outputDir);
                            if (File.Exists(outputFile))
                                File.Delete(outputFile);
                            File.Move(AppDomain.CurrentDomain.BaseDirectory + "/UDGB/" + version + ".zip", outputFile);
                        }
                        else
                        {
                            failed.Add(version);
                            ColorConsole.Error("\nFailed to generate dependencies for Unity " + version);
                        }
                    }

                    if (failed.Count == 0)
                    {
                        ColorConsole.Success("\nSuccessfully generated all dependencies.");
                    }
                    else
                    {
                        ColorConsole.Warning("\nNo dependencies could be generated for the following versions:");
                        foreach (string version in failed)
                        {
                            ColorConsole.Msg("Unity " + version);
                        }
                    }
                }
            }

            while (Console.KeyAvailable)
                Console.ReadKey(true);

            ColorConsole.Msg("\n\nPress any key to exit.");
            Console.ReadKey();
        }

        private static void ProcessOutputStream(object obj, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                ColorConsole.Msg("[UDGB] " + e.Data);
        }

        private static void ProcessErrorStream(object obj, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                ColorConsole.Error("[UDGB Error] " + e.Data);
        }
    }
}
