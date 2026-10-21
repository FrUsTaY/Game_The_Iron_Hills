using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using EpicBattle.Managers;
using EpicBattle.Services;

namespace EpicBattle.Views
{
    public partial class IntroVideoView : UserControl
    {
        private readonly Action _onFinished;
        private readonly List<string> _playlist = new();
        private int _currentIndex = -1;
        private bool _isFinishing = false;
        private DispatcherTimer? _hideHintTimer;

        public IntroVideoView(Action? onFinished = null, bool playOnlyStory = false)
        {
            InitializeComponent();
            _onFinished = onFinished ?? (() => NavigationManager.NavigateTo(new MainMenu()));

            // Остановка фоновой музыки, чтобы не конфликтовать со звуковой дорожкой видео
            AudioEngine.StopMusic();

            // Формируем плейлист
            if (playOnlyStory)
            {
                var storyPath = VideoService.ResolveVideoPath("intro_story.mp4");
                if (storyPath != null) _playlist.Add(storyPath);
            }
            else
            {
                var logoPath = VideoService.ResolveVideoPath("intro_logo.mp4");
                var storyPath = VideoService.ResolveVideoPath("intro_story.mp4");

                if (logoPath != null) _playlist.Add(logoPath);
                if (storyPath != null) _playlist.Add(storyPath);
            }

            // Установка громкости из настроек игры
            double volume = SaveManager.Settings?.SfxVolume ?? 0.85;
            VideoPlayer.Volume = Math.Clamp(volume, 0.0, 1.0);

            Loaded += IntroVideoView_Loaded;
            Unloaded += IntroVideoView_Unloaded;
            KeyDown += IntroVideoView_KeyDown;

            InitHideHintTimer();
        }

        public IntroVideoView() : this(null, false)
        {
        }

        private void InitHideHintTimer()
        {
            _hideHintTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _hideHintTimer.Tick += (s, e) =>
            {
                _hideHintTimer.Stop();
                AnimateSkipPanel(0.2);
            };
        }

        private void IntroVideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
            Keyboard.Focus(this);

            if (_playlist.Count == 0)
            {
                Finish();
                return;
            }

            PlayNext();
        }

        private void IntroVideoView_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanupPlayer();
        }

        private void PlayNext()
        {
            if (_isFinishing) return;

            _currentIndex++;
            if (_currentIndex >= _playlist.Count)
            {
                Finish();
                return;
            }

            string videoPath = _playlist[_currentIndex];

            // Затемняем экран перед переключением видео
            AnimateFade(1.0, 200, () =>
            {
                try
                {
                    VideoPlayer.Stop();
                    VideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
                    VideoPlayer.Play();

                    // Обновляем подсказку в зависимости от оставшихся роликов
                    bool hasMore = _currentIndex < _playlist.Count - 1;
                    SkipHintText.Text = hasMore
                        ? "⏭️ Пробел / Клик — далее  •  Esc — в меню"
                        : "⏭️ Пробел / Клик / Esc — в главное меню";

                    _hideHintTimer?.Stop();
                    AnimateSkipPanel(0.85);
                    _hideHintTimer?.Start();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[IntroVideoView] Play error: {ex.Message}");
                    PlayNext();
                }
            });
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Плавное проявление видео из черного экрана
            AnimateFade(0.0, 350);
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        private void VideoPlayer_MediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[IntroVideoView] MediaFailed: {e.ErrorException?.Message}");
            PlayNext();
        }

        private void IntroVideoView_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isFinishing) return;

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Finish(); // Пропустить всё сразу в меню
            }
            else if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                e.Handled = true;
                PlayNext(); // Пропустить текущее видео
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                PlayNext();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            AnimateSkipPanel(0.85);
            _hideHintTimer?.Stop();
            _hideHintTimer?.Start();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            PlayNext();
        }

        private void Finish()
        {
            if (_isFinishing) return;
            _isFinishing = true;

            _hideHintTimer?.Stop();

            AnimateFade(1.0, 250, () =>
            {
                CleanupPlayer();
                _onFinished.Invoke();
            });
        }

        private void CleanupPlayer()
        {
            try
            {
                VideoPlayer.Stop();
                VideoPlayer.Close();
                VideoPlayer.Source = null;
            }
            catch
            {
                // Игнорируем ошибки при освобождении
            }
        }

        private void AnimateFade(double targetOpacity, int durationMs, Action? onCompleted = null)
        {
            var anim = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                FillBehavior = FillBehavior.HoldEnd
            };

            if (onCompleted != null)
            {
                anim.Completed += (s, e) => onCompleted.Invoke();
            }

            FadeOverlay.BeginAnimation(OpacityProperty, anim);
        }

        private void AnimateSkipPanel(double targetOpacity)
        {
            var anim = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = TimeSpan.FromMilliseconds(200),
                FillBehavior = FillBehavior.HoldEnd
            };
            SkipPanel.BeginAnimation(OpacityProperty, anim);
        }
    }
}
