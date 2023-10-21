// GitCity Tool
// - Fetch Git repository data
// 

using System.Reflection;
using Octokit;
using Polly;
using System.Text.RegularExpressions;
using GC;
using GC.Contracts.Services;
using GC.Data;
using GC.Data.Contracts.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

///  MAIN ///

Banner();

try
{
    
    var serviceProvider = Configure();

    var app = serviceProvider.GetService<App>();

    await app.Run(args);
    

    Done();
}
catch (Exception ex)
{
    Console.WriteLine($"Phark: {ex.Message}");
}



static ServiceProvider Configure()
{
    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
        .Build();

    //setup our DI
    var serviceCollection = new ServiceCollection();

    serviceCollection
        .AddSingleton<App>()
        .AddSingleton<IGCDataService, GCDataService>();

    serviceCollection.AddOptions<ApplicationOptions>().Bind(config.GetSection("GitCity"));

    return serviceCollection.BuildServiceProvider();
}

static void Banner()
{
    Console.WriteLine("GitCity CLI");
    Console.WriteLine("-----------");
}


static void Done()
{
    Console.WriteLine();
    Console.WriteLine("Done :->");
    Console.WriteLine();
}

