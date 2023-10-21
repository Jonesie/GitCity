using GC.Data.Contracts.Models;

namespace GC.Contracts.Services;

public interface IGCDataService
{
    User AddUser(User user);
    Repository AddRepository(Repository repo);
}