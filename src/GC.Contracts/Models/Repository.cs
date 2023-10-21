using System.Globalization;

namespace GC.Data.Contracts.Models;

public class Repository
{
    public Repository()
    {
    }

    public Repository(long id)
    {
        Id = id;
    }

    
    public string Url { get; set; }
    public string HtmlUrl { get; set; }
    public string CloneUrl { get; set; }
    public string GitUrl { get; set; }
    public string SshUrl { get; set; }

    public long Id { get; set; }

    public string NodeId { get; set; }

    public string Owner { get; set; }

    public string Name { get; set; }

    public string FullName { get; set; }

    public bool IsTemplate { get; set; }

    public string Description { get; set; }

    public string Homepage { get; set; }

    public string Language { get; set; }

    public bool Private { get; set; }

    public bool Fork { get; set; }

    public int ForksCount { get; set; }

    public int StargazersCount { get; set; }

    public string DefaultBranch { get; set; }

    public int OpenIssuesCount { get; set; }

    public DateTimeOffset? PushedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Repository Parent { get; set; }

    public Repository Source { get; set; }

    public bool HasDiscussions { get; set; }

    public bool HasIssues { get; set; }

    public bool HasWiki { get; set; }

    public bool HasDownloads { get; set; }

    public bool? AllowRebaseMerge { get; set; }

    public bool? AllowSquashMerge { get; set; }

    public bool? AllowMergeCommit { get; set; }

    public bool HasPages { get; set; }

    public int SubscribersCount { get; set; }

    public long Size { get; set; }

    public bool Archived { get; set; }

    public IReadOnlyList<string> Topics { get; set; }

    public bool? DeleteBranchOnMerge { get; set; }

    public string? Visibility { get; set; }

    public bool? AllowAutoMerge { get; set; }

    public bool? AllowUpdateBranch { get; set; }

    public bool? WebCommitSignoffRequired { get; set; }

    internal string DebuggerDisplay
    {
        get
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Repository: Id: {0} Owner: {1}, Name: {2}", Id, Owner, Name);
        }
    }
}