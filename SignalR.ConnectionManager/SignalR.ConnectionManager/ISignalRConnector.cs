using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SignalR.ConnectionManager
{
    public interface ISignalRConnector
    {
        bool IsConnected { get; }
        Task Invoke(string hub, string method, params object[] args);
        Subscription Subscribe(string hub, string eventName);
    }
}