public class DoubleExperienceCalculator : AbstractExperienceCalculator
{
    public override float AddExperiencePoints(float p_experiencePoints)
    {
        return p_experiencePoints * 2;
    }
}