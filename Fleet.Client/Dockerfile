# Use the official Node.js image as the base image
FROM node:18.13 as build

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json to the container
COPY package*.json ./

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Install project dependencies
RUN npm install

# Copy the entire project to the container
COPY . .

# Build the Angular app for production
RUN ng build --configuration production

# Use a smaller, production-ready image as the final image
FROM nginx:alpine

# Copy the custom nginx configuration file to the container in the default
# location
COPY nginx.conf /etc/nginx/nginx.conf

# Copy the production-ready Angular app to the Nginx webserver's root directory
COPY --from=build /app/dist/client /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
