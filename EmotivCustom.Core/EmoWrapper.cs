using System;
using System.Linq;
using Emotiv;
using System.Reactive.Linq;
using System.Reactive;
using EmotivCustom.Core.Extensions;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using static Emotiv.EmoEngine;
using static Emotiv.EdkDll;

namespace MultiDongles
{
    /// <summary>
    /// Se ecarga de Wrappear el framework de eventos a Observables
    /// </summary>
    public class EmoWrapper
    {
        #region Const

        private const String IP_EMULATOR = "127.0.0.1";
        private const UInt16 PORT_EMULATOR = 1726;

        #endregion

        #region Private vars

        private int _processEvents;
        private EmoEngine _engine;
        private FacialExpression _facialExpressionDto = new FacialExpression();
        private MentalCommand _mentalCommand = new MultiDongles.MentalCommand();
        private uint _userId;
        private ReplaySubject<bool> source;

        #endregion

        #region Observables

        public IObservable<GiroscopioMovement> GiroscopioMovement { get; private set; }
        public IObservable<MentalCommand> MentalCommand { get; private set; }
        public IObservable<FacialExpression> FacialExpression { get; private set; }
        public ReplaySubject<bool> EngineConnected { get; private set; }
        public ReplaySubject<bool> EngineDisconnected { get; private set; }
        public ReplaySubject<bool> EngineStateUpdated { get; private set; }

        #endregion

        #region Public Obversables


        #endregion

        #region Construct

        public EmoWrapper(ConnectedTypeEnum connectedType, int processEvents = 1000)
        {
            _engine = EmoEngine.Instance;

            _processEvents = processEvents;

            CreateObservablesFacial();

            CreateObservablesEngine();

            CreateObservableMental();

            if (connectedType == ConnectedTypeEnum.Emulator)
                _engine.RemoteConnect(IP_EMULATOR, PORT_EMULATOR);
            else if (connectedType == ConnectedTypeEnum.Device)
                _engine.Connect();

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mantengo un While, para que cheque el estado del casco
        /// </summary>
        public void ProcessEngine()
        {
            while (true)
                _engine.ProcessEvents(_processEvents);
        }

        #endregion

        #region Privated Methods

        /// <summary>
        /// Creo los obervables correspondiente a los eventos de Connected, Disconnected y Updated
        /// </summary>
        private void CreateObservablesEngine()
        {
            EngineConnected = new ReplaySubject<bool>();
            _engine.EmoEngineConnected += Engine_EmoEngineConnected;

            EngineDisconnected = new ReplaySubject<bool>();
            _engine.EmoEngineDisconnected += Engine_EmoEngineDisconnected;

            EngineStateUpdated = new ReplaySubject<bool>();
            _engine.EmoEngineEmoStateUpdated += Engine_EmoEngineEmoStateUpdated;
        }

        private void Engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            //Logger.Write($"StateUpdated");
            EngineStateUpdated.OnNext(true);
        }

        private void Engine_EmoEngineDisconnected(object sender, EmoEngineEventArgs e)
        {
            Logger.Write($"Disconnected");
            EngineDisconnected.OnNext(true);
        }

        private void Engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
        {
            Logger.Write($"Connected");
            Logger.Write($"User : {e.userId}");
            EngineConnected.OnNext(true);
        }
        private void CreateObservableMental()
        {


            MentalCommand = Observable.FromEventPattern<EmoStateUpdatedEventArgs>(_engine, nameof(_engine.MentalCommandEmoStateUpdated))
                .Select(evt =>
                {
                    if (CheckMentalCommand(evt.EventArgs.emoState))
                        Logger.Write($"Mental Command - Type: {_mentalCommand.Type} - Power : {_mentalCommand.Power}");


            return _mentalCommand;
                });



        }

        private void CreateObservablesFacial()
        {
            GiroscopioMovement =
               Observable.FromEventPattern<EmoStateUpdatedEventArgs>(_engine, nameof(_engine.EmoStateUpdated))
               .Select(evt =>
               {
                   //Recibe la posición X y la posición Y al moverse la cabeza
                   EmoState es = evt.EventArgs.emoState;
                   int x = 1, y = 1;
                   _engine.HeadsetGetGyroDelta(evt.EventArgs.userId, out x, out y);

                   if (x != 0 && y != 0)
                       Logger.Write($"Gyro Delta - X: {x} - Y: {y}");

                   return new GiroscopioMovement(x, y);
               });

            FacialExpression = Observable.FromEventPattern<EmoStateUpdatedEventArgs>(_engine, nameof(_engine.FacialExpressionEmoStateUpdated))
                .Select(evt =>
                {
                    //recibo todos los eventos de la face
                    EmoState es = evt.EventArgs.emoState;

                    //Indica si el evento recibido es del tipo face
                    //tambien en caso que se reciba un evento repetido, y se verifique que ninguna propiedad se modifico de face
                    //esta variable tmb mantendra en false
                    var wasFaceExpression = false;
                    //IDEM anterior
                    //indica si el evento es del tipo eye
                    //tambien en caso que se reciba un evento repetido, y se verifique que ninguna propiedad se modifico de face
                    //esta variable tmb mantendra en false
                    var wasEye = CheckEyeEvent(es);
                    if (wasEye)
                        Logger.Write($"Facial Expression - Type: {_facialExpressionDto.Eyes.Type}");

                    if (!wasEye)
                    {
                        //Si no fue un evento del tipo eye
                        wasFaceExpression = CheckFaceExpression(es);
                        if (wasFaceExpression)
                            Logger.Write($"Facial Expression - Type: {_facialExpressionDto.LowerFace.Type} - Power: {_facialExpressionDto.LowerFace.Power}");
                    }
                    //En caso que el evento haya sido de alguno de los tipos, quiere decir que alguna propiedad se haya upgradeado se retorna el dto
                    if (wasEye || wasFaceExpression)
                        return _facialExpressionDto;

                    else

                        //no debo retornar nada, debo anular el observable
                        return _facialExpressionDto;

                });



        }

        /// <summary>
        /// Chequea si el evento que recibe es de alguno de los del tipo face
        /// en caso de serlo setea la propiedad del objeto _facialExpressionDto
        /// </summary>
        /// <param name="emoState">Evento recibido</param>
        /// <returns></returns>
        private bool CheckFaceExpression(EmoState emoState)
        {
            var wasLowerExpression = false;
            var power = emoState.FacialExpressionGetLowerFaceActionPower();
            if (emoState.FacialExpressionGetLowerFaceAction() == EdkDll.IEE_FacialExpressionAlgo_t.FE_SMILE)
            {
                wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power);
                _facialExpressionDto.LowerFace.Power = power;
                _facialExpressionDto.LowerFace.Type = LowerFaceEnum.Smile;
            }
            if (emoState.FacialExpressionGetLowerFaceAction() == EdkDll.IEE_FacialExpressionAlgo_t.FE_CLENCH)
            {
                wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power); ;
                _facialExpressionDto.LowerFace.Power = power;
                _facialExpressionDto.LowerFace.Type = LowerFaceEnum.Clench;
            }
            if (emoState.FacialExpressionGetLowerFaceAction() == EdkDll.IEE_FacialExpressionAlgo_t.FE_LAUGH)
            {
                wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power); ;
                _facialExpressionDto.LowerFace.Power = power;
                _facialExpressionDto.LowerFace.Type = LowerFaceEnum.Laugh;
            }

            if (emoState.FacialExpressionGetLowerFaceAction() == EdkDll.IEE_FacialExpressionAlgo_t.FE_SMIRK_LEFT)
            {
                wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power); ;
                _facialExpressionDto.LowerFace.Power = power;
                _facialExpressionDto.LowerFace.Type = LowerFaceEnum.SmirkLeft;
            }
            if (emoState.FacialExpressionGetLowerFaceAction() == EdkDll.IEE_FacialExpressionAlgo_t.FE_SMIRK_RIGHT)
            {
                wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power); ;
                _facialExpressionDto.LowerFace.Power = power;
                _facialExpressionDto.LowerFace.Type = LowerFaceEnum.SmirkRight;
            }
            return wasLowerExpression;
        }


        /// <summary>
        /// Verifica si el evento recibido es del tipo eye
        /// </summary>
        /// <param name="emoState">Evento recibido </param>
        /// <returns></returns>
        private bool CheckEyeEvent(EmoState emoState)
        {
            //Se pone en true en caso de que el evento fue del tipo de los ojos
            var wasEyes = false;
            if (emoState.FacialExpressionIsBlink())
            {
                wasEyes = true;

                _facialExpressionDto.Eyes.Type = EyeExpressionEnum.Blink;
            }
            else if (emoState.FacialExpressionIsLeftWink())
            {
                wasEyes = true;
                _facialExpressionDto.Eyes.Type = EyeExpressionEnum.WinkLeft;
            }
            else if (emoState.FacialExpressionIsRightWink())
            {
                wasEyes = true;
                _facialExpressionDto.Eyes.Type = EyeExpressionEnum.WinkRight;
            }
            else if (emoState.FacialExpressionIsLookingLeft() == 1)
            {
                wasEyes = true;
                _facialExpressionDto.Eyes.Type = EyeExpressionEnum.LookLeft;
            }
            else if (emoState.FacialExpressionIsLookingRight() == 1)
            {
                wasEyes = true;
                _facialExpressionDto.Eyes.Type = EyeExpressionEnum.LookRight;
            }
            return wasEyes;
        }

        /// <summary>
        /// Verifica si el evento recibido es del tipo eye
        /// </summary>
        /// <param name="emoState">Evento recibido </param>
        /// <returns></returns>
        private bool CheckMentalCommand(EmoState emoState)
        {
            //Se pone en true en caso de que el evento fue del tipo de los ojos
            var wasMentalCommand = false;
            var power = emoState.MentalCommandGetCurrentActionPower();

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_NEUTRAL, power, MentalCommandEnum.Neutral) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_NEUTRAL, power, MentalCommandEnum.Neutral);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_PUSH, power, MentalCommandEnum.Push) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_PUSH, power, MentalCommandEnum.Push);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_PULL, power, MentalCommandEnum.Pull) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_PULL, power, MentalCommandEnum.Pull);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_LIFT, power, MentalCommandEnum.Lift) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_LIFT, power, MentalCommandEnum.Lift);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_DROP, power, MentalCommandEnum.Drop) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_DROP, power, MentalCommandEnum.Drop);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_LEFT, power, MentalCommandEnum.Left) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_LEFT, power, MentalCommandEnum.Left);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_RIGHT, power, MentalCommandEnum.Right) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_RIGHT, power, MentalCommandEnum.Right);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_LEFT, power, MentalCommandEnum.RotateLeft) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_LEFT, power, MentalCommandEnum.RotateLeft);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_RIGHT, power, MentalCommandEnum.RotateRight) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_RIGHT, power, MentalCommandEnum.RotateRight);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_CLOCKWISE, power, MentalCommandEnum.RotateClockWise) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_CLOCKWISE, power, MentalCommandEnum.RotateClockWise);

            //TODO: chequear el estado si es correcto la correspondencia
            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_COUNTER_CLOCKWISE, power, MentalCommandEnum.RoSe) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_COUNTER_CLOCKWISE, power, MentalCommandEnum.RoSe);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_FORWARDS, power, MentalCommandEnum.RotateFoward) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_FORWARDS, power, MentalCommandEnum.RotateFoward);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_REVERSE, power, MentalCommandEnum.RotateReverse) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_ROTATE_REVERSE, power, MentalCommandEnum.RotateReverse);

            wasMentalCommand = !wasMentalCommand ? wasMentalCommand = _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_DISAPPEAR, power, MentalCommandEnum.Disappear) : _mentalCommand.IsToSetType(emoState, IEE_MentalCommandAction_t.MC_DISAPPEAR, power, MentalCommandEnum.Disappear);
            return wasMentalCommand;
        }



        #endregion
    }
}
