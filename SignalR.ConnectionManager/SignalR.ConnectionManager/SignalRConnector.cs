using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalR.ConnectionManager
{
    public class SignalRConnector : ISignalRConnector
    {
        private ISignalRConnectorConfiguration _configuration;
        private HubConnection _hubConnection;
        private readonly ConcurrentDictionary<string, IHubProxy> _hubProxies = new ConcurrentDictionary<string, IHubProxy>();

        public SignalRConnector(ISignalRConnectorConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
        }

        private async void InitializeComponent()
        {
            OnDisconnected();
            _hubConnection.Closed += OnDisconnected;
        }

        private void OnDisconnected()
        {
            if (_hubConnection != null && _hubConnection.State != ConnectionState.Disconnected)
            {
                return;
            }
            if (_hubConnection != null)
            {
                _hubConnection.Closed -= OnDisconnected;
                _hubConnection.Stop();
                _hubConnection = null;
                _hubProxies.Clear();
            }

            _hubConnection = new HubConnection(_configuration.ServiceUrl);
            _hubConnection.TraceLevel = TraceLevels.All;
            _hubConnection.TraceWriter = new DebugWriter();

            bool result;
            do
            {
                Debug.WriteLine("SignalRConnector Hubconnection closed");

                var t = _hubConnection.Start();

                result = false;
                t.ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        result = true;
                    }
                }).Wait();

                if (!result)
                {
                    Task.Delay(5000).Wait();
                }
            } while (!result);
            _hubConnection.Closed += OnDisconnected;
        }

        private IHubProxy GetProxy(string hub)
        {
            IHubProxy proxy;
            if (!_hubProxies.ContainsKey(hub))
            {
                proxy = _hubConnection.CreateHubProxy(hub);
                _hubProxies[hub] = proxy;
            }
            else
            {
                proxy = _hubProxies[hub];
            }
            return proxy;
        }

        public bool IsConnected
        {
            get { return _hubConnection != null && _hubConnection.State == ConnectionState.Connected; }
        }

        public Task Invoke(string hub, string method, params object[] args)
        {
            var proxy = GetProxy(hub);
            return proxy.Invoke(method, args);
        }

        public void Subscribe(string hub, string eventName)
        {
            var proxy = GetProxy(hub);
            proxy.Subscribe(eventName);
        }
    }
}