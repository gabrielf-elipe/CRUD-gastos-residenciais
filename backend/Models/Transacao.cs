using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace projetinho.Models
{
    public class Transacao
    {
        [Key]
        public Guid Id { get; private set; }

        public string? Descricao { get; set; }
        [Required]
        public double Valor { get; set; }

        [Required]
        public string? Tipo { get; set; }
        [Required]
        public Guid PessoaId { get; set; }

        [ForeignKey("PessoaId")]
        // pra poder ligar as 2 tabelas e poder puxar info de pessoa por meio de uma transacao
        public Pessoa? Pessoa { get; set; }
    }
}