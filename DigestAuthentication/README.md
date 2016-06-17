# Digest Authentication

This project provides a Digest Authentication example with ASP.NET Identity. The idea is to configure a RESTful API that will use
Digest Authentication to verify the User Credentials.

## What is Digest Authentication

In the HTTP Protocol [Digest Authentication](https://tools.ietf.org/html/rfc2617) is a encrypted version of the Basic Authentication Strategy.
It requires much more information from the client in order to create a encrypted Authentication token. Otherwise the approach is identical to the Basic Authentication process.

The Process for Digest Authentication is as follows

* Client Sends a Request to the Server and gets a 401 Unauthorized response with the following header values
    ```
    WWW-Authenticate: Digest realm="testrealm@host.com",
                      qop="auth,auth-int",
                      nonce="dcd98b7102dd2f0e8b11d0f600bfb0c093",
                      opaque="5ccc069c403ebaf9f0171e9517f40e41"
    ```  

* Client takes the username and password and creates a MD5 hash including the nonce value. Different approaches can be used to determine the method used
  to generate the hash and is given by the *qop* header. The following approaches are available which determine the calculation of either side of the MD5 hash.
  The hash is generally calculated as `RESPONSE = MD5(HA1:NOUNCE:HA2)`. 
  If the *qop* header provides **MD5** or blank then `HA1=MD5(username:realm:password)` else if it provides **MD5-sess** then `HA1=MD5(MD5(username:realm:password):nonce:cnonce)`
  If the *qop* header provides **auth** or blank then `HA2=MD5(method:digestURI)` else if it provides **auth-int** then `HA2=MD5(method:digestURI:MD5(entityBody))`
  In addition the response is calculated as `RESPONSE=MD5(HA1:nonce:nonceCount:cnonce:qop:HA2)` if **auth** or **auth-int** is used.

* Client sends a new request including the authentication headers to the server

* Server will decode the headers and check that the MD5 hash results match the servers MD5 result.

A big change from the Basic Authentication is that a password is not sent in the clear anymore and a actual attempt to mask the password is made. The nonce
should also only be a single use and can be generated with respect to the calling client (IP Address, Requested End Point, TimeStamps, etc). The **auth** and **auth-int**
schemes attempt to prevent replay attacks by making the requests sequential via the nonceCount header.

## Advantages of Digest Authentication

* **No Password in the Clear**

No actual password is sent in the headers, instead a mulpiple hashed value is sent using one of the possible strategies as defined in the RFC. The possible
strategies are MD5, MD5-SESS, AUTH, AUTH-INT. Since the Password is hashed and included in a more complex string so that it will not be easy to look up 
in a [Rainbow Table](https://en.wikipedia.org/wiki/Rainbow_table).

* **Authentication can be location aware**

Since the client must make a request upfront, it allows the server to generate a nonce that is unique to that particular request. Which means if the request
originates from another source it can be immediately discarded. In addition, it is possible to add a sequential number to the requests.

* **Can increase the Security**

The Quality of Protection header allows for different hashing strategies if so desired. Also the server can expire a nonce or only allow it be used once
to help prevent security breaches.

## Disadvantages of Digest Authentication

* **Quality of Protection is optional**

The Quality of Protection header is optional and if not specified in the server will use the least secure mode. It also makes the server more complicated to
implement in order to handle the different hashing approaches.

* **Client cannot verify the Server**

The Client cannot actually determine if the server is who it says it is and can be vunerable to a man in the middle attack.

* **Server cannot store the passwords securely**

Because the Digest requires the password as part of the hash, the server actually needs to store the password in a recoverable way (Plain Text for example).
Since most passwords are stored as [bcrypt](https://www.usenix.org/legacy/events/usenix99/provos/provos_html/node1.html) encrypted values, it is nearly impossible
to actually implement Digest Authentication today. Some servers attempt to store the hashed values instead, but this means that if the username or password is
altered the stored hashes would need to be updated.

## Why use Digest Authentication

Unlike Basic Authentication, Digest Authentication provides a security layer with no easy method to decode the password. Digest Authentication is really
only an extension to Basic Authentication in that regard and should be considered for much the same reasons Basic Authentication would be used. While it is
much more secure, it does include a lot of extra overhead in requests made and forces the client to either store the username and password locally or ask
the user to authenticate on each request. It also does not provide any proper methods of performing Authorization, although it does provide a opaque header.

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
* Add a Authentication Filter for Digest Authentication (Controllers\DigestAuthentication.cs)

# Usage

In the UsersController it is possible to see a Authenitcation Filter `[DigestAuthenticator(realm: "Basic", opaque: "Some Values")]`. This
is what the Controller needs to enable Basic Authentication on a Controller or Route.

The only available end point is http://localhost/api/users/:id with the only valid id being 1. Other
ids will be accepted but will return a 404 status code on a authenticated route.

In order to Authenticate a Digest Authentication Request must be sent with the Username and Password
being the same, but not blank.

# Testing

Run the Application in Visual Studio. Once running start up your favourite REST client and try
send the following

* **Unauthenticated Request**

*Request*: `GET http://localhost/api/users/1`

*Response*: 401 Unauthorised. Note that the Headers contain a Entry `WWW-Authenticate →Digest realm="Basic",qop="MD5",nonce="7f5b27a78a714ccbb1164952104a6c7c",opaque="VGVzdCBPcGFxdWU="`

* **Authenticated Request**

*Request*: `GET http://localhost/api/users/1`
           `Header: Authorization:Digest username="test", realm="test", nonce="test", uri="/api/users/1", response="b14beaa440278b94c6e1273893559b6b", opaque="VGVzdCBPcGFxdWU="
           The Header contains the UserName *test* and password *test*

*Response*: 200
            `{
                "id": "1",
                "name": "Test 1"
             }`