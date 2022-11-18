let MiniCssExtractPlugin = require('mini-css-extract-plugin');
let path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
    mode: 'production',
    entry: {
        main: ['./src/js/site.js', './src/css/site.scss'],
        'google-analytics-events': './src/js/google-analytics-events.js',
    },
    plugins: [new MiniCssExtractPlugin({ filename: 'site.css' }),
        new CopyPlugin({
            patterns: [ { from: path.join(__dirname, 'node_modules/accessible-autocomplete/dist'), to: path.join(__dirname, 'dist') } ],
        })
    ],
    module: {
        rules: [
            {
                test: /\.s[ac]ss$/i,
                use: [
                    // Creates `style` nodes from JS strings
                    MiniCssExtractPlugin.loader,
                    // Translates CSS into CommonJS
                    "css-loader",
                    // Compiles Sass to CSS
                    "sass-loader",
                ],
            },
            { test: /\.css$/, use: ['style-loader', 'css-loader'] },
            { test: /\.(jpe?g|png|gif|svg)$/i, use: 'file-loader' },
            { test: /\.(woff2?)$/i, use: 'file-loader' }
        ]
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: "[name].bundle.js",
    }
};
