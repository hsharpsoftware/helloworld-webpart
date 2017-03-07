
## helloworld-webpart

journey from [helloworld-webpart](https://dev.office.com/sharepoint/docs/spfx/web-parts/get-started/build-a-hello-world-web-part) to [Fable](http://fable.io/)

### How to

1. Put the F# client into `src\Client`.
2. In your TypeScript webpart `*.ts` add `import { renderTodoApp } from "../../Client/out/react-component";`
3. In the same file change `public render(): void` to add `id` to the enclosing `div` and call `renderTodoApp` at the end
4. In `gulpfile.js` add 

```
build.configureWebpack.mergeConfig({ 
  additionalConfiguration: (generatedConfiguration) => { 
    generatedConfiguration.module.loaders.push([ 
      {
      test: /\.js$/,
      //exclude: /(node_modules|bower_components)/,
      loader: 'babel-loader',
      query: {
        presets: ['es2015']
      }
    } 
    ]); 

    return generatedConfiguration; 
  } 
});
```

before the last line `build.initialize(gulp);`

5. in `package.json` and to dependecies 

```
    "fable-core": "^0.7.25",
    "fable-powerpack": "^0.0.19",
    "fable-react": "^0.8.0",
    "fable-plugins-nunit": "^0.7.2",
    "fable-import-d3": "^0.0.4",
    "fable-import-pixi": "^0.0.10",
    "fable-import-three": "^0.0.4",
```

and to devDependencies

```
    "babel-core": "^6.23.1",
    "babel-loader": "^6.3.2",
    "babel-preset-es2015": "^6.22.0",
    "fable-compiler": "^0.7.34",
    "http-server": "^0.9.0",
    "source-map-loader": "^0.1.5",
    "webpack": "^2.2.1"
```

6. in `tsconfig.json` add `"allowJs": true,` and remove `"declaration": true,`

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

In one console window

```bash
cd src\Client
fable --watch
```

In second console window

```bash
gulp serve
```
