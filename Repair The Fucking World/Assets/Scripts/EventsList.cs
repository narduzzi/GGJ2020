using System.Collections.Generic;
using UnityEngine;

public class EventsList
{
    private static EventsList instance = null;
    private static readonly Object mutex = new Object();
    private Dictionary<int, Event> Events;

    private EventsList()
    {
        Events = new Dictionary<int, Event>();
    }

    public static EventsList GetInstance()
    {
        lock (mutex)
        {
            if (instance == null)
            {
                instance = new EventsList();
            }
            return instance;
        }
    }

    public void AddEvent(Event _event)
    {
        Events.Add(_event.ID, _event);
    }

    public Event NextEvent(int questionID)
    {
        return Events[questionID];
    }
}
