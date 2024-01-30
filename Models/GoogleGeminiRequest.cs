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


//This is a correct request JSON sample

//{
//    "contents": {
//        "role": "user",
//    "parts": {
//            "text": "Give me a recipe for banana bread."
//    }
//    },
//  "safety_settings": {
//        "category": "HARM_CATEGORY_SEXUALLY_EXPLICIT",
//    "threshold": "BLOCK_LOW_AND_ABOVE"
//  },
//  "generation_config": {
//        "temperature": 0.2,
//    "topP": 0.8,
//    "topK": 40,
//    "maxOutputTokens": 1000
//  }
//}
