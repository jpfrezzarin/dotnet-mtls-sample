# Server certificate

## Generate

> To generate the certificates, [openssl](https://www.openssl.org/) is required

1) Generate `.key`:

```shell
openssl genrsa -out server.key 2048
```

2) Generate signing request `.csr`:

- The signing request will be created based on the configuration file (`-config server.config`)

```shell
openssl req -new -key server.key -out server.csr -config server.config
```

3) Generate the certificate `.pem`:

- The signing request will be created based on the extented file (`-extfile server.ext`)

```shell
openssl x509 -req -in server.csr -CA ../ca/ca.pem -CAkey ../ca/ca.key -CAcreateserial -out server.pem -days 3650 -sha256 -extfile server.ext
```

4) Generate `.crt` from `.pem`:

```shell
openssl x509 -outform PEM -in server.pem -out server.crt
```

5) Generate `.pfx` from `.pem` and `.key`:

- A password will be requested. In this sample, no password was used (just press enter)

```shell
openssl pkcs12 -export -out server.pfx -inkey server.key -in server.crt
```
