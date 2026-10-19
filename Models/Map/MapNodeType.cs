namespace EpicBattle.Models.Map
{
    public enum MapNodeType
    {
        NormalBattle, // ⚔️ Обычный бой
        EliteBattle,  // 💀 Элитный бой
        Encounter,    // 📜 Случайное событие
        Merchant,     // 💰 Торговец
        Campfire,     // ⛺ Привал / Лагерь
        Blacksmith,   // 🔨 Кузница (Этап 3.2: улучшение экипировки)
        Tavern,       // 🍺 Таверна (Этап 3.2: отдых за золото, слухи, продажа рун)
        Boss          // 👑 Босс
    }
}
