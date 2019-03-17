using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpQuit : UiElementAbstract
{

    protected bool canInteract;
    protected Vector3 targetPosition;
    public PopUpQuit(string uiElementName) : base(uiElementName)
    {
        if (uiElementGameObject)
        {
            //uiElementGameObject.SetActive(false);
            canInteract = true;
            targetPosition = new Vector3(0, -Screen.height*2, 0);
            GameManager.GameUpdate += Open;
            Debug.Log(uiElementGameObject.name + " LOADED");
        }
        else
        {
            throw new ScreenException(Name.ToString() + " Game Object not found");
        }
    }


    public void Open()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canInteract)
        {
            GameManager.singleton.StartCouroutineInGameManager(SmoothyOpen());
        }    
    }

    public void Close()
    {
        GameManager.singleton.StartCouroutineInGameManager(SmoothyClose());
    }

    IEnumerator SmoothyClose()
    {
        canInteract = false;
        targetPosition = new Vector3(0, -Screen.height*2, 0);
        while (uiElementGameObject.transform.localPosition.y >= targetPosition.y)
        {
            Vector3 currentPosition = uiElementGameObject.transform.localPosition;
            uiElementGameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition - new Vector3(0, 5, 0), Time.deltaTime * 9.5f);
            yield return 0;
        }
        targetPosition = new Vector3(0, -Screen.height, 0);
        uiElementGameObject.transform.localPosition = targetPosition - new Vector3(0, Screen.height, 0);
        canInteract = true;
        uiElementGameObject.SetActive(false);
    }

    IEnumerator SmoothyOpen()
    {
        while (!canInteract)
        {
            yield return 0;
        }
        uiElementGameObject.SetActive(true);
        canInteract = false;
        uiElementGameObject.transform.localPosition = targetPosition - new Vector3(0, Screen.height, 0);

        while (uiElementGameObject.transform.localPosition.y < targetPosition.y)
        {
            Vector3 currentPosition = uiElementGameObject.transform.localPosition;
            uiElementGameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition + new Vector3(0, 2, 0), Time.deltaTime * 10);
            yield return 0;
        }
        yield return 0;
    }
}

