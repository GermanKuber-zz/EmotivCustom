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
    public interface ISocketServerService
    {
        WebSocket WebSocket { get; set; }
        HttpContext Context { get; set; }
        Task AcceptWebSocketAsync(HttpContext context);
        Task SendMessage(string message);
    }
}
