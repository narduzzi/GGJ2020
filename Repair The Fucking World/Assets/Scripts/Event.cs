using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Event
{
    public int ID { get; }
    public string Title { get; }

    public string Question { get; }
    
    public List<Tuple<string, int>> Answers { get; }

    public Event(int ID, string Title, string Question, List<Tuple<string, int>> Answers)
    {
        this.ID = ID;
        this.Title = Title;
        this.Question = Question;
        this.Answers = new List<Tuple<string, int>>();
        foreach(var Answer in Answers)
        {
            this.Answers.Add(Answer);
        }
    }

    public static Event LoadFromJSON(string json)
    {
        return JsonUtility.FromJson<Event>(json);
    }
}
