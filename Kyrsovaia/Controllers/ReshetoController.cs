using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using Prog.Services;

namespace Prog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReshetoController : ControllerBase
    {
        private readonly ILogger<ReshetoController> _logger;

        public ReshetoController(ILogger<ReshetoController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("sieve")]
        public async Task<IActionResult> SieveParamsJson()
        {
            try
            {
                var request = await HttpContext.Request.ReadFromJsonAsync<Border?>();

                if (request == null || request.border <= 0 || request.border > 1000000)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid border value. Please provide a positive integer between 1 and 1,000,000."
                    });
                }

                int parsedBorder = request.border;

                SieveOfEratosthenes sieve = new SieveOfEratosthenes(parsedBorder);
                var primes = sieve.GetPrimesAsString();

                return Ok(new { Primes = primes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sieve request.");
                return Problem("An error occurred while processing the request.");
            }
        }
    }


    public class Border
    {
        public int border { get; set; }
    }
}