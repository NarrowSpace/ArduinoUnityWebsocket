/*
This project is using Glitch Websocket
*/

#include "arduino_secrets.h"  //Store Senstive WIFI Info
#include <ArduinoHttpClient.h>
#include <WiFiNINA.h>
#include <ArduinoJson.h>

//Fill the wifi info in the 'arduino_secrets' file
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;

//Web Socket
char serverAddress[] = "myrobotcoach.glitch.me";

//Standard HTTPS port:
int port = 443;

// set up an SSL client (use WiFiClient if not using SSL):
WiFiSSLClient wifi;
// initialize the webSocket client
WebSocketClient client = WebSocketClient(wifi, serverAddress, port);
// message sending interval, in ms:
int interval = 100;
// last time a message was sent, in ms:
long lastSend = 0;

//JSON object
StaticJsonDocument<300> socketDataSend;
//for parsing the input
StaticJsonDocument<300> socketDataReceive;
//My Arduino Name
String dataID = "WTArduino";


//Arduino Part
int sensorPin = A0;
int sensorVal;
//Counter
int countVal = 0;
int currentState = 0;
int preState = 0;

void setup() {
  Serial.begin(9600);

  String printConnection = "CONNECT " + String(ssid);

  while (WiFi.status() != WL_CONNECTED) {
    Serial.print("Attempting to connect to Network named: ");
    Serial.println(ssid);
    WiFi.begin(ssid, pass);
  }

  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  Serial.print("You're connected to the network");
  printCurrentNet();

  // API endpoint to connect to:
  client.begin("/");
}

void loop() {
  Serial.println("starting WebSocket client");

  //If the client is connected then it will start and keep running
  while (client.connected()) {

    if (millis() - lastSend > interval) {

      sensorVal = analogRead(sensorPin);

      if (sensorVal > 950) {
        currentState = 1;
        delay(3000);
      }

      else {
        currentState = 0;
      }

      if (currentState != preState) {

        if (currentState == 1) {
          countVal++;
          Serial.println(countVal);
        }
      }

      //save the data into your JSON Object
      //the parameter names you create are important
      //these names are how you will retrieve the data when read from the server
      socketDataSend["id"] = dataID;
      // socketDataSend["sensor"] = sensorVal;
      socketDataSend["Counter"] = countVal;

      String message;                          //create variable to hold message
      serializeJson(socketDataSend, message);  //convert the JSON object to a String

      // send the message:
      client.beginMessage(TYPE_TEXT);
      client.print(message);
      client.endMessage();
      // update the timestamp:
      lastSend = millis();
    }

    // check if a message is available to be received
    int messageSize = client.parseMessage();

    if (messageSize > 0) {
      Serial.println("Received a message:");
      Serial.println(client.readString());

      /* Since the project Only with One Arduino direct talk to Unity
      so the following code will not be excuted

      // DeserializationError error = deserializeJson(socketDataReceive, client);
      // if (!error) {
      //   //get the id
      //   String messageID = socketDataReceive["id"];

      //   if (messageID != dataID) {
      //     Serial.println("Received new message: ");
      //   } else {
      //     Serial.println("I sent this message, Go check it!");
      //   }
      // }
    } */
    }
  }

  Serial.println("disconnected");
}

//Print Relate Info
void printCurrentNet() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print the MAC address of the router you're attached to:
  byte bssid[6];
  WiFi.BSSID(bssid);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.println(rssi);

  // print the encryption type:
  byte encryption = WiFi.encryptionType();
  Serial.print("Encryption Type:");
  Serial.println(encryption, HEX);
  Serial.println();
}
