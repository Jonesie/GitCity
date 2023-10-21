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
    
    
    public User AddUser(User user)
    {
        var usersCollection = _db.GetCollection<User>("users");
        usersCollection.EnsureIndex(u => u.Name, true);

        var existing = usersCollection.FindOne(u => u.Name == user.Name);

        if (existing is null)
        {
            usersCollection.Insert(user);
        }
        else
        {
            // update other properties when we have any
            
            user = existing;
        }

        return user;
    }

    public Repository AddRepository(Repository repo)
    {
        throw new NotImplementedException();
    }
}