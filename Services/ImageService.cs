using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace EpicBattle.Services
{
    /// <summary>
    /// Централизованный сервис загрузки и кеширования визуальных ассетов игры.
    /// Поддерживает автоматический поиск форматов .jpg, .png, .jpeg.
    /// </summary>
    public static class ImageService
    {
        private static readonly Dictionary<string, BitmapImage> _cache = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Загружает изображение по относительному пути (например, "Backgrounds/main_menu").
        /// Автоматически пробует расширения .jpg, .png, .jpeg.
        /// </summary>
        public static BitmapImage? GetImage(string relativePathWithoutExtOrWithExt)
        {
            if (string.IsNullOrWhiteSpace(relativePathWithoutExtOrWithExt))
                return null;

            if (_cache.TryGetValue(relativePathWithoutExtOrWithExt, out var cached))
                return cached;

            string? resolvedPath = ResolveFilePath(relativePathWithoutExtOrWithExt);
            if (resolvedPath == null || !File.Exists(resolvedPath))
                return null;

            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(resolvedPath, UriKind.Absolute);
                bmp.EndInit();
                bmp.Freeze();

                _cache[relativePathWithoutExtOrWithExt] = bmp;
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает фоновую иллюстрацию для сюжетного диалога по nodeId.
        /// </summary>
        public static BitmapImage? GetStoryImage(string nodeId)
        {
            string? subPath = MapStoryNodeToImage(nodeId);
            return subPath != null ? GetImage(subPath) : null;
        }

        /// <summary>
        /// Возвращает портрет персонажа или NPC.
        /// </summary>
        public static BitmapImage? GetCharacterImage(string characterKey) => GetImage($"Characters/{characterKey}");

        /// <summary>
        /// Возвращает иллюстрацию карточки случайной встречи.
        /// </summary>
        public static BitmapImage? GetEncounterImage(string encounterKey) => GetImage($"Encounters/{encounterKey}");

        /// <summary>
        /// Возвращает портрет монстра или босса.
        /// </summary>
        public static BitmapImage? GetEnemyImage(string enemyKey) => GetImage($"Enemies/{enemyKey}");

        /// <summary>
        /// Возвращает UI изображение (логотип, иконка, баннер).
        /// </summary>
        public static BitmapImage? GetUiImage(string uiKey) => GetImage($"UI/{uiKey}");

        /// <summary>
        /// Возвращает аватар врага по имени или идентификатору.
        /// </summary>
        public static BitmapImage? GetEnemyAvatar(string? name, string? id = null)
        {
            string key = ResolveEnemyKey(name, id);
            return GetEnemyImage(key);
        }

        private static string ResolveEnemyKey(string? name, string? id)
        {
            string combined = $"{name} {id}".ToLowerInvariant();

            // Сюжетные Боссы и ключевые враги
            if (combined.Contains("углук") || combined.Contains("ugluk")) return "enemy_ugluk";
            if (combined.Contains("варг") || combined.Contains("varg") || combined.Contains("шаман")) return "enemy_shaman_varg";
            if (combined.Contains("громгар") || combined.Contains("gromgar")) return "enemy_gromgar";
            if (combined.Contains("тень малкор") || combined.Contains("воплощение тьмы") || combined.Contains("shadow_form") || combined.Contains("malkor_shadow")) return "enemy_malkor_shadow";
            if (combined.Contains("малкор") || combined.Contains("malkor")) return "enemy_malkor";
            if (combined.Contains("сердце") || combined.Contains("ядро") || combined.Contains("core") || combined.Contains("shard")) return "enemy_titan_core";
            if (combined.Contains("титан") || combined.Contains("titan")) return "enemy_titan";

            // Элитные противники
            if (combined.Contains("тролль") || combined.Contains("troll")) return "enemy_troll";
            if (combined.Contains("голем") || combined.Contains("golem")) return "enemy_rune_golem";
            if (combined.Contains("инквизитор") || combined.Contains("inquisitor")) return "enemy_shadow_inquisitor";
            if (combined.Contains("корол") || combined.Contains("king")) return "enemy_shadow_king";

            // Рядовые противники
            if (combined.Contains("берсерк") || combined.Contains("рубака") || combined.Contains("berserk")) return "enemy_orc_berserker";
            if (combined.Contains("щит") || combined.Contains("shield") || combined.Contains("страж")) return "enemy_orc_shield";
            if (combined.Contains("разведчик") || combined.Contains("scout") || combined.Contains("лучник") && combined.Contains("орк")) return "enemy_orc_scout";
            if (combined.Contains("телохранитель") || combined.Contains("гвардеец") || combined.Contains("bodyguard") || combined.Contains("воин")) return "enemy_orc_bodyguard";
            if (combined.Contains("лучник") || combined.Contains("archer")) return "enemy_shadow_archer";
            if (combined.Contains("адепт") || combined.Contains("культист") || combined.Contains("adept")) return "enemy_shadow_adept";
            if (combined.Contains("гончая") || combined.Contains("hound")) return "enemy_void_hound";
            if (combined.Contains("осколок")) return "enemy_heart_shard";

            // Дефолтный портрет
            return "enemy_orc_berserker";
        }

        /// <summary>
        /// Возвращает фоновое изображение для боевой арены.
        /// </summary>
        public static BitmapImage? GetBattleBackground(string? returnNodeId, bool isArcade, bool isMapMode, int mapFloor = 0)
        {
            // 1. Аркада
            if (isArcade)
            {
                return GetImage("Battle/battle_arcade") ?? GetImage("Battle/battle_mountain_pass");
            }

            // 2. Сюжетные узлы
            if (!string.IsNullOrEmpty(returnNodeId))
            {
                if (returnNodeId.Contains("Scene1") || returnNodeId.Contains("PostBattle1"))
                    return GetImage("Battle/battle_ash_creek");

                if (returnNodeId.Contains("Scene2") || returnNodeId.Contains("Scene3") || returnNodeId.Contains("Scene4"))
                    return GetImage("Battle/battle_mountain_pass");

                if (returnNodeId.StartsWith("Act2_Gates"))
                    return GetImage("Battle/battle_citadel_gates");

                if (returnNodeId.StartsWith("Act2_Dungeons"))
                    return GetImage("Battle/battle_citadel_dungeons");

                if (returnNodeId.Contains("Armory") || returnNodeId.Contains("Golem"))
                    return GetImage("Story/act2_forge_golem") ?? GetImage("Backgrounds/blacksmith");

                if (returnNodeId.Contains("Act2_Boss") || returnNodeId.Contains("Malkor"))
                    return GetImage("Battle/battle_citadel_sanctum");

                if (returnNodeId.StartsWith("Act3_"))
                    return GetImage("Battle/battle_titan_depths");
            }

            // 3. Режим Карты Похода
            if (isMapMode)
            {
                if (mapFloor <= 3)
                    return GetImage("Battle/battle_ash_creek");
                if (mapFloor <= 6)
                    return GetImage("Battle/battle_mountain_pass");
                if (mapFloor <= 8)
                    return GetImage("Battle/battle_citadel_dungeons");
                if (mapFloor == 9)
                    return GetImage("Battle/battle_citadel_gates");
                return GetImage("Battle/battle_citadel_sanctum") ?? GetImage("Battle/battle_titan_depths");
            }

            // Фоллбэк по умолчанию
            return GetImage("Battle/battle_mountain_pass") ?? GetImage("Battle/battle_ash_creek");
        }

        private static string? MapStoryNodeToImage(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return null;

            // Акт 1: Осада Пепельного Ручья
            if (nodeId == "Start" || nodeId == "Scene1_Pragmatic" || nodeId == "Scene1_Aggressive")
                return "Story/act1_scene1_village_ruins";

            if (nodeId == "Scene1_Ugluk")
                return "Story/act1_scene1_ugluk_standoff";

            if (nodeId == "PostBattle1" || nodeId == "PostBattle1_Dialogue" || nodeId.StartsWith("Interrogate_") || nodeId.StartsWith("EndChapter"))
                return "Story/act1_scene1_interrogation";

            if (nodeId.StartsWith("Scene2_") || nodeId == "EndScene2")
                return "Story/act1_scene2_gorge_ambush";

            if (nodeId == "Scene3_Start" || nodeId == "Scene3_Varg" || nodeId == "PostBattle_Scene3")
                return "Story/act1_scene3_shaman_varg";

            if (nodeId.StartsWith("Scene3_InspectTome") || nodeId.StartsWith("Scene3_Tome"))
                return "Story/act1_scene3_tome_discovery";

            if (nodeId.StartsWith("Scene4_") || nodeId == "PostBattle_Scene4")
                return "Story/act1_scene4_gromgar_throne";

            if (nodeId == "Ending_Light")
                return "Story/act1_ending_light";
            if (nodeId == "Ending_Dark")
                return "Story/act1_ending_dark";
            if (nodeId == "Ending_Neutral")
                return "Story/act1_ending_neutral";

            // Акт 2: Тени Цитадели
            if (nodeId == "Act2_Start")
                return "Story/act2_scene1_citadel_approach";

            if (nodeId.StartsWith("Act2_Gates") || nodeId == "Act2_PostBattle_Gates")
                return "Story/act2_gates_assault";

            if (nodeId == "Act2_Courtyard")
                return "Story/act2_courtyard_crossroads";

            if (nodeId.StartsWith("Act2_Dungeons") || nodeId.StartsWith("Act2_Kael"))
                return "Story/act2_dungeons_kael";

            if (nodeId.StartsWith("Act2_Library"))
                return "Story/act2_library_archives";

            if (nodeId.StartsWith("Act2_Armory") || nodeId.StartsWith("Act2_PostBattle_Golem"))
                return "Story/act2_forge_golem";

            if (nodeId.StartsWith("Act2_PreBoss") || nodeId == "Act2_Boss_Entry" || nodeId == "Act2_Boss_Dialogue")
                return "Story/act2_boss_malkor";

            if (nodeId.Contains("ShadowForm") || nodeId.Contains("Malkor_Shadow"))
                return "Story/act2_malkor_shadow_form";

            if (nodeId == "Act2_Ending_Decisions")
                return "Story/act2_boss_malkor";

            if (nodeId == "Act2_Ending_Light")
                return "Story/act2_ending_light";
            if (nodeId == "Act2_Ending_Dark")
                return "Story/act2_ending_dark";
            if (nodeId == "Act2_Ending_Neutral")
                return "Story/act2_ending_neutral";

            // Акт 3: Сердце Титана
            if (nodeId == "Act3_Start" || nodeId == "Act3_Ascent")
                return "Story/act3_hall_of_kings";

            if (nodeId == "Act3_Boss_Entry")
                return "Story/act3_titan_awakening";

            if (nodeId == "Act3_Ending_Decisions")
                return "Story/act3_titan_shattered_core";

            if (nodeId == "Act3_Ending_Light")
                return "Story/act3_ending_light";
            if (nodeId == "Act3_Ending_Dark")
                return "Story/act3_ending_dark";
            if (nodeId == "Act3_Ending_Neutral")
                return "Story/act3_ending_neutral";

            return null;
        }

        private static string? ResolveFilePath(string path)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string cleanPath = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            // 1. Точное совпадение
            string directApp = Path.Combine(baseDir, "Images", cleanPath);
            if (File.Exists(directApp)) return directApp;

            string directCwd = Path.Combine(Directory.GetCurrentDirectory(), "Images", cleanPath);
            if (File.Exists(directCwd)) return directCwd;

            // 2. Перебор расширений
            string[] extensions = { ".jpg", ".png", ".jpeg" };
            foreach (var ext in extensions)
            {
                string p1 = Path.Combine(baseDir, "Images", cleanPath + ext);
                if (File.Exists(p1)) return p1;

                string p2 = Path.Combine(Directory.GetCurrentDirectory(), "Images", cleanPath + ext);
                if (File.Exists(p2)) return p2;
            }

            return null;
        }
    }
}
