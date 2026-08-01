using System.ComponentModel.DataAnnotations;

namespace projetinho.classesDTOs
{
    // decidi usar DTO pra evitar conflitos por causa do id na hora de usar put
    public class PessoaDTO
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public int Idade { get; set; }
    }
}