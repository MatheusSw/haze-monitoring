namespace HazeApi.Options;

public class DatabaseOptions
{
    public const string Section = "Database";
    
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Database { get; set; }
}