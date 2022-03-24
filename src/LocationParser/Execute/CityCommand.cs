using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using LocationParser.Model;
using Newtonsoft.Json;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LocationParser.Execute;

public sealed class CityCommand :Command<CityCommand.CitySettings>
{
    public sealed class CitySettings : Settings
    {
        [Description("Country URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/countries.json'")]
        [CommandArgument(2, "[country-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/countries.json")]
        public string? CountryURL { get; init; }
        [Description("State URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/states.json'")]
        [CommandArgument(3, "[state-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/states.json")]
        public string? StateURL { get; init; }
        
        [Description("City URL, exclusion of domain. Eg. '/dr5hn/countries-states-cities-database/master/cities.json'")]
        [CommandArgument(4, "[city-api]")]
        [DefaultValue("/dr5hn/countries-states-cities-database/master/cities.json")]
        public string? CityURL { get; init; }
        
        
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CitySettings settings)
    {
        var client = new RestClient(settings.Domain);
        
        var requestCountry = new RestRequest((settings.CountryURL), Method.Get);
        var querCountryResult = client.ExecuteAsync(requestCountry).Result.Content;
        IList<Country> countries = JsonConvert.DeserializeObject<IList<Country>>(querCountryResult);
        
        var requestState = new RestRequest((settings.StateURL), Method.Get);
        var queryStateResult = client.ExecuteAsync(requestState).Result.Content;
        IList<State> states = JsonConvert.DeserializeObject<IList<State>>(queryStateResult);
        
        var requestCity = new RestRequest((settings.CityURL), Method.Get);
        var queryCityResult = client.ExecuteAsync(requestCity).Result.Content;
        IList<City> cities = JsonConvert.DeserializeObject<IList<City>>(queryCityResult);

        //foreach (var country in countries)
        {
            foreach (var state in states)
            {
                var selectedCities = cities
                    .Where(ct => ct.state_id == state.id)
                    .ToList();
                AnsiConsole.Markup( $"[underline blue]Country[/] {state.country_name} - State {state.Name} City Count :{selectedCities.Count}");
                Console.WriteLine(string.Empty);
                var jsonStates = JsonConvert.SerializeObject(selectedCities);
                string cityFileName = $"{settings.SavePath}\\cities\\{state.state_code}-{state.id}.json";
                
                File.WriteAllTextAsync(cityFileName, jsonStates);
                AnsiConsole.Markup(
                    $"[underline green]States [/] Country Name - {state.country_name} - State/Province Name saved as {cityFileName}");
            }
        }
        
        AnsiConsole.Markup(
            $"[underline red]State[/] Json Storage completed. Stored in [green] [/] {settings.SavePath}");
        return 0;
    }
    
}