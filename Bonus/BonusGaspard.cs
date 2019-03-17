public class BonusGaspard : AbstractBonus
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
    public BonusGaspard(BonusEntity p_bonusEntity) : base(p_bonusEntity)
    {
        
    }
    public BonusGaspard(int p_id, int p_amountUsage) : base(p_id, p_amountUsage)
    {
        SET_Name("Gaspard");
        SET_AmountUsage(p_amountUsage);
    }
    public BonusGaspard(int p_id) : base(p_id, 1)
    {
    }
    #endregion
}