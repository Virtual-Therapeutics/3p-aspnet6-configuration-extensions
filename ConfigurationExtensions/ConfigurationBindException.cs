namespace DevTrends.ConfigurationExtensions;

public class ConfigurationBindException : Exception
{
    public ConfigurationBindException(string message) : base(message)
    {
    }

    public ConfigurationBindException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
