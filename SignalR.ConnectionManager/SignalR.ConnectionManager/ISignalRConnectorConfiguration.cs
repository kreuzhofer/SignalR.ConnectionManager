namespace SignalR.ConnectionManager
{
    public interface ISignalRConnectorConfiguration
    {
        string ServiceUrl { get; }
        string[] RequestedHubProxies { get; set; }
    }
}