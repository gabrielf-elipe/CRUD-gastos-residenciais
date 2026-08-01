using System.ComponentModel.DataAnnotations;

namespace projetinho.Models
{
    public class Pessoa
    {
        [Key]
        public Guid Id { get; private set; } 
        // ^ gera um id unico pra cada pessoa no instante q um obj pessoa é criado
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public int Idade { get; set; }

        public List<Transacao> Transacoes = new();

    }
}