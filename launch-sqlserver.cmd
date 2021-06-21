START cmd /k "cd src/Website/SimpleIdServer.Gateway.Host && dotnet run"
START cmd /k "cd src/Website/SimpleIdServer.Website && npm install && npm run start"
START cmd /k "cd src/OpenID/SimpleIdServer.OpenID.SqlServer.Startup && dotnet run"
START cmd /k "cd src/Scim/SimpleIdServer.Scim.SqlServer.Startup && dotnet run"
echo Applications are running ...