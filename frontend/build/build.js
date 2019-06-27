require('./check-versions')()

process.env.NODE_ENV = 'production'

var ora = require('ora')
var rm = require('rimraf')
var path = require('path')
var chalk = require('chalk')
var webpack = require('webpack')
var config = require('../config')
var webpackConfig = require('./webpack.prod.conf')
var ncp = require('ncp').ncp;

var spinner = ora('building for production...')
spinner.start()

var buildPath = config.build.assetsRoot;
var destPath = path.resolve(__dirname, "../../MQTT-Proxy/bin/Debug/public");
var destPath1 = path.resolve(__dirname, "../../MQTT-Proxy/bin/Release/public");

rm(path.join(config.build.assetsRoot, config.build.assetsSubDirectory), err => {
  if (err) throw err
  webpack(webpackConfig, function (err, stats) {
    spinner.stop()
    if (err) throw err
    /*COPY FILES TO DEBUG FOLDER*/
    console.log("Copying from", buildPath,"to", destPath);
    ncp(buildPath, destPath, (err) => {
      if (err) throw err
      process.stdout.write(stats.toString({
        colors: true,
        modules: false,
        children: false,
        chunks: false,
        chunkModules: false
      }) + '\n\n')

      console.log(chalk.cyan('  Build complete.\n'))
      console.log(chalk.yellow(
        '  Tip: built files are meant to be served over an HTTP server.\n' +
        '  Opening index.html over file:// won\'t work.\n'
      ))
      console.log("Files at", destPath);
    });
    /*COPY FILES TO RELEASE FOLDER*/
    console.log("Copying from", buildPath,"to", destPath1);
    ncp(buildPath, destPath1, (err) => {
      if (err) throw err
      process.stdout.write(stats.toString({
        colors: true,
        modules: false,
        children: false,
        chunks: false,
        chunkModules: false
      }) + '\n\n')

      console.log(chalk.cyan('  Build complete.\n'))
      console.log(chalk.yellow(
        '  Tip: built files are meant to be served over an HTTP server.\n' +
        '  Opening index.html over file:// won\'t work.\n'
      ))
      console.log("Files at", destPath);
    });
  })
})
