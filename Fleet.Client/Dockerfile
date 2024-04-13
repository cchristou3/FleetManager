# Use an official Node runtime as a parent image
FROM node:18.13 as builder

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy package.json and package-lock.json to the working directory
COPY package*.json ./

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Install app dependencies
RUN npm install

# Copy the application files to the container
COPY . .

# Build the Angular app
RUN ng build --configuration production --source-map=true

# Use an official Node runtime as a parent image for the final stage
FROM node:14-alpine

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy the built app from the builder stage to the final stage
COPY --from=builder /usr/src/app/dist/client /usr/src/app

# Expose port 4200 for the Angular development server
EXPOSE 4200

# Start the Angular development server
CMD ["npm", "start"]
