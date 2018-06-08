using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;

namespace WhatMessenger.Services
{
    public class SocketManagerDataStore
    {
        private static SocketManagerDataStore instance { get; set; }
        public static SocketManagerDataStore GetInstance()
        {
            if (instance == null)
                instance = new SocketManagerDataStore();
            return instance;
        }

        public HubConnection Connection;
        public IHubProxy ChatHubProxy;
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public bool SocketStatus { get; set; }

        public SocketManagerDataStore()
        {
            SocketStatus = false;
            Connection = null;
        }

        public void StartSocketConnection(int userId)
        {
            //if (SocketStatus)
                //return;
            if (Connection != null)
            {
                Connection.StateChanged -= Connection_StateChanged;
                Connection = null;
                ChatHubProxy = null;
            }

            this.UserId = userId;
            Connection = new HubConnection(ServerURL.BaseURL);
            Connection.StateChanged += Connection_StateChanged;
            ChatHubProxy = Connection.CreateHubProxy("ChatHub");
            ChatHubProxy_ConnectedSocketId();
            Connection.Start();
        }

        private void ChatHubProxy_ConnectedSocketId()
        {
            ChatHubProxy.On<string>("ConnectedSocketId", (connectionId) => {
                ConnectionId = connectionId;
                SocketStatus = true;
            });
        }

        void Connection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
            {
                //invoke connected so server can save user connection id
                SocketStatus = true;
                ChatHubProxy.Invoke("OnConnected", $"{this.UserId}");
                Connection.Closed -= Connection_ClosedHandler;
                Connection.Closed += Connection_ClosedHandler;
            }
            else if (obj.NewState == ConnectionState.Disconnected)
            {
                SocketStatus = false;
            }
        }

        async void Connection_ClosedHandler()
        {
            await Task.Delay(200);
            StartSocketConnection(UserId);
        }

        public void StopSocketConnection()
        {
            if (Connection == null) return;
            Connection.StateChanged -= Connection_StateChanged;
            Connection.Closed -= Connection_ClosedHandler;
            Connection.Stop();
            Connection = null;
            ChatHubProxy = null;
        }
    }
}
