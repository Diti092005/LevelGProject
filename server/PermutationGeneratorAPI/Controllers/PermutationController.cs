using Microsoft.AspNetCore.Mvc;
using PermutationGeneratorAPI.Models.DTOs;
using PermutationGeneratorAPI.Services.Interfaces;

namespace PermutationGeneratorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermutationController : ControllerBase
    {
        private readonly IPermutationBusinessLogicService _businessLogic;
        private readonly ILogger<PermutationController> _logger;

        public PermutationController(
            IPermutationBusinessLogicService businessLogic,
            ILogger<PermutationController> logger)
        {
            _businessLogic = businessLogic;
            _logger = logger;
        }

        [HttpPost("start")]
        public IActionResult Start([FromBody] PermutationRequest request)
        {
            try
            {
                if (request?.N < 1 || request?.N > 20)
                    return BadRequest(new { error = "N חייב להיות בין 1 ל-20" });

                var (sessionId, totalPermutations, n) = _businessLogic.StartNewSession(request.N);

                return Ok(new
                {
                    sessionId = sessionId,
                    totalPermutations = totalPermutations,
                    n = n
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Start: {ex.Message}");
                return StatusCode(500, new { error = "שגיאת שרת פנימית" });
            }
        }

        [HttpPost("next")]
        public IActionResult GetNext([FromBody] SessionRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.SessionId))
                    return BadRequest(new { error = "sessionId נדרש" });

                var response = _businessLogic.GetNextPermutation(request.SessionId);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetNext: {ex.Message}");
                return StatusCode(500, new { error = "שגיאת שרת פנימית" });
            }
        }

        [HttpPost("all")]
        public IActionResult GetAll([FromBody] PagedRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.SessionId))
                    return BadRequest(new { error = "sessionId נדרש" });

                int pageSize = request.PageSize > 0 ? request.PageSize : 100;
                if (pageSize > 1000)
                    return BadRequest(new { error = "pageSize חייב להיות בין 1 ל-1000" });

                var response = _businessLogic.GetPermutationsPage(request.SessionId, pageSize, request.StartIndex);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAll: {ex.Message}");
                return StatusCode(500, new { error = "שגיאת שרת פנימית" });
            }
        }

        [HttpPost("reset")]
        public IActionResult Reset([FromBody] SessionRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.SessionId))
                    return BadRequest(new { error = "sessionId נדרש" });

                _businessLogic.ResetSession(request.SessionId);

                return Ok(new { message = "Session cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Reset: {ex.Message}");
                return StatusCode(500, new { error = "שגיאת שרת פנימית" });
            }
        }
    }
}
