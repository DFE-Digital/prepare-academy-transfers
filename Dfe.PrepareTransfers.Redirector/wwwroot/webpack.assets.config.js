const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
	plugins: [
		new CopyPlugin({
			patterns: [
				{ from: path.join(__dirname, 'node_modules/govuk-frontend/govuk/assets'), to: path.join(__dirname, 'assets') },
				{ from: path.join(__dirname, 'node_modules/accessible-autocomplete/dist'), to: path.join(__dirname, 'dist') },
				{ from: path.resolve(__dirname, 'node_modules/@ministryofjustice/frontend/moj/assets'), to: path.resolve(__dirname, 'assets') },
			],
		})
	],
	output: {
		path: path.resolve(__dirname, 'dist'),
		filename: 'site.js',
	}
};