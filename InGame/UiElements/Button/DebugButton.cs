using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButton : MonoBehaviour {
    protected Animator animator;
    protected Button button;
    protected bool isAnimated;

    void Awake()
    {
        animator = transform.GetComponentInParent<Animator>();
        button = transform.GetComponent<Button>();
    }
    public void FadeIn(string p_direction)
    {
        GameManager.singleton.StartCouroutineInGameManager(FadeInRoutine(p_direction));
    }
    public IEnumerator FadeInRoutine(string p_direction)
    {
        if (isAnimated)
            yield return new WaitWhile(() => isAnimated);
        string l_animationState = "FadeIn" + p_direction;
        animator.Play(l_animationState);
        yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName(l_animationState));
        isAnimated = true;
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        animator.Play("Idle");
        button.interactable = true;
        isAnimated = false;
        yield return null;
    }

    public void FadeOut(string p_direction)
    {
        GameManager.singleton.StartCouroutineInGameManager(FadeOutRoutine(p_direction));
    }
    public IEnumerator FadeOutRoutine(string p_direction)
    {
        if (isAnimated)
            yield return new WaitWhile(() => isAnimated);
        string l_animationState = "FadeOut" + p_direction;
        animator.Play(l_animationState);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(l_animationState));
        isAnimated = true;
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        //animator.StopPlayback();
        button.interactable = false;
        isAnimated = false;
        yield return null;
    }

	public void SkipToWin()
    {
        //FieldAssistant.main.RemoveField();
        //You Won
        SessionAssistant.main.StopCurrentSession(true);
        if(animator)
            FadeOut("Right");
    }
    public void SkipToFail()
    {
        //FieldAssistant.main.RemoveField();
        //You lost
        SessionAssistant.main.StopCurrentSession(false);
        if(animator)
            FadeOut("Right");
    }
    public void SkipToQuitLevel()
    {
        SessionAssistant.main.Quit();
    }
    public void SkipToLoadLevel()
    {
        //Insert level design
        SessionAssistant.main.StartSession(/*Here Level Design*/);
    }
}
