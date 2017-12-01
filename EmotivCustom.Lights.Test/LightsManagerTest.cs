using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmotivCustom.Lights.Test
{
    [TestClass]

    public class LightsManagerTest
    {
        [TestMethod]
        public async Task Reset_Vuelve_Todos_Los_Valores_A_Cero()
        {
            // Arrange

            var lights = new List<Light> {
                new Light {
                    Pin = 1,
                   Value = 250
                },
                 new Light {
                    Pin = 3,
                   Value = 120
                }
            };
            var lightsManager = new LightsManager(lights);

            // Act
            lightsManager.Reset();





            // Assert
            Assert.Equals(lightsManager.GetLightByPin(1).Value, 0);
            Assert.Equals(lightsManager.GetLightByPin(3).Value, 0);
        }
    }
}


