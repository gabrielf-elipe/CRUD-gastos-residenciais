
using Microsoft.EntityFrameworkCore;
using projetinho.Models;

namespace projetinho.data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        // as 2 tabelas diferentes
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
    }
}