namespace LocationParser.Helper;

public class MongoSettings
{
    /// <summary>
    /// Server or Cluster name of MongoDB
    /// </summary>
    public string? Server { get; set; }
    /// <summary>
    /// Database Name of MongoDB
    /// </summary>
    public string? DataBaseName { get; set; }

    /// <summary>
    /// Full connection string this has to be used.
    /// </summary>
    public string Connection
    {
        get
        {
            return string.Format(Server, DataBaseName);
        }
    }
}