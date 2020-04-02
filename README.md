# AspNetCoreApi.Boilerplate

Base classes, Autofac modules and extensions methods to help configure ASP.Net Core Web API apps.

## Build Status

[![Build Status](https://saji.visualstudio.com/Open%20Source/_apis/build/status/AspNetCoreApi-Boilerplate?branchName=master)](https://saji.visualstudio.com/Open%20Source/_build/latest?definitionId=37&branchName=master)

## Dependencies

AspNetCoreApi.Boilerplate is a .Net Standard 2.1 class library that has the following dependencies.

This is not a full-dependency tree, but just the major dependencies listed as close to the top of the tree as possible.

- [AspNetCore.Mediatr](https://www.nuget.org/packages/AspNetCore.Mediatr/): Mediatr configuration
  - [MediatR](https://www.nuget.org/packages/MediatR/)
  - [EntityManagement](https://www.nuget.org/packages/EntityManagement/): `EntityFramework.Core` repository pattern
  - [EntityManagement.Core](https://www.nuget.org/packages/EntityManagement.Core/): Base entites and interfaces for `EntityManagment` written in a DDD-style
    - [FluentValidation](https://www.nuget.org/packages/FluentValidation/): Used for validation rules in domain entities
  - [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)R](https://www.nuget.org/packages/MediatR/)
- [Serilog](https://www.nuget.org/packages/Serilog/)
- [Autofac](https://www.nuget.org/packages/Autofac/)
- [AutoMapper](https://www.nuget.org/packages/AutoMapper/)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)

## How to use

1. Create a new ASP.Net Core 3.1 API website using the "blank" template.
2. Add the `AspNetCoreApi.Boilerplate` package reference.
3. Add the other non-analyser package references shown in the sample applications.

   ```csproj
   <Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
      <TargetFramework>netcoreapp3.1</TargetFramework>
    </PropertyGroup>
    <ItemGroup>
      <PackageReference Include="Autofac.Extensions.DependencyInjection" Version="6.0.0" />
      <PackageReference Include="AspNetCoreApi.Boilerplate" Version="0.2.1" />
      <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="3.1.2">
        <PrivateAssets>all</PrivateAssets>
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      </PackageReference>
      <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="3.1.2" />
    </ItemGroup>
   </Project>
   ```

4. Configure `Program.cs`

   ```cs
       public static class Program
       {
           public static void Main(string[] args)
           {
               var host = Host.CreateDefaultBuilder(args)
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.AddSerilog();
                   })
                   .UseSerilog()
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                   .ConfigureWebHostDefaults(webHostBuilder =>
                   {
                       webHostBuilder
                           .UseContentRoot(Directory.GetCurrentDirectory())
                           .UseStartup<Startup>();
                   })
                   .Build();

               host.Run();
           }
       }
   ```

5. For your `Startup.cs` there are `abstract` memebers for items that need to be explicitly specified and `protected virtual` memmbers for optional configuration.
6. Add the settings to your `appsettings.json`.

   ```json
   {
     "ApplicationSettings": {
       "Name": "My.App",
       "Environment": "Dev",
       "Version": "0.0.1"
     },
     "SeqSettings": {
       "Uri": "http://localhost:5341",
       "Key": ""
     },
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Initial Catalog=My.App.Db;Trusted_Connection=True"
      },
      "AllowedOrigins":  "http://localhost:4200"
   }
   ```

7. Implement the rest of your application.
  