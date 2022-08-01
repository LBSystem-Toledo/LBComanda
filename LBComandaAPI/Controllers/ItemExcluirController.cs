using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LBComandaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemExcluirController : ControllerBase
    {
        private readonly IItemExcluir _queryDAO;
        public ItemExcluirController(IItemExcluir queryDAO) { _queryDAO = queryDAO; }

        [HttpGet, Route("GetItemExcluirAsync")]
        public async Task<IActionResult> GetItemExcluirAsync([FromQuery]string Cd_grupo)
        {
            if (!Request.Headers.ContainsKey("token"))
                return StatusCode(500, "Acesso não autorizado");
            try
            {
                var ret = await _queryDAO.GetAsync(Request.Headers["token"].ToString(), Cd_grupo);
                return Ok(ret);
            }
            catch { return BadRequest(); }
        }
    }
}
