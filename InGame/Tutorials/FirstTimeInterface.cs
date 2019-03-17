using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimeInterface : AbstractTutorial
{
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    public override void GoToSequence(int p_index)
    {
        sequences[sequenceIndex].Stop();
        sequenceIndex = p_index;
        sequences[sequenceIndex].Start();
    }

    public override void SetSequences(List<AbstractSequence> p_sequences)
    {
        sequences = p_sequences;
    }

    public override void StartSequence(int p_index)
    {
        Debug.Log("FirstTimeInterface-> START SEQUENCE INDEX:" + p_index);
        sequences[p_index].Start();
    }

    public override void StopSequence(int p_index)
    {
        sequences[p_index].Stop();
    }


    #endregion
    #region CONSTRUCTORS
    public FirstTimeInterface()
    {
        List<AbstractSequence> l_sequences = new List<AbstractSequence>();
        GameObject l_dialogsGO = GameObject.Find("[TUTORIAL_DIALOGS]").gameObject;
        List<GameObject> l_list = new List<GameObject>();
        SequenceDialogs l_firstSequence = new SequenceDialogs("Premier Dialogue", 0);
        for (int i = 0; i < l_dialogsGO.transform.childCount; i++)
        {
            if (i == l_dialogsGO.transform.childCount - 1)
            {
                l_dialogsGO.transform.GetChild(i).GetChildByPath("Content_Viewport/GoToEnd_Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    l_firstSequence.End();
                    PlayerPrefs.SetString("firstTimeInInterface", "false");
                    GameManager.singleton.uiManager.popUps[UiManager.PopUpEnum.Conection].Open();
                });
            }
            else
            {
                l_dialogsGO.transform.GetChild(i).GetChildByPath("Content_Viewport/GoToNext_Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("GoNext Button");
                    l_firstSequence.GoNext();
                });
            }
            l_list.Add(l_dialogsGO.transform.GetChild(i).gameObject);

        }
        l_firstSequence.SetDialogs(l_list);

        l_sequences.Add(l_firstSequence);
        SetSequences(l_sequences);
    }
    #endregion
}
