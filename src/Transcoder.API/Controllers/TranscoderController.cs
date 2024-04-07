using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transcoder.Application.Mediator.Transcoder;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transcoder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscoderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TranscoderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("transcoder/{idProduct}/{filename}")]
        public async Task<IActionResult> GetFileToTranscoderAsync(string idProduct, string filename)
        {
            var result = await _mediator.Send(new TranscoderRequest(idProduct, filename));
            return Ok(result);
        }
    }
}
