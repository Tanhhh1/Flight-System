FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json .
RUN npm install

COPY . .

ARG VITE_API_URL=http://localhost/api/v1
ARG VITE_HUB_URL=http://localhost
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_HUB_URL=$VITE_HUB_URL

RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80