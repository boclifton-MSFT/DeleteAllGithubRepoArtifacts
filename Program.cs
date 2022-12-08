using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtifactDeleter
{
    class Program
    {
        class ArtifactsPage
        {
            public List<Artifact> Artifacts { get; set; }
        }

        class Artifact
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public long SizeInBytes { get; set; }
        }

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintError("Please provide an access token and repo owner name (and, optionally, a repo name) as arguments");
                return;
            }

            var token = args.Length >= 1
                ? args[0]
                : string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                PrintError("Please provide an access token");
                return;
            }

            var owner = args.Length >= 2
                ? args[1]
                : string.Empty;
            if (string.IsNullOrEmpty(owner))
            {
                PrintError("Please provide the name of the Github repo owner");
                return;
            }
            
            var repo = args.Length >= 3
                ? args[2]
                : string.Empty;
            if (string.IsNullOrEmpty(repo))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("-- No repo name provided, deleting artifacts from all repos --");
                Console.WriteLine("-- ENTER Y OR PRESS ENTER TO CONFIRM OR N TO CANCEL --");
                var userInput = Console.ReadLine();
                if (userInput.ToLowerInvariant().Contains("n"))
                {
                    Console.ResetColor();
                    return;
                }
                Console.ResetColor();
            }

            var client = new GitHubClient(new ProductHeaderValue("ArtifactDeleter", "1.0.0"))
            {
                Credentials = new Credentials(token)
            };

            var apiConnection = new ApiConnection(client.Connection);

            var repoClient = new RepositoriesClient(apiConnection);
            var res = await repoClient.GetAllForCurrent();
            double sizeDeleted = 0;

            var reposToDeleteFrom = string.IsNullOrEmpty(repo) 
                ? res 
                : res.Where(r => r.Name == repo);
            
            foreach (var r in reposToDeleteFrom)
            {
                try
                {
                    var results = await apiConnection.GetAll<ArtifactsPage>(
                        new Uri(
                            $"repos/{owner}/{r.Name}/actions/artifacts", 
                            UriKind.Relative));
                    
                    Console.WriteLine($"---- {r.Name} ----");
                    
                    foreach (var page in results)
                    {
                        foreach (var artifact in page.Artifacts)
                        {
                            var artifactSize = (double)artifact.SizeInBytes / 1024 / 1024;
                            Console.WriteLine($"Deleting artifact {artifact.Name}: {artifactSize}MB");
                            await apiConnection.Delete(new Uri($"repos/{owner}/{r.Name}/actions/artifacts/{artifact.Id}", UriKind.Relative));
                            sizeDeleted += artifactSize;
                        }
                    }
                }
                catch (Exception e)
                {
                    PrintError($"ERROR: {e.Message}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Done. Deleted {sizeDeleted}MB");
            Console.ResetColor();
        }

        static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}