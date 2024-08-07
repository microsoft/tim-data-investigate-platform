# Getting Started

This guide will walk you through a sample setup step-by-step so you understand how the components interact with each other. This is a simple application and should take about 10 minutes to deploy the recommended starter setup using this guide. This guide assumes limited prior experience deploying Azure resources or applications. If you are more experienced, you're free to use this simply as a reference for common questions.

## Environment Variables Reference

For your reference, here are the sources of the secrets TIM uses. You can create these resources in advance if you'd like or define them in your containers after spinning them up:

### Your Azure Tenant

- `AUTH_TENANT_ID`: Your tenant ID
- `AZURE_TENANT_ID`: Your tenant ID

### Enterprise Application/SP

- `AUTH_CLIENT_ID`: Your Enterprise Application's Client ID
- `AZURE_CLIENT_ID`: Your EA's Client ID
- `AZURE_CLIENT_SECRET`: Your EA's secret (you can generate this for your EA and store it in its key vault)

### ADX Cluster

- `KUSTO_CLUSTER_URI`: Your cluster's full URL
- `KUSTO_DATABASE_NAME`: Your cluster's TIM DB name
- `KUSTO_INGEST_URI`: Your cluster's ingest URL, starts with "ingest"

### Database

- `DATABASE_TYPE`: Which database you're using, in our case, "mongodb"
- `MONGO_WITH_COSMOSDB`: "true" in our case
- `MONGO_CONNECTION_STRING`: Your connection string
- `MONGO_DATABASE_NAME`: The name of your Cosmos MongoDB

### Application Registration

Note: These values all use the domain of your frontend and backend containers. Your Container Apps URL = `https://<name of container>.<randomstring>-<buildstring>.<region>.azurecontainerapps.io`

- `REDIRECT_URI`: This is set in your application redirection URI for your "Single Page Web Application" - your frontend container URL
- `API_BASEPATH`: This is the TIM API basepath - the frontend container URL
- `BACKEND_URI`: This is the URL of the backend container so the frontend can direct API requests accordingly

### Community vs. Enterprise

- `AGGRID_LICENSE`: Your Enterprise license would go here if you're using tha Enteprise version of the library

### Deprecated Values

These are no longer used anymore - some still require values due to legacy code which is still being fixed:

- `SIGNING_KEY`: Please set this to 16-character alpha string e.g. "AAAAAAAA..."
- `AUTH_USERNAME`
- `AUTH_PASSWORD`
- `AUTH_CLIENT_SECRET`

## Step 1 - Initial Infrastructure

As of now, we only supply Azure deployments for the infrastructure, so all steps here will be setting up Azure resources.

To begin, we recommend the creation of an Azure resource group to help track your resources - we'll assume you "tim-dev-testdeploy" in this guide. You can use this resource group to organize all of TIM's resources.

We'll need our databases stood up for now - we use a Cosmos Mongo DB ("tim-dev-mongodb") and an Azure Data Explorer cluster ("tim-dev-adx-cluster"). For Mongo, please make sure under "Features" you have 16MB documents enabled.

![Mongo Feature 16MB](/docs/images/tim_mongodb_features.png)

Save this connection string for later - this will go into our environment vars above:

![Mongo Conn String](/docs/images/tim_mongodb_connstring.png)


For Azure Data Explorer cluster, please create a database ("tim-dev-adx-db" or your preference). You can set data retention at your preferred value. Your tags and comments will be stored here. Provision the appropriate access to the cluster to your target user account so they will be able to query the database later.

For those of you not using pre-built TIM images for the frontend and backend, you will want to set up an Azure Container Repository you can push locally built images to and deploy to your Container App you'll make later. We'll assume your ACR is called "tim-dev-acr".

## Building and Pushing Docker Images

You can use the following commands to build your images using Docker:

```sh
docker build ./frontend/ -t tim-dev-acr.azurecr.io/tim-dev-frontend-image
docker build ./backend/ -t tim-dev-acr.azurecr.io/tim-dev-backend-image
```

For simplicity, you can install Azure CLI and use the following commands to push your container images:
```sh
az login --tenant YOUR_AZURE_TENANT_ID
az acr login --name tim-dev-acr
docker push tim-dev-acr.azurecr.io/tim-dev-frontend-image
docker push tim-dev-acr.azurecr.io/tim-dev-backend-image
```

When you deploy your Container App, you will select this Azure Container Registry as the source of your new images and set your environment variables accordingly.


## Step 2 - Enterprise Application

Now we'll create our Enterprise Application and App Registration. The app will be called "tim-dev-testapp". The application in Azure handles our authentication via Entra and our delegated access to the user's ADX clusters.

Set up your Enterprise Application and App Registration. Your Enterprise Application "Application ID" is what we refer to as your `AZURE_CLIENT_ID`. We recommend populating your environment variables as you go through deployment so you're ready to immediately deploy at the end.

On the App Registration's "Authentication" page, set up a profile for a Single-Page application. We'll populate the redirect URI at the end once the containers are ready. While you're here, generate your preferred client secret (for simplicity, we'll just use a secret token string). This will be your `AZURE_CLIENT_SECRET`, which if you're using Azure Container Apps, you can reference the key in a vault rather than populate the actual secret.

### Expose An API

Select `api://<AZURE_CLIENT_ID>`

Set your API config to: `api://<AZURE_CLIENT_ID>/user_impersonation` with your preferred consent settings (Admin and users is fine for isolated test instances).

Here's an example of what this might look like:
![expose api](/docs/images/tim_appreg_exposeapi.png) 

### API Permissions

TIM uses Delegated `AzureDataExplorer.user_impersonation` and `MicrosoftGraph.User.Read` permissions. Set your preferred Admin consent requirement. Make sure the appropriate tenant administrator grants admin consent for this delegated access for your tenant.

 ![API consent](/docs/images/tim_appreg_APIConsent.png)

## Step 3 - Container Deployment

We'll be creating two Azure Container Apps - "tim-dev-frontend" and "tim-dev-backend". We recommend if you're unfamiliar with building and deploying containers and simply are looking to try TIM out, you can deploy prebuilt containers via the public container repo, or deploy your own built containers via a private Azure Container Repo.

You can use this as an example for your ingress for testing:
![ingress settings](/docs/images/tim_frontend_ingress.png)

You can manage your environment variable definitions as part of your Azure Container configs - this makes it easy to adjust envvars for troubleshooting and quickly redeploy. Set the environment variables you currently have for each of the frontend and backend containers using the included reference above. Take note of the Application URL for both the frontend and backend - we will use these later.

Here's an example of what setting environment var configuration in Container Apps for TIM might look like:
![container env vars](/docs/images/tim_frontend_envvars.png)

## Step 4 - Finishing Up

We're going to populate some remaining fields using our new frontend and backend URLs provided by Azure Container App.

### Azure Resource Configurations

App Registration > Authentication > Single-Page App - Redirect: `<FRONTEND_APP_URL>`

### Environment Vars

#### Frontend

- `REDIRECT_URI`: `https://<FRONTEND_APP_URL>/`
- `API_BASEPATH`: `https://<FRONTEND_APP_URL>/`
- `BACKEND_URI`: `https://<FRONTEND_APP_URL>` [note: no trailing slash]

### Container App Configs

Configure your preferred Ingress settings for each app - you'll need to reach the frontend via your browser to access the app, but the backend can be isolated to the container network. You can place your frontend behind your preferred proxy. TIM does require successful authentication and app consent in order to function. Either way, secure your deployment in your preferred way.

### Container Env Vars

Go populate the remaining frontend envvars noted above and redeploy the frontend container with this new config.

## Step 5 - All Done

You should be able to access and authenticate via a pop-up (pop-ups may be blocked) using Azure Entra. Assuming your delegated consent rights are approved for the tenant and the tenant user account you are using for the app has the necessary application and ADX database access, you can now issue your Kusto query.

For simplicity, we included a KC7 query pack which you can import and immediately test the querying, navigation, and tagging components to ensure they are all working.