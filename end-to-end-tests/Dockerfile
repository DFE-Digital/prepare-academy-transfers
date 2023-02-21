FROM cypress/base:16.18.1

COPY ./cypress ./cypress
COPY cypress.config.js .
COPY package-lock.json package-lock.json
COPY package.json package.json

RUN npm install

ENTRYPOINT ["npm", "run", "cypress:run"]