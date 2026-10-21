using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using EpicBattle.Managers;

namespace EpicBattle.Services
{
    /// <summary>
    /// Профессиональный аудио-движок игры «Железный Кряж».
    /// Поддерживает:
    /// 1. Воспроизведение фоновой музыки (.mp3) с зацикливанием и регулировкой громкости в реальном времени.
    /// 2. Полифонический пул звуковых эффектов (SFX, .wav) с независимой громкостью.
    /// 3. Встроенная защита от наложения дублирующихся звуков (debounce).
    /// 4. Избирательный парсинг боевого лога для действий врагов (исключает двойные звуки на ход игрока).
    /// </summary>
    public static class AudioEngine
    {
        private static readonly string[] SearchRoots = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds"),
            Path.Combine(Directory.GetCurrentDirectory(), "Sounds"),
            @"C:\Users\admin\Desktop\The Iron Hills\Sounds",
            @"C:\Users\admin\Desktop\sound"
        };

        // ===== ФОНОВАЯ МУЗЫКА =====
        private static MediaPlayer? _musicPlayer;
        private static string? _currentMusicTrack;

        // ===== ПУЛ ДЛЯ SFX =====
        private const int SfxPoolSize = 10;
        private static readonly MediaPlayer[] _sfxPlayers = new MediaPlayer[SfxPoolSize];
        private static int _nextSfxIndex = 0;
        private static bool _initialized = false;

        private static readonly Random _rng = new Random();

        // Кэш проверенных путей к файлам
        private static readonly Dictionary<string, string> _resolvedPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Защита от дублирования звуков (debounce)
        private static readonly Dictionary<string, DateTime> _lastPlayTimes = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private static DateTime _lastAnyClickTime = DateTime.MinValue;

        static AudioEngine()
        {
            EnsureInitialized();
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            RunOnUi(() =>
            {
                try
                {
                    _musicPlayer = new MediaPlayer();
                    _musicPlayer.MediaEnded += (s, e) =>
                    {
                        if (_musicPlayer != null)
                        {
                            _musicPlayer.Position = TimeSpan.Zero;
                            _musicPlayer.Play();
                        }
                    };

                    for (int i = 0; i < SfxPoolSize; i++)
                    {
                        _sfxPlayers[i] = new MediaPlayer();
                    }

                    _initialized = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AudioEngine init error: {ex.Message}");
                }
            });
        }

        private static void RunOnUi(Action action)
        {
            var app = Application.Current;
            if (app?.Dispatcher != null)
            {
                if (app.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    app.Dispatcher.InvokeAsync(action);
                }
            }
            else
            {
                action();
            }
        }

        // ==========================================
        // === УПРАВЛЕНИЕ МУЗЫКОЙ ===
        // ==========================================

        public static void PlayMusic(string trackName)
        {
            if (string.IsNullOrWhiteSpace(trackName)) return;
            if (_currentMusicTrack == trackName) return; // Уже играет этот трек

            _currentMusicTrack = trackName;

            RunOnUi(() =>
            {
                try
                {
                    EnsureInitialized();
                    if (_musicPlayer == null) return;

                    double vol = SaveManager.Settings?.MusicVolume ?? 0.3;
                    _musicPlayer.Volume = Math.Clamp(vol, 0.0, 1.0);

                    string fileName = trackName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) 
                        ? trackName 
                        : $"{trackName}.mp3";

                    string? path = ResolveAssetPath("Music", fileName);
                    if (path != null && File.Exists(path))
                    {
                        _musicPlayer.Open(new Uri(path, UriKind.Absolute));
                        _musicPlayer.Play();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PlayMusic error ({trackName}): {ex.Message}");
                }
            });
        }

        public static void StopMusic()
        {
            _currentMusicTrack = null;
            RunOnUi(() =>
            {
                try
                {
                    _musicPlayer?.Stop();
                }
                catch { }
            });
        }

        public static void UpdateMusicVolume()
        {
            RunOnUi(() =>
            {
                try
                {
                    if (_musicPlayer != null)
                    {
                        double vol = SaveManager.Settings?.MusicVolume ?? 0.3;
                        _musicPlayer.Volume = Math.Clamp(vol, 0.0, 1.0);
                    }
                }
                catch { }
            });
        }

        // ==========================================
        // === УПРАВЛЕНИЕ ЗВУКОВЫМИ ЭФФЕКТАМИ (SFX) ===
        // ==========================================

        public static void Play(string effect)
        {
            if (string.IsNullOrWhiteSpace(effect)) return;

            DateTime now = DateTime.UtcNow;

            // Защита от дублирования одного и того же звука в пределах 75 мс
            if (_lastPlayTimes.TryGetValue(effect, out var lastPlay) && (now - lastPlay).TotalMilliseconds < 75)
            {
                return;
            }

            // Защита от быстрого клик-спама в пределах 50 мс
            if (effect.StartsWith("click", StringComparison.OrdinalIgnoreCase))
            {
                if ((now - _lastAnyClickTime).TotalMilliseconds < 50) return;
                _lastAnyClickTime = now;
            }

            _lastPlayTimes[effect] = now;

            RunOnUi(() =>
            {
                try
                {
                    EnsureInitialized();
                    double vol = SaveManager.Settings?.SfxVolume ?? 0.8;
                    if (vol <= 0.005) return;

                    string? filePath = GetSfxFilePath(effect);
                    if (filePath != null && File.Exists(filePath))
                    {
                        var player = _sfxPlayers[_nextSfxIndex];
                        _nextSfxIndex = (_nextSfxIndex + 1) % SfxPoolSize;

                        player.Stop();
                        player.Volume = Math.Clamp(vol, 0.0, 1.0);
                        player.Open(new Uri(filePath, UriKind.Absolute));
                        player.Play();
                    }
                    else
                    {
                        // Fallback: процедурный генератор
                        PlayProceduralTone(effect, vol);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Play SFX error ({effect}): {ex.Message}");
                }
            });
        }

        private static string? GetSfxFilePath(string effect)
        {
            string clean = effect.Trim().ToLowerInvariant();

            return clean switch
            {
                // UI
                "click" => ResolveAssetPath("SFX/UI", "click.wav"),
                "click_alt" => ResolveAssetPath("SFX/UI", "click_alt.wav") ?? ResolveAssetPath("SFX/UI", "click.wav"),
                "chest_open" => ResolveAssetPath("SFX/UI", "chest_open.wav"),
                "chest_close" => ResolveAssetPath("SFX/UI", "chest_close.wav"),
                "gate_open" => ResolveAssetPath("SFX/UI", "gate_open.wav"),
                "gate_close" => ResolveAssetPath("SFX/UI", "gate_close.wav"),
                "door_wood" => ResolveAssetPath("SFX/UI", "door_wood.wav"),
                "door_close" => ResolveAssetPath("SFX/UI", "door_close.wav"),
                "footsteps" => PickOne("SFX/UI/footsteps.wav", "SFX/UI/footsteps_gravel.wav"),
                "footsteps_gravel" => ResolveAssetPath("SFX/UI", "footsteps_gravel.wav"),

                // Оружие и атаки
                "hit" or "hit_sword" or "sword_hit" => PickOne("SFX/Combat/sword_hit1.wav", "SFX/Combat/sword_hit2.wav", "SFX/Combat/sword_hit3.wav"),
                "sword_attack" => PickOne("SFX/Combat/sword_attack1.wav", "SFX/Combat/sword_attack2.wav", "SFX/Combat/sword_attack3.wav"),
                "sword_unsheath" => ResolveAssetPath("SFX/Combat", "sword_unsheath.wav"),
                "sword_sheath" => ResolveAssetPath("SFX/Combat", "sword_sheath.wav"),
                "defend" or "sword_block" => PickOne("SFX/Combat/sword_block1.wav", "SFX/Combat/sword_block2.wav"),
                "sword_parry" => PickOne("SFX/Combat/sword_parry1.wav", "SFX/Combat/sword_parry2.wav"),
                "hit_dagger" => ResolveAssetPath("SFX/Combat", "sword_attack3.wav"),
                "hit_axe" => PickOne("SFX/Combat/axe_hit1.wav", "SFX/Combat/axe_hit2.wav"),
                "bow_attack" => ResolveAssetPath("SFX/Combat", "bow_attack.wav"),
                "bow_hit" => ResolveAssetPath("SFX/Combat", "bow_hit.wav"),
                "bow_blocked" => ResolveAssetPath("SFX/Combat", "bow_blocked.wav"),

                // Магия и способности
                "magic" or "fireball" => PickOne("SFX/Combat/fireball.wav", "SFX/Combat/fireball2.wav"),
                "firebuff" => ResolveAssetPath("SFX/Combat", "firebuff.wav"),
                "firespray" => ResolveAssetPath("SFX/Combat", "firespray.wav"),
                "ice_throw" => ResolveAssetPath("SFX/Combat", "ice_throw.wav"),
                "ice_freeze" => ResolveAssetPath("SFX/Combat", "ice_freeze.wav"),
                "ice_barrage" => ResolveAssetPath("SFX/Combat", "ice_barrage.wav"),
                "ice_wall" => ResolveAssetPath("SFX/Combat", "ice_wall.wav"),
                "hit_staff" or "spell_impact" => PickOne("SFX/Combat/spell_impact1.wav", "SFX/Combat/spell_impact2.wav"),
                "rock_throw" => ResolveAssetPath("SFX/Combat", "rock_throw.wav"),
                "rock_swarm" => ResolveAssetPath("SFX/Combat", "rock_swarm.wav"),
                "rock_wall" => ResolveAssetPath("SFX/Combat", "rock_wall.wav"),
                "wave_attack" => ResolveAssetPath("SFX/Combat", "wave_attack.wav"),
                "heal" or "waterspray" => ResolveAssetPath("SFX/Combat", "waterspray.wav"),

                // Боевые исходы
                "crit" => ResolveAssetPath("SFX/Combat", "spell_impact1.wav"),
                "miss" => ResolveAssetPath("SFX/Combat", "sword_attack1.wav"),
                "victory" => ResolveAssetPath("SFX/UI", "gate_open.wav"),
                "death" => ResolveAssetPath("SFX/UI", "gate_close.wav"),

                // Мир
                "hammer" => PickOne("SFX/World/hammer1.wav", "SFX/World/hammer2.wav", "SFX/World/hammer3.wav"),
                "torch" => ResolveAssetPath("SFX/World", "torch.wav"),

                _ => ResolveAssetPath("SFX", $"{clean}.wav")
            };
        }

        private static string? PickOne(params string[] relativePaths)
        {
            if (relativePaths == null || relativePaths.Length == 0) return null;
            string selected = relativePaths[_rng.Next(relativePaths.Length)];
            string[] parts = selected.Split('/');
            if (parts.Length == 3)
            {
                return ResolveAssetPath($"{parts[0]}/{parts[1]}", parts[2]);
            }
            return ResolveAssetPath(parts[0], parts[1]);
        }

        private static string? ResolveAssetPath(string subFolder, string fileName)
        {
            string key = $"{subFolder}/{fileName}";
            if (_resolvedPaths.TryGetValue(key, out var cached) && File.Exists(cached))
            {
                return cached;
            }

            foreach (var root in SearchRoots)
            {
                if (!Directory.Exists(root)) continue;

                // 1. Полноценный путь: root/subFolder/fileName
                string p1 = Path.Combine(root, subFolder.Replace('/', Path.DirectorySeparatorChar), fileName);
                if (File.Exists(p1))
                {
                    _resolvedPaths[key] = p1;
                    return p1;
                }

                // 2. Без подпапки: root/fileName
                string p2 = Path.Combine(root, fileName);
                if (File.Exists(p2))
                {
                    _resolvedPaths[key] = p2;
                    return p2;
                }
            }

            return null;
        }

        // ==========================================
        // === ЛОГИКА БОЕВОГО ЛОГА ДЛЯ ВРАГОВ ===
        // ==========================================

        // Только атаки противников (действие игрока уже озвучивается по кнопке действия!)
        private static readonly Regex EnemyAttackLine = new Regex(@"⚔️\s+(?!Вы\b).*атакует|атакует и наносит", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex EnemyDefendLine = new Regex(@"встает в глухую защиту", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex MissLine = new Regex(@"уклоняется|промахивается", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Озвучивание событий хода противников из боевого лога.
        /// Не дублирует действия игрока (атаки, навыки, зелья и победу), которые озвучиваются напрямую кнопками.
        /// </summary>
        public static void PlayForLogLine(string logMessage)
        {
            if (string.IsNullOrEmpty(logMessage)) return;

            // Пропускаем строки с действиями игрока, чтобы не было двойного звука
            if (logMessage.StartsWith("⚔️ Вы атакуете", StringComparison.OrdinalIgnoreCase) ||
                logMessage.StartsWith("🔮 Вы атакуете", StringComparison.OrdinalIgnoreCase) ||
                logMessage.StartsWith("🛡️ Вы приготовились", StringComparison.OrdinalIgnoreCase) ||
                logMessage.StartsWith("🧪 Вы", StringComparison.OrdinalIgnoreCase) ||
                logMessage.StartsWith("🏆 Вы", StringComparison.OrdinalIgnoreCase) ||
                logMessage.StartsWith("💀 Вы", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (MissLine.IsMatch(logMessage)) { Play("miss"); return; }
            if (EnemyDefendLine.IsMatch(logMessage)) { Play("defend"); return; }
            if (EnemyAttackLine.IsMatch(logMessage)) { Play("hit"); return; }
        }

        public static void PlayWeaponHit(string? weaponName)
        {
            string w = (weaponName ?? "").ToLowerInvariant();
            if (w.Contains("посох") || w.Contains("жезл") || w.Contains("staff")) Play("hit_staff");
            else if (w.Contains("лук") || w.Contains("арбалет") || w.Contains("bow")) Play("bow_attack");
            else if (w.Contains("кинжал") || w.Contains("катана") || w.Contains("daggers")) Play("hit_dagger");
            else if (w.Contains("топор") || w.Contains("молот") || w.Contains("axe") || w.Contains("hammer")) Play("hit_axe");
            else Play("hit_sword");
        }

        // ==========================================
        // === РЕЗЕРВНЫЙ ПРОЦЕДУРНЫЙ СИНТЕЗАТОР ===
        // ==========================================

        private static readonly string FallbackDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EpicBattle", "Sounds");

        private static void PlayProceduralTone(string effect, double volume)
        {
            try
            {
                Directory.CreateDirectory(FallbackDir);
                string file = Path.Combine(FallbackDir, $"{effect}_{(int)(volume * 100)}.wav");
                if (!File.Exists(file))
                {
                    byte[] wav = GenerateFallbackWav(effect, volume);
                    File.WriteAllBytes(file, wav);
                }

                var player = _sfxPlayers[_nextSfxIndex];
                _nextSfxIndex = (_nextSfxIndex + 1) % SfxPoolSize;
                player.Stop();
                player.Volume = volume;
                player.Open(new Uri(file, UriKind.Absolute));
                player.Play();
            }
            catch { }
        }

        private const int SampleRate = 22050;

        private static byte[] GenerateFallbackWav(string effect, double volume)
        {
            var ms = effect switch
            {
                "hit" or "hit_sword" => new[] { T(0.07, 420, Wave.Sawtooth, 0.35) },
                "hit_staff"          => new[] { T(0.12, 300, Wave.Sine, 0.0) },
                "hit_dagger"         => new[] { T(0.04, 900, Wave.Sawtooth, 0.4) },
                "hit_axe"            => new[] { T(0.09, 120, Wave.Square, 0.45) },
                "crit"               => new[] { T(0.05, 260, Wave.Square, 0.3), T(0.10, 520, Wave.Sawtooth, 0.2) },
                "magic"              => new[] { T(0.14, 700, Wave.Sine, 0.0) },
                "heal"               => new[] { T(0.10, 523, Wave.Sine, 0.0), T(0.14, 784, Wave.Sine, 0.0) },
                "miss"               => new[] { T(0.08, 500, Wave.Sine, 0.6) },
                "defend"             => new[] { T(0.06, 150, Wave.Square, 0.15) },
                "death"              => new[] { T(0.18, 140, Wave.Sawtooth, 0.2) },
                "victory"            => new[] { T(0.12, 523, Wave.Square, 0.0), T(0.20, 784, Wave.Square, 0.0) },
                "click"              => new[] { T(0.03, 800, Wave.Sine, 0.1) },
                _                    => new[] { T(0.05, 440, Wave.Sine, 0.1) },
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
                    double env = Math.Exp(-tt * tone.DecayRate);
                    double v = tone.WaveType == Wave.Sine ? Math.Sin(2 * Math.PI * tone.Freq * tt) : (Math.Sin(2 * Math.PI * tone.Freq * tt) >= 0 ? 1.0 : -1.0);
                    samples[pos++] = (float)(v * env * volume * 0.5);
                }
            }

            using var mem = new MemoryStream();
            using var bw = new BinaryWriter(mem);
            int dataLen = samples.Length * 2;
            bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(36 + dataLen);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(SampleRate);
            bw.Write(SampleRate * 2);
            bw.Write((short)2);
            bw.Write((short)16);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            bw.Write(dataLen);
            foreach (var s in samples)
                bw.Write((short)(Math.Clamp(s, -1f, 1f) * short.MaxValue));
            bw.Flush();
            return mem.ToArray();
        }

        private enum Wave { Sine, Square, Sawtooth }

        private readonly struct Tone
        {
            public readonly double Duration; public readonly double Freq;
            public readonly Wave WaveType; public readonly double NoiseMix;
            public readonly double DecayRate;
            public Tone(double d, double f, Wave w, double noise)
            { Duration = d; Freq = f; WaveType = w; NoiseMix = noise; DecayRate = 18.0; }
        }

        private static Tone T(double d, double f, Wave w, double noise) => new Tone(d, f, w, noise);
    }
}
