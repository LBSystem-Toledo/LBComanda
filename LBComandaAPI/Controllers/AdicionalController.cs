using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdicionalController : ControllerBase
    {
        private readonly IAdicional _adicionalDAO;
        public AdicionalController(IAdicional adicionalDAO) { _adicionalDAO = adicionalDAO; }
        [HttpGet, Route("GetAdicionalAsync")]
        public async Task<IActionResult> GetAdicionalAsync(string Cd_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _adicionalDAO.GetAsync(Request.Headers["token"].ToString(), Cd_produto);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
