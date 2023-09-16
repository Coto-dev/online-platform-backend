using HW.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

    /// <summary>
    /// Controller for module management
    /// </summary>
    [ApiController]
    [Route("api/parser")]
    public class ParserController : ControllerBase {
        private readonly IParserService _parserService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parserService"></param>
        public ParserController(IParserService parserService) {
            _parserService = parserService;
        }

        /// <summary>
        /// Parse docx to sub modules and chapters
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("parse")]
        public async Task<ActionResult> UploadFiles(IFormFile file) {
            await _parserService.ParseFile(file);
            return Ok();
        }
    }
