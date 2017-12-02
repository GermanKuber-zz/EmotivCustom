namespace MultiDongles
{
    public class MentalCommand
    {
        public MentalCommandEnum Type { get; private set; }
        private bool _change = false;
        public void Change(MentalCommandEnum type, float power)
        {
            _change = true;
            this.Type = type;
        }
        public bool WasChanged
        {
            get
            {
                var returnValue = _change;
                _change = false;
                return returnValue;
            }
        }

        public float Power { get; private set; }

    }
}
