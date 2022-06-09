using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class IngredienteController : ControllerBase
    {
        private readonly IIngrediente _ingredienteDAO;
        public IngredienteController(IIngrediente ingredienteDAO) { _ingredienteDAO = ingredienteDAO; }
        [HttpGet, Route("GetIngredienteAsync")]
        public async Task<IActionResult> GetIngredienteAsync(string Cd_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _ingredienteDAO.GetAsync(Request.Headers["token"].ToString(), Cd_produto);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
