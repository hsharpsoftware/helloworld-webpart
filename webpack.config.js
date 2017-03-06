var path = require("path");
var webpack = require("webpack");

var cfg = {
  devtool: "source-map",
  entry: "./dist/hello-world.bundle.js",
  output: {
    path: path.join(__dirname, "public"),
    filename: "bundle.js",
    libraryTarget: 'var',
    library: 'Greeter'    
  },
  module: {
    loaders: [
      {
        test: /\.js$/,
        exclude: /(node_modules|bower_components)/,
        loader: 'babel-loader',
        query: {
          presets: ['es2015']
        }
      }
    ]
  }
};

module.exports = cfg;