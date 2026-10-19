namespace EpicBattle.Models.Combat
{
    /// <summary>
    /// P2: семантический тип намерения врага. Заменяет магические строки вида
    /// intent.Description.Contains("Ослепление") — переименование текста больше
    /// не ломает AI-обработку. Description остаётся чисто визуальным.
    /// </summary>
    public enum IntentKind
    {
        None,
        Attack,          // обычная атака
        HeavyAttack,     // усиленная атака (x1.5)
        Defend,
        RageCast,        // впасть в ярость
        BlindCast,       // ослепление (Тактик)
        CleaveCast,      // рассекающий удар по площади
        FireballCast,    // огненный шар (магия/AoE)
        Heal             // самолечение (Первая помощь)
    }
}
