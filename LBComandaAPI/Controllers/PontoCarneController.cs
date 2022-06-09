using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PontoCarneController : ControllerBase
    {
        private readonly IPontoCarne _pontocarneDAO;
        public PontoCarneController(IPontoCarne pontocarneDAO) { _pontocarneDAO = pontocarneDAO; }

        [HttpGet, Route("GetPontoCarneAsync")]
        public async Task<IActionResult> GetAsync(string Cd_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _pontocarneDAO.GetAsync(Request.Headers["token"].ToString(), Cd_produto);
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
    }
}
