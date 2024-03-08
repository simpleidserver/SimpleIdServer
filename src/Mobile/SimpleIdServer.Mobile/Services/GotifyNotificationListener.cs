using Plugin.Firebase.CloudMessaging;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile.Services
{
    public class GotifyNotificationListener
    {
        private static GotifyNotificationListener _instance;
        private bool _isStarted;
        private ClientWebSocket _client;
        private CancellationTokenSource _cancellationTokenSource;

        public static GotifyNotificationListener New()
        {
            if(_instance == null)
            {
                _instance = new GotifyNotificationListener();
            }

            return _instance;
        }

        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
        }

        public async Task Start(string wsServer, string token, CancellationToken cancellationToken)
        {
            if (_isStarted) return;
            _client = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            await _client.ConnectAsync(new Uri($"{wsServer}/stream?token={token}"), cancellationToken);
            Task.Run(async () =>
            {
                while(true)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                    var buffer = new byte[2048];
                    var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                    if(result.MessageType == WebSocketMessageType.Text)
                    {
                        var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var jObj = JsonObject.Parse(msg);
                        var data = (jObj["extras"] as JsonObject).ToDictionary(j => j.Key, k => k.Value.ToString());
                        App.Current.Dispatcher.DispatchAsync(async () =>
                        {
                            var fcmNotification = new Plugin.Firebase.CloudMessaging.FCMNotification(jObj["message"].ToString(),
                                jObj["title"].ToString(),
                                data: data);
                            if (NotificationReceived != null) NotificationReceived(this, new NotificationEventArgs(fcmNotification));
                        });
                    }
                    
                    if(result.MessageType == WebSocketMessageType.Close)
                    {
                        await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
                        break;
                    }
                }
            });
            _isStarted = true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            _client.Dispose();
            _isStarted = false;
        }
    }

    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(FCMNotification notification)
        {
            Notification = notification;
        }

        public FCMNotification Notification { get; private set; }
    }
}
