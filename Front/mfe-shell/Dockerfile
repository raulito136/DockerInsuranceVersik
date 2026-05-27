FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci --legacy-peer-deps
COPY . .
RUN npx nx build mfe-shell --configuration=production

FROM nginx:alpine
COPY mfe-shell/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/mfe-shell /usr/share/nginx/html
EXPOSE 80
