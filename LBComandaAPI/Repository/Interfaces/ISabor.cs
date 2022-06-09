using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface ISabor
    {
        Task<IEnumerable<Sabor>> GetAsync(string token, string Cd_produto);
    }
}
