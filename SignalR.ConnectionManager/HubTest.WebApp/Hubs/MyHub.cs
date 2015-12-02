using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace HubTest.WebApp.Hubs
{
    public class MyHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello("This is the message", "second parameter", 100, true, DateTime.UtcNow);
        }
    }
}