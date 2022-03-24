// See https://aka.ms/new-console-template for more information

using CliFx;
using CliWrap;
using LocationParser;
using LocationParser.Execute;
using Spectre.Console.Cli;

//var app = new CommandApp<CountryCommands>();

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<CountryCommands>("country")
        .WithAlias("countries")
        .WithDescription("User application-name.exe {command} -h for more details.");

    config.AddCommand<StateCommand>("state")
        .WithAlias("states")
        .WithDescription("User application-name.exe {command} -h for more details.");
    //states\\{country.iso3}-{country.id}.json";

    config.AddCommand<CityCommand>("city")
        .WithAlias("cities")
        .WithDescription("User application-name.exe {command} -h for more details. ");
    //cities\\{state.state_code}-{state.id}.json
});

return await app.RunAsync(args);
