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


@app.route('/question', methods=["GET"])
def index():
    current_id = request.args.get("current_id", default=-1, type=int)
    question_id = request.args.get("question_id", default=0, type=int)

    response = e.get_event(current_id)

    question = response["questions"][0]["text"]
    buttons = response["answers"][0]["responses"]

    list_buttons = []
    for i in range(len(buttons)):
        list_buttons.append((i, buttons[i]))

    print(list_buttons)

    return render_template("quiz.html", question=question, buttons=list_buttons, current_id=current_id)


@app.route('/display_answer', methods=["GET"])
def display_answer():
    current_id = request.args.get("current_id", default=-1, type=int)
    question_id = request.args.get("question_id", default=-1, type=int)
    response_id = request.args.get("response_id", default=-1, type=int)

    response = e.get_event(int(current_id))

    message = response["answers"][0]["responses"][int(response_id)]["solution"]
    next_event = response["answers"][0]["responses"][int(response_id)]["next_event_id"]
    current_id = response["id"]
    return render_template("solution.html", message=message, current_id=current_id, question_id=question_id,
                           response_id=response_id, next_id=next_event)


if __name__ == '__main__':
    app.run("0.0.0.0", port=5000, threaded=True)
