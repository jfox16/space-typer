using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITypingInputSubscriber
{
    void OnKeyDown(Event e);
}