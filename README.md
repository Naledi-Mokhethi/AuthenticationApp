# Authentication App RESTful API 
## Overview 
- This is a RESTful API using ASP.NET Core Web API
- It is a C# app using .NET 8 
- The API is being built to decouple authentication from the [HospitalQueue](https://github.com/Naledi-Mokhethi/HospitalQueue) MVC application

## Detailed Walkthrough
- This is a custom authentication API.
- It is built on .NET 8 using visual studio.
- The structure incorporates Data Transfer objects to protect the model/entity data.
- The database communication is established using dapper and stored procedures.
- After a login has been successfully established we use Json Web Tokens for the client to enhance security 

## Developers Guide
- You need visual studio or visual studio code as an editor
- You need SQL Server or SQL Express
- You need SSMS for the stored procedures and database management
- You need the .NET 8 SDK (Current LTS)
