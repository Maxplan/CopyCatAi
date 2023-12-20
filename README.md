# CopyCatAI

## Overview
CopyCatAI is a web application that integrates OpenAI's API to enable dynamic conversations with AI, the app allows the user to send text prompt and also text prompts along with a pdf file. The PDF requests are enriched by context from uploaded PDF documents. It uses a React frontend and C# backend to process and include the content of the PDFs in the AI interactions.

## Features
- **Text requests**: User can send text prompts and get a response from OpenAI.
- **PDF Upload**: Users can upload PDF files which are then processed by the app.
- **Text Extraction & Processing**: Extracts text from PDFs, splits it into text blocks, and converts them into embeddings.
- **Embedding-Based Similarity Search**: Performs a similarity search in a vector database to find the most relevant text blocks.
- **Enhanced AI Conversations**: Combines user prompts with relevant text blocks to provide context-rich AI responses.

## How It Works
1. **Upload PDF**: Users start by uploading a PDF file.
2. **Text Extraction**: The app extracts text from the PDF and divides it into manageable blocks.
3. **Embedding and Search**: Each text block is converted into an embedding. These are compared against the embedding of the user's prompt to find the most relevant content.
4. **AI Interaction**: The app sends the combined prompt (user's text + relevant text blocks) to OpenAI's API for a response.

## Technologies Used
- **Frontend**: React.js
- **Backend**: C# (.NET)
- **AI API**: OpenAI
- **Database**: Azure Cosmos DB for MongoDB for vectors and Sqlite Entity Framwork Core database for storing users, requests, responses and conversations.

## Getting Started
To start the app you need to start both the backend API and the React app. You will have to set your own OpenAI API key in user secrets.

```bash
# Example command line instructions
git clone https://gitlab.com/lia-1/CopyCatAi.git
cd copycatai
cd CopyCatAIApi
dotnet run
cd ..
cd copycatai-react
npm start
