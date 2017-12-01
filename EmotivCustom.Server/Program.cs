using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hangfire;
using MultiDongles;

namespace EmotivCustom.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
      
            
            BuildWebHost(args).Run();
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
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
