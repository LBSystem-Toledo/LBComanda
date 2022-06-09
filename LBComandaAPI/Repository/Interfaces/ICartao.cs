using System.Threading.Tasks;

namespace LBComandaAPI.Repository.Interfaces
{
    public interface ICartao
    {
        Task<bool> AbrirCartaoAsync(string token, int Nr_cartao, string Celular, string Nome, bool MenorIdade);
        Task<bool> ConsultarCartaoAbertoAsync(string token, int Nr_cartao);
        Task<string> ConsultaClienteAsync(string token, string Celular);
    }
}
