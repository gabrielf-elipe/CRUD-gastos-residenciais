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
        /// <response code="201">Pessoa adicionada com sucesso</response>
        /// <param name="pessoaDTO">dados da pessoa: nome e idade, o id GUID é gerado automaticamente</param>
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

            return CreatedAtAction(nameof(BuscarPessoaId), new { id = pessoa.Id }, pessoa);
        }
        /// <summary>
        /// Retorna uma lista das pessoas no banco de dados.
        /// </summary>
        /// <remarks>
        /// Lista todas as pessoas presentes no banco de dados
        /// </remarks>
        /// <response code="200">Listagem retornada com sucesso</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> ListarPessoas()
        {
            var pessoas = await _appDbContext.Pessoas.ToListAsync();
            // sempre no async await pq as requisições não são instantâneas, é necessário esperar o retorno de cada requisição   

            return Ok(pessoas);
        }
        /// <summary>
        /// Retorna pessoas com o nome especificado (é case sensitive). 
        /// </summary>
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
        /// <summary>
        /// Buscar a pessoa que tem o id especificado.
        /// </summary>
        /// <returns>
        /// Pessoa específica com esse id.
        /// </returns>
        /// <response code="200"> Pessoa encontrada e retornada com sucesso.</response>
        /// <response code="400"> Id em formato inválido, deve ser GUID.</response>
        /// <response code="404"> Id inválido, não foi encontrado nenhuma pessoa.</response>
        /// <param name="id">id GUID da pessoa.</param>  
        // a partir daqui não vou mais documentar completão pq nesse contexto de projeto de estudos, é meio redundante e acho que já deu pra ver que entendi como funciona
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
        /// <summary>
        /// Atualiza informações de uma pessoa com o id especificado.
        /// </summary>
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
        /// <summary>
        /// Deleta uma pessoa com o id especificado
        /// </summary>
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
            return NoContent();
        }
    }
}