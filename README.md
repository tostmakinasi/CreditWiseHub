
# CreditWise Hub 

Development of a Banking RESTful API with .NET

<p align="center">
  <img src="./_ca5cb410-fb83-439d-963f-317523b33512.jpeg" align =center width="400">
</p>



## Technologies Used
 - .Net 7.0
 - C#
 - Postman
 - JWT
 - N-Tier Architecture
 - Repository and Unit of Work Design Patterns
 ### Libraries Used
 - EntityFramework Core
 - Microsoft Identity EntityFramework Core
 - Auto Mapper
 - Fluent Validation
 - Hangfire

## Install
--

This project can be run using .NET 7 and docker-compose. You can install the project by following the steps below:

1. Clone or download the project from GitHub.
   ```bash 
        git clone https://github.com/tostmakinasi/CreditWiseHub.git
    ```
2. Navigate to the project directory in the command line.
    ```bash 
        cd CreditWiseHub
    ```

3. Convert the project to docker images by running the `docker-compose build` command.
    ```bash
        docker-compose build
    ```

4. Start the project as docker containers by running the `docker-compose up -d` command.
    ```bash
        docker-compose up -d
    ```

5. Use the project by navigating to `https://localhost:5000` n your browser. If it doesn't connect, restart the `creditwise-hub` container in Docker.

Note: This project can run on Windows, Linux, or Mac operating systems. However, Docker and docker-compose need to be installed and operational. You can refer to the Docker documentation or docker-compose documentation to learn how to install these tools.

--

[TR]Bu proje, .net 7 ve docker-compose kullanarak çalıştırılabilir. Aşağıdaki adımları izleyerek projeyi yükleyebilirsiniz:

1. Projeyi GitHub'dan klonlayın veya indirin.
   ```bash 
        git clone https://github.com/tostmakinasi/CreditWiseHub.git
    ```
2. Proje dizinine komut satırından girin.
    ```bash 
        cd CreditWiseHub
    ```

3. `docker-compose build` komutunu çalıştırarak projeyi docker imajlarına dönüştürün.
    ```bash
        docker-compose build
    ```

4. `docker-compose up -d` komutunu çalıştırarak projeyi docker containerları olarak başlatın.
    ```bash
        docker-compose up -d
    ```

5. Tarayıcınızda `https://localhost:5000` adresine giderek projeyi kullanın.

Not: Bu proje, Windows, Linux veya Mac işletim sistemlerinde çalışabilir. Ancak, docker ve docker-compose'in kurulu ve çalışır durumda olması gerekir. Bu araçları nasıl kuracağınızı öğrenmek için, docker belgelerine veya docker-compose belgelerine bakabilirsiniz.

## Docker Compose Configuration 
 ```bash
version: '3.4'
name: docker-finalcase-creditwisehub-compose
services:
 postgres:
  container_name: container-pg
  image: postgres
  hostname: localhost
  ports:
  - "5432:5432"
  environment:
   POSTGRES_USER: admin
   POSTGRES_PASSWORD: root
   POSTGRES_DB: "HangfireDB"
  volumes:
  - postgres-data:/var/lib/postgresql/data
  - "./scripts/init.sql:/docker-entrypoint-initdb.d/script.sql"
  restart: unless-stopped

 creditwisehub.api:
  container_name: creditwise-hub
  image: ${DOCKER_REGISTRY-}creditwisehubapi
  build:
    context: .
    dockerfile: CreditWiseHub.API/Dockerfile
  depends_on:
  - postgres

volumes:
  postgres-data:
  
```
### [Click for Postman API documentation](https://documenter.getpostman.com/view/28275528/2s9YsJAXVi) 

<a href="https://documenter.getpostman.com/view/28275528/2s9YsJAXVi"><img src="https://raw.githubusercontent.com/postmanlabs/postmanlabs.github.io/develop/global-artefacts/postman-logo%2Btext-320x132.png" /></a>



### Swagger

![image](https://github.com/tostmakinasi/CreditWiseHub/blob/master/Assets/SwaggerUI-localhost.png)

