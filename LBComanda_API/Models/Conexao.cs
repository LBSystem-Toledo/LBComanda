using System.Configuration;
using System.Data.SqlClient;

namespace LBComanda_API.Models
{
    public static class TConexao
    {
        public static SqlConnection Conexao { get; private set; }
        public static bool Ativar(bool ativar)
        {
            if (ativar)
            {
                try
                {
                    string con = ConfigurationManager.ConnectionStrings["minhaConexao"].ConnectionString;
                    Conexao = new SqlConnection(con);
                    Conexao.Open();
                    return true;
                }
                catch { return false; }
            }
            else
            {
                if (Conexao.State == System.Data.ConnectionState.Open)
                    Conexao.Close();
                return true;
            }
        }

        public static void ExcluirConexao()
        {
            if (Conexao.State == System.Data.ConnectionState.Open)
                Conexao.Close();
            Conexao.Dispose();
        }
    }
}
