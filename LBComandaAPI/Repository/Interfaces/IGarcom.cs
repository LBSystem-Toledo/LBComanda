using LBComandaAPI.Models;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface IGarcom
    {
        Task<Garcom> ValidarGarcomAsync(string Login, string Senha, string Cnpj);
        Task<Token> ValidarTokenAsync(string token, string garcom);
        Task<bool> SolicitarToken(string token, string garcom);
    }
}
