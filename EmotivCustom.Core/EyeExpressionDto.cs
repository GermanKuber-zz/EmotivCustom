namespace MultiDongles
{
    public class EyeExpression
    {
        public EyeExpressionEnum Type { get; private set; }
        private bool _change = false;
        public void Change(EyeExpressionEnum type)
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
    }
}
