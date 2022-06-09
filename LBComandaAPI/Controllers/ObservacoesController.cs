using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ObservacoesController : ControllerBase
    {
        private readonly IObservacoes _obsDAO;
        public ObservacoesController(IObservacoes obsDAO) { _obsDAO = obsDAO; }
        [HttpGet, Route("GetObsAsync")]
        public async Task<IActionResult> GetObsAsync([FromQuery] string Cd_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _obsDAO.GetAsync(Request.Headers["token"].ToString(), Cd_produto);
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
    }
}
