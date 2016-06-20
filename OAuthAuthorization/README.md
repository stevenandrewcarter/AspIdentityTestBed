# OAuth Authentication

This project provides a OAuth Authentication example with ASP.NET Identity. The idea is to provide an example of using OAuth to validate a user against
a third party end point. For the example the Google OAuth sign in will be used to authenticate the user.

## What is OAuth

[OAuth](http://oauth.net/) is a open standard for providing secure authorization. OAuth is not actually a Authentication scheme in the standard sense, since
it does not actually authenticate a user on *your* system, instead it allows *your* system to interact with another system. What this means is the *other* system
is responsible for authentication, and *your* system trusts that system authenticated the user correctly.

It is possible to use OAuth to provide an Authentication Strategy, and [Open Id Connect](http://openid.net/connect/) provides such a process. Using OAuth to help
authenticate a user.

## Advantages of OAuth

* **Delegated Authentication**

Your application does not actually need to authenticate against the third party service. So usernames and passwords are not required for your application. All that
is required is a OAuth token provided from the third party. That token can be used to make requests against the third party application on your users behalf.

* **User has Control**

The User is able to enable or disable access as they want. This means that your application cannot do an action the user doesn't want. This allows the user
to have control over the access to the third party site. In addition the user will be informed about what your application wants access to, so if they don't
feel comfortable allowing that access they can stop the connection.

* **Well Supported**

A lot of the big names on the internet provide OAuth services. Meaning it should be well supported and simple to get a OAuth registration if required.

## Disadvantages of OAuth

* **Not for Authentication**

It's not really a good system by itself for your applications authentication. It does not do anything to help secure **YOUR** application. In fact the way OAuth
works is that your applications end-points are checked against the third party.

* **Can be Revoked without notice**

Since the User maintains control, it means that the access can be revoked without notice. This means that any type of service that requires that token will
stop working.

* **Need to register**

OAuth requires registration with the third party site. If you are not registered then it's not even possible to generate any requests. It's also important
to realize that in order to test the OAuth requests, a user will also need to be registered on the third party site (Unless a test user is provided).

## Why use OAuth

OAuth is only really required when access to a third party site is required. OAuth is not suitable for internal authentication strategies, and other techniques
work better in those situations anyway. 

# The Project

OAuth is really simple to enable for a standard ASP.NET Identity Application. The [Microsoft Documentation](http://www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on)
is fairly comprehensive. This project shows the completed result for Google OAuth Authorization.

**Note**: The Google Development Console must have the Google+ API enabled. This can be found under *Overview->Social APIs->Google+ API*

# Usage

Run the application and click the sign in. Then select the Google button on the right and authorize the google application. Please provide your own API keys
under the *Startup.Auth.cs*