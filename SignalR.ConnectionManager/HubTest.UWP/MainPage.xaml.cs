using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json.Linq;
using SignalR.ConnectionManager;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HubTest.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SignalRConnector _connection;
        private Subscription _sub;

        public MainPage()
        {
            this.InitializeComponent();
            this._connection = new SignalRConnector(new SignalRConfig());
            _connection.OnConnected += ConnectionOnOnConnected;
            _connection.Start();
        }

        private void ConnectionOnOnConnected(object sender, EventArgs eventArgs)
        {
            if (_sub != null)
            {
                _sub.Received -= SubOnReceived;
            }
            _sub = _connection.Subscribe("MyHub", "hello");
            _sub.Received += SubOnReceived;
        }

        private void SubOnReceived(IList<JToken> list)
        {
            Debug.WriteLine("SubOnReceived");
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => textBox.Text += String.Format("SubOnReceived {0} {1} {2} {3} {4}\n\r", list[0], list[1], list[2], list[3], list[4]));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _connection.Invoke("MyHub", "Hello");
        }
    }

    public class SignalRConfig : ISignalRConnectorConfiguration
    {
        public string ServiceUrl { get { return "http://localhost:4929/"; } }
        public string[] RequestedHubProxies { get { return new string[] {"MyHub"}; } }
    }
}
