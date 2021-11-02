const webpack = require('webpack');
var MiniCssExtractPlugin = require('mini-css-extract-plugin');
var path = require('path');
const commitHash = require('child_process').execSync('git rev-parse HEAD').toString()

module.exports = {
    mode: 'production',
    entry: ['./src/js/site.js', './src/css/site.scss'],
    plugins: [
        new MiniCssExtractPlugin({filename: 'site.css'}),
        new webpack.DefinePlugin({
        'process.env.COMMIT_HASH': JSON.stringify(commitHash),
        })],
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
            {test: /\.css$/, use: ['style-loader', 'css-loader']},
            {test: /\.(jpe?g|png|gif|svg)$/i, use: 'file-loader'},
            {test: /\.(woff2?)$/i, use: 'file-loader'}
        ]
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'index.bundle.js',
    }
};
