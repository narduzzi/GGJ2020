import json
import os
import numpy as np
from datetime import datetime as dt

events_json = "data/events.json"
log_file = "data/logs.json"


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


def log(message):
    with open(log_file, "w") as f:
        f.writelines("{} - {}".format(dt.now(), message))


def get_event_ids():
    log("Get events IDs")
    with open(events_json, "r") as f:
        data = json.load(f)
    all_ids = sorted([d["id"] for d in data])
    print(all_ids)
    return all_ids


def get_event_by(event=""):
    log("Get event by ", event)
    with open(events_json, "r") as f:
        data = json.load(f)

    for d in data:
        if d["event"] == event:
            return d
    return None


def get_next_event(current_id, question_id, response_id):
    log("Get next event of {}-{}-{}".format(current_id, question_id, response_id))
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
    log("Get random event")
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
    log("Get related to ", idx)
    with open(events_json, "r") as f:
        data = json.load(f)
    return data


def sort_by_key(data):
    return sorted(data, key=lambda x: x["id"])


def get_event(event_id):
    log("Get event " + event_id)
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
    log("Save event " + new_event["id"])
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
    log("Add event from json " + event["id"])
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

    log("Add event {}".format(new_event["id"]))
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)
    return new_event


def add_adjacent(eventA, eventB):
    with open(events_json, "r") as f:
        data = json.load(f)

    log("Adjacent {} - {}".format(eventA["id"], eventB["id"]))

    if "next_ids" not in data[eventA["id"]].keys():
        data[eventA["id"]]["next_ids"] = []
    if "next_ids" not in data[eventB["id"]].keys():
        data[eventB["id"]]["next_ids"] = []
    if "previous_ids" not in data[eventA["id"]].keys():
        data[eventA["id"]]["previous_ids"] = []
    if "previous_ids" not in data[eventB["id"]].keys():
        data[eventB["id"]]["previous_ids"] = []

    data[eventA["id"]]["next_ids"].append(eventB["id"])
    data[eventB["id"]]["previous_ids"].append(eventA["id"])

    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)


def remove_adjacent(eventA, eventB):
    log("Remove adjacent {} {}".format(eventA["id"], eventB["id"]))
    with open(events_json, "r") as f:
        data = json.load(f)

    if eventB["id"] in data[eventA["id"]]["next_ids"]:
        data[eventA["id"]]["next_ids"].remove(eventB["id"])
        data[eventB["id"]]["previous_ids"].remove(eventA["id"])

    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)


def add_response(event_id, response):
    log("Add response to {}".format(event_id))
    with open(events_json, "r") as f:
        data = json.load(f)

    data[event_id]["responses"].append(response)
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)

    return data["event_id"]


def add_question(event_id, question):
    log("Add question to {}".format(event_id))
    with open(events_json, "r") as f:
        data = json.load(f)

    if "questions" not in data[event_id].keys():
        data[event_id]["questions"] = []

    new_id = len(data[event_id]["questions"])
    data[event_id]["questions"].append({"id": new_id, "text": question})
    with open(events_json, "w") as f:
        json.dump(data, f, indent=4)

    return data["event_id"]


def remove_question(event_id, next_event_id):
    log("Remove question from {}".format(event_id))
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
