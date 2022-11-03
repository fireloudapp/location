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

public class StateDBCommand : Command<StateDBCommand.StateDBSettings>
{
   public sealed class StateDBSettings : Settings
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

    public override int Execute([NotNull] CommandContext context, [NotNull] StateDBSettings settings)
    {
        #region MongoDB Configuration
        MongoSettings? mongoSettings = context.Data as MongoSettings;

        SQLExtension.SQLConfig = new SQLConfig()
        {
            Compiler = SQLCompiler.MongoDB,
            DBType = DBType.NoSQL,
            ConnectionString = mongoSettings.Connection,
            DataBaseName = mongoSettings.DataBaseName,
            CollectionName = "State"
        };
        #endregion

        #region MongoDB Insert States

        var states = GetStates(settings);
        AnsiConsole.Markup($"[underline blue] State Count[/] {states.Count}");
        AnsiConsole.WriteLine();
        foreach (var state in states)
        {
            try
            {
                var st = state.Save().Result;
                Console.WriteLine($"[underline red]State[/] {JsonConvert.SerializeObject(st)}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        #endregion
        

        AnsiConsole.Markup(
            $"[underline green]State[/] Data Storage MongoDB [blue] [/] {mongoSettings.DataBaseName}");
        return 0;
    }
    private static IList<State> GetStates(StateDBSettings settings)
    {
        var client = new RestClient(settings.Domain);
        var requestState = new RestRequest((settings.StateURL), Method.Get);
        var queryStateResult = client.ExecuteAsync(requestState).Result.Content;
        IList<State> states = JsonConvert.DeserializeObject<IList<State>>(queryStateResult);
        return states;
    }
}