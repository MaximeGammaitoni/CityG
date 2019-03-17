using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUpWinManager : MonoBehaviour {

    public GameObject winPopUpGameObject;
    public Vector3 targetPosition;
    public bool canInteract;
    [HideInInspector] public delegate void WinPopUpEvent();
    [HideInInspector] public static event WinPopUpEvent OnOpen;
    [HideInInspector] public static event WinPopUpEvent OnClose;
    [HideInInspector] public static event WinPopUpEvent OnCloseTips;

    protected GameObject scoreGameObject;
    protected GameObject popUpTips;
    protected GameObject popUpInfo;
    protected JaugeXp gaugeXP;
    protected PopUpLevelUp levelUpPopUp;
    protected Vector3 popUpTipsOriginScale;
    protected Vector3 popUpInfoOriginScale;
    protected Vector3 winPopUpOriginPosition;
    protected Animator animator;

    protected RectTransform popUpWinRT;
    void CallLevelUpPopUp()
    {
        levelUpPopUp.Open();
        gaugeXP.OnAnimationEnd -= CallLevelUpPopUp;
    }
    void ManagePopUpTips()
    {

    }
    public void AnimateXPGauge()
    {
        if (gaugeXP != null)
        {
            gaugeXP.AnimateFill();
        }
    } 
    public void UpdateScore(int p_value)
    {
        if(scoreGameObject!=null && scoreGameObject.GetComponent<Text>())
            scoreGameObject.GetComponent<Text>().text = "Score : " + p_value.ToString();
    }
    public void LoadAndAssignGazTips()
    {
        GameManager.singleton.StartCouroutineInGameManager(AskForTips());
    }
    protected IEnumerator AskForTips()
    {
        System.Threading.Tasks.Task<Firebase.Database.DataSnapshot> l_output = GameManager.singleton.inGameUiManager.gazTipsGateway.GET_RandomGazTips();
        System.Func<bool> l_waiter = () =>
        {
            return (l_output.IsCompleted || l_output.IsCanceled || l_output.IsFaulted) ? true : false;
        };
        yield return new WaitUntil(l_waiter);
        if (!l_output.IsCompleted || l_output.IsCanceled || l_output.IsFaulted)
            yield break;
        //Convert JSON TO Dictionary<string,GazTipEntity>
        Dictionary<string, string> l_dicGazTips = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(l_output.Result.GetRawJsonValue());
        //Dictionary<string, KeyValuePair<string,string>> l_dicGazTips = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, KeyValuePair<string, string>>>(l_output.Result.GetRawJsonValue());
        //Get Random Tip
        if(l_dicGazTips==null || l_dicGazTips.Count == 0)
        {
            yield break;
        }
        List<string> l_keyList = new List<string>(l_dicGazTips.Keys);

        string randKey = l_keyList[Random.Range(0, l_keyList.Count)];

        GazTipsEntity l_gazToAssign = new GazTipsEntity();
        l_gazToAssign.rawTextToDisplay = l_dicGazTips[randKey];
        //Assign GazTip To GameObject
        popUpTips.transform.GetChildByPath("SpeechBubble_Graphic/Content_Text").GetComponent<UnityEngine.UI.Text>().text = l_gazToAssign.rawTextToDisplay;
    }
    public void UpdateXPGauge()
    {
        gaugeXP.UpdateGaugeFill();
    }
    void Awake()
    {
        if (PlayerPrefs.GetString("firstTimePlayingMatchThree","true") == "true")
        {
            winPopUpGameObject.transform.GetChildByPath("Content_Viewport/WinInfo_Viewport/NextLevel_Button").gameObject.SetActive(false);
        }    
    }
    void Start() {
        canInteract = true;

        OnOpen += LaunchOpen;
        OnClose += LaunchClose;
        OnCloseTips += LaunchCloseTipsOpenInfo;
		//targetPosition = new Vector3(0, Screen.height*2,0);
        SceneManager.sceneUnloaded += OnSceneUnloaded;

		popUpTips = winPopUpGameObject.transform.GetChildByPath ("Content_Viewport/Tips_Viewport").gameObject;
		popUpInfo = winPopUpGameObject.transform.GetChildByPath ("Content_Viewport/WinInfo_Viewport").gameObject;
        scoreGameObject = winPopUpGameObject.transform.GetChildByPath("Content_Viewport/WinInfo_Viewport/Score_Text").gameObject;
        gaugeXP = new JaugeXp("XPGauge_Viewport");
        levelUpPopUp = new PopUpLevelUp("LevelUp_PopUp");
        gaugeXP.OnAnimationStart += () => {
            if (gaugeXP.levelDisplayed != GameManager.singleton.playerManager.GetPlayerLevel())
            {
                levelUpPopUp.SET_UpdateLevelToDisplay();
                gaugeXP.OnAnimationEnd += CallLevelUpPopUp;
            }
        };

        popUpTipsOriginScale = popUpTips.transform.localScale;
        popUpInfoOriginScale = popUpInfo.transform.localScale;
        winPopUpOriginPosition = winPopUpGameObject.transform.position;

        popUpWinRT = winPopUpGameObject.GetComponent<RectTransform>();
        animator = winPopUpGameObject.GetComponent<Animator>();

        winPopUpGameObject.SetActive(false);
    }

    public void Open()
    {
        OnOpen();
    }

    public void Close()
    {
        OnClose();
    }

    public void CloseTips()
    {
        OnCloseTips();
    }

    public void CloseLevelUpPopUp()
    {
        levelUpPopUp.Close();
    }

    public void OnSceneUnloaded (Scene scene)
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        OnOpen = null;
        OnClose = null;
        OnCloseTips = null;
    }
    private void LaunchOpen()
    {
        StartCoroutine(SmoothyOpen());
    }
    private void LaunchClose()
    {
        StartCoroutine(SmoothyClose());
    }

    private void LaunchCloseTipsOpenInfo()
    {
        StartCoroutine(CloseTipsOpenInfo());
    }

    IEnumerator SmoothyOpen()
    {
        //START Animation, Can't interact with it
        while (!canInteract)
        {
            yield return 0;
        }
        //Init parameters
        //Disable interaction
        canInteract = false;
        winPopUpGameObject.SetActive(true);
        popUpTips.transform.localScale = Vector3.zero;
        popUpTips.SetActive(false);
        popUpInfo.SetActive(false);
        popUpWinRT.anchoredPosition = new Vector2(0,0);
        //Play Fade In Animation
        animator.Play("FadeInUp");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeInUp"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        //Activate PopUptips GameObject for next animation
        popUpTips.SetActive(true);
        //Do next animation "Tips"
        while (popUpTips.transform.localScale.y < popUpTipsOriginScale.y)
        {
            Vector3 currentScale = popUpTips.transform.localScale;
            popUpTips.transform.localScale = Vector3.Lerp(currentScale, popUpTipsOriginScale + new Vector3(0.15f, 0.15f, 0), Time.deltaTime * 10);
            yield return 0;
        }
        popUpTips.transform.localScale = popUpTipsOriginScale;
        //Enable interaction
        canInteract = true;
    }

    IEnumerator CloseTipsOpenInfo()
    {
        while (!canInteract)
        {
            yield return 0;
        }
        canInteract = false;

        while (popUpTips.transform.localScale.y > Vector3.zero.y)
        {
            Vector3 currentScale = popUpTips.transform.localScale;
            popUpTips.transform.localScale = Vector3.Lerp(currentScale, Vector3.zero - new Vector3(0.15f, 0.15f, 0), Time.deltaTime * 10);
            yield return 0;
        }
        popUpTips.transform.localScale = popUpTipsOriginScale;
        popUpTips.SetActive(false);
        popUpInfo.SetActive(true);
        UpdateScore(SessionInfo.current.GetScore());
        popUpInfo.transform.localScale = Vector3.zero;
        while (popUpInfo.transform.localScale.y < popUpInfoOriginScale.y)
        {
            Vector3 currentScale = popUpInfo.transform.localScale;
            popUpInfo.transform.localScale = Vector3.Lerp(currentScale, popUpInfoOriginScale + new Vector3(0.15f, 0.15f, 0), Time.deltaTime * 10);
            yield return 0;
        }
        //Start XpUpdate;
        AnimateXPGauge();
        popUpInfo.transform.localScale = popUpInfoOriginScale;
        canInteract = true;
    }

    IEnumerator SmoothyClose()
    {

        canInteract = false;
        
        animator.Play("FadeOutUp");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOutUp"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        popUpInfo.SetActive(false);
        popUpTips.SetActive(false);
        canInteract = true;
        winPopUpGameObject.SetActive(false);

        LevelAssistant.SelectDesign(GameManager.singleton.playerManager.GetPlayerCurrentLevel());
    }
}
