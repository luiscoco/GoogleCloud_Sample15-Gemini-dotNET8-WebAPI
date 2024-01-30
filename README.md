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

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/737d05b9-89c0-4b74-a0bb-a95cfe3ee1da)

### 3.4. Modify the appsettings.json file

We print the Access Token, and in the following section we teach you how to set the Postman authorization request

```
gcloud auth print-access-token
```

We also copy the endpoint from this URL: https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini

```
https://us-central1-aiplatform.googleapis.com/
v1/projects/PROJECT_ID/locations/us-central1/
publishers/google/models/gemini-pro:streamGenerateContent?alt=sse
```

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/024d5560-461b-4804-9a49-512a528ef259)

This is the final code including the access token and the Google Cloud Gemini endpoint

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

**GoogleGeminiRequest.cs**

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

This is a **JSON request** sample

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

**GoogleGeminiService.cs**

```csharp
// GoogleGeminiService.cs
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GoogleGeminiWebAPI
{
    public class GoogleGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleGeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetResponseStringAsync(GoogleGeminiRequest request)
        {
            var chatGptApiUrl = _configuration["GoogleGeminiApiUrl"];
            var apiKey = _configuration["AccessToken"];

            var apiUrl = chatGptApiUrl;

            var requestContent = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.PostAsync(apiUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"HTTP request failed with status code {response.StatusCode}. " +
                    $"Response content: {await response.Content.ReadAsStringAsync()}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
```

### 3.7. Create the Controller

**GoogleGeminiController.cs**

```csharp
// GoogleGeminiController.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleGeminiWebAPI
{
    [ApiController]
    [Route("[controller]")]
    public class GoogleGeminiController : ControllerBase
    {
        private readonly GoogleGeminiService _googleGeminiService;

        public GoogleGeminiController(GoogleGeminiService googleGeminiService)
        {
            _googleGeminiService = googleGeminiService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GoogleGeminiRequest request)
        {
            try
            {
                var responseString = await _googleGeminiService.GetResponseStringAsync(request);
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "An error occurred while processing the response.");
            }
        }
    }
}
```

### 3.8. Modify the middleware program.cs file

**program.cs**

```csharp
using GoogleGeminiWebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<GoogleGeminiService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 4. Run and test the application

