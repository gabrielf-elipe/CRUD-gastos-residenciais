using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projetinho.classesDTOs;
using projetinho.data;
using projetinho.Models;

namespace projetinho.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController(AppDbContext appDbContext) : ControllerBase
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        /// <summary>
        /// Adiciona uma nova pessoa ao banco de dados.
        /// </summary>
        /// <remarks>
        /// teste
        /// </remarks>
        /// <response code="201">Pessoa adicionada com sucesso</response>
        /// <param name="pessoaDTO">dados da pessoa: nome e idade</param>
        [HttpPost]
        public async Task<IActionResult> AdicionarPessoa([FromBody] PessoaDTO pessoaDTO)
        {
            Pessoa pessoa = new Pessoa
            {
                Nome = pessoaDTO.Nome,
                Idade = pessoaDTO.Idade
            };
            _appDbContext.Pessoas.Add(pessoa);
            await _appDbContext.SaveChangesAsync();

            return Created("Pessoa adicionada: ",pessoa);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> ListarPessoas()
        {
            var pessoas = await _appDbContext.Pessoas.ToListAsync();
            // sempre no async await pq as requisições não são instantâneas, é necessário esperar o retorno de cada requisição   

            return Ok(pessoas);
        }
        [HttpGet("buscar-por-nome")]
        public async Task<ActionResult<IEnumerable<Pessoa>>> BuscaPessoaNome([FromQuery] string nome)
        {
            var pessoas = await _appDbContext.Pessoas
                .Where(p => p.Nome.Contains(nome))
                .ToListAsync();
            if (pessoas == null)
            {
                return NotFound("não tem pessoa com esse nome");
            }
            return Ok(pessoas);
        }
        /// <summary>
        /// Consultar total de receitas, despesas e saldo líquido individual de cada pessoa e o geral.
        /// </summary>
        /// <returns>
        /// total de receitas, despesas e saldo líquido para cada pessoa cadastrada, e um total geral desses valores.
        /// </returns>
        /// <response code="200"> totais retornados com sucesso</response>
        [HttpGet("pessoas/totais")]
        public async Task<IActionResult> ConsultarTotalPorPessoa()
        {
    // buscar todas as pessoas e todas as transações do banco em listas separadas
    var pessoas = await _appDbContext.Pessoas.ToListAsync();
    var transacoes = await _appDbContext.Transacoes.ToListAsync();

    // transformar a lista de Pessoas em TotalPorPessoaDTO
    var totalPorPessoa = pessoas.Select(pessoa => 
    {
        // filtrar transacoes que pertencem a pessoa x
        var transacoesDaPessoa = transacoes.Where(t => t.PessoaId == pessoa.Id).ToList();

        var totalReceitas = transacoesDaPessoa
            .Where(t => t.Tipo != null && t.Tipo.Equals("receita", StringComparison.OrdinalIgnoreCase))
            .Sum(t => t.Valor);

        var totalDespesas = transacoesDaPessoa
            .Where(t => t.Tipo != null && t.Tipo.Equals("despesa", StringComparison.OrdinalIgnoreCase))
            .Sum(t => t.Valor);

        // montar o dto agora
        return new TotalPorPessoaDTO
        {
            Nome = pessoa.Nome,
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            Saldo = totalReceitas - totalDespesas
        };
    }).ToList();

   // calcular o total geral somando os valores da lista gerada acima
    var receitaGeral = totalPorPessoa.Sum(p => p.TotalReceitas);
    var despesaGeral = totalPorPessoa.Sum(p => p.TotalDespesas);
    var saldoLiquidoGeral = receitaGeral - despesaGeral;

    return Ok(new 
    {
        Dados = totalPorPessoa,
        ResumoGeral = new 
        {
            TotalReceitasGeral = receitaGeral,
            TotalDespesasGeral = despesaGeral,
            SaldoLiquidoGeral = saldoLiquidoGeral
        }
    });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> BuscarPessoaId([FromRoute] Guid id)
        {
            //busca por id
            var pessoa = await _appDbContext.Pessoas.FindAsync(id);
            if (pessoa == null)
            // caso o user digite um id errado
            {
                return NotFound("Não existe pessoa com esse ID.");
            }
            return Ok(pessoa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPessoa(Guid id, [FromBody] PessoaDTO pessoaAtualizado)
        {
            var pessoa = await _appDbContext.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound("Não existe pessoa com esse ID.");
            }

            pessoa.Nome = pessoaAtualizado.Nome;
            pessoa.Idade = pessoaAtualizado.Idade;
            await _appDbContext.SaveChangesAsync();
            return Ok(pessoa);

        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletarPessoa([FromRoute]Guid id)
        {
            var pessoa = await _appDbContext.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound("Não existe pessoa com esse id");
            }
            _appDbContext.Pessoas.Remove(pessoa);
            await _appDbContext.SaveChangesAsync();
            return Ok(pessoa + "\n DELETADO.");
        }
    }
}