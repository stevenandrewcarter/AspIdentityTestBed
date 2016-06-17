# Role-based Access Control

Role-based Access Control is the most common approach to Authorization that can be found in Applications. A Role is defined as a set of permissions assigned
to a particular user. A Permission is defined as a operation that can be performed on a resource. A user is generally assigned a set of Roles that allow that
user to perform the required job functions they are assigned. A example of a Role might be as general as Administrator and End User or specific such as Sales 
Manager Administrator and Stock Room User. The flexibility of Roles allows for most access control environments to be modelled as roles.

Roles have also been extended to Attribute-based Access Control which includes on a role a set of attributes that must be satisfied in order for the Role to be active.
An example of Attributes could be location, time of day or running jobs. So it would be possible to prevent a User from having full access if they are not located
at a particular terminal.

## What is Role-based Access Control

[Role-based Access Control](http://csrc.nist.gov/rbac/sandhu96.pdf)(RBAC) is a concept in which access to various functions on a system are restricted by permissions.
Permissions are grouped together and called a Role. For example, possible permissions could be create, read, update or delete a document. From the given permissions
the Role Document-Admin might consist of all of those permissions. A Document-Manager on the other hand might not have the permission to delete a document, 
and a Document-Reader might only have the read permission.

By separating users from roles and roles from permissions allows for a lot of flexibility in a system. The flexibility does include a complexity cost as the
number of permissions and roles is increased. It is also not uncommon for a Role to contain a subset of Roles instead of direct permissions. Such *Role Hierarchies*
can increase the complexity of the system.  

## Advantages of Role-based Access Control

* **Widely Accepted**

Most systems and techniques recognise Role-based Access Control. In fact a lot of frameworks already provide techniques for implementing Roles. In fact a
set of standards such as [SAML](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=security) and [XACML](http://xml.coverpages.org/xacml.html) exist
to provide a method of defining standard roles.

* **Simple**

Roles are easy to define and in small systems easy to implement. Roles are also very easy to model as data structures.

* **Matches User Responsibilities**

Roles can easily be matched to the user, and generally match the users responsibility or job. For example, the job-title Sales Manager could also be the Role that
is assigned to the user. As such, Roles are easily understood and assigned.

## Disadvantages of Role-based Access Control

* **Role Explosion**

As systems grow in size, the number of Roles will likely grow as well. Often resulting in minor differences between Roles (EG: Sales Manager and Senior Sales Manager). 
It can become quite unweidly to manage massive numbers of Roles.

* **Not Transferrable**

A Role is not always transferrable from system to system. So if a user is a Sales Manager on more than one system, then each system will have to model the
permissions for that role.

* **Not encoded**

Roles are checked on the system against a user, and will need to be retrieved for each request against a protected end point. This means that in a complex 
role system it can add a lot of extra overhead to each request.

## Why use Role-based Access Control

In almost every system the concept of Roles is required. It's fairly common to require that specific users are restricted to particular actions in order to 
prevent accidental or unauthorised actions on the system. Implementing Attributes also allows for additional control by preventing operations when the attributes
are not satisfied. 

Restricted Access Control is also useful for the User, since they should only be able to see / access the portions of the system that they are responsible for.
Users can be trained quicker and will be confident in a system that only displays the components they have access to. 

# The Project

The Project provides an example ASP.NET Identity WebAPI Application. The Web API is protected by Authentication via [JSON Web Tokens](https://jwt.io/) and
Authorization via Roles. Roles are stored in the Database according to ASP.NET Identity default setup. Only one API end point is provided (/api/users) and
must be provided authentication to access. A access token can be retrieved by making a (POST:/oauth/token) with valid credentials.

The structure of the project is

* **\Controllers**: The Controllers for the Routes. Provides the RESTful endpoints for the WebAPI.

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

Header
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJuYW1laWQiOiJlMGI5OWZkMC05NDBlLTQ4Y2UtOTkyMi03MTE2NmEwMWVjZjgiLCJ1bmlxdWVfbmFtZSI6IlN1cGVyUG93ZXJVc2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS9hY2Nlc3Njb250cm9sc2VydmljZS8yMDEwLzA3L2NsYWltcy9pZGVudGl0eXByb3ZpZGVyIjoiQVNQLk5FVCBJZGVudGl0eSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiMjY0ODM3OTYtZTZiNC00YjQwLWE2YmUtYzA5MzhiNmI0NzA5Iiwicm9sZSI6WyJTdXBlckFkbWluIiwiQWRtaW4iXSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1Njk4NyIsImF1ZCI6IjA5OTE1M2MyNjI1MTQ5YmM4ZWNiM2U4NWUwM2YwMDIyIiwiZXhwIjoxNDY2MjU4MjgzLCJuYmYiOjE0NjYxNzE4ODN9.0onVkg_dvsbebBdmfLKcE8-LXVI3O6p_zNfBcx-RDTw

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

