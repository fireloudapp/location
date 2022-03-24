using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LocationParser.Execute;

public sealed class CountryCommands : Command<CountryCommands.CountrySettings>
{
    public sealed class CountrySettings : Settings
    {
        [Description("Country URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/countries.json'")]
        [CommandArgument(2, "[country-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/countries.json")]
        public string? CountryURL { get; init; }
        
        [Description("Country File Name, Example. 'countries.json'")]
        [CommandArgument(3, "[file-name]")]
        [DefaultValue("countries.json")]
        public string? FileName { get; init; }
        
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CountrySettings settings)
    {
        var client = new RestClient(settings.Domain);
        var request = new RestRequest((settings.CountryURL), Method.Get);
        var queryResult = client.ExecuteAsync(request).Result.Content;
        File.WriteAllTextAsync($"{settings.SavePath}\\{settings.FileName}", queryResult);
        AnsiConsole.Markup(
            $"[underline red]Country[/] Json Storage completed. Stored[green] [/] in {settings.SavePath} ");
        return 0;
    }

}