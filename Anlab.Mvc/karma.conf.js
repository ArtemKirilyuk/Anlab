const webpackConfig = require('./webpack.config')();

module.exports = function (config) {
  config.set({
    frameworks: ["jasmine"],
    files: [
        'webpack.tests.js',
    ],
    preprocessors: {
      "webpack.tests.js": [ "webpack", 'sourcemap' ],
    },
    reporters: ["progress"],
    browsers: ["jsdom"],
    singleRun: true,
    webpack: {
      devtool: 'inline-source-map',
      resolve: webpackConfig.resolve,
      module: webpackConfig.module,
      externals: {
        'react/addons': 'react',
        'react/lib/ExecutionEnvironment': 'react',
        'react/lib/ReactContext': 'react',
      },
    },
    webpackMiddleware: {
      stats: 'errors-only'
    },
  });
};