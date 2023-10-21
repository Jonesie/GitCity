using GC.Data.Contracts.Models;

namespace GC.Contracts.Services;

public interface IGCDataService
{
    User UpsertUser(User user);
    Repository UpsertRepository(Repository repo);
} 