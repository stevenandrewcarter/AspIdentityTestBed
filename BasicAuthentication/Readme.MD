﻿# Basic Authentication

This project a Basic Authentication example with ASP.NET Identity. The idea is to configure a RESTful API that will use
Basic Authentication to verify the User Credentials.

## What is Basic Authentication

In the HTTP Protocol [Basic Authentication](https://tools.ietf.org/html/rfc2617) is created by the following steps.

* Take the Username and Password and concatenate them together with a ':'. Username:Password
* Take that result and [Base64 Encode](https://tools.ietf.org/html/rfc4648) the string
* Create a Header Entry call Authorization
* Set the Value of Authorization to Basic *Base64 Encoded String*

That Header must be included on all requests made to the end point. That means that the client is responsible for creating
the Basic Authentication Header and the Server just parses the Username and Password out of the header. 

If a unauthenticated response is generated, then the server is expected to provide a WWW-Authenticate header in the form
`WWW-Authenticate: Basic realm={REALM}`, where Realm is a server identifier that the client can use to determine the correct
username and password combination. In addition the response header indicates that the Server is using Basic Authentication as
the authentication strategy.

## Advantages of Basic Authentication

* **Easy for the client and the server to implement**

Basic Authentication does not require any particular external libraries or support to generate. Most modern languages will have the techniques to
generate a Basic Authentication Token. The Authentication Strategy is placed in the headers and does not require any special knowledge to
decode or read.

* **Easy to Decode**

The Username and Password can be easily decoded for debugging purposes, or when looking for a particular request. For example if the Basic Token is 
always the same it becomes possible to identify the Authentication user for each request just on the HTTP Protocol.

* **It is built right into the HTTP Protocol**

So it can always be expected to be available since it does not require a secondary library or technique to implement.

* **Does not pollute the Message Body**

The Token is provided in the Header, so it will always be possible to send the token. For example some RESTful API's do not expect a body on 
a GET request. In addition it does not bloat the message with extra data that may not be required.

## Disadvantages of Basic Authentication

* **Man in the Middle Attacks**

If a message is intercepted, then the Authentication Header is easily decoded. Base64 encoding is not for encryption, but rather to ensure that characters
are in a acceptable format for the HTTP Protocol.

* **Cannot prevent Replay Attacks**

It provides no technique to identify the origin of the message, so the same message can be sent repeatably to the Server.

* **Client is Responsible**

The Client must encode the header, and as such if any changes occur to the password the client must be aware of the change. This can be impossible
to maintain if the Client is not aware of changes.

* **Only deals with Authentication**

The only details on the header are a username / password combination. The End Point is still responsible for authorization checks.

## Why use Basic Authentication

Basic Authentication is useless as a process to ensure security. It does not provide encryption or ease of use with the Client when a password change
occurs. What it does provide is a simple process of Authentication, which can be useful in a closed system (IE: Internal API Services). In general 
basic authentication should not be used in a system with public facing end points.

# The Project

The following are steps required to create a ASP.NET Identity project. The steps provided will
generate a Project from Scratch as part of the explaination and learning. Visual studio does provide
a template that will already set most of the Identity Framework up for you.

## Initial Setup

* Create a Blank ASP.NET Project in Visual Studio
* Install the Following Packages from Nuget
```
Install-Package Microsoft.AspNet.Identity.Owin
Install-Package Microsoft.Owin.Host.SystemWeb
Install-Package Microsoft.AspNet.WebApi.Owin
Install-Package Microsoft.Owin.Cors
Install-Package Microsoft.Owin.Security.OAuth
```
* Create a User Controller (Controllers\UsersController.cs)
* Add a Authentication Filter for Basic Authentication (Controllers\BasicAuthenticator.cs)

# Usage

In the UsersController it is possible to see a Authenitcation Filter `[BasicAuthenticator(realm: "Basic")]`. This
is what the Controller needs to enable Basic Authentication on a Controller or Route.

The only available end point is http://localhost/api/users/:id with the only valid id being 1. Other
ids will be accepted but will return a 404 status code on a authenticated route.

In order to Authenticate a Basic Authentication Request must be sent with the Username and Password
being the same, but not blank.

# Testing

Run the Application in Visual Studio. Once running start up your favourite REST client and try
send the following

* **Unauthenticated Request**

*Request*: `GET http://localhost/api/users/1`

*Response*: 401 Unauthorised. Note that the Headers contain a Entry `[WWW-Authenticate: Basic realm=Basic]`

* **Authenticated Request**

*Request*: `GET http://localhost/api/users/1`
           `Header: Authorization:Basic dGVzdDp0ZXN0`
           The Header contains the UserName *test* and password *test*

*Response*: 200
            `{
                "id": "1",
                "name": "Test 1"
             }`

# References

[ASP.NET Identity Example](http://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/BasicAuthentication/)

[ASP.NET Identity Explaination](http://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/BasicAuthentication/)