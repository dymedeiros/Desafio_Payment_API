using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioPottencial.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome {get; set;}
        [Required]
        public int Quantidade {get; set;}
        [Required]
        public decimal Valor {get; set;}
        
        [ForeignKey("FK_Venda")]
        public int VendaID {get; set;}
    }
}