# Basic Authentication Sample

This project demonstrates the most simple use of Basic Authentication.

The username and password are not verified in any way, only their presence is verified.

The /Secret page requires authentication.
* If you visit this page without being authenticated, you will be challenged for a username and password.
* Entering any non-empty username and password will grant access.
* The page will show the identities and claims that the basic authentication infrastructure produced.

## How to recreate this project

Summary of the steps needed.
* Create a new Razor Pages website (.NET Core 2.1.0 latest version).
* Install the `Microsoft.AspNetCore.Authentication.Basic` package.
* Add a page and add `[Authorize(Policy = "<you choose a name>")]` attribute
  to the code behind file (`PageName.cshtml.cs`).
* Edit the razor file to enumerate the identities and claims in `HttpContext.User` and show their values.
* Add authentication and add `.AddBasicAuthentication()` after it to `ConfigureServices` in `Startup.cs`.
* Add authorization and provide a configure action with a call to `.AddPolicy("<you choose a name>", configurePolicy)`
  and in the `configurePolicy` action method call `.AddAuthenticationSchemes("Basic")` and `.RequireAuthenticatedUser()`.
