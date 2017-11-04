/****************************************************************************
**
** Copyright 2015 by Emotiv. All rights reserved
** Example - Multi Dongle Connection
** this example demonstrates how to connect to two headsets at the same time
** It captures event when you plug or unplug dongle .
** Every time you plug or unplug dongle, there is a notice that dongle ID
** is added or removed
****************************************************************************/


namespace MultiDongles
{
    public class GiroscopioMovement
    {
        public GiroscopioMovement(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
