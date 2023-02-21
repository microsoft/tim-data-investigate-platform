#!/bin/sh

# Exit script in case of error
set -e

if [ -z "${BACKEND_URI}" ]; then
  echo "Must provide BACKEND_URI in environment" 1>&2
  exit 1
fi

if [ -z "${REDIRECT_URI}" ]; then
  echo "Must provide REDIRECT_URI in environment" 1>&2
  exit 1
fi

if [ -z "${AUTH_CLIENT_ID}" ]; then
  echo "Must provide AUTH_CLIENT_ID in environment" 1>&2
  exit 1
fi

if [ -z "${AUTH_TENANT_ID}" ]; then
  echo "Must provide AUTH_TENANT_ID in environment" 1>&2
  exit 1
fi

if [ -z "${KUSTO_CLUSTER_URI}" ]; then
  echo "Must provide KUSTO_CLUSTER_URI in environment" 1>&2
  exit 1
fi

if [ -z "${KUSTO_DATABASE_NAME}" ]; then
  echo "Must provide KUSTO_DATABASE_NAME in environment" 1>&2
  exit 1
fi

echo "Replacing environment variables"
envsubst '\$AUTH_CLIENT_ID \$AUTH_TENANT_ID \$REDIRECT_URI \$BACKEND_URI \$API_BASEPATH \$AGGRID_LICENSE \$KUSTO_CLUSTER_URI \$KUSTO_DATABASE_NAME' < /config.js.envsubst > /usr/share/nginx/html/config.js
envsubst '\$BACKEND_URI' < /nginx.conf.envsubst > /etc/nginx/nginx.conf

echo "Running command"
# Run the CMD
exec "$@"
