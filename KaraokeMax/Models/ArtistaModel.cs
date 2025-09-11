using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace KaraokeMax.Models
{

    [Table("artista")]
    class ArtistaModel: BaseModel
    {
        [PrimaryKey("id")]
        public String id { get; set; }
        [Column("nome")]
        public String nome { get; set; }
        [Column("qtdMusicas")]
        public int quantidadeMusicas { get; set; }

        public ArtistaModel(String id, String nome, int quantidadeMusicas)
        {
            this.id = id;
            this.nome = nome;
            this.quantidadeMusicas = quantidadeMusicas;
        }
    }
}
