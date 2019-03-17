using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ReturnToMenuButon : MonoBehaviour {

	public void OnClick () 
	{
        StartCoroutine(LauchScreenLoader());
		
	}
    IEnumerator LauchScreenLoader()
    {
        float timer = 0;
        GameManager.singleton.inGameUiManager.screenLoading.Open();
        while (timer < 0.7f)
        {
            timer += Time.deltaTime;
            yield return 0;
        }
        SceneStatic.GoToMenuScene();
    }
}
