using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FC.Extension.SQL.Engine;
using FC.Extension.SQL.Helper;
using LocationParser.Helper;
using LocationParser.Model;
using Newtonsoft.Json;
using RestSharp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LocationParser.Execute;

public class CityDBCommand : Command<CityDBCommand.CityDBSettings>
{
  public sealed class CityDBSettings : Settings
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

    public override int Execute([NotNull] CommandContext context, [NotNull] CityDBSettings settings)
    {
        #region MongoDB Configuration
        MongoSettings? mongoSettings = context.Data as MongoSettings;

        SQLExtension.SQLConfig = new SQLConfig()
        {
            Compiler = SQLCompiler.MongoDB,
            DBType = DBType.NoSQL,
            ConnectionString = mongoSettings.Connection,
            DataBaseName = mongoSettings.DataBaseName,
            CollectionName = "City"
        };
        #endregion

        #region MongoDB Insert City

        var cities = GetCities(settings);
        AnsiConsole.Markup($"[underline blue] City Count[/] {cities.Count}");
        AnsiConsole.WriteLine();
        foreach (var city in cities)
        {
            try
            {
                var ct = city.Save().Result;
                Console.WriteLine($"[underline red]City[/] {JsonConvert.SerializeObject(ct)}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        #endregion
        
        AnsiConsole.Markup(
            $"[underline green]City[/] Data Storage MongoDB [blue] [/] {mongoSettings.DataBaseName}");
        return 0;
    }
    
    private static IList<City> GetCities(CityDBSettings settings)
    {
        var client = new RestClient(settings.Domain);
        var requestCity = new RestRequest((settings.CityURL), Method.Get);
        var queryCityResult = client.ExecuteAsync(requestCity).Result.Content;
        IList<City> cities = JsonConvert.DeserializeObject<IList<City>>(queryCityResult);
        return cities;
    }
}