const path = require('path');
const rxPaths = require('rxjs/_esm5/path-mapping');

const webpack = require('webpack');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const FilterWarningsPlugin = require('webpack-filter-warnings-plugin');

const ENV = process.env.ENV = process.env.NODE_ENV = 'development';
const API_URL = process.env.API_URL = "http://localhost:5001";
const OPENID_URL = process.env.OPENID_URL = "https://localhost:60000";
const REDIRECT_URL = process.env.REDIRECT_URL = "http://localhost:4200";
const BASE_URL = process.env.BASE_URL || "/";

const ROOT = path.resolve(__dirname, '..');

console.log('@@@@@@@@@ USING DEVELOPMENT @@@@@@@@@@@@@@@');

module.exports = {
    mode: 'development',
    devtool: 'source-map',
    performance: {
        hints: false
    },
    entry: {
        polyfills: './angularApp/polyfills.ts',
        vendor: './angularApp/vendor.ts',
        app: './angularApp/main.ts'
    },

    output: {
        path: ROOT + '/wwwroot/',
        filename: 'dist/[name].bundle.js',
        chunkFilename: 'dist/[id].chunk.js',
        publicPath: BASE_URL
    },

    resolve: {
        extensions: ['.ts', '.js', '.json'],
        alias: rxPaths()
    },

    devServer: {
        historyApiFallback: true,
        contentBase: path.join(ROOT, '/wwwroot/'),
        watchOptions: {
            aggregateTimeout: 300,
            poll: 1000
        }
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                use: [
                    'awesome-typescript-loader',
                    'angular-router-loader',
                    'angular2-template-loader',
                    'source-map-loader',
                    'tslint-loader'
                ]
            },
            {
                test: /\.(png|jpg|gif|woff|woff2|ttf|svg|eot|webmanifest)$/,
                use: 'file-loader?name=assets/[name]-[hash:6].[ext]'
            },
            {
                test: /favicon.ico$/,
                use: 'file-loader?name=/[name].[ext]'
            },
            {
                test: /\.css$/,
                use: ['to-string-loader', 'style-loader', 'css-loader']
            },
            {
                test: /\.scss$/,
                include: path.join(ROOT, 'angularApp/styles'),
                use: ['to-string-loader', 'style-loader', 'css-loader', 'sass-loader']
            },
            {
                test: /\.scss$/,
                exclude: path.join(ROOT, 'angularApp/styles'),
                use: ['raw-loader', 'sass-loader']
            },
            {
                test: /\.html$/,
                use: 'raw-loader'
            }
        ],
        exprContextCritical: false
    },
    plugins: [
        function () {
            this.plugin('watch-run', function (watching, callback) {
                console.log(
                    '\x1b[33m%s\x1b[0m',
                    `Begin compile at ${new Date().toTimeString()}`
                );
                callback();
            });
        },

        new webpack.optimize.ModuleConcatenationPlugin(),

        // new webpack.optimize.CommonsChunkPlugin({ name: ['vendor', 'polyfills'] }),

        new CleanWebpackPlugin({
            cleanOnceBeforeBuildPatterns: ['./wwwroot/dist', './wwwroot/assets'],
            root: ROOT
        }),

        new HtmlWebpackPlugin({
            filename: 'index.html',
            inject: 'body',
            template: 'angularApp/index.ejs',
            templateParameters: {
                baseUrl: BASE_URL
            }
        }),

        new CopyWebpackPlugin([
            { from: './angularApp/assets/i18n/*.*', to: 'assets/i18n', flatten: true },
        ]),

        new FilterWarningsPlugin({
            exclude: /System.import/
        }),

        new webpack.DefinePlugin({
            'ENV': JSON.stringify(ENV),
            'API_URL': JSON.stringify(API_URL),
            'OPENID_URL': JSON.stringify(OPENID_URL),
            'REDIRECT_URL': JSON.stringify(REDIRECT_URL),
            'process.env': {
                'ENV': JSON.stringify(ENV),
                'API_URL': JSON.stringify(API_URL),
                'BASE_URL': JSON.stringify(BASE_URL),
                'OPENID_URL': JSON.stringify(OPENID_URL),
                'REDIRECT_URL': JSON.stringify(REDIRECT_URL)
            }
        })
    ]
};
