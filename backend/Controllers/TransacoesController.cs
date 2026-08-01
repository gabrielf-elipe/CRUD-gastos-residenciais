using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projetinho.classesDTOs;
using projetinho.data;
using projetinho.Models;


namespace projetinho.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController(AppDbContext appDbContext) : ControllerBase
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        [HttpPost]

        public async Task<IActionResult> AdicionarTransacao([FromBody] TransacaoDTO transacaoDTO)
        {
            // validação pra checar se o PessoaId passado na criação de uma transação realmente existe
            var pessoa = await _appDbContext.Pessoas.FirstOrDefaultAsync(p => p.Id == transacaoDTO.PessoaId);
            if (pessoa == null)
            {
                return NotFound("Não foi possível criar a transação: PessoaId inválido.");
            }
            if (!new[] { "despesa", "receita" }.Contains(transacaoDTO.Tipo, StringComparer.CurrentCultureIgnoreCase))
            {
                return BadRequest("tipo de transação inválido, deve ser 'despesa' ou 'receita'");
            }

            if (transacaoDTO.Tipo.Equals("receita", StringComparison.OrdinalIgnoreCase) && pessoa.Idade < 18)
            {
                return BadRequest("Não foi possível criar a transação: menores de idade não podem ter receita");
            }
            Transacao transacao = new Transacao
            {
                Descricao = transacaoDTO.Descricao,
                Tipo = transacaoDTO.Tipo,
                Valor = transacaoDTO.Valor,
                PessoaId = transacaoDTO.PessoaId
            };


            _appDbContext.Transacoes.Add(transacao);
            await _appDbContext.SaveChangesAsync();
            return Ok(transacao);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transacao>>> ListarTransacoes()
        {
            var transacoes = await _appDbContext.Transacoes.ToListAsync();
            return Ok(transacoes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarTransacaoId(Guid id)
        {
            var transacao = await _appDbContext.Transacoes.FindAsync(id);
            if(transacao == null)
            {
                return NotFound("Não existe transação com esse ID");
            }
            return Ok(transacao);
        }
    }   
}