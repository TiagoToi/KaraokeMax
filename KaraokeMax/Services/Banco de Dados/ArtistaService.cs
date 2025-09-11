using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using KaraokeMax.Models;
using Npgsql;

namespace KaraokeMax.Services.Banco_de_Dados
{
    static class ArtistaService
    {
        // String de conexão centralizada para uso em todos os métodos
        private static readonly string connString = "Host=db.mlhoecnsqszrhhhuxekm.supabase.co;Username=postgres;Password=KaraokePucSecreto;Database=postgres";

        public static List<ArtistaModel> GetArtistasFromDatabase()
        {
            List<ArtistaModel> artistas = new List<ArtistaModel>();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, nome, qtdMusicas FROM Artistas", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        artistas.Add(new ArtistaModel(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetInt32(2)
                        ));
                    }
                }
            }
            return artistas;
        }
    }
}
