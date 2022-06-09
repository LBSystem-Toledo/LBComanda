using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProduto _produtoDAO;
        public ProdutoController(IProduto produtoDAO) { _produtoDAO = produtoDAO; }
        [HttpGet, Route("GetProdutoAsync")]
        public async Task<IActionResult> GetAsync()
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _produtoDAO.GetAsync(Request.Headers["token"].ToString());
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
        [HttpGet, Route("GetGrupoAsync")]
        public async Task<IActionResult> GetGrupoAsync([FromQuery]string ds_produto)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var result = await _produtoDAO.GetGruposAsync(Request.Headers["token"].ToString(), ds_produto);
                return Ok(result);
            }
            catch { return BadRequest(); }
        }
    }
}
