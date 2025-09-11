using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeMax.Models
{
    public class MusicaModel
    {
        public String id { get; set; }
        public String nome { get; set; }
        public String idArtista { get; set; }
        public String caminhoVoz { get; set; }
        public String caminhoInstrumental { get; set; }
        public String caminhoLetra { get; set; }
        public int duracaoSegundos { get; set; } // duração da música em segundos
        public String status { get; set; } // CARREGANDO, OK ou ERRO
        public String idUsuario { get; set; } // id do usuário que adicionou a música

        public MusicaModel(String id, String nome, String idArtista, String caminhoVoz, String caminhoInstrumental, String caminhoLetra, int duracaoSegundos, String status, string idUsuario)
        {
            this.id = id;
            this.nome = nome;
            this.idArtista = idArtista;
            this.caminhoVoz = caminhoVoz;
            this.caminhoInstrumental = caminhoInstrumental;
            this.caminhoLetra = caminhoLetra;
            this.duracaoSegundos = duracaoSegundos;
            this.status = status;
            this.idUsuario = idUsuario;
        }
    }
}
