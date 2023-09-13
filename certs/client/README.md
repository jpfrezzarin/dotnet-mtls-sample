# Client certificate

## Generate

> To generate the certificates, [openssl](https://www.openssl.org/) is required

1) Generate `.key`:

```shell
openssl genrsa -out client.key 2048
```

2) Generate signing request `.csr`:

- The signing request will be created based on the configuration file (`-config client.config`)

```shell
openssl req -new -key client.key -out client.csr -config client.config
```

3) Generate the certificate `.pem`:

- The signing request will be created based on the extented file (`-extfile client.ext`)

```shell
openssl x509 -req -in client.csr -CA ../ca/ca.pem -CAkey ../ca/ca.key -CAcreateserial -out client.pem -days 3650 -sha256 -extfile client.ext
```

4) Generate `.crt` from `.pem`:

```shell
openssl x509 -outform PEM -in client.pem -out client.crt
```

5) Generate `.pfx` from `.pem` and `.key`:

- A password will be requested. In this sample, no password was used (just press enter)

```shell
openssl pkcs12 -export -out client.pfx -inkey client.key -in client.crt
```
