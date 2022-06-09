namespace LBComandaPrism.Models
{
    public class RetornoStone
    {
        public int status { get; set; }
        public bool success { get; set; } = false;
        public string error { get; set; } = string.Empty;
        public string msg { get; set; } = string.Empty;
        public PreTransacao pre_transaction { get; set; }
        public Transacao transacao { get; set; }
    }

    public class TokenStone
    {
        public string token { get; set; } = string.Empty;
        public int expira_em { get; set; }
    }

    public class PreTransacao
    {
        //Valor da transacao
        public int amount { get; set; }
        //id do estabelecimento
        public string establishment_id { get; set; } = string.Empty;
        /*Número de referência da maquininha que a estação de trabalho ou ponto de venda do sistema do integrador disponibilizará a pré-transação. 
         * Esse atributo se torna opcional de acordo com as configurações realizadas nas rotas de 'Configuração de maquininha'. 
         * Caso a configuração exija um vinculo da pré-transação criada pela estação de trabalho, 
         * o ID enviado aqui deve ser de uma maquininha que possua configuração para tal ação.
         */
        public string pos_reference_id { get; set; } = string.Empty;
        //Possui a configuração do tipo de pagamento na maquininha
        public Pagamento payment { get; set; }
        //Título a ser exibido na listagem de operacões.
        public string information_title { get; set; } = string.Empty;
        //Retorno API
        public string pre_transaction_token { get; set; } = string.Empty;
        public string pre_transaction_id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }

    public class Pagamento
    {
        /*Representa o tipo de pagamento que será realizado e deve ser enviado 
         * seguindo as formas possíveis que são, 1 débito, 2 crédito e 3 voucher.
         */
        public int type { get; set; }
        /*representa o parcelamento das transações do tipo 2 crédito, somente podendo ser enviado de 2 a 12. 
         * Caso queira utilizar crédito a vista basta não enviar esse campo ou enviar o mesmo como null.
         */
        public string installment { get; set; }
        /*O tipo de parcelamento utilizado, somente será enviado em caso de transações no crédito e com parcelamentos. 
         * Os 2 tipos possíveis são 1 sem juros e 2 com juros.
         * (Caso não seja enviado esse campo o sistema irá entender que o parcelamento deve ser sem juros.)
         */
        public int installment_type { get; set; }
    }

    public class Transacao
    {
        public string stone_transaction_id { get; set; } = string.Empty;
        public string card_brand { get; set; } = string.Empty;
        public string payment_type { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }
}
