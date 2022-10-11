using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioPottencial.Models
{
    public class Vendedor
    {
        [Key]
        public int Id {get; set;}
        [Required]
        [MaxLength(200)]
        public string Nome {get;set;}
        [Required]
        public string CPF {get; set;}
        [Required]
        public string Email {get; set;}
        [Required]
        public string Telefone {get; set;}

        public List<Venda> Vendas {get; set;}
    }
}