from flask import Flask, render_template, request
import io
import base64
import azure.cosmos.cosmos_client as cosmos_client
import uuid

app = Flask(__name__)

cosmos_url = 'https://numbertaker.documents.azure.com:443/'
cosmos_primary_key = ''
cosmos_collection_link = 'dbs/Photos/colls/Text'

client = cosmos_client.CosmosClient(url_connection=cosmos_url, 
                                    auth={'masterKey': cosmos_primary_key})

@app.route('/')
def home():
    docs = list(client.ReadItems(cosmos_collection_link))
    return render_template('home.html', result = docs)