# whereismybox-web

You can run the applications together in two different ways.

## Pre-requisites
* .NET 6.0
* Azure core functions tools version 4
* Npm v 16.*
* Vue

## Alternative 1: Starting the applications individually
1. Start the backend api: 
    1. Alternative: Using IDE: Rider or visual studio is recommended
    2. Using the terminal with Azure core functions tools. Navigate to /api/Functions and type ```func host start```
2. The function app will start at http://localhost:7071
3. Start the frontend application: ```npm run dev```. The dev FE application will start at http://localhost:5173
4. Start the reverse proxy emulation environment: ```npm run swa```

The frontend application will hot reload. Note: If your applications start at different ports you have to edit the npm script to match the ports.

## Alternative 2: 
1. Build the frontend application: ```npm run build```
2. Start the emulation environment: ```npm run swa-dev```

The swa environment will tell you where the application can be reached.
