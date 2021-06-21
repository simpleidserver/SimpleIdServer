START cmd /k "cd src/Website/SimpleIdServer.Gateway.Host && dotnet run"
START cmd /k "cd src/Website/SimpleIdServer.Website && npm run start"
START cmd /k "cd src/OpenID/SimpleIdServer.OpenID.Startup && dotnet run"
START cmd /k "cd src/Scim/SimpleIdServer.Scim.Startup && dotnet run"
echo Applications are running ...