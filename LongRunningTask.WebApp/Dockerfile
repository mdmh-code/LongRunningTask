FROM node:20-alpine AS build

WORKDIR /app
COPY LongRunningTask.WebApp/package*.json ./
RUN npm install
COPY LongRunningTask.WebApp/ .
RUN npm run build
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY LongRunningTask.WebApp/nginx.conf /etc/nginx/nginx.conf
EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]
