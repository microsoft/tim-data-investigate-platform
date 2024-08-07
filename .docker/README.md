# Docker Compose

## Build and run locally

```
docker compose up --build
```

## Environment Variables

These variables can be set in a file named [.env](https://docs.docker.com/compose/environment-variables/set-environment-variables/#substitute-with-an-env-file).

### Common

* AUTH_TENANT_ID
  * Tenant ID for the client application. Used for authenticating tokens for the API.
* AUTH_CLIENT_ID
  * Client ID for the client application. Used for authenticating tokens for the API.
* KUSTO_CLUSTER_URI
  * The Kusto cluster for the tagged events.
* KUSTO_DATABASE_NAME
  * The Kusto database for the tagged events.

### Backend

* SIGNING_KEY
  * Currently not used.
* AUTH_USERNAME
  * Currently not used.
* AUTH_PASSWORD
  * Currently not used.
* AUTH_CLIENT_SECRET
  * Client secret for the client application. Used for authenticating tokens for the API.
* DATABASE_TYPE
  * Can be one of `mongodb`, `couchbase`, `redis`. 
* AZURE_*
  * Credentials used to ingest tagged events into Kusto (refer to [Azure.Identity](https://azuresdkdocs.blob.core.windows.net/$web/dotnet/Azure.Identity/1.0.0/api/index.html#environment-variables)).
* COUCHBASE_CONNECT_STRING
  * Couchbase connection string. 
* COUCHBASE_USER_NAME
* COUCHBASE_USER_PASSWORD
* COUCHBASE_DATABASE_NAME
  * Couchbase bucket name to store all collections. 
* KUSTO_INGEST_URL
  * URI for ingesting tagged events into Kusto. 
* REDIS_CONNECTION_STRING
* MONGO_CONNECTION_STRING
* MONGO_DATABASE_NAME
* MONGO_WITH_COSMOSDB
  * Whether the MongoDB instance is using CosmosDB or not. Set to `true` if using CosmosDB.

### Frontend

* REDIRECT_URI
  * The URI used to redirect back to the frontend after logging in. Typically, the frontend URI.
* API_BASEPATH
  * This *MUST* include a trailing slash.
  * The URI to the API. Typically, the frontend URI.
* BACKEND_URI
  * This must *NOT* include a trailing slash.
  * Where NGINX will proxy requests for `/api` requests to e.g. `https://backend`
* AGGRID_LICENSE
  * If using the `frontend-enterprise` image, this is your AgGrid license. 
