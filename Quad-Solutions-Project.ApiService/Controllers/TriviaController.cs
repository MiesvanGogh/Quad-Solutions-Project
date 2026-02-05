using Microsoft.AspNetCore.Mvc;
using Quad_Solutions_Project.Models;
using Quad_Solutions_Project.Models.Interfaces;

namespace Quad_Solutions_Project.ApiService.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class TriviaController : ControllerBase
    {
        private readonly ITriviaService _triviaService;

        public TriviaController(ITriviaService triviaService)
        {
            _triviaService = triviaService;
        }

        [HttpGet("questions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TriviaQuestionResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<List<TriviaQuestionResponse>>> GetQuestions(
            [FromQuery] GetQuestionsParameters parameters,
            CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _triviaService
                    .GetQuestionsAsync(parameters, cancellationToken)
                    .ConfigureAwait(false);

                return Ok(questions);
            }
            catch (Exception)
            {
                // Let .NET 8 / Aspire global exception handling deal with it
                throw;
            }
        }

        [HttpPost("checkanswers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckAnswerResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CheckAnswerResponse> CheckAnswer(
            [FromBody] CheckAnswerRequest request)
        {
            try
            {
                var result = _triviaService.CheckAnswer(request);

                if (result is null)
                {
                    return NotFound(new
                    {
                        message = $"Question '{request.QuestionId}' not found. It may have already been answered or the ID is invalid."
                    });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
