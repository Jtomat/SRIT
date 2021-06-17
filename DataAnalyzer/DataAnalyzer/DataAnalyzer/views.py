"""
Routes and views for the flask application.
"""

from datetime import datetime
from flask import render_template
from DataAnalyzer import app
import ReportAnalyzer
from flask import jsonify
from flask import Flask, request, jsonify
import json

@app.route('/')
@app.route('/home')
def home():
    """Renders the home page."""
    return render_template(
        'index.html',
        title='Home Page',
        year=datetime.now().year,
    )

@app.route('/contact')
def contact():
    """Renders the contact page."""
    return render_template(
        'contact.html',
        title='Contact',
        year=datetime.now().year,
        message='Your contact page.'
    )

@app.route('/about')
def about():
    """Renders the about page."""
    return render_template(
        'about.html',
        title='About',
        year=datetime.now().year,
        message='Your application description page.'
    )

@app.route('/ReportAnalyze',methods=['POST'])
def ReportAnalyze():
    """API function for text analyze."""
    content = request.json#(silent=True)
    return app.response_class(
        response=json.dumps(ReportAnalyzer.Find_KeyWords(content)),
        status=200,
        mimetype='application/json'
    );

@app.route('/Solution',methods=['POST'])
def Solution():
    """API function for find solutions."""
    content = request.json#(silent=True)
    return app.response_class(
        response=json.dumps(ReportAnalyzer.Find_KeyWords(content)),
        status=200,
        mimetype='application/json'
    );
