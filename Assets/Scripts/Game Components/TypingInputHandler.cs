using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Looks for key down events and turns them into events
public class TypingInputHandler : MonoBehaviour
{
    public static TypingInputHandler Instance;
    
    List<ITypingInputSubscriber> subscribers = new List<ITypingInputSubscriber>();

    void Awake()
    {
        Instance = this;
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            KeyCode keyCode = e.keyCode;

            foreach (ITypingInputSubscriber subscriber in subscribers)
            {
                subscriber.OnKeyDown(e);
            }
        }
    }

    public void Subscribe(ITypingInputSubscriber subscriber)
    {
        subscribers.Add(subscriber);
    }
}
