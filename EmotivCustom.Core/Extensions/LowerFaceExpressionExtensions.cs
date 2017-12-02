using Emotiv;
using MultiDongles;
using System;
using System.Collections.Generic;
using System.Text;
using static Emotiv.EdkDll;

namespace EmotivCustom.Core.Extensions
{
    public static class LowerFaceExpressionExtensions
    {
        public static bool IsTheSameThanPrevious(this LowerFaceExpression current, LowerFaceEnum newType, float newPower)
        {
            if (current.Type == newType && current.Power == newPower)
                return true;

            return false;

        }
        /// <summary>
        /// Chequea si el evento que recibe es del tipo typeToCompare, en caso de serlo setea las propiedades del MentalCommand
        /// </summary>
        /// <param name="emoState">Evento contra el cual se comparara</param>
        /// <param name="typeToCompare">Tipo de evento sobre el cual se intenta comparar</param>
        /// <param name="powerToSet">Si la comparación de tipo de eventos fue exitosá se setea este power</param>
        /// <param name="typeToSet">Si la comparación de tipo de eventos fue exitosá se setea este type</param>
        public static bool IsToSetType(this MentalCommand mentalCommand, EmoState emoState, IEE_MentalCommandAction_t typeToCompare, float powerToSet, MentalCommandEnum typeToSet)
        {

            if (emoState.MentalCommandGetCurrentAction() == typeToCompare)
            {
                //wasLowerExpression = _facialExpressionDto.LowerFace.IsTheSameThanPrevious(_facialExpressionDto.LowerFace.Type, power);
                mentalCommand.Change(typeToSet, powerToSet);
                return true;
            }
            return false;
        }
    }

    public static class UpperFaceExpressionExtensions
    {
        public static bool IsTheSameThanPrevious(this UpperFaceExpression current, UpperFaceEnum newType, float newPower)
        {
            if (current.Type == newType && current.Power == newPower)
                return true;

            return false;

        }
       
    }
}
