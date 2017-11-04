namespace MultiDongles
{
    public class FacialExpression
    {
        public EyeExpression Eyes { get; set; } = new EyeExpression();
        public UpperFaceExpression UpperFace { get; set; } = new UpperFaceExpression();
        public LowerFaceExpression LowerFace { get; set; } = new LowerFaceExpression();

    }
}
