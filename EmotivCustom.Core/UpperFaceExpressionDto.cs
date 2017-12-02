namespace MultiDongles
{
    public class UpperFaceExpression
    {
        public UpperFaceEnum Type { get; private set; }
        public float Power { get; private set; }
        private bool _change = false;
        public void Change(UpperFaceEnum type, float power)
        {
            _change = true;
            this.Type = type;
            this.Power = power;
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
    }
}
