
namespace projetinho.classesDTOs
{
    public class TotalPorPessoaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public double TotalReceitas { get; set; }
        public double TotalDespesas { get; set; }
        public double Saldo{ get; set; }
    }
}