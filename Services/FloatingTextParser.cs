using System;
using System.Text.RegularExpressions;
using EpicBattle.Models.Combat;

namespace EpicBattle.Services
{
    /// <summary>
    /// Этап 3.3: парсер строк боевого лога в всплывающие цифры (Floating Combat Text).
    /// Лог остаётся единым источником правды — FX извлекаются из него регулярками,
    /// что даёт совместимость со ВСЕМИ существующими и будущими сообщениями движка.
    /// </summary>
    public static class FloatingTextParser
    {
        // "⚔️ Вы атакуете {target} и наносите {damage} урона." / "⚔️ {enemy} атакует и наносит {damage} урона."
        private static readonly Regex AttackPlayerTarget = new Regex(@"Вы атакуете (?<t>.+?) и наносите (?<d>\d+) урона", RegexOptions.Compiled);
        private static readonly Regex AttackEnemySource = new Regex(@"(?<s>.+?) атакует и наносит (?<d>\d+) урона", RegexOptions.Compiled);

        // Skill damage: "... использует '{skill}' и наносит {dmg} урона {target}."
        private static readonly Regex SkillOnTarget = new Regex(@"использует '.+?' и наносит (?<d>\d+) урона (?<t>.+?)\.", RegexOptions.Compiled);
        // Cleave main: "{caster} использует '{skill}'! {target} получает {dmg} урона и кровотечение."
        private static readonly Regex SkillMainTarget = new Regex(@"'.+?'! (?<t>.+?) получает (?<d>\d+) урона и кровотечение", RegexOptions.Compiled);
        // Cleave adjacent / generic: "{name} получает {dmg} урона ..."
        private static readonly Regex GenericGetsDamage = new Regex(@"^(?:💥|⚔️|🔥|🩸|🤢)? ?(?<t>.+?) получает (?<d>\d+)(?: маг\.| магического)? урона", RegexOptions.Compiled);
        // Magic hit on player target list: "{t} получает {dmg} маг. урона."
        // DoT: "{entity} получает {damage} урона от кровотечения/яда" | "...магического урона от горения"
        private static readonly Regex DotSource = new Regex(@"(?<t>.+?) получает (?<d>\d+) (?:магического )?урона от \S+", RegexOptions.Compiled);

        // Heal: "... восстанавливает {heal} HP ..."
        private static readonly Regex HealRegex = new Regex(@"(?<t>.+?) восстанавливает (?<d>\d+) HP", RegexOptions.Compiled);
        // Enemy heals ally: "{enemy} исцеляет {ally} на {heal} HP."
        private static readonly Regex HealAllyRegex = new Regex(@"исцеляет (?<t>.+?) на (?<d>\d+) HP", RegexOptions.Compiled);
        // Player self-heal skill: "{caster} применяет Первую помощь и восстанавливает {heal} HP."
        private static readonly Regex SelfHealRegex = new Regex(@"^🌿 (?<t>.+?) применяет .*восстанавливает (?<d>\d+) HP", RegexOptions.Compiled);

        // Misses
        private static readonly Regex BlindMiss = new Regex(@"промахивается из-за слепоты", RegexOptions.Compiled);
        private static readonly Regex Dodge = new Regex(@"(?<t>.+?) ловко уклоняется", RegexOptions.Compiled);

        // Crit marker line: "🎯 КРИТИЧЕСКИЙ УДАР ({source})!"
        private static readonly Regex CritLine = new Regex(@"КРИТИЧЕСКИЙ УДАР \((?<s>.+?)\)", RegexOptions.Compiled);

        // Mana shield absorb: "🛡️ Щит маны поглощает {n} урона..."
        private static readonly Regex ShieldAbsorb = new Regex(@"Щит маны поглощает (?<d>\d+) урона", RegexOptions.Compiled);

        public static FloatingText Parse(string msg, string playerKey)
        {
            if (string.IsNullOrEmpty(msg)) return null;

            Match m;

            if ((m = CritLine.Match(msg)).Success)
                return null; // крит показывается на строке удара ниже (флаг isCritLast)

            if (msg.Contains("КРИТИЧЕСКИЙ УДАР")) return null;

            if ((m = AttackPlayerTarget.Match(msg)).Success)
                return Make(m.Groups["d"].Value, FloatingTextKind.Damage, m.Groups["t"].Value, playerKey);

            if ((m = AttackEnemySource.Match(msg)).Success)
            {
                int dmg = int.Parse(m.Groups["d"].Value);
                return new FloatingText($"-{dmg}", FloatingTextKind.Damage, playerKey, triggersShake: dmg >= 25);
            }

            if ((m = HealAllyRegex.Match(msg)).Success)
                return Make(m.Groups["d"].Value, FloatingTextKind.Heal, m.Groups["t"].Value, playerKey, prefix: "+");

            if ((m = SelfHealRegex.Match(msg)).Success)
                return Make(m.Groups["d"].Value, FloatingTextKind.Heal, m.Groups["t"].Value, playerKey, prefix: "+");

            if ((m = HealRegex.Match(msg)).Success && !msg.Contains("MP"))
                return Make(m.Groups["d"].Value, FloatingTextKind.Heal, m.Groups["t"].Value, playerKey, prefix: "+");

            if ((m = SkillOnTarget.Match(msg)).Success)
                return Make(m.Groups["d"].Value, FloatingTextKind.Damage, m.Groups["t"].Value, playerKey);

            if ((m = SkillMainTarget.Match(msg)).Success)
                return Make(m.Groups["d"].Value, FloatingTextKind.Damage, m.Groups["t"].Value, playerKey);

            if (BlindMiss.IsMatch(msg))
            {
                var src = Regex.Match(msg, @"👁️ (?<s>.+?) промахивается");
                string key = src.Success && src.Groups["s"].Value.Trim() != "Вы" ? src.Groups["s"].Value : playerKey;
                return new FloatingText("ПРОМАХ 👁", FloatingTextKind.Miss, key);
            }

            if ((m = Dodge.Match(msg)).Success)
                return new FloatingText("УКЛОН 💨", FloatingTextKind.Miss, m.Groups["t"].Value == "вы" ? playerKey : m.Groups["t"].Value);

            if ((m = ShieldAbsorb.Match(msg)).Success)
                return new FloatingText($"🛡 -{m.Groups["d"].Value}", FloatingTextKind.Magic, playerKey);

            if ((m = DotSource.Match(msg)).Success)
            {
                bool magic = msg.Contains("магического") || msg.Contains("маг.");
                return Make(m.Groups["d"].Value, magic ? FloatingTextKind.Magic : FloatingTextKind.Damage,
                            m.Groups["t"].Value, playerKey, negative: true);
            }

            if ((m = GenericGetsDamage.Match(msg)).Success)
                return Make(m.Groups["d"].Value, msg.Contains("маг.") ? FloatingTextKind.Magic : FloatingTextKind.Damage,
                            m.Groups["t"].Value, playerKey, negative: true);

            if (msg.StartsWith("💀") && msg.Contains("погиб"))
            {
                var name = Regex.Match(msg, @"💀 (?<t>.+?) повержен");
                if (name.Success) return new FloatingText("☠️", FloatingTextKind.Info, name.Groups["t"].Value);
            }

            return null;
        }

        /// <summary>Пометка крита для следующего числа (движок пишет crit-строку перед итоговой строкой удара).</summary>
        public static bool IsCritMarker(string msg) => msg != null && msg.Contains("КРИТИЧЕСКИЙ УДАР");

        private static FloatingText Make(string dmgStr, FloatingTextKind kind, string rawName, string playerKey,
                                         string prefix = null, bool negative = false)
        {
            string name = NormalizeKey(rawName, playerKey);
            string text = prefix != null ? prefix + dmgStr : (negative ? "-" + dmgStr : dmgStr);
            bool shake = kind != FloatingTextKind.Heal && name == playerKey && int.TryParse(dmgStr, out int d) && d >= 25;
            var ft = new FloatingText(text, kind == FloatingTextKind.Damage && shake ? FloatingTextKind.Damage : kind, name, triggersShake: shake);
            return ft;
        }

        private static string NormalizeKey(string rawName, string playerKey)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return playerKey;
            string n = rawName.Trim().TrimEnd('.');
            if (n.Equals("вы", StringComparison.OrdinalIgnoreCase) || n.Equals("вас", StringComparison.OrdinalIgnoreCase) || n == "Вы")
                return playerKey;
            return n;
        }
    }
}
