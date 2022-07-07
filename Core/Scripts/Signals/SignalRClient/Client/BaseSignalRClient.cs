using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.General;
using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Util;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client
{
    public static class SignalRIn
    {

        public static string NotifyOnGameBayState => "NotifyOnGameBayState";
        public static string NotifyOnGameActionChange => "NotifyOnGameActionChange";
        public static string NotifyOnGameBayStateUpdate => "NotifyOnGameBayStateUpdate";
        public static string NotifyOnTestMode => "NotifyOnTestMode";
        public static string NotifyOnOfflineMode => "NotifyOnOfflineMode";

        public static string NotifyOnResetGame => "NotifyOnResetGame";
        public static string NotifyOnSendACK => "NotifyOnSendACK";
    }


    public static class SignalROut
    {
        public static string UpdateCameraStatus => "UpdateCameraStatus";
        public static string AddGameBayToGroup => "AddGameBayToGroup";
        public static string GetGameBayState => "GetGameBayState";
        public static string GameBayStateUpdate => "GameBayStateUpdate";

        public static string SendACK => "SendACK";
    }

    public class SignalRMessage
    {
        public string ActionName { get; set; }
        public DataModelBase Data { get; set; }
        public int Key { get; set; } = -1;
        public bool SendBool { get; set; }
        public bool BoolVal { get; set; }
        public Guid Guid { get; set; }

        public SignalRMessage(string actionName, DataModelBase data, int key = -1, bool sendBool = false,
            bool boolVal = false)
        {
            ActionName = actionName;
            Data = data;
            Key = key;
            SendBool = sendBool;
            BoolVal = boolVal;
            Guid = Guid.NewGuid();
        }
    }

    public class BaseSignalRClient : Singleton<BaseSignalRClient>
    {
        public static string status = "connecting...";
        public static string ServerHealth = "Not Connected";
        protected static Dictionary<string, Guid> ConsumedGuids;
        protected static bool Closed;
        protected readonly ConcurrentQueue<SignalRMessage> _messageQueue = new ConcurrentQueue<SignalRMessage>();
        private readonly int _messageQueueInterval = 250; // ms
        private readonly bool _useMessageQueue = true;
        private readonly List<Guid> _ackGuid = new List<Guid>();
        private int _ackTries;
        private readonly int _maxAckTries = 100;
        private readonly int _reconnectTime = 5; // sec, total += timeout
        private readonly int _reconnectTimeout = 5; // sec
        protected HubConnection connection;

        [SerializeField] private string uri = "https://manusignalrserver20200521010942.azurewebsites.net/gameBayHub";

        protected virtual void Start()
        {
            ConsumedGuids = new Dictionary<string, Guid>
            {
                [SignalRIn.NotifyOnGameBayState] = Guid.Empty,
                [SignalRIn.NotifyOnGameBayStateUpdate] = Guid.Empty,
                [SignalRIn.NotifyOnGameActionChange] = Guid.Empty,
                [SignalRIn.NotifyOnTestMode] = Guid.Empty,
                [SignalRIn.NotifyOnOfflineMode] = Guid.Empty,
            };

            uri = ConfigManager.Instance.generalSettings.SignalRUri;
            if (string.IsNullOrWhiteSpace(uri))
            {
                Log.Error("No SignalR URI. A connection will NOT be made.");
                Closed = true;
                return;
            }

            connection = new HubConnectionBuilder()
                .WithUrl(uri, HttpTransportType.WebSockets)
                .Build();

            Task.Run(async () =>
            {
                while (true)
                {
                    if (connection.State == HubConnectionState.Disconnected && Closed == false)
                    {
                        var task = Task.Run(Connect);
                        await Task.WhenAny(task, Task.Delay(_reconnectTimeout * 1000));
                    }

                    await Task.Delay(_reconnectTime * 1000);
                }

                ;
            });

            Task.Run(async () =>
            {
                while (_useMessageQueue)
                {
                    if (connection.State == HubConnectionState.Connected && _messageQueue.IsEmpty == false)
                        if (_messageQueue.TryPeek(out var message))
                        {
                            if (_ackGuid.Contains(message.Guid) || _ackTries >= _maxAckTries)
                            {
                                if (_messageQueue.TryDequeue(out var usedMessage))
                                {
                                    Log.Debug($"Message queue removing: {usedMessage.Guid} {usedMessage.ActionName}");
                                    _ackGuid.Remove(message.Guid);
                                }

                                if (_ackTries >= _maxAckTries)
                                    Log.Error(
                                        $"Message queue reached max attempts: {usedMessage.Guid} {usedMessage.ActionName}");
                                _ackTries = 0;
                                await Task.Delay(1);
                            }
                            else
                            {
                                _ackTries += 1;
                                Log.Debug(
                                    $"Message queue sending: {message.Guid} {message.ActionName} ({_ackTries} attempts)");
                                await SendData(message);
                            }
                        }

                    await Task.Delay(_messageQueueInterval);
                }

                ;
            });
        }

        private static Task OnClosed(Exception error)
        {
            Log.Warning($"Disconnected from the SignalR server {error.Message}");
            ServerHealth = "Server disconnected";
            return Task.CompletedTask;
        }


        protected override async void OnApplicationQuit()
        {
            if (Closed) return;
            await Close();
            if (connection != null) await connection.DisposeAsync();
            base.OnApplicationQuit();
        }

        protected async Task Close()
        {
            Log.Debug("SignalR connection attempting close");
            Closed = true;
            if (connection != null) await connection.StopAsync();
            Log.Debug("SignalR connection closed");
        }

        protected virtual void connectToEvents()
        {
            RemoveEvents();
            connection.Closed += OnClosed;

            connection.On(SignalRIn.NotifyOnGameBayState, (Guid guid, GameBayData message) =>
            {
                SendGuid(guid);

                if (ConsumedGuids[SignalRIn.NotifyOnGameBayState] == guid) return;
                ConsumedGuids[SignalRIn.NotifyOnGameBayState] = guid;

                status = message.ToString();
                Dispatcher.RunOnMainThread(() => { Log.Debug($"~~~~~~ SignalRIn.NotifyOnGameBayState {status}"); });
                Dispatcher.RunOnMainThread(() => EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage,
                    SignalRIn.NotifyOnGameBayState, null, message));
            });

            connection.On(SignalRIn.NotifyOnGameBayStateUpdate, (Guid guid, GameBayData message) =>
            {
                SendGuid(guid);

                if (ConsumedGuids[SignalRIn.NotifyOnGameBayStateUpdate] == guid) return;
                ConsumedGuids[SignalRIn.NotifyOnGameBayStateUpdate] = guid;

                status = message.ToString();
                Dispatcher.RunOnMainThread(
                    () => { Log.Debug($"~~~~~~ SignalRIn.NotifyOnGameBayStateUpdate {status}"); });
                Dispatcher.RunOnMainThread(() => EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage,
                    SignalRIn.NotifyOnGameBayStateUpdate, null, message));
            });

            connection.On(SignalRIn.NotifyOnGameActionChange, (Guid guid, ACTIONS_TYPE action) =>
            {
                SendGuid(guid);

                if (ConsumedGuids[SignalRIn.NotifyOnGameActionChange] == guid) return;
                ConsumedGuids[SignalRIn.NotifyOnGameActionChange] = guid;

                status = action.ToString();
                var message = new IntData {AInt = (int) action};

                // clear the message queue with restart actions
                if (action == ACTIONS_TYPE.ACTIONS_TYPE_REFRESH || action == ACTIONS_TYPE.ACTIONS_TYPE_STOP)
                    ClearMessageQueue();

                Dispatcher.RunOnMainThread(() => EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage,
                    SignalRIn.NotifyOnGameActionChange, null, message));
            });


            connection.On(SignalRIn.NotifyOnTestMode, (Guid guid, bool status) =>
            {
                SendGuid(guid);

                if (ConsumedGuids[SignalRIn.NotifyOnTestMode] == guid) return;
                ConsumedGuids[SignalRIn.NotifyOnTestMode] = guid;

                var message = new BoolData {ABool = status};
                Dispatcher.RunOnMainThread(() => EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage,
                    SignalRIn.NotifyOnTestMode, null, message));
            });

            connection.On(SignalRIn.NotifyOnOfflineMode, (Guid guid, bool status) =>
            {
                SendGuid(guid);

                if (ConsumedGuids[SignalRIn.NotifyOnOfflineMode] == guid) return;
                ConsumedGuids[SignalRIn.NotifyOnOfflineMode] = guid;

                var message = new BoolData {ABool = status};
                Dispatcher.RunOnMainThread(() => EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage,
                    SignalRIn.NotifyOnOfflineMode, null, message));
            });



            connection.On(SignalRIn.NotifyOnSendACK, (Guid guid) =>
            {
                if (_ackGuid.Contains(guid) == false)
                    _ackGuid.Add(guid);
            });
        }

        protected virtual void RemoveEvents()
        {
            connection.Closed -= OnClosed;
            connection.Remove(SignalRIn.NotifyOnGameBayState);
            connection.Remove(SignalRIn.NotifyOnGameBayStateUpdate);
            connection.Remove(SignalRIn.NotifyOnGameActionChange);
            connection.Remove(SignalRIn.NotifyOnTestMode);
            connection.Remove(SignalRIn.NotifyOnOfflineMode);
            connection.Remove(SignalRIn.NotifyOnSendACK);
        }

        protected virtual async Task Connect()
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                await connection.StartAsync();
                status = "Connection started";
                ServerHealth = "Server Connecting";
                //

                //TODO: Add gameBay to group must happen before everything
                Task.Run(() =>
                        AddGameBayToGroup(SignalROut.AddGameBayToGroup,
                            ConfigManager.Instance.generalSettings.DreamCube))
                    .GetAwaiter().OnCompleted(() =>
                    {
                        Dispatcher.RunOnMainThread(Initialize);
                        //OnTaskComplete();
                    });


                Log.Debug(status);
            }
        }

        protected virtual void Initialize()
        {
            ServerHealth = "Server Connected";
            connectToEvents();
            Task.Run(GetGameBayState).GetAwaiter().OnCompleted(() => OnTaskComplete(SignalROut.GetGameBayState));
        }

        public virtual async void GetGameBayState()
        {
            try
            {
                status = "getting game state";
                await connection.InvokeAsync(SignalROut.GetGameBayState);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error(status);
                if (connection.State == HubConnectionState.Disconnected) await Connect();
            }
        }

        public virtual async void UpdateGameState(GameBayData data)
        {
            try
            {
                Log.Debug("-------------------------updating game state\n" + data);
                status = "updating game state";
                await SendData(SignalROut.GameBayStateUpdate, data);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error(status);
                if (connection.State == HubConnectionState.Disconnected) await Connect();
            }
        }

        protected virtual async Task SendData(SignalRMessage message)
        {
            try
            {
                if (message.Key != -1)
                    await connection.InvokeAsync(message.ActionName, message.Guid, message.Key);
                else if (message.Data != null)
                    await connection.InvokeAsync(message.ActionName, message.Guid, message.Data);
                else if (message.SendBool)
                    await connection.InvokeAsync(message.ActionName, message.Guid, message.BoolVal);
                else
                    await connection.InvokeAsync(message.ActionName, message.Guid);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error(message.ActionName + " >>> failed " + status);

                if (_useMessageQueue) QueueMessage(message);
                else if (connection.State == HubConnectionState.Disconnected) await Connect();
            }
        }

        protected virtual async Task SendData(string actionName, DataModelBase data, int key = -1,
            bool sendBool = false, bool boolVal = false)
        {
            var message = new SignalRMessage(actionName, data, key, sendBool, boolVal);
            if (_useMessageQueue) QueueMessage(message);
            else await SendData(message);
        }

        private void QueueMessage(SignalRMessage message)
        {
            var found = false;
            foreach (var x in _messageQueue)
                if (x.Guid == message.Guid)
                    found = true;
            if (found == false) _messageQueue.Enqueue(message);
        }

        protected virtual async void SendGuid(Guid guid)
        {
            try
            {
                await connection.InvokeAsync(SignalROut.SendACK, guid);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error($"{SignalROut.SendACK} >>> failed {status}");
            }
        }

        protected virtual async Task AddGameBayToGroup(string actionName, string groupName)
        {
            Log.Debug($"{actionName} {groupName}");
            try
            {
                await connection.InvokeAsync(actionName, groupName);
                status = "Adding to " + groupName;
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error(actionName + " failed " + status);
                if (connection.State == HubConnectionState.Disconnected) await Connect();
            }
        }

        public virtual async void UpdateCameraStatus(TrackingSystemData data, bool startup)
        {
            try
            {
                status = $"Sending {SignalROut.UpdateCameraStatus}";
                await connection.InvokeAsync(SignalROut.UpdateCameraStatus, data, startup);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                Log.Error(status);
                if (connection.State == HubConnectionState.Disconnected) await Connect();
            }
        }

        protected virtual void OnTaskComplete(string msg = "")
        {
            Log.Debug($"OnTaskComplete {msg} | {status}");
        }

        protected void ClearMessageQueue()
        {
            while (_messageQueue.IsEmpty == false)
            {
                _messageQueue.TryDequeue(out _);
            }
        }
    }
}