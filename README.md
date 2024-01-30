# How to create a .NET8 Web API to invoke Google Cloud Gemini service (Vertex AI)

## 1. Prerequisites

**Commands summary**

```
gcloud init
gcloud config set project xxxxxxxxxxxxxxxx
gcloud auth login
gcloud auth list
gcloud auth print-access-token
gcloud ai endpoints list --region=us-central1
```

We first log in to **Google Cloud CLI**

```
gcloud init
```

![image](https://github.com/luiscoco/GoogleCloud_Sample14-API-Gemini-with-Postman/assets/32194879/fd12f35b-1da5-45e4-9f34-c51b359b37d1)

![image](https://github.com/luiscoco/GoogleCloud_Sample14-API-Gemini-with-Postman/assets/32194879/6ecbca48-e210-4ba4-a777-3576cae1dae9)

We select the **Google Cloud Project**

```
gcloud config set project xxxxxxxxxxxxxx
```

We log in with the authorization 

```
gcloud auth login
```

We can list the **authorizations** with the following command

```
gcloud auth list
```

![image](https://github.com/luiscoco/GoogleCloud_Sample14-API-Gemini-with-Postman/assets/32194879/2498f6fb-b906-4993-b451-b301a09fe574)

We print the **Access Token**, and in the following section we teach you how to set the Postman authorization request

```
gcloud auth print-access-token
```

We type the following command to retrieve the **Gemini Endpoint**

```
gcloud ai endpoints list --region=us-central1
```

![image](https://github.com/luiscoco/GoogleCloud_Sample14-API-Gemini-with-Postman/assets/32194879/2b37b266-08f2-418c-a382-27078c20aa8a)

## 2. Gemini API Reference documentation

We recomend you to visit the following web pages to start working with **Gemini**

https://cloud.google.com/vertex-ai/docs/generative-ai/start/quickstarts/quickstart-multimodal

https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini

## 3. Create the .NET8 WebAPI

### 3.2. Run Visual studio and 

### 3.3. Create the project folders structure

### 3.4. Modify the appsettings.json file

### 3.5. Create the Model

### 3.6. Create the Service

### 3.7. Create the Controller

### 3.8. Modify the middleware program.cs file

## 4. Run and test the application

