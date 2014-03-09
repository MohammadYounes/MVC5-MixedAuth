# ASP.NET MVC 5 Mixed Authentication
Mixing Windows and Forms Authentication (Windows + Forms) 

![Login](https://raw2.github.com/MohammadYounes/MVC5-MixedAuth/screens/screens/Login.PNG)


#### How its done ?

The basic idea is to have a managed handler to perform windows authentication, then hand control over to the cookies authentication middleware.

> It will appear as if its an external provider. [Sample Screens](https://github.com/MohammadYounes/MVC5-MixedAuth/wiki/Screens)


#### Running the solution locally

> No special requirements! Visual Studio Express 2013 is all you need.

* Clone the repository: ```git clone git@github.com:MohammadYounes/MixedAuth.git```

* Open the solution using Visual Studio, build and run.


#### Enabling Windows Authentication on IIS Express.

* From Solution Explorer, select MixedAuth project then press F4 to view Project Properties and  Make sure "Windows Authentication" is enabled.

      ![IIS Express](https://raw2.github.com/MohammadYounes/MVC5-MixedAuth/screens/screens/WinAuth.Enabled.PNG)


#### Importing AD Groups as Role Claims:

 All AD groups asscociated with the user windows account are available when you hit the [WindowsLogin Action], you can fetch all of them by iterating over `Request.LogonUserIdentity.Claims`:


``` C#
private void MapGroupSidToRoleClaims(ApplicationUser user)
{
  foreach (var claim in Request.LogonUserIdentity.Claims)
  {
    if (claim.Type == ClaimTypes.GroupSid)
      user.Claims.Add(
        new IdentityUserClaim()
        {
          ClaimType = ClaimTypes.Role,
          ClaimValue = new SecurityIdentifier(claim.Value)
                             .Translate(typeof(NTAccount)).ToString()
        });
  }
}
```

[WindowsLogin Action]: https://github.com/MohammadYounes/MVC5-MixedAuth/blob/master/src/Controllers/AccountController.Windows.cs#L38


------

##### Please [share any issues](https://github.com/MohammadYounes/MVC5-MixedAuth/issues?state=open) you may have.

