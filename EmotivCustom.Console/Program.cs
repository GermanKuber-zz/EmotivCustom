/****************************************************************************
**
** Copyright 2015 by Emotiv. All rights reserved
** Example - Multi Dongle Connection
** this example demonstrates how to connect to two headsets at the same time
** It captures event when you plug or unplug dongle .
** Every time you plug or unplug dongle, there is a notice that dongle ID
** is added or removed
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Emotiv;
using System.Reactive.Linq;
using EmotivCustom.SerialPortDriver;
using System.Threading.Tasks;

namespace MultiDongles
{
    class Program
    {
        EmoEngine engine;
        private  SerialPortInterface _serialPortInterface;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Serial();
            program.mainLoop();
        }
        void Serial() {
            _serialPortInterface = new SerialPortInterface("COM5");
        }
        void mainLoop()
        {
            var emoWrapper = new EmoWrapper(ConnectedTypeEnum.Emulator);

            emoWrapper.GiroscopioMovement.Subscribe(position =>
            {
            });

            emoWrapper.FacialExpression.Subscribe(e =>
            {
                if (e.Eyes.Type == EyeExpressionEnum.Blink) {
                    _serialPortInterface.Write(07, 0);
                    _serialPortInterface.Write(07, 250);
                    Task.Factory.StartNew(() =>
                    {
                        Task.Delay(500).Wait();
                        Console.WriteLine("Done");
                        _serialPortInterface.Write(07, 0);
                    });
                }
             
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
            //engine = EmoEngine.Instance;
            //engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);

            //IObservable<EventPattern<EmoStateUpdatedEventArgs>> move =
            //    Observable.FromEventPattern<EmoStateUpdatedEventArgs>(engine, "EmoStateUpdated");

            //move.Subscribe(evt =>
            //{
            //    EmoState es = evt.EventArgs.emoState;
            //    int x = 1;
            //    int y = 1;
            //    engine.HeadsetGetGyroDelta(evt.EventArgs.userId, out x, out y);
            //    Console.WriteLine($"X: {x}, Y: {y}");
            //});

            //engine.FacialExpressionEmoStateUpdated += Engine_FacialExpressionEmoStateUpdated;
            //engine.MentalCommandEmoStateUpdated += Engine_MentalCommandEmoStateUpdated;
            ////engine.EmoStateUpdated += ;
            //engine.MentalCommandSignatureUpdated += Engine_MentalCommandSignatureUpdated;
            //engine.EmoEngineConnected += Engine_EmoEngineConnected;
            //engine.EmoEngineDisconnected += Engine_EmoEngineDisconnected;
            //engine.EmoEngineEmoStateUpdated += Engine_EmoEngineEmoStateUpdated;
            //engine.MentalCommandAutoSamplingNeutralCompleted += Engine_MentalCommandAutoSamplingNeutralCompleted;

            //engine.MentalCommandTrainingStarted += Engine_MentalCommandTrainingStarted;
            //engine.MentalCommandTrainingSucceeded += Engine_MentalCommandTrainingSucceeded;
            //engine.MentalCommandTrainingDataErased += Engine_MentalCommandTrainingDataErased;
            //engine.MentalCommandTrainingCompleted += Engine_MentalCommandTrainingCompleted;
            //engine.MentalCommandTrainingFailed += Engine_MentalCommandTrainingFailed;
            //engine.MentalCommandTrainingRejected += Engine_MentalCommandTrainingRejected;
            //engine.MentalCommandTrainingReset += Engine_MentalCommandTrainingReset;

            ////engine.Connect();

            //engine.RemoteConnect("127.0.0.1", 1726);
            //while (true)
            //{
            //    engine.ProcessEvents(1000);
            //}

        }

        private void Engine_MentalCommandTrainingReset(object sender, EmoEngineEventArgs e)
        {
        }
        private void Engine_MentalCommandTrainingRejected(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_MentalCommandTrainingFailed(object sender, EmoEngineEventArgs e)
        {
        }
        private void Engine_MentalCommandTrainingCompleted(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_MentalCommandTrainingDataErased(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_MentalCommandTrainingSucceeded(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_MentalCommandTrainingStarted(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            foreach (var item in e.emoState.GetContactQualityFromAllChannels())
            {
                if (item == EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_POOR)
                {
                    var a = "";
                }

            }
        }

        private void Engine_MentalCommandAutoSamplingNeutralCompleted(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_MentalCommandSignatureUpdated(object sender, EmoEngineEventArgs e)
        {
        }



        private void Engine_EmoEngineDisconnected(object sender, EmoEngineEventArgs e)
        {
        }

        private void Engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
        {
            //EmoEngine.Instance.MentalCommandSetTrainingAction(1, EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_REVERSE);


            //EmoEngine.Instance.MentalCommandSetTrainingControl(1, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
        }

        private void Engine_MentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            if (es.MentalCommandGetCurrentAction() == EdkDll.IEE_MentalCommandAction_t.MC_LEFT)
            {
                var a = "";
                var aa = es.MentalCommandGetCurrentActionPower();
            }
            if (es.MentalCommandGetCurrentAction() == EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL)
            {
                var a = "";
                var aa = es.MentalCommandGetCurrentActionPower();
            }
            var facialExpressionGetUpperFaceActionPower = es.FacialExpressionGetUpperFaceActionPower();
            Console.WriteLine("User " + e.userId + "  Time :  " + es.GetTimeFromStart());
        }

        private void Engine_FacialExpressionEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            if (es.FacialExpressionIsBlink())
            {

                var a = "";
            }
            var facialExpressionGetUpperFaceActionPower = es.FacialExpressionGetUpperFaceActionPower();
            Console.WriteLine("User " + e.userId + "  Time :  " + es.GetTimeFromStart());
        }

        private void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            int x = 1;
            int y = 1;
            engine.HeadsetGetGyroDelta(e.userId, out x, out y);
            Console.WriteLine($"X: {x}, Y: {y}");
            Console.WriteLine("User " + e.userId + "  Time :  " + es.GetTimeFromStart());
        }
    }
}
