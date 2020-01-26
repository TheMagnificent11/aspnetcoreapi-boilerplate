# AspNetCoreApi.Boilerplate

Base classes, Autofac modules and extensions methods to help configure ASP.Net Core Web API apps.

## Build Status
[![Build Status](https://saji.visualstudio.com/Open%20Source/_apis/build/status/TheMagnificent11.aspnetcoreapi-boilerplate?branchName=master)](https://saji.visualstudio.com/Open%20Source/_build/latest?definitionId=37&branchName=master)

## Dependencies

AspNetCoreApi.Boilerplate is a .Net Standard 2.1 class library that has the following dependencies.

This is not a full-dependency tree, but just the major dependencies listed as close to the top of the tree as possible.

- [RequestManagement](https://www.nuget.org/packages/RequestManagement/): wrapper for simplifying repreated code in `MediatR`
  - [EntityManagement](https://www.nuget.org/packages/EntityManagement/): `EntityFramework.Core` repository pattern
    - [EntityManagement.Core](https://www.nuget.org/packages/EntityManagement.Core/): Base entites and interfaces for `EntityManagment` written in a DDD-style
      - [FluentValidation](https://www.nuget.org/packages/FluentValidation/): Used for validation rules in domain entities
    - [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
  - [MediatR](https://www.nuget.org/packages/MediatR/)
  - [Serilog](https://www.nuget.org/packages/Serilog/)
  - [Autofac](https://www.nuget.org/packages/Autofac/)
- [AutoMapper](https://www.nuget.org/packages/AutoMapper/)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)

## How to use

1. Create a new ASP.Net Core 3.1 API website using the "blank" template.
2. Add the `AspNetCoreApi.Boilerplate` package reference.
3. Add the other non-analyser package references shown in the sample applications [csproj-file](/SampleApiWebApp/SampleApiWebApp.csproj).
4. Configure [Program.cs](/SampleApiWebApp/Program.cs) as shown the linked sample.
5. Create [Startup.cs](/SampleApiWebApp/Startup.cs) class and inherits from `AspNetCoreApi.Boilerplate.AppStartupBase`.
6. Implement the abstract properties and methods.
7. Add the settings to your [appsettings.json](/SampleApiWebApp/appsettings.json) shown in the linked sample.
8. Implement the rest of your application.
  