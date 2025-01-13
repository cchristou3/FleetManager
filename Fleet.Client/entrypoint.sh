#!/bin/sh
# Replace placeholders with environment variables
sed -i "s|__API_URL__|${API_URL}|g" /usr/share/nginx/html/*.js

# Starts Nginx by running the remaining CMD arguments in the Dockerfile
exec "$@"
