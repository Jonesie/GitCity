using System.Text.RegularExpressions;
using GC.Contracts.Services;
using GC.Data.Contracts.Models;
using Microsoft.Extensions.Options;
using Octokit;
using Polly;
using Repository = GC.Data.Contracts.Models.Repository;
using User = GC.Data.Contracts.Models.User;

namespace GC;

public class App
{
    private readonly IOptions<ApplicationOptions> _options;
    private readonly IGCDataService _gcDataService;

    enum Modes
    {
        Help,
        Fetch,
        Delete,
        ArgError
    }


    public App(
        IOptions<ApplicationOptions> options,
        IGCDataService gcDataService)
    {
        _options = options;
        _gcDataService = gcDataService;
    }

    public async Task Run(string[] args)
    {
        var (mode, parms) = ParseCommandLine(args);
        switch (mode)
        {
            case Modes.Help:
                HelpText();
                break;
            case Modes.Fetch:
                await Fetch(parms[0]);
                break;
            case Modes.Delete:
                await Delete(parms[0]);
                break;
            case Modes.ArgError:
                HelpText(true);
                break;
        }
    }

    private (Modes, string[]) ParseCommandLine(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "help":
                    return (Modes.Help, null);
                case "fetch":
                    // expect 1 parameter for the user/repository pattern
                    if (args.Length > 1)
                    {
                        return (Modes.Fetch, new[] {args[1]});
                    }

                    break;

                case "delete":
                    // expect 1 parameter for the user name
                    if (args.Length > 1)
                    {
                        return (Modes.Delete, new[] {args[1]});
                    }

                    break;
            }


            return (Modes.ArgError, null);
        }

        return (Modes.Help, null);
    }

    private void HelpText(bool error = false)
    {
        if (error)
        {
            Console.WriteLine("Invalid command! :-< ");
            Console.WriteLine();
        }

        Console.WriteLine("Specify command and options, e.g.:");
        Console.WriteLine(" GC command [options]");
        Console.WriteLine("");
        Console.WriteLine("Commands:");
        Console.WriteLine("  Fetch <user/wildcard> to download repository data.");
        Console.WriteLine("    - wildcard can contain * or ?");
        Console.WriteLine("  Delete <user> to remove downloaded repository data.");
        Console.WriteLine("  Help to view this help.");
    }

    private async Task Fetch(string repoPattern)
    {
        var github = new GitHubClient(new ProductHeaderValue("GCCity"));
        github.Credentials =
            new Credentials(
                _options.Value.GitPat);

        var client = github.Repository;

        var retryPolicy = Policy
            .Handle<RateLimitExceededException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
            });

        // var policyWrap = Policy.WrapAsync(retryPolicy);

        string[] parts = repoPattern.Split('/');
        if (parts.Length != 2)
        {
            Console.WriteLine("Invalid repository format. Please use 'username/repo' or 'username/*'.");
            return;
        }

        string username = parts[0];
        string repositoryNameWildcard = parts[1];

        var repositories = await retryPolicy.ExecuteAsync(async () => await client.GetAllForUser(username));

        foreach (var repository in repositories)
        {
            _gcDataService.UpsertUser(new User {Name = repository.Owner.Name});
            
            if (IsWildcardMatch(repository.Name, repositoryNameWildcard))
            {
                Console.WriteLine($"Repository: {repository.Name}");

                _gcDataService.UpsertRepository(new Repository
                {
                    AllowAutoMerge = repository.AllowAutoMerge,
                    AllowMergeCommit = repository.AllowMergeCommit,
                    AllowRebaseMerge = repository.AllowRebaseMerge,
                    AllowSquashMerge = repository.AllowSquashMerge,
                    AllowUpdateBranch = repository.AllowUpdateBranch,
                    Archived = repository.Archived,
                    CloneUrl = repository.CloneUrl,
                    CreatedAt = repository.CreatedAt,
                    DefaultBranch = repository.DefaultBranch,
                    DeleteBranchOnMerge = repository.DeleteBranchOnMerge,
                    Description = repository.Description,
                    Fork = repository.Fork,
                    ForksCount = repository.ForksCount,
                    FullName = repository.FullName,
                    GitUrl = repository.GitUrl,
                    HasDiscussions = repository.HasDiscussions,
                    HasDownloads = repository.HasDownloads,
                    HasIssues = repository.HasIssues,
                    HasPages = repository.HasPages,
                    HasWiki = repository.HasWiki,
                    Homepage = repository.Homepage,
                    IsTemplate = repository.IsTemplate,
                    Language = repository.Language,
                    Name = repository.Name,
                    NodeId = repository.NodeId,
                    OpenIssuesCount = repository.OpenIssuesCount,
                    Owner = repository.Owner.Name,
                    Private = repository.Private,
                    PushedAt = repository.PushedAt,
                    Size = repository.Size,
                    SshUrl = repository.SshUrl,
                    StargazersCount = repository.StargazersCount,
                    SubscribersCount = repository.SubscribersCount,
                    Topics = repository.Topics,
                    UpdatedAt = repository.UpdatedAt,
                    Url = repository.Url,
                    Visibility = repository.Visibility.ToString(),
                    WebCommitSignoffRequired = repository.WebCommitSignoffRequired,
                });




                // Console.WriteLine($"Description: {repository.Description}");
                // Console.WriteLine($"Stars: {repository.StargazersCount}");
                // Console.WriteLine("Forks:");
                //
                // var forks = await client.Forks.GetAll(repository.Owner.Login, repository.Name);Xero
                // foreach (var fork in forks)
                // {
                //     Console.WriteLine($"- {fork.FullName}");
                // }
                //
                // Console.WriteLine("------------------------------");
            }
        }
    }

    private async Task Delete(string userName)
    {
    }

    private bool IsWildcardMatch(string input, string wildcard)
    {
        return Regex.IsMatch(input, "^" +
                                    Regex
                                        .Escape(wildcard)
                                        .Replace(@"\*", ".*")
                                        .Replace(@"\?", ".") + "$");
    }
}