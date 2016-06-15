# Basic Authentication

This project helps with building a basic authentication example with ASP.NET Identity. The idea is configure a RESTful API that will use
Basic Authentication to verify the User Credentials.

## What is Basic Authentication

In the HTTP Protocol [Basic Authentication](https://tools.ietf.org/html/rfc2617) is created by the following steps.
* Take the Username and Password and concatenate them together with a ':'. Username:Password
* Take that result and [Base64 Encode](https://tools.ietf.org/html/rfc4648) the string
* Create a Header Entry call Authorization
* Set the Value of Authorization to Basic *Base64 Encoded String*
That Header must be included on all requests made to the end point.

## Advantages of Basic Authentication

* Easy for the client and the server to implement
Basic Authentication does not require any particular external libraries or support to generate. Most modern languages will have the techniques to
generate a Basic Authentication Token.
* Easy to Decode
The Username and Password can be easily decoded for debugging purposes, or when looking for a particular request. For example if the Basic Token is 
always the same it becomes possible to identify the Authentication user for each request just on the HTTP Protocol.
* It is built right into the HTTP Protocol
So it can always be expected to be available
* Does not polute the Message Body
The Token is provided in the Header, so it will always be possible to send the token. For example some RESTful API's do not expect a body on 
a GET request. In addition it does not bloat the message with extra data that may not be required.

## Disadvantages of Basic Authentication

* Man in the Middle Attacks
If a message is intercepted, then the Authentication is easily decoded. Base64 encoding is not for encryption, but rather to ensure that characters
are in a acceptable format for the HTTP Protocol.
* Client is Responsible
The Client must encode the header, and as such if any changes occur to the password the client must be aware of the change. This can be impossible
to maintain if the Client is not aware of changes.
* Only deals with Authentication
The only details on the header are a username / password combination. The End Point is still responsible for authorization checks.

## Why use Basic Authentication

Basic Authentication is useless as a process to ensure security. It does not provide encryption or ease of use with the Client when a password change
occurs. What it does provide is a simple process of Authentication, which can be useful in a closed system (IE: Internal API Services).

# 