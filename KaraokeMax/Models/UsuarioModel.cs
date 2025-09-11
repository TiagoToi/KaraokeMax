using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeMax.Models
{
    class UsuarioModel
    {
        String id { get; set; }
        bool primeiroAcesso { get; set; }
        String nome { get; set; }
        String email { get; set; }
        String senha { get; set; }
        String tipo { get; set; } // admin convidado ou user

        public UsuarioModel(String id, bool primeiroAcesso, String nome, String email, String senha, String tipo)
        {
            this.id = id;
            this.primeiroAcesso = primeiroAcesso;
            this.nome = nome;
            this.email = email;
            this.senha = senha;
            this.tipo = tipo;
        }
    }
}
