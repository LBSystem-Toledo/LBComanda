using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MesaController : ControllerBase
    {
        private readonly IMesa _mesaDAO;
        public MesaController(IMesa mesaDAO) { _mesaDAO =mesaDAO; }
        [HttpGet, Route("GetMesaAsync")]
        public async Task<IActionResult> GetMesaAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _mesaDAO.GetAsync(Request.Headers["token"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
        [HttpGet, Route("GetLocalMesaAsync")]
        public async Task<IActionResult> GetLocalMesaAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _mesaDAO.GetLocalAsync(Request.Headers["token"].ToString());
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
