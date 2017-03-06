## helloworld-webpart

journey from [helloworld-webpart](https://dev.office.com/sharepoint/docs/spfx/web-parts/get-started/build-a-hello-world-web-part) to [Fable](http://fable.io/)

### Building the code

```bash
git clone the repo
npm i
npm i -g gulp
build
gulp
```

This package produces the following:

* lib/* - intermediate-stage commonjs build artifacts
* dist/* - the bundled script, along with other resources
* deploy/* - all resources which should be uploaded to a CDN.

### Build options

gulp clean - TODO
gulp test - TODO
gulp serve - TODO
gulp bundle - TODO
gulp package-solution - TODO

### Run

```bash
cd src\Client
fable --watch
```

```bash
gulp serve
```
