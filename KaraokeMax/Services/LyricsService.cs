using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KaraokeMax.Services
{
    public class LyricsService : IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _baseFolder;

        /// <param name="baseFolder">
        /// Pasta raiz onde salvar: default = "musicas"
        /// </param>
        public LyricsService(string baseFolder = "musicas", HttpClient httpClient = null)
        {
            _baseFolder = string.IsNullOrWhiteSpace(baseFolder) ? "musicas" : baseFolder.Trim();
            _http = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Busca a LRC (LRCLIB /api/get) e salva em:
        /// musicas/{artista}/{musica}/LRCS/lyrics.lrc
        /// Retorna o caminho do arquivo salvo ou null se não encontrou.
        /// </summary>
        public async Task<string> GetAndSaveLrcAsync(string title, string artist = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título é obrigatório.", nameof(title));

            // 1) Monta URL do LRCLIB (apenas GET simples)
            var url = new StringBuilder("https://lrclib.net/api/get?")
                .Append("track_name=").Append(Uri.EscapeDataString(title.Trim()));

            if (!string.IsNullOrWhiteSpace(artist))
                url.Append("&artist_name=").Append(Uri.EscapeDataString(artist.Trim()));

            // 2) Chama a API
            using (var res = await _http.GetAsync(url.ToString()))
            {
                if (!res.IsSuccessStatusCode)
                    return null;

                var json = await res.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement synced;
                    if (!root.TryGetProperty("syncedLyrics", out synced) ||
                        synced.ValueKind != JsonValueKind.String)
                    {
                        return null; // não veio LRC sincronizada
                    }

                    var lrc = synced.GetString();
                    if (string.IsNullOrWhiteSpace(lrc))
                        return null;

                    // 3) Monta estrutura de pastas: musicas/{artista}/{musica}/LRCS/
                    var artistName = string.IsNullOrWhiteSpace(artist) ? "Desconhecido" : artist.Trim();
                    var safeArtist = SanitizeName(artistName);
                    var safeTitle = SanitizeName(title.Trim());

                    var folder = Path.Combine(_baseFolder, safeArtist, safeTitle);
                    Directory.CreateDirectory(folder);

                    var filePath = Path.Combine(folder, "lyrics.lrc");

                    // 4) Salva arquivo
                    File.WriteAllText(filePath, lrc, Encoding.UTF8);

                    return filePath;
                }
            }
        }

        /// <summary>
        /// Remove caracteres inválidos para nome de arquivo/pasta e aparas.
        /// </summary>
        private static string SanitizeName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var ch in name)
            {
                if (Array.IndexOf(invalid, ch) >= 0)
                    sb.Append('_');
                else
                    sb.Append(ch);
            }
            // colapsa espaços e troca barras invertidas/perigosas por _
            var clean = sb.ToString().Trim();
            clean = clean.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
            return string.IsNullOrWhiteSpace(clean) ? "_" : clean;
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
