FROM node:20-alpine

WORKDIR /app

# Instala dependencias de la app
COPY package*.json ./
# Use legacy peer deps to avoid ERESOLVE failures during CI in the container
# --no-audit and --no-fund reduce build noise
RUN npm ci --legacy-peer-deps --no-audit --no-fund

# Copia el frontend
COPY . ./

EXPOSE 4200 4201 4202 4203

ENV HOST=0.0.0.0
CMD ["npm", "run", "start"]
