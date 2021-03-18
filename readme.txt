Sample SMS Service - Graham Rook

Pre-Requisites
******************
1. .Net Framework 5 needs to be installed on your machine (This is available in the latest version of Visual Studio).

Optional Configuration
************************
1. In appsettings.json there is a setting for the logging destination for value "Path", this can be changed to a Path of your choice.

Testing the Microservice
*************************
1. Run/Debug the the project, this will open the browser with swagger ui.
2. This will allow for the testing of the available API methods.

*An Alternative option for testing you could also use Postman to call the end points.


Assumptions
*************************
1. The Assumption is that the Message Queue has some sort of message broker that will subcribe to the queue and push these messages to the SMS Microservice.
2. I have attached a diagram (SMS Microservice.png) of the assumed architectural approach.


