#!/bin/sh

# Exit script in case of error
set -e

if [ -z "${BACKEND_API}" ]; then
  echo "Must provide BACKEND_API in environment" 1>&2
  exit 1
fi

if [ -z "${REDIRECT_URI}" ]; then
  echo "Must provide REDIRECT_URI in environment" 1>&2
  exit 1
fi

if [ -z "${APP_CLIENT_ID}" ]; then
  echo "Must provide APP_CLIENT_ID in environment" 1>&2
  exit 1
fi

if [ -z "${KUSTO_CLUSTER}" ]; then
  echo "Must provide KUSTO_CLUSTER in environment" 1>&2
  exit 1
fi

if [ -z "${KUSTO_DATABASE}" ]; then
  echo "Must provide KUSTO_DATABASE in environment" 1>&2
  exit 1
fi

echo "Replacing environment variables"
envsubst '\$APP_CLIENT_ID \$APP_TENANT_ID \$REDIRECT_URI \$BACKEND_API \$AGGRID_LICENSE \$KUSTO_CLUSTER \$KUSTO_DATABASE' < /config.js.envsubst > /usr/share/nginx/html/config.js
envsubst '\$BACKEND_API' < /nginx.conf.envsubst > /etc/nginx/nginx.conf

echo "Running command"
# Run the CMD
exec "$@"
