# AspIdentityTestBed

<p align="center">
    <img src="https://ci.appveyor.com/api/projects/status/github/stevenandrewcarter/aspidentitytestbed"height="130">
</p>

Examples of using [ASP.NET Identity](http://www.asp.net/identity). The Projects are split into different concepts that try to display the different 
aspects of Authentication / Authorization.

The projects are split as follows

## BasicAuthentication

Example of User HTTP Basic Authentication as a Authentication Strategy using the ASP.NET Identity Framework.
Basic Authentication is **not a recommended technique** and is just provided for completeness.

## DigestAuthentication

A more secure strategy compared to HTTP Basic Authentication.

## OAuthAuthentication

Use OAuth 2.0 as a Authentication / Authorization strategy.

## RoleBasedAuthorization

Simple example to demostrate Roles in the ASP.NET Identity Framework

## ClaimsBasedAuthorization

Simple example to demostrate Claims in the ASP.NET Identity Framework. Claims Authorization is really just an extension of the Role Based Authorization.

# References

[ASP.NET Identity 2.1 with ASP.NET Web API 2.2](http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/): Claims and Role Based Authorization is based on this series of articles.
