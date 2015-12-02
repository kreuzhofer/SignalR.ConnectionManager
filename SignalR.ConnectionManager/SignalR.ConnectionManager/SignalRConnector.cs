using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SignalR.ConnectionManager
{
    public class SignalRConnector : ISignalRConnector
    {
        public event EventHandler OnConnected;
        private ISignalRConnectorConfiguration _configuration;
        private HubConnection _hubConnection;
        private readonly ConcurrentDictionary<string, IHubProxy> _hubProxies = new ConcurrentDictionary<string, IHubProxy>();

        public SignalRConnector(ISignalRConnectorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Start()
        {
            Task.Run(() => InitializeComponent());
        }

        private void InitializeComponent()
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

            _hubConnection = new HubConnection(_configuration.ServiceUrl)
            {
                TraceLevel = TraceLevels.All,
                TraceWriter = new DebugWriter()
            };
            foreach (var hub in _configuration.RequestedHubProxies)
            {
                _hubProxies[hub] = _hubConnection.CreateHubProxy(hub);
            }

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
            if (OnConnected != null)
            {
                OnConnected(this, new EventArgs());
            }
        }

        private IHubProxy GetProxy(string hub)
        {
            if (!_hubProxies.ContainsKey(hub))
            {
                return null;
            }
            return _hubProxies[hub];
        }

        public bool IsConnected
        {
            get { return _hubConnection != null && _hubConnection.State == ConnectionState.Connected; }
        }

        public async Task Invoke(string hub, string method, params object[] args)
        {
            var proxy = GetProxy(hub);
            await proxy.Invoke(method, args);
        }

        public Subscription Subscribe(string hub, string eventName)
        {
            var proxy = GetProxy(hub);
            return proxy.Subscribe(eventName);
        }
    }
}