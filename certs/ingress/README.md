# Ingress certificate

## Generate

> To generate the certificates, [openssl](https://www.openssl.org/) is required

1) Generate `.key`:

```shell
openssl genrsa -out ingress.key 2048
```

2) Generate signing request `.csr`:

- The signing request will be created based on the configuration file (`-config ingress.config`)

```shell
openssl req -new -key ingress.key -out ingress.csr -config ingress.config
```

3) Generate the certificate `.pem`:

- The signing request will be created based on the extented file (`-extfile ingress.ext`)

```shell
openssl x509 -req -in ingress.csr -CA ../ca/ca.pem -CAkey ../ca/ca.key -CAcreateserial -out ingress.pem -days 3650 -sha256 -extfile ingress.ext
```

4) Generate `.crt` from `.pem`:

```shell
openssl x509 -req -in ingress.csr -CA ../ca/ca.pem -CAkey ../ca/ca.key -CAcreateserial -out ingress.pem -days 3650 -sha256 -extfile ingress.ext
```

5) Generate `.pfx` from `.pem` and `.key`:

- A password will be requested. In this sample, no password was used (just press enter)

```shell
openssl pkcs12 -export -out ingress.pfx -inkey ingress.key -in ingress.crt
```