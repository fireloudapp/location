using System.ComponentModel;
using Spectre.Console.Cli;

namespace LocationParser.Execute;

public class Settings : CommandSettings
{
    [Description("Country, State, City DB Git Domain. Eg. https://raw.githubusercontent.com")]
    [CommandArgument(0, "[domain]")]
    [DefaultValue("https://raw.githubusercontent.com")]
    public string? Domain { get; init; }
    
    [Description("Path to Save the JSON file")]
    [CommandArgument(1, "[save-path]")]
    [DefaultValue(@"C:\Works\_FireCloudApps\location\world\")]
    public string? SavePath { get; init; }
    
    
}