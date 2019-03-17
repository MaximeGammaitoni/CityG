using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpLoose : UiElementAbstract {

    protected bool canInteract;
    protected Vector3 targetPosition;
    public PopUpLoose(string uiElementName) : base(uiElementName) {
        if (uiElementGameObject)
        {
            uiElementGameObject.SetActive(false);
            canInteract = true;
            targetPosition = new Vector3(0, -575, 0);
            Project.onLevelFailed.AddListener(Open);
            Debug.Log(uiElementGameObject.name + " LEVELSKIN LOADED");
        }
        else
        {
            throw new ScreenException(Name.ToString() + " Game Object not finded");
        }
    }

    public void Open()
    {
        GameManager.singleton.StartCouroutineInGameManager(SmoothyOpen());
    }

    IEnumerator SmoothyOpen()
    {
        while (!canInteract)
        {
            yield return 0;
        }
        uiElementGameObject.SetActive(true);
        canInteract = false;
        uiElementGameObject.transform.localPosition = targetPosition - new Vector3(0, 500, 0);
       

        while (uiElementGameObject.transform.localPosition.y < targetPosition.y)
        {
            Vector3 currentPosition = uiElementGameObject.transform.localPosition;
            uiElementGameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition + new Vector3(0, 2, 0), Time.deltaTime * 10);
            yield return 0;
        }
        canInteract = true;
        yield return 0;
    }
}
