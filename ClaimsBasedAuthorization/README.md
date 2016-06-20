# Claims-based identity

Claims-based identity is a method for applications to gather identity information for users inside the users organization, other organizations or the internet.
It provides a consistent approach for internal applications or cloud based applications. The individual elements of the users identity are abstracted into two parts:
a notion of claims and the concept of an issuer or authority.

A claim is a statement about a subject (User or organization). Claims are often packaged into one or more tokens (JWT are commonly used for this). Claims do not 
model what a user can and cannot do, but are what the user is or is not. The application is still responsible for using that information to determine if the user
can perform a particular action (Role-based access control).

## What is Claims-based identity

[Claims-based identity](http://download.microsoft.com/download/7/D/0/7D0B5166-6A8A-418A-ADDD-95EE9B046994/Claims-Based%20Identity%20for%20Windows.pdf) is the concept of encoding identity information
with a token or tokens. The token is often a JSON Web Token, but could also be a signed Cookie. Claims transfer the information about a user with the requests, instead of having the receiving application
looking up the various roles and access permissions for the user. Claims as a result can be sent between different systems (as long as they both trust the claims provider), without having to look up
the user authorization information again. 

Claims do not replace a Role-based access control approach, but instead allow the Roles to be defined on a provider instead of each individual system. Cloud based systems find some benefit in having Claims
provided, as the system might have different users from different organizations attempting to access the application.

## Advantages of Claim-based Identity

* **Reduced Duplication**

Since the Claims provider is responsible for determining a users possible identity, it means that the applications that use those claims just need to trust the provider. The applications then do not require
additional lookups or internal storage for the various claims that the provider generates.

* **No Sessions**

Claims are encoded into the header and passed with each request. Which means the concepts of a session is not required for a claim to function. Claims are also possible to send between different systems without
any handshakes between the systems beforehand.

* **Can use Cookies or JWT**

Claims can be encoded into Cookies or JWTs. The flexibility this provides allows for the concept to work in systems where one technology might be disabled or not allowed.

* **Is a process**

Claim-based Idenitity is easy to extend on existing Role-based access control systems. It just changes the process to use tokens to encode claims instead of checking 
the Roles for the user.

## Disadvantages of Claim-based Identity

* **Does not elimate problems with Role-based Access Control**

All the challenges with a Role-based Access Control system are still present in a Claims-based Identity.

* **Must trust the Provider**

A Claims provider is required for the different systems to trust. When a new application is added it must also be able to trust the claims provider.

* **Adds overhead for the client**

A claim must be generated for the client to be able to perform operations on the systems. The client is responsible for retrieving the claims information
and sending that information on each request. 

## Why use Claims-based Identity

Claims provided a stateless process of sending around identity information to various systems. A claim allows a provider to describe a user to many
systems without the need for each system to model the Roles internally.  

# The Project

The Project provides an example ASP.NET Identity WebAPI Application. The Web API is protected by Authentication via [JSON Web Tokens](https://jwt.io/) and
Authorization via Roles. Roles are stored in the Database according to ASP.NET Identity default setup. Only one API end point is provided (/api/users) and
must be provided authentication to access. A access token can be retrieved by making a (POST:/oauth/token) with valid credentials.

The structure of the project is

* **\Controllers**: The Controllers for the Routes. Provides the RESTful endpoints for the WebAPI.

* **\Filters**: Example of a Claims Filter

* **\Infrastructure**: The underlying implementation that connects the database and the ASP.NET Identity concepts together.

* **\Migratations**: Database migrations and seeding. Review the Configuration.cs and the Seed Method for the Default Administrator User

* **\Models**: Database model entities

* **\Providers**: Providers for the JWT Framework

## Initial Setup

* Check the connection string. It will attempt to connect to the SQL instance on the localhost.
* Run the Migrations
```
update-database
```
* Start the Server so that the end points are available.

# Usage

## Generating a JWT

Send a {POST:\oauth\token} with *x-www-form-urlencoded* body

```
POST:\oauth\token

x-www-form-urlencoded
username: SuperPowerUser
password: MySuperP@ssword!
grant_type: password

Response
{
  "access_token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJuYW1laWQiOiJlMGI5OWZkMC05NDBlLTQ4Y2UtOTkyMi03MTE2NmEwMWVjZjgiLCJ1bmlxdWVfbmFtZSI6IlN1cGVyUG93ZXJVc2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS9hY2Nlc3Njb250cm9sc2VydmljZS8yMDEwLzA3L2NsYWltcy9pZGVudGl0eXByb3ZpZGVyIjoiQVNQLk5FVCBJZGVudGl0eSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiMjY0ODM3OTYtZTZiNC00YjQwLWE2YmUtYzA5MzhiNmI0NzA5Iiwicm9sZSI6WyJTdXBlckFkbWluIiwiQWRtaW4iXSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1Njk4NyIsImF1ZCI6IjA5OTE1M2MyNjI1MTQ5YmM4ZWNiM2U4NWUwM2YwMDIyIiwiZXhwIjoxNDY2MjYxODc2LCJuYmYiOjE0NjYxNzU0NzZ9.f4-80qOEGBAukxcdVCCFwkiS_ir6Uq0iLxwL8901gzw",
  "token_type": "bearer",
  "expires_in": 86399
}
```

From that response it is possible to send a request to any of end points on {\api\users}. An example request is provided below
```
GET:\api\users

HEADER:
Authorization: Bearer <TOKEN>

Response
[
  {
    "url": "http://localhost:56987/api/users/e0b99fd0-940e-48ce-9922-71166a01ecf8",
    "id": "e0b99fd0-940e-48ce-9922-71166a01ecf8",
    "userName": "SuperPowerUser",
    "fullName": "Test Testser",
    "email": "test@test.com",
    "emailConfirmed": true,
    "level": 1,
    "joinDate": "2013-06-10T15:10:24.707",
    "roles": [
      "SuperAdmin",
      "Admin"
    ],
    "claims": []
  }
]
```

## Testing the Claims

Retrieving the Claims for a Token is as simple as sending a GET request to the /api/claims endpoint.

```
GET: /api/claims

HEADER: 
Authorization: Bearer <TOKEN>

BODY: [
  {
    "subject": "TestNormalUser",
    "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
    "value": "ab80f898-d44c-4f62-8586-3d70198bc060"
  },
  {
    "subject": "TestNormalUser",
    "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
    "value": "TestNormalUser"
  },
...
  ]
```

The user creation endpoint {POST:/api/users} has been updated to include a Claims check (Full Time Employee). That means a User must be both a Admin and a 
Full time employee to access that API end point.