# Digest Authentication

This project provides a Digest Authentication example with ASP.NET Identity. The idea is to configure a RESTful API that will use
Digest Authentication to verify the User Credentials.

## What is Digest Authentication

In the HTTP Protocol [Digest Authentication](https://tools.ietf.org/html/rfc2617) is created by the following steps.

* Take the Username and Password and concatenate them together with a ':'. Username:Password
* Take that result and [Base64 Encode](https://tools.ietf.org/html/rfc4648) the string
* Create a Header Entry call Authorization
* Set the Value of Authorization to Basic *Base64 Encoded String*

That Header must be included on all requests made to the end point.

## Advantages of Digest Authentication

## Disadvantages of Digest Authentication

## Why use Digest Authentication

# The Project

The following are steps required to create a ASP.NET Identity project. The steps provided will
generate a Project from Scratch as part of the explaination and learning. Visual studio does provide
a template that will already set most of the Identity Framework up for you.

## Initial Setup

* Create a Blank ASP.NET Project in Visual Studio
* Install the Following Packages from Nuget
```
Install-Package Microsoft.AspNet.Identity.Owin -Version 2.1.0
Install-Package Microsoft.Owin.Host.SystemWeb -Version 3.0.0
Install-Package Microsoft.AspNet.WebApi.Owin -Version 5.2.2
Install-Package Microsoft.Owin.Cors -Version 3.0.0
Install-Package Microsoft.Owin.Security.OAuth -Version 3.0.0
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