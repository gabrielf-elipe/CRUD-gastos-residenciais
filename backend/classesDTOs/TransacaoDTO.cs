
using System.ComponentModel.DataAnnotations;

namespace projetinho.classesDTOs
{
    public class TransacaoDTO
    {
        public string Descricao { get; set; } = string.Empty;
        [Required]
        public double Valor { get; set; }
        [Required]
        public string Tipo { get; set; } = string.Empty;
        [Required]
        public Guid PessoaId { get; set; }
    }
}
             
                