namespace EpicBattle.Models.Map
{
    public enum MapNodeType
    {
        NormalBattle, // ⚔️ Обычный бой
        EliteBattle,  // 💀 Элитный бой
        Encounter,    // 📜 Случайное событие
        Merchant,     // 💰 Торговец
        Campfire,     // ⛺ Привал / Лагерь
        Boss          // 👑 Босс
    }
}
