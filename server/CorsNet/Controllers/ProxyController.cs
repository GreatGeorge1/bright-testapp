using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CorsNet.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient client;

        public ProxyController()
        {
            client = new HttpClient();
        }

        [HttpGet]
        public IActionResult Help()
        {
            return Ok("proxy page");
        }

        
        [HttpGet("{*url}")]
        public async Task<IActionResult> GetUrlData([FromRoute] string url)
        {
            try
            {
                var baseUri = url;
                if (baseUri.StartsWith("http:/"))
                    baseUri = url.Replace("http:/", "http://");

                if (baseUri.StartsWith("https:/"))
                    baseUri = url.Replace("https:/", "https://");

                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                var response = await client.GetAsync(baseUri);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
                return BadRequest("Not succesfull");
            }
            catch (HttpRequestException e)
            {
                return BadRequest("Exception :" + e.Message);
            }
        }
    }
}