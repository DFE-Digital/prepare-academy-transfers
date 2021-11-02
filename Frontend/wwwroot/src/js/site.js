// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

import * as Sentry from "@sentry/browser";
import { Integrations } from "@sentry/tracing";

import { initAll } from 'govuk-frontend'
initAll()

console.log("version:", process.env.COMMIT_HASH);

Sentry.init({
    dsn: "https://4bbb8d5429b54d38a993483492a6aad0@o1042804.ingest.sentry.io/6034139",
    integrations: [new Integrations.BrowserTracing()],
    release: "academy-transfers-api:" + process.env.COMMIT_HASH,
    // Set tracesSampleRate to 1.0 to capture 100%
    // of transactions for performance monitoring.
    // We recommend adjusting this value in production
    tracesSampleRate: 1.0,
});

myUndefinedFunction();