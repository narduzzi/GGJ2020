using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Event
{
    public int ID { get; }
    public string Title { get; }

    public string Question { get; }

    public List<Tuple<string, int, string>> Answers { get; }

    public Event(int ID, string Title, string Question, List<Tuple<string, int, string>> Answers)
    {
        this.ID = ID;
        this.Title = Title;
        this.Question = Question;
        this.Answers = new List<Tuple<string, int, string>>();

        foreach (var Answer in Answers)
        {
            this.Answers.Add(Answer);
        }
    }

    public static Event LoadFromJSON(string json)
    {
        return JsonUtility.FromJson<JSONEvent>(json).ConvertToEvent();
    }

    [System.Serializable]
    private class JSONEvent
    {
        public Answer[] answers;
        public string date;
        public string description;
        public string event_title;
        public int id;
        public int[] next_ids;
        public int[] previous_ids;
        public Question[] questions;
        public string tag;
        public string theme;

        public Event ConvertToEvent()
        {
            List<Tuple<string, int, string>> EventAnswers = new List<Tuple<string, int, string>> {
                new Tuple<string, int, string>(
                    answers[0].responses[0].text,
                    answers[0].responses[0].next_event_id,
                    answers[0].responses[0].solution
                ),
                new Tuple<string, int, string>(
                    answers[0].responses[1].text,
                    answers[0].responses[1].next_event_id,
                    answers[0].responses[1].solution
                )
            };

            return new Event(id, event_title, questions[0].text, EventAnswers);
        }

        [System.Serializable]
        public class Question
        {
            public int id;
            public string text;
        }

        [System.Serializable]
        public class Answer
        {
            public int question_id;
            public Response[] responses;

            [System.Serializable]
            public class Response
            {
                public string next_event_title;
                public int next_event_id;
                public string solution;
                public string text;
                public string type;
            }
        }
    }
}

