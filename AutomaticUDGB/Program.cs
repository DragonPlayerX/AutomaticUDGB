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
        private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string UdgbPath = Path.Combine(AppPath, "UDGB");
        private static readonly string ExePath = Path.Combine(UdgbPath, "UDGB.exe");
        private static readonly string ZipPath = Path.Combine(AppPath, "UDGB.zip");
        private static readonly string LogPath = Path.Combine(AppPath, "Logs");
        private static readonly string OutputPath = Path.Combine(AppPath, "Output Dependencies");
        private static readonly string UnityUrl = "https://alpha.release-notes.ds.unity3d.com/";
        private static readonly string GitHubUrl = "https://api.github.com/repos/LavaGang/Unity-Runtime-Libraries/git/trees/master";

        public static void Main(string[] args)
        {
            if (!File.Exists(ExePath))
            {
                ColorConsole.Msg("Extracting UDGB...");
                try
                {
                    Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("AutomaticUDGB.Resources.UDGB.zip");
                    FileStream file = new FileStream(ZipPath, FileMode.Create, FileAccess.Write);
                    resource.CopyTo(file);
                    resource.Close();
                    file.Close();

                    ZipFile.ExtractToDirectory(ZipPath, UdgbPath);
                    File.Delete(ZipPath);

                    ColorConsole.Msg("Successfully extracted UDGB.");
                }
                catch (Exception e)
                {
                    ColorConsole.Error(e.Message);
                }

                ColorConsole.Msg("");
            }

            UnityVersionManager unityVersionManager = new UnityVersionManager(UnityUrl);
            List<string> unityVersions = unityVersionManager.FetchVersions();

            GitHubVersionManager gitHubVersionManager = new GitHubVersionManager(GitHubUrl);
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

                ColorConsole.Msg("\nDo you want to automatically generate the zip files? (y/n): ", false);

                string input = Console.ReadLine();

                if (input.ToLower().Replace(" ", "").Equals("y"))
                {
                    List<string> failed = new List<string>();
                    foreach (string version in missingVersions)
                    {
                        ColorConsole.Msg("Generating dependencies for Unity " + version + "...\n");
                        Process process = new Process();
                        process.StartInfo.FileName = ExePath;
                        process.StartInfo.WorkingDirectory = UdgbPath;
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

                        string logFile = Path.Combine(LogPath, version + ".log");

                        if (!Directory.Exists(LogPath))
                            Directory.CreateDirectory(LogPath);
                        if (File.Exists(logFile))
                            File.Delete(logFile);

                        string udgbLogFile = Path.Combine(UdgbPath, "output.log");

                        File.Move(udgbLogFile, logFile);
                        File.Delete(udgbLogFile);

                        if (process.ExitCode == 0)
                        {
                            ColorConsole.Success("\nSuccessfully generated dependencies for Unity " + version);

                            string dependencyFile = Path.Combine(OutputPath, version + ".zip");

                            if (!Directory.Exists(OutputPath))
                                Directory.CreateDirectory(OutputPath);
                            if (File.Exists(dependencyFile))
                                File.Delete(dependencyFile);

                            File.Move(Path.Combine(UdgbPath, version + ".zip"), dependencyFile);
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
                            ColorConsole.Msg("Unity " + version);
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
