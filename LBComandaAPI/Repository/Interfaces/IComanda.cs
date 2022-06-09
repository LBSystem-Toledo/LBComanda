using LBComandaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IComanda
    {
        Task<IEnumerable<Comanda>> GetAsync(string token);
        Task GravarImpressoAsync(string token, List<Comanda> Itens);
    }
}
