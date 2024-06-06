# Build stage
FROM node:20-alpine AS build

RUN npm install -g corepack && corepack enable && yarn set version stable

WORKDIR /app

COPY package*.json ./
RUN rm -rf node_modules && yarn install --immutable

COPY . .
RUN yarn build

# Production stage
FROM nginx:stable-alpine AS production

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]