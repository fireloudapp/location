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

public sealed class CountryDBCommand: Command<CountryDBCommand.CountryDBSettings>
{
    public sealed class CountryDBSettings : Settings
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

    public override int Execute([NotNull] CommandContext context, [NotNull] CountryDBSettings settings)
    {
        MongoSettings? mongoSettings = context.Data as MongoSettings;

        SQLExtension.SQLConfig = new SQLConfig()
        {
            Compiler = SQLCompiler.MongoDB,
            DBType = DBType.NoSQL,
            ConnectionString = mongoSettings.Connection,
            DataBaseName = mongoSettings.DataBaseName,
            CollectionName = "Country"
        };
        //mongodb+srv://fc_client_admin:fc.Serverless.mongo@cluster0.acxm4.mongodb.net/ClientDB?retryWrites=true&w=majority&connect=replicaSet
        
        
        var countries = GetCountry(settings);
        AnsiConsole.Markup($"[underline blue] Country Count[/] {countries.Count}");
        AnsiConsole.WriteLine();
        foreach (var country in countries)
        {
            try
            {
                var ctry = country.Save().Result;
                Console.WriteLine($"[underline red]Country[/] {JsonConvert.SerializeObject(ctry)}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        //File.WriteAllTextAsync($"{settings.SavePath}\\{settings.FileName}", queryResult);
        AnsiConsole.Markup(
            $"[underline red]Country[/] Json Storage completed. Stored[green] [/] in MongoDB. ");
        return 0;
    }

    private static IList<Country> GetCountry(CountryDBSettings settings)
    {
        var client = new RestClient(settings.Domain);
        var request = new RestRequest((settings.CountryURL), Method.Get);
        var queryResult = client.ExecuteAsync(request).Result.Content;

        IList<Country> countryList = JsonConvert.DeserializeObject<IList<Country>>(queryResult);
        return countryList;
    }
}