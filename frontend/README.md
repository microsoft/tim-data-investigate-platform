# Frontend

## Project setup
```
yarn install
```

### Compiles and hot-reloads for development
```
yarn dev --mode development --port 3000
```

### Compiles and minifies for production
```
yarn build
```

### Lints and fixes files 
```
yarn lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).


## Docker

### Build enterprise edition
```
docker build --build-arg BUILD_VERSION=enterprise -t frontend-enterprise .
```

### Run enterprise edition
Copy the `config.js.example` to a new file e.g. `config.js` and enter your environment settings.
```
docker run -p 3000:80 -v c:\<yourdir>\config.js:/config.js:ro frontend-enterprise
```
