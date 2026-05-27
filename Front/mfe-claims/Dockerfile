FROM node:20-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci --legacy-peer-deps
COPY . .
RUN npx nx build mfe-claims --configuration=production

FROM nginx:alpine
COPY mfe-claims/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/mfe-claims /usr/share/nginx/html
EXPOSE 80
