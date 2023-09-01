# CA certificate

## Generate

> To generate the certificates, [openssl](https://www.openssl.org/) is required

Generate `.pem` and `.key`:

- The certificate will be breated based on the configuration file (`-config ca.config`)

```shell
openssl req -x509 -nodes -days 3650 -keyout ca.key -out ca.pem -config ca.config
```

Generate `.crt` from `.pem`:

```shell
openssl x509 -outform PEM -in ca.pem -out ca.crt
```

## Import certificate

To import the CA in Windows:

```shell
Import-Certificate -FilePath "ca.crt" -CertStoreLocation Cert:\CurrentUser\Root
```

To import the CA on Debian or Ubuntu distros:

```shell
cp ca.crt /usr/local/share/ca-certificates/ca.crt && update-ca-certificates
```

For other Linux distros or OS, google it! 😉