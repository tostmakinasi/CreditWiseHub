# FinalCase

<p align="center">
  <img src="./_ca5cb410-fb83-439d-963f-317523b33512.jpeg" align =center width="400">

</p>

# [Click for Postman API documentation](https://documenter.getpostman.com/view/28275528/2s9YsJAXVi) 


# CreditWise Hub 

Development of a Banking RESTful API with .NET

Introduction:
In today's world, banking services are increasingly becoming digitized, and customer expectations are constantly evolving. This transformation necessitates the equipping of banking systems with secure, efficient, and user-friendly web services. This assignment aims to enhance students' skills in developing RESTful APIs on the .NET platform.
Students will gain in-depth knowledge and experience in modern web technologies and application security by creating an API that simulates real-world banking functions.


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


## Yükleme

Bu proje, .net 7 ve docker-compose kullanarak çalıştırılabilir. Aşağıdaki adımları izleyerek projeyi yükleyebilirsiniz:

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

4. `docker-compose up` komutunu çalıştırarak projeyi docker containerları olarak başlatın.
    ```bash
        docker-compose up
    ```

5. Tarayıcınızda `http://localhost:5000` adresine giderek projeyi kullanın.

Not: Bu proje, Windows, Linux veya Mac işletim sistemlerinde çalışabilir. Ancak, docker ve docker-compose'in kurulu ve çalışır durumda olması gerekir. Bu araçları nasıl kuracağınızı öğrenmek için, docker belgelerine veya docker-compose belgelerine bakabilirsiniz.
