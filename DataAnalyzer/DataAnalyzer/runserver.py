"""
This script runs the DataAnalyzer application using a development server.
"""

from os import environ
from DataAnalyzer import app

if __name__ == '__main__':
    HOST = environ.get('SERVER_HOST', 'localhost')
    try:
        PORT = int(environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run(host="localhost", port=8000, debug=True)# костыль, иначе порт не работает
        #app.run(HOST, PORT)
