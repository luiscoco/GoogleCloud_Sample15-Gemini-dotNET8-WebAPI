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

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "GoogleGeminiApiUrl": "https://us-central1-aiplatform.googleapis.com/v1/projects/endless-set-412215/locations/us-central1/publishers/google/models/gemini-pro:streamGenerateContent?alt=sse",
  "AccessToken": "ya29.a0AfB_byAJC7kdN7JmSbXTl56kXeuyeilB6AopPCScrW0j2U7n9ruKflUYQSuI-Zo-qvF9D9TFp0-5hcYZSF7vTcPDHG3oQcwGTd0oYTSZodVZYJFh61HaDflFqpcNOhNBMMUuXO674fXvl-URmhIgB7acmTh2a1pbUbIDxk0M9QaCgYKAU8SARESFQHGX2MiVziO9To0tNFpcCWiwqsIgw0177",
  "AllowedHosts": "*"
}
```

### 3.5. Create the Model

```csharp
namespace GoogleGeminiWebAPI
{
    public class GoogleGeminiRequest
    {
        public Contents contents { get; set; }
        public SafetySettings safety_settings { get; set; }
        public GenerationConfig generation_config { get; set; }
    }

    public class Contents
    {
        public string role { get; set; }
        public PartRequest parts { get; set; } // Changed from List<PartRequest> to PartRequest
    }

    public class PartRequest
    {
        // Removed FileData property since it's not present in the JSON format
        public string text { get; set; }
    }

    // The FileData class can be removed if it's not used elsewhere

    public class SafetySettings
    {
        public string category { get; set; }
        public string threshold { get; set; }
    }

    public class GenerationConfig
    {
        public double temperature { get; set; }
        public double topP { get; set; }
        public int topK { get; set; }
        public int maxOutputTokens { get; set; } // This property is missing in the provided JSON
    }
}
```

This is a JSON request sample

```JSON
{
    "contents": {
        "role": "user",
    "parts": {
            "text": "Give me a recipe for banana bread."
    }
    },
  "safety_settings": {
        "category": "HARM_CATEGORY_SEXUALLY_EXPLICIT",
    "threshold": "BLOCK_LOW_AND_ABOVE"
  },
  "generation_config": {
        "temperature": 0.2,
    "topP": 0.8,
    "topK": 40,
    "maxOutputTokens": 1000
  }
}
```

### 3.6. Create the Service

### 3.7. Create the Controller

### 3.8. Modify the middleware program.cs file

## 4. Run and test the application

