public class BonusPunto : AbstractBonus
{
    #region EVENTS
    #endregion
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #endregion
    #region CONSTRUCTORS
    public BonusPunto(BonusEntity p_bonusEntity) : base(p_bonusEntity)
    {

    }
    public BonusPunto(int p_id, int  p_amountUsage) : base(p_id, p_amountUsage)
    {
        SET_Name("Punto");
        SET_AmountUsage(p_amountUsage);
        SET_ID(p_id);
    }
    public BonusPunto(int p_id) : base(p_id, 1)
    {
        SET_Name("Punto");
        SET_AmountUsage(1);
        SET_ID(p_id);
    }
    #endregion
}