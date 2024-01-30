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
