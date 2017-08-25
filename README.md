# BotFramework-ProActiveFunction
A basic sample how to send a Bot Framework ProActive message, by using an Azure Function, written in C#. You can open this project in Visual Studio 2017, and then publish it to an Azure Function (or debug it locally).

All parameters can be sent either in the query string (HTTP GET) or in the body (HTTP POST). Here is a sample JSON body:
```javascript
{
    "messagetext" : "message text to send",
    "fromid" : "",
    "fromname" : "",
    "toid" : "",
    "toname" : "",
    "serviceurl" : "",
    "appid" : "",
    "apppassword" : ""
}
```
