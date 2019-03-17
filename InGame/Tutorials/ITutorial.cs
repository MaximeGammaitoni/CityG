using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorial  {
    void StartSequence(int p_index);
    void StopSequence(int p_index);
    void GoToSequence(int p_index);
    void SetSequences(List<AbstractSequence> p_sequences);
}
