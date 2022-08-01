using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IItemExcluir
    {
        Task<IEnumerable<ItemExcluir>> GetAsync(string token, string Cd_grupo);
    }
}
