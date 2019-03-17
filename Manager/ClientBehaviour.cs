using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour {
    public FirebaseClientV2 client;
	// Use this for initialization
	void Start () {
        client = new FirebaseClientV2("https://city-gaz.firebaseio.com/");
        client.GET_LifeCycleAssistant().AddListenerToInitialize("GET PLAYER PROFILE", (obj) => {
            FirebaseAuthenticationCredentials l_userCredentials = new FirebaseAuthenticationCredentials("TIM", "ROOT");
            client.SET_UserCredentials(l_userCredentials);
            client.Initialize();
            FirebaseRequest l_fbreq = new FirebaseRequest();
            FirebaseRequestBuilder.SET_Permission(l_fbreq, AbstractRequest.REQUEST_PERMISSION.SELF_ONLY);
            FirebaseRequestBuilder.SET_Path(l_fbreq, "PLAYER_PROFILE");
            FirebaseRequestBuilder.SET_Key(l_fbreq, "TIM");
            FirebaseRequestBuilder.SET_Listener(l_fbreq, (p_objectResult, p_eventArgs) =>
            {
                Debug.Log("Result came in listener !");
                Debug.Log(p_objectResult.ToString());
            });
            client.GET_Data(l_fbreq);
        });
        client.Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
