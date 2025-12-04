# Documentación del proyecto "Sistema de Gestión de talento Humano (Work Force Nexxus)"
![net](https://img.shields.io/badge/dotnet-purple?logo=dotnet&label=.NET%208.0) ![Vue](https://img.shields.io/npm/v/vue?label=Vue&color=green)
![TypeScript](https://img.shields.io/npm/v/typescript?style=flat&label=TypeScript) ![Tailwind](https://img.shields.io/npm/v/tailwindcss?style=flat&label=Tailwind%20CSS&color=06B6D4) ![Vite](https://img.shields.io/npm/v/vite?style=flat&label=Vite&color=646CFF) ![Axios](https://img.shields.io/npm/v/axios?label=Axios)
![Pinia](https://img.shields.io/npm/v/pinia?logo=vuedotjs&logoColor=%234FC08D&label=Pinia&color=%234FC08D)





### 🌀 Proyecto Work Force Nexxus 🌀

> WorkForce Nexxus es un sistema moderno de gestión de Talento Humano, diseñado para automatizar procesos críticos como: 
> - Registro y administración de empleados 
> - Procesamiento de nómina basado en reglas parametrizables
> - Ingresos, egresos y novedades mensuales 
> - Generación de reportes estratégicos.

> Este proyecto es una aplicación web FullStack desarrollada con .NET en el backend y VueJs con TypeScript en el frontend. El objetivo principal de este proyecto es gestionar y administrar los recursos humanos de una empresa de manera eficiente y efectiva.
> Como ultima entrega de la tarea es demostrar el sistema y de como esta compuesto la arquitectura, y sobre como va funcionar la escalabilidad del sistema
> se aplico los patrones de diseño tanto Facade que es para el llamado de las APIs y el patron Observer que es para la gestion de estados globales con Pinia.

> El backend fue construido con .NET 8 Web API, siguiendo principios de diseño limpio, arquitectura desacoplada y despliegue 100% serverless en AWS.
El objetivo: un sistema ágil, seguro y escalable que respalde a departamentos de RR.HH. que ya no pueden sobrevivir con hojas de Excel, correos perdidos y “luego lo reviso” administrativos.

## 🚀 Arquitectura General del Backend

> El backend se construye sobre pilares sólidos y servicios totalmente administrados de AWS, diseñados para soportar una carga creciente sin sacrificar rendimiento. 
> Tecnologías principales:
> - .NET 8 Web API (C#) – núcleo del backend y lógica empresarial.
> - Amazon DynamoDB – base de datos NoSQL para empleados, nómina, contratos y reglas 
> - AWS Lambda – hosting serverless del API compilado 
> - API Gateway – puerta de entrada para exponer endpoints REST 
> - GitHub Actions (CI/CD) – despliegue automático hacia Lambda 
> - AWS IAM – control estricto de roles y seguridad 
> - Vue 3 + TypeScript (Frontend) – consume el backend 
> - Supabase – autenticación y autorización

> Un sistema serverless, rápido y eficiente… casi tan rápido como procesos de RR.HH. cuando ya es fin de mes.

## Lista de elementos aplicados y aprendidos en este proyecto

| #  | Tema                   | Descripción                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Complejidad |
|----|------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------|
| 00 | **Patron Repositorio** | Este es uno de mis patrones favoritos al implementar mi backend en .NET porque gracias a ello me facilita la separación de responsabilidades, debido a la implementación de la interfaz. Con uno con varias interfaces es posible crear varios repositorio basados en las operaciones CRUD, y con ello facilita implementar el controlador.                                                                                                                                                                                            |![Static Badge](https://img.shields.io/badge/100-green?style=flat&label=Baja)
| 01 | **WebAPI**             | Una de las caracteristicas mas importantes de .NET es el WebApi donde como uusario es facil implementar APIRest tales como operaciones CRUD, autenticacion, JWT, Autorizacion, GraphQL entre otros. Es decir posee una versatilidad al momento de implementar el Backend.                                                                                                                                                                                                                                                              |![Static Badge](https://img.shields.io/badge/100-green?style=flat&label=Baja)
| 02 | **MVC**                | **MVC** es considerado el patron mas antiguo lo cual en su tiempo era el mas efectivo al momento de desarrollar una aplicación. Este patron cosiste en la separacion de resposabilidades, el modelo, la vista y el controlador. Existen varias maneras de implementar el patron MVC, en mi caso aplique el MC para el backend y el V para la vista que es el FrontEnd y lo unifique en un solo proyecto. Este patron sirve mucho para nivel educativo pero para proyectos mas grandes se hae el uso de patrones modernos como el MVVM. |![Static Badge](https://img.shields.io/badge/90-green?style=flat&label=Baja)
| 03 | **AWS Lambda**         | Al haber aprendido este servicio y sus funciones, realice la implementación y empaquetación del API usando uno de los servicios mas demandados de AWS, antes de empaquetar mi API, realice ciertas modificaciones desde .NET para poder enviar hacia la nube. Tuve exito al migrar hacia la nube de AWS ya que me resulto mas facil ejecutar desde mi FrontEnd.                                                                                                                                                                        |![Static Badge](https://img.shields.io/badge/60-yellow?style=flat&label=Medio)
| 04 | **AWS DynamoDB**       | Al implementar en Lambda mi backend yo use una base de datos diferente en este caso DynamoDB ya que es parte de AWS y me ayudaba de la forma mas facil a almacenar datos y no desde terceros como MongoDB. Ahora mi Backend esta con el sistema completo ligado a los servicios de AWS lo cual me da ciertas ventajas a la hora de consumir mi APIRest                                                                                                                                                                                 |![Static Badge](https://img.shields.io/badge/100-green?style=flat&label=Baja)

## Tecnologías usadas

![Static Badge](https://img.shields.io/badge/.NET%208.0-%23512BD4?style=for-the-badge&logo=dotnet&label=TOOL&labelColor=black) ![Static Badge](https://img.shields.io/badge/CSharp-%23512BD4?style=for-the-badge&logo=dotnet&label=LANGUAGE&labelColor=black)![Vue](https://img.shields.io/badge/VueJs--TS-%234FC08D?style=for-the-badge&logo=vue.js&label=FRAMEWORK&labelColor=black)
![TypeScript](https://img.shields.io/badge/typescript-blue?style=for-the-badge&logo=typescript&label=language&labelColor=black) ![Tailwind](https://img.shields.io/badge/tailwind--css-06B6D4?style=for-the-badge&logo=tailwindcss&label=framework&labelColor=black) ![Vite](https://img.shields.io/badge/vite-646CFF?style=for-the-badge&logo=Vite&label=Tool&labelColor=black)



## Recursos donde se aprendio la implementación
- [Vue Mastery](https://www.vuemastery.com/)
- [C#: Empieza tu camino en el lenguaje (DevTalles)](https://cursos.devtalles.com/courses/csharp?coupon=learn-01)
- [Udemy TypeScript: Tu completa guía y manual de mano. ](https://www.udemy.com/course/typescript-guia-completa/)
- [Udemy Vue.js: De cero a experto](https://www.udemy.com/course/vuejs-fh/)
- [Udemy Vue.js - Intermedio: Lleva tus bases al siguiente nivel)](https://www.udemy.com/course/vue-intermedio/)
- [Udemy Vue.js 3 - La Guía Completa](https://www.udemy.com/course/vuejs-la-guia-completa-composition-pinia-mevn-creando-proyectos-reales)
- [Curso NetMentor](https://www.youtube.com/@NetMentor)
- [.NET web API Zero to Hero (AWS)](https://www.youtube.com/@codewithmukesh)


## Repositorios en GitHub
> - [Proyecto Work Force Nexxus](https://github.com/AngelDavidStudios/WFN-Backend)


## Link de la Aplicación Web en Netlify
> - [workforcenexxus.netlify.app](https://workforcenexxus.netlify.app/)


## Video explicacion del proyecto.
> - [Explicacion del proyecto Administracion MVC. ](https://youtu.be/c55FISqVAnw)

## Instrucciones

**Si desea revisar el proyecto, puede clonar con git clone o descargar Zip.**

**Para usuarios de Visual Studio o Rider**

1. Una vez clonado o descargado el proyecto, solamente debera abrir la solución con el IDE de su preferencia, en este caso Visual Studio o Rider.

**Para usuarios de Visual Studio Code**

1. Una vez clonado o descargado el proyecto, debera abrir la carpeta del proyecto en Visual Studio Code. Para la ejecucion debe tener instalado el SDK de .NET 8.0 y el plugin de C#.

**Nota adicional de AWS**

Para poder ejecutar el proyecto desde su IDE, debera tener instalado el plugin de AWS Toolkit para su IDE, y
tener configurado su cuenta de AWS. Para la base de datos DynamoDB, debera crear una tabla con el nombre "RulerCore", "VendedorCore", "VentaCore" y
los atributos "Id". Para la API Gateway, debera crear un endpoint con el nombre "Comision.API" y asociarlo a la Lambda creada.

---

## <img src="https://github.com/AngelDavidStudios/calculadora-propinas/blob/main/src/resources/ads-emote.JPG" width="55" height="55"> Hola, mi nombre es David Angel.
### Multimedia Desginer & Software Architect

Soy diseñador Multimedia cursando una segunda carrera en Ingeniería de Software, me estoy especializando en el desarrollo Backend como arquitecto de software, en este recorrido dia tras dia aprendo tecnologias nuevas.

David Angel Studios es mi marca personal donde mi misión es desarrollar diversos proyectos basado en mi apredizaje de nuevas tecnologias y almacenarlas en mi portafolio personal:

[![Linkedin](https://img.shields.io/badge/Linkedin-4479A1?style=for-the-badge&logo=9gag&label=Angel%20David%20Studios&labelColor=black)](https://www.linkedin.com/in/angeldavidstudios/)
[![Instagram](https://img.shields.io/badge/Instagram-FF0069?style=for-the-badge&logo=instagram&label=Angel%20David%20Studios&labelColor=black)](https://www.instagram.com/angeldavidstudios/) [![Youtube](https://img.shields.io/badge/Angel--David--Studios-FF0000?style=for-the-badge&logo=youtube&label=Youtube.com%2F&labelColor=black)](https://www.youtube.com/channel/UC2VYRq169QluoLeagCYrjVg)
