using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IProduto
    {
        Task<IEnumerable<Produto>> GetAsync(string token);
        Task<IEnumerable<Grupo>> GetGruposAsync(string token, string ds_produto);
    }
}
