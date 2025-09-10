using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace KaraokeMax
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private List<LrcLine> _lines = new List<LrcLine>();
        private int _currentIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50); // ~20 fps
            _timer.Tick += OnTick;

            // exemplos para testar rápido
            //TxtAudio.Text = @"C:\musicas\minha\instrumental.wav";
            //TxtLrc.Text   = @"\musicas\Coldplay\Fix You\lyrics.lrc";
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            String audioPath = TxtAudio.Text.TrimStart('"').TrimEnd('"');
            String lrcPath = TxtLrc.Text.TrimStart('"').TrimEnd('"');
            if (!File.Exists(audioPath) || !File.Exists(lrcPath))
            {
                MessageBox.Show("Informe caminhos válidos para o WAV e o LRC.");
                return;
            }

            // carrega audio
            Player.Source = new Uri(audioPath);

            // carrega e parseia LRC
            var lrc = File.ReadAllText(lrcPath);
            _lines = ParseLrc(lrc);
            LstLyrics.ItemsSource = _lines; // ToString do item mostrará o texto
            _currentIndex = -1;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source == null)
            {
                MessageBox.Show("Carregue primeiro o áudio e a letra.");
                return;
            }
            Player.Play();
            _timer.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Player.Pause();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_lines.Count == 0 || Player.NaturalDuration.HasTimeSpan == false) return;

            double t = Player.Position.TotalSeconds;

            // encontra a última linha com tempo <= t (busca linear simples; para listas grandes, use busca binária)
            int idx = _currentIndex;
            if (idx < 0 || idx >= _lines.Count || _lines[idx].Time > t)
                idx = 0;

            while (idx + 1 < _lines.Count && _lines[idx + 1].Time <= t)
                idx++;

            if (idx != _currentIndex)
            {
                _currentIndex = idx;
                LstLyrics.SelectedIndex = idx;
                LstLyrics.ScrollIntoView(LstLyrics.SelectedItem);
            }
        }

        // ---- LRC simples: [mm:ss.xx]texto (suporta múltiplos carimbos por linha) ----
        private static List<LrcLine> ParseLrc(string lrcText)
        {
            var result = new List<LrcLine>();
            if (string.IsNullOrWhiteSpace(lrcText)) return result;

            var lines = lrcText.Replace("\r", "").Split('\n');
            var rxTag = new Regex(@"^\[(ar|ti|al|by|offset):", RegexOptions.IgnoreCase);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;
                if (rxTag.IsMatch(line)) continue;

                int i = 0;
                var stamps = new List<double>();
                while (i < line.Length && line[i] == '[')
                {
                    int j = line.IndexOf(']', i);
                    if (j < 0) break;
                    var stamp = line.Substring(i + 1, j - i - 1);
                    double? sec = TryParseStamp(stamp);
                    if (sec.HasValue) stamps.Add(sec.Value);
                    i = j + 1;
                }

                var text = line.Substring(Math.Min(i, line.Length)).Trim();
                if (stamps.Count == 0 || text.Length == 0) continue;

                foreach (var s in stamps)
                    result.Add(new LrcLine { Time = s, Text = text });
            }

            result.Sort((a, b) => a.Time.CompareTo(b.Time));
            return result;
        }

        private static double? TryParseStamp(string s)
        {
            // mm:ss | mm:ss.xx | mm:ss.xxx
            try
            {
                var parts = s.Split(':');
                if (parts.Length != 2) return null;
                int mm = int.Parse(parts[0]);
                int ss;
                int ms = 0;
                if (parts[1].Contains("."))
                {
                    var dot = parts[1].IndexOf('.');
                    ss = int.Parse(parts[1].Substring(0, dot));
                    var frac = parts[1].Substring(dot + 1);
                    if (frac.Length == 1) frac += "00";
                    else if (frac.Length == 2) frac += "0";
                    else if (frac.Length > 3) frac = frac.Substring(0, 3);
                    ms = int.Parse(frac);
                }
                else
                {
                    ss = int.Parse(parts[1]);
                }
                return mm * 60 + ss + ms / 1000.0;
            }
            catch { return null; }
        }

        private class LrcLine
        {
            public double Time { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }
    }
}
