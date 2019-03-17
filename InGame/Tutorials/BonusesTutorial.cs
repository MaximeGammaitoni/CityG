using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusesTutorial : AbstractTutorial {
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
        Debug.LogError("BONUSES Tutorial-> START SEQUENCE INDEX:" + p_index);
        sequences[p_index].Start();
    }

    public override void StopSequence(int p_index)
    {
        sequences[p_index].Stop();
    }


    #endregion
    #region CONSTRUCTORS
    public BonusesTutorial(bool p_punto, bool p_tubes, bool p_gaspard)
    {
        List<AbstractSequence> l_sequences = new List<AbstractSequence>();
        GameObject l_bombsDialogGO = GameObject.Find("Canvas/SafeArea/[TUTORIALS_BONUSES_DIALOGS]");
        GameObject l_puntoDialogGO = l_bombsDialogGO.transform.GetChildByPath("[PUNTO]").gameObject;
        GameObject l_tubesDialogGO = l_bombsDialogGO.transform.GetChildByPath("[TUBES]").gameObject;
        GameObject l_gaspardDialogGO = l_bombsDialogGO.transform.GetChildByPath("[GASPARD]").gameObject;

        List<GameObject> l_list = new List<GameObject>();
        SequenceDialogs l_sequence = new SequenceDialogs("Bonuses Dialogs", 0);

        if (p_punto)
        {
            for (int i = 0; i < l_puntoDialogGO.transform.childCount; i++)
            {
                l_list.Add(l_puntoDialogGO.transform.GetChild(i).gameObject);
            }
            PlayerPrefs.SetString("Tutorial_Bonuses_Punto", "true");
        }
        if (p_tubes)
        {
            for (int i = 0; i < l_tubesDialogGO.transform.childCount; i++)
            {
                l_list.Add(l_tubesDialogGO.transform.GetChild(i).gameObject);
            }
            PlayerPrefs.SetString("Tutorial_Bonuses_Tubes", "true");
        }
        if (p_gaspard)
        {
            for (int i = 0; i < l_gaspardDialogGO.transform.childCount; i++)
            {
                l_list.Add(l_gaspardDialogGO.transform.GetChild(i).gameObject);
            }
            PlayerPrefs.SetString("Tutorial_Bonuses_Gaspard", "true");
        }

        for(int i = 0; i < l_list.Count; i++)
        {
            if(i == l_list.Count - 1)
            {
                l_list[i].transform.GetChildByPath("Content_Viewport/GoToEnd_Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("GoToEnd Button");
                    l_sequence.End();
                });
            }
            else
            {
                l_list[i].transform.GetChildByPath("Content_Viewport/GoToEnd_Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("GoNext Button");
                    l_sequence.GoNext();
                });
            }
        }

        l_sequence.SetDialogs(l_list);
        l_sequences.Add(l_sequence);
        SetSequences(l_sequences);
    }
    #endregion
}
