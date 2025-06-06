using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReckonApp.Application.StringSearch.Commands;
using ReckonApp.Application.StringSearch.Mappings;

namespace ReckonApp.API.Controllers
{
    [Route("")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringSearchCommandResultMapper _mapper;

        public IndexController(IMediator mediator, IStringSearchCommandResultMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> ShowResults()
        {
            try
            {
                var query = new StringMatchCommand();
                var result = await _mediator.Send(query);
                var mapped = _mapper.Map(result);

                return Ok(mapped);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Failed to retrieve subtexts: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
