const path = require("path");
const webpack = require("webpack");
const MinifyPlugin = require("terser-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

function resolve(filePath) {
  return path.join(__dirname, filePath);
}

const CONFIG = {
  fsharpEntry: {
    app: [resolve("./FantomasTools/FantomasTools.fsproj")]
  },
  historyApiFallback: {
    index: resolve("./public/index.html")
  },
  contentBase: resolve("./public"),
  // Use babel-preset-env to generate JS compatible with most-used browsers.
  // More info at https://github.com/babel/babel/blob/master/packages/babel-preset-env/README.md
  babel: {
    presets: [
      [
        "@babel/preset-env",
        {
          targets: {
            browsers: ["last 2 versions"]
          },
          modules: false
        }
      ],
      "@babel/preset-react"
    ],
    plugins: ["@babel/plugin-proposal-class-properties"]
  }
};

const isProduction = process.env.NODE_ENV === 'production'
console.log(
  "Bundling for " + (isProduction ? "production" : "development") + "..."
);

const commonPlugins = [
  new MiniCssExtractPlugin({
    filename: isProduction ? "[name].[fullhash].css" : "[name].css",
    chunkFilename: isProduction ? "[name].[fullhash].css" : "[name].css"
  }),
  new HtmlWebpackPlugin({
    filename: resolve("./output/index.html"),
    template: resolve("./public/index.html")
  }),
  // ensure that we get a production build of any dependencies
  // this is primarily for React, where this removes 179KB from the bundle
  new webpack.DefinePlugin({
    "process.env.NODE_ENV": isProduction ? '"production"' : '"development"'
  }),
  // https://webpack.js.org/plugins/environment-plugin/
  new webpack.EnvironmentPlugin([
    "FSHARP_TOKENS_BACKEND",
    "AST_BACKEND",
    "TRIVIA_BACKEND",
    "FANTOMAS_V2",
    "FANTOMAS_V4",
    "FANTOMAS_V3",
    "FANTOMAS_PREVIEW"
  ])
];

module.exports = {
  entry: CONFIG.fsharpEntry,
  output: {
    path: resolve("./output"),
    filename: isProduction ? "[name].[fullhash].js" : "[name].js",
    publicPath: isProduction ? "/fantomas-tools/" : "/"
  },
  mode: isProduction ? "production" : "development",
  devtool: isProduction ? undefined : "source-map",
  optimization: {
    // Split the code coming from npm packages into a different file.
    // 3rd party dependencies change less often, let the browser cache them.
    splitChunks: {
      cacheGroups: {
        commons: {
          test: /node_modules/,
          name: "vendors",
          chunks: "all"
        }
      }
    },
    minimizer: isProduction ? [new MinifyPlugin()] : []
  },
  // DEVELOPMENT
  //      - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
  plugins: isProduction
    ? commonPlugins.concat([
        new CopyWebpackPlugin({patterns:[{ from: resolve("./public") }]})
      ])
    : commonPlugins.concat([new webpack.HotModuleReplacementPlugin()]),
  // Configuration for webpack-dev-server
  devServer: {
    hot: true,
    inline: true,
    historyApiFallback: CONFIG.historyApiFallback,
    contentBase: CONFIG.contentBase,
    port: process.env.FRONTEND_PORT || "8080"
  },
  // - fable-loader: transforms F# into JS
  // - babel-loader: transforms JS to old syntax (compatible with old browsers)
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: "fable-loader"
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader",
          options: CONFIG.babel
        }
      },
      {
        test: /\.(sass|scss|css)$/,
        use: [
          isProduction ? MiniCssExtractPlugin.loader : "style-loader",
          "css-loader",
          "sass-loader"
        ]
      },
      {
        test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
        use: ["file-loader"]
      }
    ]
  }
};
