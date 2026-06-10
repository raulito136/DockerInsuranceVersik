# InsuranceApp - Guía de Inicio Rápido en Kubernetes (WSL2 + Kind)

Este proyecto es una aplicación de seguros compuesta por un frontend modularizado con **Angular Microfrontends (Module Federation)** y un backend basado en **APIs REST de ASP.NET Core**, todo desplegado en un clúster local de **Kubernetes (Kind)** y base de datos **SQL Server**.

Esta guía describe cómo iniciar todo el entorno de la manera más rápida posible en Windows utilizando WSL2.

---

## 📋 Requisitos Previos

Asegúrate de tener instalado lo siguiente en tu entorno de **WSL2 (Ubuntu)**:
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (con la integración de WSL2 activa).
* [Kind](https://kind.sigs.k8s.io/) para crear el clúster local.
* [kubectl](https://kubernetes.io/docs/tasks/tools/) para administrar Kubernetes.
* [Helm](https://helm.sh/) para desplegar las aplicaciones.

---

## 🛠️ Configuración e Inicialización

### Paso 1: Configurar Recursos de WSL2 (Recomendado)
Para evitar que los contenedores de Kubernetes fallen por falta de memoria (Out-of-Memory), se recomienda asignar al menos 12 GB de RAM a WSL2. 
Crea o edita el archivo `.wslconfig` en tu carpeta de usuario de Windows (`C:\Users\<TuUsuario>\.wslconfig`):

```ini
[wsl2]
memory=12GB
processors=6
```
*(Reinicia WSL en una terminal de Windows ejecutando `wsl --shutdown` para aplicar los cambios).*

### Paso 2: Crear el Clúster de Kubernetes (Kind)
Crea el clúster de un solo nodo utilizando el archivo de configuración del proyecto:
```bash
kind create cluster --config cluster-config.yml
```

### Paso 3: Crear el Namespace de Kubernetes
Crea el namespace `backend` donde se desplegarán todos nuestros servicios:
```bash
kubectl create namespace backend
```

---

## 📦 Compilación y Despliegue de Aplicaciones

### Paso 4: Compilar las Imágenes de Docker Locales
Compila las imágenes de Docker para cada servicio desde la raíz del proyecto:

```bash
# Compilar las APIs del Backend
docker build -t referencedata-api:latest -f ./Back/ReferenceDataAPI/Dockerfile ./Back/ReferenceDataAPI
docker build -t policies-api:latest -f ./Back/policyAPI/PoliciesService/Dockerfile ./Back/policyAPI/PoliciesService
docker build -t claims-api:latest -f ./Back/ClaimsAPI/Dockerfile ./Back/ClaimsAPI

# Compilar el Frontend Angular
docker build -t frontend:latest -f ./Front/Dockerfile ./Front
```

### Paso 5: Cargar las Imágenes en el Clúster de Kind
Carga las imágenes compiladas en los nodos del clúster para que Kubernetes pueda usarlas localmente:
```bash
kind load docker-image referencedata-api:latest --name insurance-cluster
kind load docker-image policies-api:latest --name insurance-cluster
kind load docker-image claims-api:latest --name insurance-cluster
kind load docker-image frontend:latest --name insurance-cluster
```

### Paso 6: Desplegar con Helm
Despliega los servicios en el namespace `backend` en el orden correcto:

1. **Desplegar la base de datos SQL Server:**
   ```bash
   helm install sqlserver ./helm/sqlserver -n backend
   ```
2. **Esperar a que la Base de Datos esté lista:**
   ```bash
   kubectl wait --namespace backend --for=condition=ready pod -l app=sqlserver --timeout=90s
   ```
3. **Desplegar las APIs y el Frontend:**
   ```bash
   helm install referencedata-api ./helm/referencedata-api -n backend
   helm install policies-api ./helm/policies-api -n backend
   helm install claims-api ./helm/claims-api -n backend
   helm install frontend ./helm/frontend -n backend
   ```
*(Las migraciones de la base de datos se ejecutarán automáticamente en cuanto los pods de las APIs terminen de iniciar).*

---

## 🚀 Lanzamiento y Redirección de Puertos

### Paso 7: Ejecutar la Redirección de Puertos (Port-Forwarding)
Para acceder a la aplicación desde el navegador de Windows evitando interferencias con el Proxy corporativo, debes levantar las conexiones mapeadas a la IP de WSL2. Ejecuta el script de port-forwarding:

```bash
./start-portforward.sh
```

El script detectará automáticamente tu IP de WSL (por ejemplo, `172.22.76.32`) y expondrá todas las URLs de acceso necesarias:
* **Frontend Shell:** `http://<WSL_IP>:4200`
* **APIs de Backend (Swagger):** `http://<WSL_IP>:5001/swagger`, `:5002/swagger`, `:5003/swagger`
* **Base de datos SQL Server:** `<WSL_IP>:1433`

---

## 🧹 Detener Servicios

* **Detener la redirección de puertos:**
  ```bash
  pkill -f 'port-forward -n backend'
  ```
* **Eliminar el clúster de Kind por completo:**
  ```bash
  kind delete cluster --name insurance-cluster
  ```
