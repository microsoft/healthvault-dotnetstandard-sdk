# HealthVault Client

This client is generated using AutoRest from the HealthVault [Swagger definition](https://developer.healthvault.com/Api).

## AutoRest

The AutoRest tool can be installed by following the instructions at the [AutoRest page](https://github.com/Azure/autorest).

## Regenerating
An example of the command to regenerate this client is as follows:

```
AutoRest -Namespace HealthVault.Client -CodeGenerator CSharp -Modeler Swagger -Input c:\temp\swagger.json -PackageName HealthVault.Client -AddCredentials true
```