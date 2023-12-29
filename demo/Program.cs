using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

class WebSocketServer
{
    public static async Task StartServerAsync()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:18991/");
        listener.Start();

        Console.WriteLine("Listening...");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                ProcessRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async void ProcessRequest(HttpListenerContext context)
    {
        HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
        WebSocket webSocket = webSocketContext.WebSocket;

        try
        {
            byte[] buffer = new byte[1024];
            var num = 0;
            while (true)
            {
                await Task.Delay(2000);

                string message = DateTime.Now.ToString("HH:mm:ss");
                buffer = System.Text.Encoding.UTF8.GetBytes(message);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, message.Length),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                num++;
                if (num >= 2)
                {
                    Console.WriteLine("Start disconnecting the test!");
                    break;
                }
            }
        }
        catch (WebSocketException) { }
        finally
        {
            webSocket.Dispose();
        }
    }

    public static void Main(string[] args)
    {
        Task.Run(async () => { await StartServerAsync(); }).Wait();
    }
}
