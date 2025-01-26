using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog.Model;
using Prog.Services;
using System.Threading.Tasks;

namespace Prog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SieveController : ControllerBase
    {
        private readonly ILogger<SieveController> _logger;

        public SieveController(ILogger<SieveController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("sieve")]
        public async Task<IActionResult> SieveParamsJson([FromBody] SieveRequest request)
        {
            try
            {
                if (request == null || request.Border <= 0 || request.Border > 1000000)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid border value. Please provide a positive integer between 1 and 1,000,000."
                    });
                }

                int parsedBorder = request.Border;

                // Используем решето Эратосфена
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

    public class SieveRequest
    {
        public int Border { get; set; } // Верхний предел для поиска простых чисел
    }
}