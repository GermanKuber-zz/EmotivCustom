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
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ISocketServerService, SocketServerService>();
            //services.AddHangfire(x => x.UseSqlServerStorage("<connection string>"));
            services.AddHangfire(configuration => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ISocketServerService sockerServerService)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Demo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebSockets();

            app.Run(async (context) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
            });
            var jobId = BackgroundJob.Enqueue(
      () => RunEmotivServer());
        }
        public static void RunEmotivServer()
        {
            var emoWrapper = new EmoWrapper(ConnectedTypeEnum.Emulator);

            emoWrapper.GiroscopioMovement.Subscribe(position =>
            {
            });

            emoWrapper.FacialExpression.Subscribe(e =>
            {

            });

            emoWrapper.EngineConnected.Subscribe(evt =>
            {
            });
            emoWrapper.EngineDisconnected.Subscribe(evt =>
            {
            });
            emoWrapper.EngineStateUpdated.Subscribe(evt =>
            {
            });
            emoWrapper.MentalCommand.Subscribe(evt =>
            {
            });
            //emoWrapper.EngineConnected.Subscribe(x =>
            //{
            //    Console.WriteLine(x);
            //});
            emoWrapper.ProcessEngine();
        }
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var buffer2 = new byte[1024 * 4];
                byte[] array = Encoding.ASCII.GetBytes("holaaa");

                await webSocket.SendAsync(new ArraySegment<byte>(buffer2, 0, array.Count()), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        private async Task Echo(ISocketServerService sockerServerService)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await sockerServerService.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await sockerServerService.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await sockerServerService.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await sockerServerService.WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
