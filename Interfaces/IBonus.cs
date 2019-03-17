using System;
public interface IBonus
{
    #region SET
    void SET_Name(string p_value);
    void SET_Description(string p_value);
    void SET_AmountUsage(int p_value);
    void SET_ID(int p_value);
    void SET_KEYID(string p_value);
    void SET_ReceivedDateTime(DateTime p_value);
    #endregion
    #region GET
    string GET_Name();
    string GET_Description();
    int GET_AmountUsage();
    int GET_ID();
    string GET_KEYID();
    DateTime GET_ReceivedDateTime();
    bool GET_Selected();
    #endregion
    #region ACTIONS
    void Select();
    void Deselect();
    void Use();
    #endregion
}