using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioPottencial.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioPottencial.Context
{
    public class PagamentosContext : DbContext
    {
        public PagamentosContext(DbContextOptions<PagamentosContext> options) : base(options)
        {

        }

        public DbSet<Venda> Vendas {get; set;}
        public DbSet<Vendedor> Vendedores {get; set;}
        public DbSet<Item> Itens {get; set;}
    }
}