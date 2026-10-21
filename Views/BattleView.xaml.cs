using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using EpicBattle.Models.Combat;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class BattleView : UserControl
    {
        // Этап 3.3: FX-слой — всплывающие цифры и тряска экрана
        private Canvas _fxLayer;
        private readonly DispatcherTimer _fxCleanupTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        private TranslateTransform _shakeTransform;
        private readonly Random _fxRng = new Random();

        public BattleView(bool isArcade = true, string returnNodeId = null, string battleModifier = null, int arcadeLevel = 1, Action onCompletedFromMap = null)
        {
            InitializeComponent();
            if (battleModifier == "Boss" || battleModifier == "Elite")
            {
                EpicBattle.Services.AudioEngine.PlayMusic("BattleBoss");
            }
            else
            {
                EpicBattle.Services.AudioEngine.PlayMusic("BattleNormal");
            }
            var vm = new BattleViewModel(isArcade, returnNodeId, battleModifier, arcadeLevel, onCompletedFromMap);

            vm.RequestOpenSkillTree = () =>
            {
                EpicBattle.Services.AudioEngine.Play("spell_impact");
                SkillTreeView skillTreeView = null;
                skillTreeView = new SkillTreeView(() =>
                {
                    if (skillTreeView != null)
                    {
                        MainGrid.Children.Remove(skillTreeView);
                        vm.UpdateSkillPointsDisplay();
                    }
                });

                Grid.SetRowSpan(skillTreeView, 5);
                Panel.SetZIndex(skillTreeView, 200);
                MainGrid.Children.Add(skillTreeView);
            };

            vm.RequestOpenInventory = () =>
            {
                EpicBattle.Services.AudioEngine.Play("chest_open");
                Managers.NavigationManager.NavigateTo(new InventoryView(() =>
                {
                    Managers.NavigationManager.NavigateTo(this);
                    vm.UpdateStatsDisplay();
                }));
            };

            vm.CombatLog.CollectionChanged += (s, e) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    LogScroller?.ScrollToEnd();
                    FullLogScroller?.ScrollToEnd();
                });
            };

            // ===== Этап 3.3: Floating Combat Text + тряска экрана =====
            _fxLayer = new Canvas
            {
                IsHitTestVisible = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Grid.SetRow(_fxLayer, 1);
            Grid.SetRowSpan(_fxLayer, 2);
            Panel.SetZIndex(_fxLayer, 150);
            MainGrid.Children.Add(_fxLayer);

            _shakeTransform = new TranslateTransform(0, 0);
            MainGrid.RenderTransform = _shakeTransform;

            vm.FloatingTexts.CollectionChanged += FloatingTexts_CollectionChanged;

            // Этап 3.3: вспышка карточки врага при получении им урона (подсветка бордера)
            vm.EnemyHitFlashRequested += enemyName =>
            {
                var card = FindEnemyCard(enemyName);
                if (card is ContentControl cc && cc.Content is Border b) FlashBorder(b);
            };

            _fxCleanupTimer.Tick += (s, e) => CleanupFxLayer();
            _fxCleanupTimer.Start();
            this.Loaded += (s, e) => this.Focus();
            this.Unloaded += (s, e) => _fxCleanupTimer.Stop();

            DataContext = vm;
        }

        private void FloatingTexts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null) return;
            foreach (FloatingText ft in e.NewItems)
                SpawnFloatingText(ft);
        }

        private void SpawnFloatingText(FloatingText ft)
        {
            Point anchor = GetFloatAnchor(ft.TargetKey, ft);
            double x = anchor.X + ft.OffsetX;
            double y = anchor.Y;

            var tb = new TextBlock
            {
                Text = ft.Text,
                FontSize = ft.FontSize,
                FontWeight = ft.Kind == FloatingTextKind.Crit ? FontWeights.Black : FontWeights.Bold,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(ft.ForegroundBrush),
                Effect = new System.Windows.Media.Effects.DropShadowEffect { Color = Colors.Black, BlurRadius = 6, ShadowDepth = 1 },
                Tag = DateTime.UtcNow
            };
            if (ft.Kind == FloatingTextKind.Crit)
                tb.Text += " ✦";

            var container = new Grid();
            container.RenderTransformOrigin = new Point(0.5, 0.5);
            var scaleT = new ScaleTransform(0.4, 0.4);
            container.RenderTransform = scaleT;
            container.Children.Add(tb);

            Canvas.SetLeft(container, x);
            Canvas.SetTop(container, y);
            _fxLayer.Children.Add(container);

            // Всплытие вверх + fade out
            var move = new DoubleAnimation(y, y - 70, TimeSpan.FromMilliseconds(950))
            {
                EasingFunction = new QuadraticEaseModeWrapper().Ease()
            };
            var fade = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(950)) { BeginTime = TimeSpan.FromMilliseconds(450) };
            var popIn = new DoubleAnimation(0.4, ft.Kind == FloatingTextKind.Crit ? 1.35 : 1.1, TimeSpan.FromMilliseconds(180))
            {
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };
            var settle = new DoubleAnimation(ft.Kind == FloatingTextKind.Crit ? 1.35 : 1.1, 1.0, TimeSpan.FromMilliseconds(220))
            {
                BeginTime = TimeSpan.FromMilliseconds(180)
            };

            container.BeginAnimation(Canvas.TopProperty, move);
            container.BeginAnimation(UIElement.OpacityProperty, fade);
            scaleT.BeginAnimation(ScaleTransform.ScaleXProperty, popIn);
            scaleT.BeginAnimation(ScaleTransform.ScaleYProperty, popIn);
            scaleT.BeginAnimation(ScaleTransform.ScaleXProperty, settle);
            scaleT.BeginAnimation(ScaleTransform.ScaleYProperty, settle);

            if (ft.TriggersShake) ShakeScreen(ft.Kind == FloatingTextKind.Crit ? 14 : 9);
        }

        /// <summary>Этап 3.3: краткая вспышка рамки карточки врага при входящем уроне.</summary>
        private void FlashBorder(Border border)
        {
            var flash = new ColorAnimation
            {
                From = Color.FromRgb(0xFF, 0x6B, 0x4A),
                To = Color.FromRgb(0x33, 0x33, 0x33),
                Duration = TimeSpan.FromMilliseconds(380)
            };
            // клонируем кисть, чтобы не ломать триггер выделения цели
            if (border.BorderBrush is not SolidColorBrush)
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
            var brush = ((SolidColorBrush)border.BorderBrush).CloneCurrentValue();
            border.BorderBrush = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, flash);
        }

        /// <summary>Тряска экрана при тяжёлом ударе (Этап 3.3).</summary>
        private void ShakeScreen(double intensity)
        {
            if (_shakeTransform == null) return;
            var anim = new DoubleAnimationUsingKeyFrames();
            int steps = 6;
            for (int i = 1; i <= steps; i++)
            {
                double amp = intensity * (1.0 - (double)i / steps);
                double dx = i % 2 == 0 ? amp : -amp;
                anim.KeyFrames.Add(new EasingDoubleKeyFrame(dx, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i * 35)))
                { EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut } });
            }
            anim.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(steps * 35 + 40))));
            _shakeTransform.BeginAnimation(TranslateTransform.XProperty, anim);

            var animY = new DoubleAnimationUsingKeyFrames();
            for (int i = 1; i <= steps / 2; i++)
            {
                double dy = -intensity * 0.5 * (1.0 - (double)i / steps);
                animY.KeyFrames.Add(new EasingDoubleKeyFrame(dy, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i * 70))));
            }
            animY.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(steps * 35 + 60))));
            _shakeTransform.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        /// <summary>Якорь над колонкой игрока или нужного врага (координаты внутри MainGrid).</summary>
        private Point GetFloatAnchor(string targetKey, FloatingText ft = null)
        {
            FrameworkElement host = null;
            string heroName = (DataContext as BattleViewModel)?.Player?.Name;
            if (targetKey == "Player" || (!string.IsNullOrEmpty(heroName) && targetKey.Equals(heroName, StringComparison.OrdinalIgnoreCase)))
            {
                host = PlayerColumnHost ?? (FrameworkElement)MainGrid;
            }
            else
            {
                // найти Border врага по имени в ItemsControl Enemies
                host = FindEnemyCard(targetKey);
            }

            if (host == null || host.ActualWidth <= 0)
            {
                if (ft != null && ft.Kind == FloatingTextKind.Heal)
                    return new Point(MainGrid.ActualWidth * 0.25, 40);
                return new Point(MainGrid.ActualWidth * 0.75, 40);
            }

            try
            {
                GeneralTransform gt = host.TransformToAncestor(MainGrid);
                Point p = gt.Transform(new Point(host.ActualWidth / 2, -6));
                return p;
            }
            catch
            {
                return new Point(MainGrid.ActualWidth / 2, 40);
            }
        }

        private FrameworkElement FindEnemyCard(string enemyName)
        {
            if (EnemiesHost == null) return null;
            foreach (var obj in EnemiesHost.Items)
            {
                if (obj is BattleEnemy be && be.Name == enemyName)
                {
                    if (EnemiesHost.ItemContainerGenerator.ContainerFromItem(obj) is ContentControl cc)
                        return cc;
                }
            }
            return null; // fallback — центр
        }

        private void CleanupFxLayer()
        {
            if (_fxLayer == null) return;
            for (int i = _fxLayer.Children.Count - 1; i >= 0; i--)
            {
                if (_fxLayer.Children[i] is FrameworkElement el && el.Tag is DateTime t &&
                    (DateTime.UtcNow - t).TotalMilliseconds > 1100)
                {
                    _fxLayer.Children.RemoveAt(i);
                }
            }
        }

        // Обёртка для плавного замедления подъёма цифр
        private class QuadraticEaseModeWrapper
        {
            public IEasingFunction Ease() => new QuadraticEase { EasingMode = EasingMode.EaseOut };
        }

        private void OpenFullLog_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            FullLogOverlay.Visibility = Visibility.Visible;
            Dispatcher.InvokeAsync(() => FullLogScroller?.ScrollToEnd());
        }

        private void CloseFullLog_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            FullLogOverlay.Visibility = Visibility.Collapsed;
            this.Focus();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            PauseOverlay.Visibility = PauseOverlay.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        // Пауза и управление клавишами
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (FullLogOverlay.Visibility == Visibility.Visible)
                {
                    FullLogOverlay.Visibility = Visibility.Collapsed;
                    e.Handled = true;
                    return;
                }
                PauseOverlay.Visibility = PauseOverlay.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            PauseOverlay.Visibility = Visibility.Collapsed;
            this.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click_alt");
            Managers.NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
