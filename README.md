# .NET mTLS Example

A dotnet WebApi and gRPC samples that establishes a connection over mTLS (SSL with client certificates).

In this sample, **self-signed** certificates was used, so the first step is generate the CA, client and server certificates. [Follow this instructions](certs/README.md) for generate this ones.

If Kubernets will be used to test, it's also necessary to generate the the Ingress certificate.

> It's imporant to, after creating the CA root certficate, import in the current machine. To this, [follow this instructions](certs/ca/README.md#import-certificate)

## Run

To run the WebApi:

```shell
# Will launch the Server
dotnet run --project src/webapi/Server/ 
# Will launch the Client
dotnet run --project src/webapi/Client/ 
```

To run the gRPC:

```shell
# Will launch the Server
dotnet run --project src/grpc/Server/ 
# Will launch the Client
dotnet run --project src/grpc/Client/ 
```

## Docker

To run the WebApi:

```shell
# Will launch the Server on Docker
docker compose -f docker-compose-webapi.yml -p mtls-webapi-server up 
# Will launch the Client
dotnet run --project src/webapi/Client/ 
```

To run the WebApi with Nginx as proxy:

```shell
# Will launch the Server and Nginx on Docker
docker compose -f docker-compose-webapi-nginx.yml -p mtls-webapi-nginx-server up 
# Will launch the Client
dotnet run --project src/webapi/Client/ 
```

To run the gRPC:

```shell
# Will launch the Server on Docker
docker compose -f docker-compose-grpc.yml -p mtls-grpc-server up 
# Will launch the Client
dotnet run --project src/grpc/Client/ 
```

To run the gRPC with Nginx as proxy:

```shell
# Will launch the Server and Nginx on Docker
docker compose -f docker-compose-grpc-nginx.yml -p mtls-grpc-nginx-server up 
# Will launch the Client
dotnet run --project src/grpc/Client/ 
```

## Kubernets

> Make sure that Nginx Ingress Controller is installed.

Prepare Kubernets creating namespace and the secrets:

```shell
# Will create the namespace for this sample
kubectl create namespace mtls-example
# Will create the CA root secret
kubectl create secret generic ca-certificate -n mtls-example --from-file=ca.crt=certs/ca/ca.crt
# Will create the TLS secret used in the pods
kubectl create secret tls pod-tls -n mtls-example --cert certs/server/server.crt --key certs/server/server.key
# Will create the TLS secret used in the ingresses
kubectl create secret tls ingress-tls -n mtls-example --cert certs/ingress/ingress.crt --key certs/ingress/ingress.key
```

Create the WebApi deployment, service and ingress:

```shell
kubectl apply -n mtls-example -f deployment.webapi.yaml
```

Create the gRPC deployment, service and ingress:

```shell
kubectl apply -n mtls-example -f deployment.grpc.yaml
```

Edit `host` file to include the hosts used in ingresses:

 - `C:\Windows\System32\drivers\etc\hosts` for Windows (edit as admin)
 - `/etc/hosts` for Linux distros (edit with sudo)

```txt
127.0.0.1 mtls-example-webapi.friday.local
127.0.0.1 mtls-example-grpc.friday.local
```

Run the WebApi client:

```shell
# Release configuration uses the host link 
dotnet run --project src/webapi/Client/ -c Release
```

Run the gRPC client:

```shell
# Release configuration uses the host link 
dotnet run --project src/grpc/Client/ -c Release
```