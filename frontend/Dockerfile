FROM node:16-alpine AS builder

RUN apk add --no-cache python3 make g++

COPY package.json yarn.lock ./
RUN yarn install

COPY . .
RUN yarn build

FROM node:16-alpine
EXPOSE 80
RUN yarn global add serve

WORKDIR /app
COPY --from=builder /dist .

ENTRYPOINT ["serve", "-l", "80"]