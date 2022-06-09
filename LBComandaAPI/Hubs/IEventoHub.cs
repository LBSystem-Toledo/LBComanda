using System.Threading.Tasks;

namespace LBComandaAPI.Hubs
{
    public interface IEventoHub
    {
        Task AtualizaMesa(string evento);
    }
}
