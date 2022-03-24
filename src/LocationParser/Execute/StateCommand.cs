using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using LocationParser.Model;
using Newtonsoft.Json;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LocationParser.Execute;

public sealed class StateCommand :Command<StateCommand.StateSettings>
{
    public sealed class StateSettings : Settings
    {
        [Description("Country URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/countries.json'")]
        [CommandArgument(2, "[country-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/countries.json")]
        public string? CountryURL { get; init; }
        [Description("State URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/states.json'")]
        [CommandArgument(3, "[state-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/states.json")]
        public string? StateURL { get; init; }
        
        
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] StateSettings settings)
    {
        var client = new RestClient(settings.Domain);
        var requestCountry = new RestRequest((settings.CountryURL), Method.Get);
        var querCountryResult = client.ExecuteAsync(requestCountry).Result.Content;
        IList<Country> countries = JsonConvert.DeserializeObject<IList<Country>>(querCountryResult);
        
        var requestState = new RestRequest((settings.StateURL), Method.Get);
        var queryStateResult = client.ExecuteAsync(requestState).Result.Content;
        IList<State> states = JsonConvert.DeserializeObject<IList<State>>(queryStateResult);

        foreach (var country in countries)
        {
            var selectedStates = states.Where(st => st.country_id == country.id).ToList();
            Console.WriteLine("");
            AnsiConsole.Markup( $"[underline blue]States[/] {selectedStates.Count}");
            Console.WriteLine("");
            var jsonStates = JsonConvert.SerializeObject(selectedStates);
            Console.WriteLine(jsonStates);
            string stateFileName = $"{settings.SavePath}\\states\\{country.iso3}-{country.id}.json";
            
            File.WriteAllTextAsync(stateFileName, jsonStates);
            AnsiConsole.Markup(
                $"[underline green]States [/] Country Name - {country.Name} - State/Province Name saved as {stateFileName}");
            Console.WriteLine("");
        }

        AnsiConsole.Markup(
            $"[underline red]State[/] Json Storage completed. Stored in [green] [/] {settings.SavePath}");
        return 0;
    }
    
}