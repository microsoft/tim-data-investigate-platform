ARG BUILD_VERSION=community

FROM node:16-alpine AS dependency

RUN apk add --no-cache python3 make g++

COPY package.json yarn.lock ./
RUN yarn install

COPY . .

FROM dependency as build-community
RUN yarn build

FROM dependency as build-enterprise
RUN yarn add ag-grid-enterprise@29.0.0
RUN sed -i -E "s|/\* LICENSE (.*?) \*/|\1|" src/main.js
RUN yarn build

FROM build-${BUILD_VERSION} AS merge-build

FROM nginx:stable-alpine

COPY --from=merge-build /dist /usr/share/nginx/html

ADD .docker/nginx.conf.envsubst /
ADD .docker/config.js.envsubst /

ADD .docker/docker-entrypoint.sh /
ENTRYPOINT ["/docker-entrypoint.sh"]
RUN chmod +x /docker-entrypoint.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
