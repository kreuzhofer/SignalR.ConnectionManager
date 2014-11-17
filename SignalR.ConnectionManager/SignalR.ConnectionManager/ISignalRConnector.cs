using System.Threading.Tasks;

namespace SignalR.ConnectionManager
{
    public interface ISignalRConnector
    {
        bool IsConnected { get; }
        Task Invoke(string hub, string method, params object[] args);
        void Subscribe(string hub, string eventName);
    }
}