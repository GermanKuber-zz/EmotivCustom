namespace MultiDongles
{
    public class LowerFaceExpression
    {
        public LowerFaceEnum Type { get; private set; }
        public float Power { get; private set; }
        private bool _change = false;
        public void Change(LowerFaceEnum type, float power)
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
