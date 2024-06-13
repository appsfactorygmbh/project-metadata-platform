# Build stage
FROM node:20-alpine AS build

ARG BUILD_ENV=production

WORKDIR /app

RUN npm install -g corepack && corepack enable && yarn set version stable

COPY yarn.lock ./
COPY package.json ./
COPY .yarnrc.yml ./

RUN rm -rf node_modules && yarn install --immutable

COPY . .
RUN yarn build --mode $BUILD_ENV

# Production stage
FROM nginx:stable-alpine AS production

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]