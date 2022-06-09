using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SaborController : ControllerBase
    {
        private readonly ISabor _saborDAO;
        public SaborController(ISabor saborDAO) { _saborDAO = saborDAO; }
        [HttpGet, Route("GetSaborAsync")]
        public async Task<IActionResult> GetAsync(string Cd_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _saborDAO.GetAsync(Request.Headers["token"].ToString(), Cd_produto);
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
    }
}
