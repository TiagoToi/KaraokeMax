using KaraokeMax.Models;
using KaraokeMax.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KaraokeMax
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public async void Application_Startup(object sender, StartupEventArgs e)
        {
            /*Console.WriteLine("Application starting up...");

            LyricsService svc = new LyricsService();
            // título + artista (opcional)
            String savedPath = await svc.GetAndSaveLrcAsync("Cidade Vizinha", "Henrique & Juliano");

            if (savedPath != null)
                Console.WriteLine("LRC salva em: " + savedPath);
            else
                Console.WriteLine("LRC não encontrada.");

            MainWindow janela = new MainWindow();
            janela.Show();*/

            List<ArtistaModel> artistas = Services.Banco_de_Dados.ArtistaService.GetArtistasFromDatabase();
            foreach (ArtistaModel artista in artistas)
            {
                Console.WriteLine($"ID: {artista.id}, Nome: {artista.nome}, Quantidade de Músicas: {artista.quantidadeMusicas}");
            }
        }
    }
}
