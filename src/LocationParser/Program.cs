// See https://aka.ms/new-console-template for more information

using CliFx;
using CliWrap;
using LocationParser;
using LocationParser.Execute;
using LocationParser.Helper;
using Spectre.Console.Cli;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

//Ref: https://spectreconsole.net/widgets/tree

var app = new CommandApp();

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", optional: false);

IConfiguration config = builder.Build();

MongoSettings mongoSettings = config.GetSection("MongoSettings").Get<MongoSettings>();
Console.WriteLine();

var root = new Tree("MongoDB Settings");
root.AddNode($"[yellow]{mongoSettings.Connection}[/]");
root.AddNode($"[yellow]{mongoSettings.DataBaseName}[/]");
root.AddNode($"[yellow]{mongoSettings.Server}[/]");
AnsiConsole.Write(root);
Console.WriteLine();

app.Configure(config =>
{
    config.AddCommand<CountryDBCommand>("country-db")
        .WithAlias("CountryDB")
        .WithData(mongoSettings)
        .WithDescription("Countries inserted into MongoDB.");
    
    config.AddCommand<CountryCommands>("country")
        .WithAlias("countries")
        .WithDescription("User application-name.exe {command} -h for more details.");

    config.AddCommand<StateCommand>("state")
        .WithAlias("states")
        .WithDescription("User application-name.exe {command} -h for more details.");
    
    config.AddCommand<StateDBCommand>("state-db")
        .WithAlias("stateDB")
        .WithData(mongoSettings)
        .WithDescription("States inserted into MongoDB.");

    config.AddCommand<CityCommand>("city")
        .WithAlias("cities")
        .WithDescription("User application-name.exe {command} -h for more details. ");
    
    config.AddCommand<CityDBCommand>("city-db")
        .WithAlias("citiesDB")
        .WithData(mongoSettings)
        .WithDescription("States inserted into MongoDB. ");
    //cities\\{state.state_code}-{state.id}.json
});

return await app.RunAsync(args);
