# ASP.NET MVC 5 Mixed Authentication
Mixing Windows and Forms Authentication (Windows + Forms) 

![Login](https://raw2.github.com/MohammadYounes/MVC5-MixedAuth/screens/screens/Login.PNG)

### Visual Studio Update 3

A new updated branch is available [here](https://github.com/MohammadYounes/MVC5-MixedAuth/tree/Update3). Also view [this comparison](https://github.com/MohammadYounes/MVC5-MixedAuth/compare/0544d70937035c6d220520c76c4e3a7df20afe28...Update3) for the list of changes required to add Mixed Authentication support.


#### How its done ?

The basic idea is to have a managed handler to perform windows authentication, then hand control over to the cookies authentication middleware.

> It will appear as if its an external provider. [Sample Screens](https://github.com/MohammadYounes/MVC5-MixedAuth/wiki/Screens)


#### Running the solution locally

> No special requirements! Visual Studio Express 2013 is all you need.

* Clone the repository: ```git clone git@github.com:MohammadYounes/MVC5-MixedAuth.git```

* Open the solution using Visual Studio, build and run.


#### Enabling Windows Authentication on IIS Express.

* From Solution Explorer, select MixedAuth project then press F4 to view Project Properties and  Make sure "Windows Authentication" is enabled.

      ![IIS Express](https://raw2.github.com/MohammadYounes/MVC5-MixedAuth/screens/screens/WinAuth.Enabled.PNG)


#### Importing AD Groups as Role Claims:

 All AD groups asscociated with the user windows account are available when you hit the [WindowsLogin Action], you can fetch all of them by iterating over `Request.LogonUserIdentity.Groups`:


``` C#
private void MapGroupsToRoleClaims(ApplicationUser user)
{
  foreach (var group in Request.LogonUserIdentity.Groups)  
    user.Claims.Add(new IdentityUserClaim()
    {
      ClaimType = ClaimTypes.Role,
      ClaimValue = new SecurityIdentifier(group.Value)
                         .Translate(typeof(NTAccount)).Value
    });
}
```

[WindowsLogin Action]: https://github.com/MohammadYounes/MVC5-MixedAuth/blob/master/src/Controllers/AccountController.Windows.cs#L38



##### Flowchart of the Windows Login / Link code flow (Contributed by [@ComboFusion](https://github.com/ComboFusion))


![mixedauth-windowsloginhandler](https://cloud.githubusercontent.com/assets/371709/4605670/e765e5ca-51f2-11e4-8f63-328cd456d120.jpg)


------

##### Please [share any issues](https://github.com/MohammadYounes/MVC5-MixedAuth/issues?state=open) you may have.
