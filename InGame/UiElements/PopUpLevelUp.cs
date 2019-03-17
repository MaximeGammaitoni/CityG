using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpLevelUp : AbstractPopUp {
    #region PUBLIC PROPERTIES
    public GameObject levelUpTextGO;
    public GameObject levelUpGraphicGO;
    public GameObject levelUpButtonGO;
    #endregion
    #region PROTECTED PROPERTIES
    protected Text levelUpText;
    protected Text LevelUpGraphicText;
    protected Button levelUpButton;
    protected Animator animator;
    #endregion
    #region PUBLIC METHODS
    public override void Close()
    {
        GameManager.singleton.StartCouroutineInGameManager(SmoothClose());
    }

    public override void Close(string p_args)
    {
        throw new System.NotImplementedException();
    }

    public override void Open()
    {
        GameManager.singleton.StartCouroutineInGameManager(SmoothOpen());
    }

    public IEnumerator SmoothOpen()
    {
        uiElementGameObject.SetActive(true);
        animator.Play("FadeInUp");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeInUp"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
    }
    public IEnumerator SmoothClose()
    {
        animator.Play("FadeOutDown");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOutDown"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        uiElementGameObject.SetActive(false);
    }

    public override void Open(string p_args)
    {
        throw new System.NotImplementedException();
    }
    public void SET_UpdateLevelToDisplay()
    {
        LevelUpGraphicText.text = GameManager.singleton.playerManager.GetPlayerLevel().ToString();
    }
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region CONSTRUCTORS
    public PopUpLevelUp (string p_uiElementName) : base(p_uiElementName)
    {
        if (uiElementGameObject != null)
        {
            //GAMEOBJECTS
            levelUpTextGO = uiElementGameObject.transform.GetChildByPath("Content_Viewport/LevelInfo_Viewport/NextLevel_Text").gameObject;
            levelUpGraphicGO = uiElementGameObject.transform.GetChildByPath("Content_Viewport/LevelInfo_Viewport/Graphic_Viewport").gameObject;
            levelUpButtonGO = uiElementGameObject.transform.GetChildByPath("Content_Viewport/LevelInfo_Viewport/NextLevel_Button").gameObject;
            //COMPONENTS
            if (levelUpTextGO)
                levelUpText = levelUpTextGO.GetComponent<Text>();
            if (levelUpGraphicGO)
                LevelUpGraphicText = levelUpGraphicGO.transform.GetChildByPath("Level_Text").GetComponent<Text>();
            if (levelUpButtonGO)
                levelUpButton = levelUpButtonGO.GetComponent<Button>();

            animator = uiElementGameObject.GetComponent<Animator>();
        }
    }

    
    #endregion
}
