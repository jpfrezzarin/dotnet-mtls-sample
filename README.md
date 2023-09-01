# .NET mTLS Example

A dotnet WebApi and gRPC samples that establishes a connection over mTLS (SSL with client certificates).

In this sample, **self-signed** certificates was used, so the first step is generate the CA, client and server certificates. [Follow this instructions](certs/README.md) for generate this ones.

> It's imporant to, after creating the CA root certficate, import in the current machine. To this, [follow this instructions](certs/ca/README.md#import-certificate)

