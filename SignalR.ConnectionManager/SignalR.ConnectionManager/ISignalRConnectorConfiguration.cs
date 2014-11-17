namespace SignalR.ConnectionManager
{
    public interface ISignalRConnectorConfiguration
    {
        string ServiceUrl { get; }
        string ApplicationName { get; }
        string MandatorName { get; }
    }
}