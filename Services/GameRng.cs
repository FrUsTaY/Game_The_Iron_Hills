using System;

namespace EpicBattle.Services
{
    /// <summary>
    /// P2: единый генератор случайных чисел для всей игры.
    /// Заменяет 5+ независимых static Random по классам.
    /// Поддерживает явный seed для воспроизводимости боёв/карт при отладке:
    /// GameRng.Reseed(12345) — и вся последовательность событий детерминирована.
    /// </summary>
    public static class GameRng
    {
        [ThreadStatic] private static Random? _local;

        private static Random Current => _local ??= new Random(Environment.TickCount);

        /// <summary>Текущий seed, использованный при последнем Reseed (0 если авто).</summary>
        public static int CurrentSeed { get; private set; }

        /// <summary>Явно задать seed для воспроизводимой сессии (отладка/спидраны).</summary>
        public static void Reseed(int seed)
        {
            CurrentSeed = seed;
            _local = new Random(seed);
        }

        public static int Next(int maxValue) => Current.Next(maxValue);
        public static int Next(int minValue, int maxValue) => Current.Next(minValue, maxValue);
        public static double NextDouble() => Current.NextDouble();
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Current.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}

namespace EpicBattle.Models.Combat
{
    /// <summary>
    /// P2: совместимая прокси-обёртка Random -> GameRng, чтобы не менять сотни вызовов rand.Next(...).
    /// Все игры используют единый поток случайных чисел; класс оставлен только как shim.
    /// </summary>
    public sealed class RngProxy
    {
        public int Next() => EpicBattle.Services.GameRng.Next(int.MaxValue);
        public int Next(int maxValue) => EpicBattle.Services.GameRng.Next(maxValue);
        public int Next(int minValue, int maxValue) => EpicBattle.Services.GameRng.Next(minValue, maxValue);
        public double NextDouble() => EpicBattle.Services.GameRng.NextDouble();
    }
}
