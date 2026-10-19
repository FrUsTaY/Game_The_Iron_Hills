using System;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using EpicBattle.Managers;

namespace EpicBattle.Services
{
    /// <summary>
    /// Этап 3.3: звуковой движок без внешних ассетов — процедурный синтез WAV-тонов (System.Media.SoundPlayer).
    /// Громкость эффектов берётся из настроек (SaveManager.Settings.SfxVolume), звук не блокирует UI (PlayLoopingAsync).
    /// Файлы генерируются один раз в %APPDATA%/EpicBattle/Sounds и переиспользуются.
    /// </summary>
    public static class AudioEngine
    {
        private static readonly string SoundDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EpicBattle", "Sounds");

        // Кэшированные проигрыватели по имени эффекта.
        private static readonly System.Collections.Generic.Dictionary<string, SoundPlayer> _players =
            new System.Collections.Generic.Dictionary<string, SoundPlayer>();

        private static bool _initFailed = false;

        // ===== Публичные события =====
        public static void Play(string effect)
        {
            try
            {
                if (_initFailed) return;
                double vol = SaveManager.Settings?.SfxVolume ?? 0.8;
                if (vol <= 0.01) return;

                var player = GetPlayer(effect, vol);
                if (player == null) return;
                player.Play(); // асинхронно, не блокирует поток боя/UI
            }
            catch
            {
                // Звук — не критичная функциональность: при проблемах с аудио просто молчим.
                _initFailed = true;
            }
        }

        private static SoundPlayer? GetPlayer(string effect, double volume)
        {
            string key = $"{effect}@{volume:F2}";
            if (_players.TryGetValue(key, out var cached)) return cached;

            string file = Path.Combine(SoundDir, $"{effect}_{(int)(volume * 100)}.wav");
            if (!File.Exists(file))
            {
                Directory.CreateDirectory(SoundDir);
                byte[] wav = GenerateWav(effect, volume);
                File.WriteAllBytes(file, wav);
            }

            var p = new SoundPlayer(file);
            _players[key] = p;
            return p;
        }

        // ===== Сопоставление событий боёвки со звуками =====
        private static readonly Regex AttackLine = new Regex(@"атакует|наносите .* урона", RegexOptions.Compiled);
        private static readonly Regex CritLine = new Regex(@"КРИТИЧЕСКИЙ УДАР", RegexOptions.Compiled);
        private static readonly Regex MagicLine = new Regex(@"маг(\.|ического)? урона|использует 'Огненный шар'|Fireball|Горение", RegexOptions.Compiled);
        private static readonly Regex HealLine = new Regex(@"восстанавливает \d+ HP|исцеляет", RegexOptions.Compiled);
        private static readonly Regex MissLine = new Regex(@"уклоняется|промахивается", RegexOptions.Compiled);
        private static readonly Regex DefendLine = new Regex(@"приготовились к защите|🛡️", RegexOptions.Compiled);
        private static readonly Regex DeathLine = new Regex(@"повержен|💀 Вы погибли", RegexOptions.Compiled);
        private static readonly Regex VictoryLine = new Regex(@"победили во всех волнах|🏆", RegexOptions.Compiled);

        /// <summary>Проигрывает эффект, подобранный по строке боевого лога.</summary>
        public static void PlayForLogLine(string logMessage)
        {
            if (string.IsNullOrEmpty(logMessage)) return;
            if (CritLine.IsMatch(logMessage)) { Play("crit"); return; }
            if (MagicLine.IsMatch(logMessage)) { Play("magic"); return; }
            if (HealLine.IsMatch(logMessage)) { Play("heal"); return; }
            if (MissLine.IsMatch(logMessage)) { Play("miss"); return; }
            if (DefendLine.IsMatch(logMessage)) { Play("defend"); return; }
            if (DeathLine.IsMatch(logMessage)) { Play("death"); return; }
            if (VictoryLine.IsMatch(logMessage)) { Play("victory"); return; }
            if (AttackLine.IsMatch(logMessage)) { Play("hit"); return; }
        }

        /// <summary>Звук обычной атаки по типу оружия игрока (клинок/посох/кинжалы/топор).</summary>
        public static void PlayWeaponHit(string? weaponName)
        {
            string w = (weaponName ?? "").ToLowerInvariant();
            if (w.Contains("посох") || w.Contains("жезл") || w.Contains("staff")) Play("hit_staff");
            else if (w.Contains("кинжал") || w.Contains("катана") || w.Contains("daggers")) Play("hit_dagger");
            else if (w.Contains("топор") || w.Contains("молот") || w.Contains("axe") || w.Contains("hammer")) Play("hit_axe");
            else Play("hit_sword");
        }

        // ===== Процедурная генерация WAV =====
        private const int SampleRate = 22050;

        private static byte[] GenerateWav(string effect, double volume)
        {
            // (длительность, частота, тип огибающей, «шумовой» микс)
            var ms = effect switch
            {
                "hit"          => new[] { T(0.06, 190, Wave.Square, 0.25) },
                "hit_sword"    => new[] { T(0.07, 420, Wave.Sawtooth, 0.35), T(0.04, 1800, Wave.Sine, 0.15) },
                "hit_staff"    => new[] { T(0.12, 300, Wave.Sine, 0.0), T(0.10, 620, Wave.Sine, 0.0) },
                "hit_dagger"   => new[] { T(0.04, 900, Wave.Sawtooth, 0.4) },
                "hit_axe"      => new[] { T(0.09, 120, Wave.Square, 0.45) },
                "crit"         => new[] { T(0.05, 260, Wave.Square, 0.3), T(0.10, 520, Wave.Sawtooth, 0.2) },
                "magic"        => new[] { T(0.14, 700, Wave.Sine, 0.0), T(0.10, 1050, Wave.Sine, 0.0) },
                "heal"         => new[] { T(0.10, 523, Wave.Sine, 0.0), T(0.14, 784, Wave.Sine, 0.0) },
                "miss"         => new[] { T(0.08, 500, Wave.Sine, 0.6) },
                "defend"       => new[] { T(0.06, 150, Wave.Square, 0.15), T(0.06, 220, Wave.Square, 0.1) },
                "death"        => new[] { T(0.18, 140, Wave.Sawtooth, 0.2), T(0.25, 80, Wave.Sine, 0.1) },
                "victory"      => new[] { T(0.12, 523, Wave.Square, 0.0), T(0.12, 659, Wave.Square, 0.0), T(0.20, 784, Wave.Square, 0.0) },
                "click"        => new[] { T(0.03, 800, Wave.Sine, 0.1) },
                _              => new[] { T(0.05, 440, Wave.Sine, 0.1) },
            };

            int totalSamples = 0;
            foreach (var t in ms) totalSamples += (int)(t.Duration * SampleRate);
            float[] samples = new float[totalSamples];

            int pos = 0;
            foreach (var tone in ms)
            {
                int n = (int)(tone.Duration * SampleRate);
                for (int i = 0; i < n; i++)
                {
                    double tt = (double)i / SampleRate;
                    double env = Math.Exp(-tt * tone.DecayRate) * (1.0 - (double)i / n * 0.3); // экспонента + линейный хвост
                    double v = Oscillate(tone.WaveType, tone.Freq, tt);
                    if (tone.NoiseMix > 0)
                        v = v * (1 - tone.NoiseMix) + ((GameRng.NextDouble() * 2 - 1)) * tone.NoiseMix;
                    samples[pos++] = (float)(v * env * volume * 0.6);
                }
            }

            return ToWavBytes(samples);
        }

        private enum Wave { Sine, Square, Sawtooth }

        private readonly struct Tone
        {
            public readonly double Duration; public readonly double Freq;
            public readonly Wave WaveType; public readonly double NoiseMix;
            public readonly double DecayRate;
            public Tone(double d, double f, Wave w, double noise)
            { Duration = d; Freq = f; WaveType = w; NoiseMix = noise; DecayRate = 18.0; }
            public override string ToString() => $"{WaveType}:{Freq}";
        }

        private static Tone T(double d, double f, Wave w, double noise) => new Tone(d, f, w, noise);

        private static double Oscillate(Wave wave, double freq, double t)
        {
            double phase = (freq * t) % 1.0;
            return wave switch
            {
                Wave.Sine => Math.Sin(2 * Math.PI * freq * t),
                Wave.Square => phase < 0.5 ? 1.0 : -1.0,
                Wave.Sawtooth => 2.0 * phase - 1.0,
                _ => 0.0
            };
        }

        private static byte[] ToWavBytes(float[] samples)
        {
            using var mem = new MemoryStream();
            using var bw = new BinaryWriter(mem);
            int dataLen = samples.Length * 2;
            bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(36 + dataLen);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16);                 // PCM chunk size
            bw.Write((short)1);           // PCM format
            bw.Write((short)1);           // mono
            bw.Write(SampleRate);
            bw.Write(SampleRate * 2);     // byte rate
            bw.Write((short)2);           // block align
            bw.Write((short)16);          // bits per sample
            bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            bw.Write(dataLen);
            foreach (var s in samples)
                bw.Write((short)(Math.Clamp(s, -1f, 1f) * short.MaxValue));
            bw.Flush();
            return mem.ToArray();
        }
    }
}
