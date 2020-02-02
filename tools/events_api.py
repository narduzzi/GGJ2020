import json
import os
import numpy as np

events_json = "data/events.json"


class Event:
    def __init__(self, theme="", event="", date="", description="", questions=[], answers=[],
                 tag="real", next_ids=[], previous_ids=[], data=None, update=False):

        int_date = None
        if len(date) > 0:
            int_date = int(date)
        self.d = {
            "id": get_new_event_id(),
            "theme": theme,
            "event": event,
            "date": int_date,
            "description": description,
            "questions": questions,
            "answers": answers,
            "tag": tag,
            "next_ids": next_ids,
            "previous_ids": previous_ids,
        }

        if data:
            self.d = data
            if not update:
                self.d["id"] = get_new_event_id()

    def save(self):
        save_event(self.d)

    def get_data(self):
        return self.d


def get_event_ids():
    with open(events_json, "r") as f:
        data = json.load(f)
    all_ids = sorted([d["id"] for d in data])
    print(all_ids)
    return all_ids


def get_event_by(event=""):
    with open(events_json, "r") as f:
        data = json.load(f)

    for d in data:
        if d["event"] == event:
            return d
    return None


def get_next_event(current_id, question_id, response_id):
    if current_id < 0:
        return get_event(0)

    with open(events_json, "r") as f:
        data = json.load(f)

    question_answers = data[int(current_id)]["answers"]
    for q in question_answers:
        if q["question_id"] == int(question_id):
            answers = q["responses"]
            for i in range(len(answers)):
                if i == int(response_id):
                    return get_event(answers[i]["next_event_id"])
    return None


def get_random_event():
    with open(events_json, "r") as f:
        data = json.load(f)
    all_ids = sorted([d["id"] for d in data])
    x = np.random.randint(0, len(all_ids))
    idx = all_ids[x]
    return get_event(idx)


def get_new_event_id():
    if len(get_event_ids()) == 0:
        last_id = 0
    else:
        last_id = int(get_event_ids()[-1])
    return last_id + 1


def get_related(idx):
    with open(events_json, "r") as f:
        data = json.load(f)
    return data


def sort_by_key(data):
    return sorted(data, key=lambda x: x["id"])


def get_event(event_id):
    if event_id < 0:
        event_id = 0
    with open(events_json, "r") as f:
        data = json.load(f)
    data = sort_by_key(data)
    for d in data:
        print(d["id"], d)
        if d["id"] == event_id:
            return d
    return None


def save_event(new_event):
    with open(events_json, "r") as f:
        data = json.load(f)

    new_database = []
    for d in data:
        if d["id"] == new_event["id"]:
            continue
        else:
            new_database.append(d)
    new_database.append(new_event)

    new_database = sorted(new_database, key=lambda x: x["id"])

    with open(events_json, "w") as f:
        json.dump(new_database, f, indent=4)
    return new_event


def add_event_from_json(event):
    with open(events_json, "r") as f:
        data = json.load(f)

    event["id"] = get_new_event_id()
    data.append(event)
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)
    return event


def add_event(event, date, question=None, responses=[], previous_id=None, next_id=None, tag="real"):
    with open(events_json, "r") as f:
        data = json.load(f)

    new_id = get_new_event_id()

    previous_ids = [previous_id]
    if previous_id is None:
        previous_ids = []

    next_ids = [next_id]
    if next_ids is None:
        next_ids = []

    new_event = {"id": new_id,
                 "event": event,
                 "date": int(date),
                 "previous_ids": previous_ids,
                 "next_ids": next_ids,
                 "question": question,
                 "responses": responses,
                 "tag": tag}

    data.append(new_event)
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)
    return new_event


def add_adjacent(eventA, eventB):
    with open(events_json, "r") as f:
        data = json.load(f)

    data[eventA["id"]]["next_ids"].append(eventB["id"])
    data[eventB["id"]]["previous_ids"].append(eventA["id"])

    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)


def remove_adjacent(eventA, eventB):
    with open(events_json, "r") as f:
        data = json.load(f)

    if eventB["id"] in data[eventA["id"]]["next_ids"]:
        data[eventA["id"]]["next_ids"].remove(eventB["id"])
        data[eventB["id"]]["previous_ids"].remove(eventA["id"])

    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)


def add_response(event_id, response):
    with open(events_json, "r") as f:
        data = json.load(f)

    data[event_id]["responses"].append(response)
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)

    return data["event_id"]


def add_question(event_id, question):
    with open(events_json, "r") as f:
        data = json.load(f)

    data[event_id]["question"] = question
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)

    return data["event_id"]


def remove_question(event_id, next_event_id):
    with open(events_json, "r") as f:
        data = json.load(f)

    questions = data[event_id]["question"]
    new_questions = []
    for q in questions:
        if q["next_id"] != next_event_id:
            new_questions.append(q)

    data[event_id]["question"] = new_questions
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)
