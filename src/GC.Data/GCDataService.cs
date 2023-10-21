using System.ComponentModel;
using System.Reflection;
using GC.Contracts.Services;
using GC.Data.Contracts.Models;
using LiteDB;
using Microsoft.Extensions.Options;

namespace GC.Data;

public class GCDataService : IGCDataService
{
    private readonly IOptions<ApplicationOptions> _options;
    private readonly LiteDatabase _db;

    private ILiteCollection<User> _userCollection = null;
    private ILiteCollection<Repository> _repoCollection = null;


    public GCDataService(IOptions<ApplicationOptions> options)
    {
        _options = options;
        var dbPath = _options.Value.DatabasePath;
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        dbPath = Path.Combine(dbPath, options.Value.DatabaseName ?? "gitcity.db");
        _db = new LiteDatabase(dbPath);

    }

    private ILiteCollection<User> UserCollection
    {
        get
        {
            if (_userCollection is null)
            {
                _userCollection = _db.GetCollection<User>("users");
                _userCollection.EnsureIndex(u => u.Name, true);
            }

            return _userCollection;
        }

    }
    
    private ILiteCollection<Repository> RepoCollection
    {
        get
        {
            if (_repoCollection is null)
            {
                _repoCollection = _db.GetCollection<Repository>("repositories");
                _repoCollection.EnsureIndex(u => u.Name, true);
            }

            return _repoCollection;
        }
    }

    
    public User UpsertUser(User user)
    {
        
        var existing = UserCollection.FindOne(u => u.Name == user.Name);

        if (existing is null)
        {
            UserCollection.Insert(user);
        }
        else
        {
            // update other properties when we have any
            
            user = existing;
        }

        return user;
    }

    public Repository UpsertRepository(Repository repo)
    {
        var existing = RepoCollection.FindOne(r => r.Name == repo.Name);

        if (existing is null)
        {
            RepoCollection.Insert(repo);
        }
        else
        {
            existing.AllowAutoMerge = repo.AllowAutoMerge;
            existing.AllowMergeCommit = repo.AllowMergeCommit;
            existing.AllowRebaseMerge = repo.AllowRebaseMerge;
            existing.AllowSquashMerge = repo.AllowSquashMerge;
            existing.AllowUpdateBranch = repo.AllowUpdateBranch;
            existing.Archived = repo.Archived;
            existing.CloneUrl = repo.CloneUrl;
            existing.CreatedAt = repo.CreatedAt;
            existing.DefaultBranch = repo.DefaultBranch;
            existing.DeleteBranchOnMerge = repo.DeleteBranchOnMerge;
            existing.Description = repo.Description;
            existing.Fork = repo.Fork;
            existing.ForksCount = repo.ForksCount;
            existing.FullName = repo.FullName;
            existing.GitUrl = repo.GitUrl;
            existing.HasDiscussions = repo.HasDiscussions;
            existing.HasDownloads = repo.HasDownloads;
            existing.HasIssues = repo.HasIssues;
            existing.HasPages = repo.HasPages;
            existing.HasWiki = repo.HasWiki;
            existing.Homepage = repo.Homepage;
            existing.IsTemplate = repo.IsTemplate;
            existing.Language = repo.Language;
            existing.Name = repo.Name;
            existing.NodeId = repo.NodeId;            
            existing.OpenIssuesCount = repo.OpenIssuesCount;
            existing.Owner = repo.Owner;
            existing.Private = repo.Private;
            existing.PushedAt = repo.PushedAt;
            existing.Size = repo.Size;
            existing.SshUrl = repo.SshUrl;
            existing.StargazersCount = repo.StargazersCount;
            existing.SubscribersCount = repo.SubscribersCount;
            existing.Topics = repo.Topics;
            existing.UpdatedAt = repo.UpdatedAt;
            existing.Url = repo.Url;
            existing.Visibility = repo.Visibility;
            existing.WebCommitSignoffRequired = repo.WebCommitSignoffRequired;
            
            repo = existing;

            RepoCollection.Update(existing);
        }

        return repo;
    }
}