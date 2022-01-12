START cmd /k "cd src/Website/SimpleIdServer.Gateway.Host && dotnet run"
START cmd /k "cd src/Website/SimpleIdServer.Website && npm install && npm run start"
START cmd /k "cd src/OpenID/SimpleIdServer.OpenID.Startup && dotnet run"
START cmd /k "cd src/Scim/SimpleIdServer.Scim.Startup && dotnet run"
START cmd /k "cd src/CaseManagement/CaseManagement.HumanTask.Host && dotnet run"
START cmd /k "cd src/CaseManagement/CaseManagement.BPMN.Host && dotnet run"
START cmd /k "cd src/UMA/SimpleIdServer.Uma.Startup && dotnet run"
echo Applications are running ...