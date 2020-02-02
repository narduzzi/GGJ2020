import os
import flask
import json
from flask import Flask, render_template, request, make_response, jsonify
import numpy as np
from flask_cors import CORS
import events_api as e
from events_api import Event

app = Flask(__name__)
CORS(app)

events_json = "data/events.json"
events_adjacency_json = "data/events_adjacency_matrix.json"
theme_json = "data/themes.json"


@app.route('/welcome')
def welcome():
    return render_template("welcome.html")


@app.route('/')
def index():
    return render_template("question_creator.html")


@app.route('/annotate')
def annotate():
    user_asking = [happened, not_happened, not_not, not_yes, create_question, provide_question]
    i = np.random.randint(0, len(user_asking))
    return user_asking[i]()


@app.route('/question_creator', methods=["GET"])
def question_creator():
    event_title = request.args.get("event_title")
    if event_title is None:
        event_title = ""

    return render_template("question_creator.html", event_title=event_title)


@app.route('/linker', methods=["GET"])
def linker():
    return render_template("linker.html")


@app.route("/create_question")
def create_question():
    return render_template("create_question.html")


@app.route("/create_answer")
def create_answer():
    return render_template("create_answers.html")


@app.route("/get_question")
def get_question():
    data = [e.get_random_event() for i in range(3)]

    return make_response(jsonify(data), 200)


@app.route("/get_themes")
def get_themes():
    with open(theme_json, "r") as f:
        data = json.load(f)

    return make_response(jsonify(data), 200)


@app.route('/happened')
def happened():
    return render_template("happened.html")


@app.route('/not_not')
def not_not():
    return render_template("not_not.html")


@app.route('/not_yes')
def not_yes():
    return render_template("not_yes.html")


@app.route('/get_random_events', methods=["GET"])
def get_random_events():
    type1 = request.args.get("event1", default="real")
    type2 = request.args.get("event2", default="real")

    with open(events_json, "r") as f:
        events = json.load(f)

    a = 0
    b = 0
    correct = False
    while not correct:
        a = np.random.randint(0, len(events))
        b = np.random.randint(0, len(events))
        if a == b:
            continue

        e1 = events[a]
        e2 = events[b]

        if (int(e1["date"]) > int(e2["date"])):
            tmp = e1
            e1 = e2
            e2 = tmp

        if type1 == e1["tag"] and type2 == e2["tag"]:
            correct = True

    data = [e1, e2]
    return make_response(jsonify(data), 201)


@app.route("/not_happened")
def not_happened():
    with open(events_json, "r") as f:
        events = json.load(f)

    a = np.random.randint(0, len(events))
    e1 = events[a]

    return render_template("not_happened.html", event1=e1)


@app.route("/submit_event", methods=["POST"])
def submit_event():
    data = request.get_json()
    event = e.add_event_from_json(data["new_event"])

    if "event2" in data:
        e.remove_adjacent(data["event1"], data["event2"])
        e.add_adjacent(event, data["event2"])
    if "event1" in data:
        e.add_adjacent(data["event1"], event)

    return success()


@app.route("/submit_link", methods=["POST"])
def submit_link():
    data = request.get_json()
    event_id = int(data["event"])
    next_id_A = int(data["next_id_a"])
    next_id_B = int(data["next_id_b"])

    event_data = e.get_event(event_id)
    event_data["answers"][0]["responses"][0]["next_event_id"] = next_id_A
    event_data["answers"][0]["responses"][1]["next_event_id"] = next_id_B

    e.update_event(id=event_data["id"], data=event_data)

    return success()


@app.route("/update_connectivity", methods=["GET"])
def update_connectivity():
    e.update_connectivity()
    return success()

@app.route("/get_all_events", methods=["GET"])
def get_all_events():
    key = request.args.get("key", type=str, default="id")
    if key == "date":
        all_events = sorted(e.get_all_events(), key=lambda x: int(x[key]))
    else:
        all_events = sorted(e.get_all_events(), key=lambda x: x[key])

    return make_response(jsonify(all_events), 200)


@app.route("/provide_question", methods=["GET"])
def provide_question():
    return render_template("provide_question.html")


@app.route("/fetch_unasked", methods=["GET"])
def fetch_unasked():
    found = False
    all_ids = e.get_event_ids()
    data = []
    i = 0
    while not found:
        for id in all_ids:
            data = e.get_event(id)
            if "questions" not in data.keys():
                found = True
            else:
                if len(data["questions"]) > i:
                    continue
                else:
                    found = True
        i = i + 1
    print("Fetching unasked:", data)
    return make_response(jsonify(data), 200)


@app.route("/get_event", methods=["GET"])
def get_event():
    id = request.args.get("id", type=int, default=0)
    event = e.get_event(id)
    return make_response(jsonify(event), 200)


@app.route("/submit_question", methods=["POST"])
def submit_question():
    data = request.get_json()
    e.add_question(event_id=data["id"], question=data["question"])
    return make_response(jsonify(data), 200)


def success(data=None):
    msg = {'message': 'Created', 'code': 'SUCCESS'}
    if data:
        msg["data"] = data

    return make_response(jsonify(msg), 200)


@app.route("/get_related_event", methods=["GET"])
def get_related_event():
    data = request.args.get("current_id")
    if data is None:
        print("DATA IS NONE, providing random")
        data = e.get_related(0)
    return make_response(jsonify(data), 201)


@app.route("/get_next_question", methods=["GET"])
def get_next_question():
    current_id = request.args.get("current_id", default=0)
    question_id = request.args.get("question_id", default=0)
    response_id = request.args.get("response_id", default=0)

    response = e.get_next_event(current_id, question_id=question_id, response_id=response_id)

    return make_response(jsonify(response), 200)


@app.route("/add_event", methods=["POST"])
def add_event():
    data = request.get_json()
    print(data)
    return make_response(jsonify(data), 200)


@app.route("/new_event", methods=["POST"])
def new_event():
    data = request.get_json()
    complete_event = Event(data=data).save()

    data = {'message': 'Created', 'code': 'SUCCESS',
            "next_event_title": data["answers"][0]["responses"][0]["next_event_title"]}
    return make_response(jsonify(data), 200)


if __name__ == '__main__':
    app.run("0.0.0.0", port=80, threaded=True)
