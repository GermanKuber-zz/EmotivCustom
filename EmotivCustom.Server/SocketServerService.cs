using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Hangfire;
using MultiDongles;

namespace EmotivCustom.Server
{
    public class SocketServerService : ISocketServerService
    {
        public WebSocket WebSocket { get; set; }
        public HttpContext Context { get; set; }

        public async Task AcceptWebSocketAsync(HttpContext context)
        {
            Context = context;
            WebSocket = await context.WebSockets.AcceptWebSocketAsync();

        }
        public async Task SendMessage(string message) {

            try
            {

                byte[] array = Encoding.ASCII.GetBytes(message);
          

                await WebSocket.SendAsync(new ArraySegment<byte>(array, 0, array.Count()), WebSocketMessageType.Text, true, CancellationToken.None);


            }
            catch (Exception ex)
            {

                throw;
            }
            }
        
    }
}
