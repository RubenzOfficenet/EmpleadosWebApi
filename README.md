Docker file

Este proyecto contiene ya el archivo Dockerfile

Caomandos

Crear image:
  docker build -t empleadoswebapi-web:1.0.0 -f .\empleadoswebapi\Dockerfile .

Crear el contenedor
  docker run -d -p 5004:8080 --name empleadoswebapi_container [ImageID]

Ejecutar:
  http://localhost:5004/api/Empleados
  
