# How to create a .NET8 Web API to invoke Google Cloud Gemini service (Vertex AI)

See the source code://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI

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

### 3.2. Run Visual studio and create a new WebAPI

We create a new project

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/390add0a-5c00-480e-b688-3da2b10c535a)

We select the project template

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/0f86eeb5-d6d8-42fb-ba61-2d1224f029d8)

We input the project name and location

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/d8598499-7eed-4e5b-9230-b63c063142af)

We select the project main features

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/7b9e485a-34e8-4bb7-b214-e70cb86669ca)

### 3.3. Create the project folders structure

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/737d05b9-89c0-4b74-a0bb-a95cfe3ee1da)

### 3.4. Modify the appsettings.json file

We print the **Access Token** to include it in the appsettings.json file

```
gcloud auth print-access-token
```

We also copy in the appsettings.json the endpoint taken from this URL: https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini

```
https://us-central1-aiplatform.googleapis.com/
v1/projects/PROJECT_ID/locations/us-central1/
publishers/google/models/gemini-pro:streamGenerateContent?alt=sse
```

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/024d5560-461b-4804-9a49-512a528ef259)

This is the final code including the **access token** and the **Google Cloud Gemini endpoint**

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

We create the request model, we are going to send to Google Cloud Gemini service

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

### 3.6. Create the Service

We create the service for invoking (via HTTP) the Google Cloud Gemini service

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

// Add services to the container

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<GoogleGeminiService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

We build and run the app in Visual Studio 2022

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/0428e301-e349-435b-bd52-2445726219e7)

We send a **POST** request

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/f3a286b1-a51f-4308-820e-bbfb16d3025a)

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

This is the **response** we get

![image](https://github.com/luiscoco/GoogleCloud_Sample15-Gemini-dotNET8-WebAPI/assets/32194879/d8fee700-e621-4bcd-807a-e8af6ed1ed4c)

See also the **detailed response** content

```
data: {"candidates": [{"content": {"role": "model","parts": [{"text": "Ingredients:\n\n- 1 1/2 cups all-purpose flour\n- 1 teaspoon baking soda\n- 1/2 teaspoon salt\n- 1/2 cup (1 stick) unsalted butter, softened\n- 3/4 cup granulated sugar\n- 3/4 cup packed light brown"}]},"safetyRatings": [{"category": "HARM_CATEGORY_HARASSMENT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_HATE_SPEECH","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_DANGEROUS_CONTENT","probability": "NEGLIGIBLE"}]}]}

data: {"candidates": [{"content": {"role": "model","parts": [{"text": " sugar\n- 2 large eggs\n- 1 teaspoon vanilla extract\n- 3 ripe bananas, mashed\n- 1 cup chopped walnuts (optional)\n\nInstructions:\n\n1. Preheat oven to 350 degrees F (175 degrees C). Grease a 9x5 inch loaf pan."}]},"safetyRatings": [{"category": "HARM_CATEGORY_HARASSMENT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_HATE_SPEECH","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_DANGEROUS_CONTENT","probability": "NEGLIGIBLE"}]}]}

data: {"candidates": [{"content": {"role": "model","parts": [{"text": "\n2. In a medium bowl, whisk together the flour, baking soda, and salt.\n3. In a large bowl, cream together the butter, granulated sugar, and brown sugar until light and fluffy. Beat in the eggs one at a time, then stir in the vanilla.\n4. Add the mashed bananas to the butter mixture and stir until just combined. Gradually add the flour mixture, stirring until just combined. Fold in the walnuts, if desired.\n5. Pour the batter into the prepared loaf pan and bake for 55-65 minutes, or until a toothpick inserted into the center comes out clean."}]},"safetyRatings": [{"category": "HARM_CATEGORY_HARASSMENT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_HATE_SPEECH","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_DANGEROUS_CONTENT","probability": "NEGLIGIBLE"}],"citationMetadata": {"citations": [{"startIndex": 396,"endIndex": 569,"uri": "https://foodrecipeexpert80.blogspot.com/2023/02/chocolate-chip-cookies-classic-american.html"},{"startIndex": 474,"endIndex": 598,"uri": "https://www.thespruceeats.com/apple-cinnamon-bread-5078662"}]}}]}

data: {"candidates": [{"content": {"role": "model","parts": [{"text": "\n6. Let the bread cool in the pan for 10 minutes before transferring to a wire rack to cool completely."}]},"finishReason": "STOP","safetyRatings": [{"category": "HARM_CATEGORY_HARASSMENT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_HATE_SPEECH","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT","probability": "NEGLIGIBLE"},{"category": "HARM_CATEGORY_DANGEROUS_CONTENT","probability": "NEGLIGIBLE"}],"citationMetadata": {"citations": [{"startIndex": 868,"endIndex": 991,"uri": "https://www.dish-it-up.com/banana-bread-recipe/"}]}}],"usageMetadata": {"promptTokenCount": 8,"candidatesTokenCount": 281,"totalTokenCount": 289}}
```

