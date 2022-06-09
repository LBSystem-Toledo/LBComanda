using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IIngrediente
    {
        Task<IEnumerable<Ingredientes>> GetAsync(string token, string Cd_produto);
    }
}
