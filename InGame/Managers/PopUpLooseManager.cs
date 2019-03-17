using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopUpLooseManager : MonoBehaviour {
    #region EVENTS
    [HideInInspector] public delegate void LoosePopUpEvent();
    [HideInInspector] public static event LoosePopUpEvent OnOpen;
    [HideInInspector] public static event LoosePopUpEvent OnClose;
    #endregion
    #region PROTECTED PROPERTIES
    protected Vector3 _originPosition;
    #endregion
    #region PUBLIC PROPERTIES
    public GameObject popUpGO;
    public Vector3 targetPosition;
    public bool canInteract;
    #endregion
    #region PROTECTED METHODS
    protected Animator animator;
    #endregion
    #region PUBLIC METHODS

    #endregion

    void Start() {
        canInteract = true;
        OnOpen += LaunchOpen;
        OnClose += LaunchClose;
        targetPosition = new Vector3(0, 0,0);
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        _originPosition = popUpGO.GetComponent<RectTransform>().position;
        animator = popUpGO.GetComponent<Animator>();
        popUpGO.SetActive(false);
    }

    public void Open()
    {
        OnOpen();
    }

    public void Close()
    {
        OnClose();
    }

    public void OnSceneUnloaded (Scene scene)
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        OnOpen = null;
        OnClose = null;
    }
    private void LaunchOpen()
    {
        StartCoroutine(SmoothyOpen());
    }
    private void LaunchClose()
    {
        StartCoroutine(SmoothyClose());
    }

    IEnumerator SmoothyOpen()
    {
        while (!canInteract)
        {
            yield return 0;
        }
        canInteract = false;
        popUpGO.SetActive(true);

        //popUpGO.transform.localPosition = targetPosition - new Vector3(0, 500, 0);
        animator.Play("FadeInDown");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeInDown"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        canInteract = true;
    }

    IEnumerator SmoothyClose()
    {
        canInteract = false;
        animator.Play("FadeOutDown");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOutDown"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        canInteract = true;
        popUpGO.SetActive(false);
        LevelAssistant.SelectDesign(GameManager.singleton.playerManager.GetPlayerCurrentLevel());
    }
}
