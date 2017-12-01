using System;
using System.Collections.Generic;

namespace EmotivCustom.Lights
{
    public class LightsManager

    {
        private List<Light> _lights;



        public LightsManager(List<Light> lights)
        {
            _lights = lights;

        }

        public void Reset()
        {
            if (_lights != null)
                foreach (var item in _lights)
                    item.Value = 0;

        }
        public Light GetLightByPin(int pin)
        {
            if (_lights != null)
                foreach (var item in _lights)
                    if (item.Pin == pin)
                        return item;
            return null;
        }

    }
}
