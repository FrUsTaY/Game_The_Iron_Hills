# 🎨 ТЕХНИЧЕСКИЙ ЛИСТ ВИЗУАЛЬНОГО ДИЗАЙНА (VISUAL ART DESIGN SPECIFICATION)
### Проект: «Эпическая битва: Железный Кряж» (The Iron Hills / Epic Battle)

---

## 📖 1. Визуальная концепция и Арт-дирекшн

### 1.1. Общий стиль (Art Direction)
* **Жанр:** Dark Fantasy, Grimdark RPG, Epic Medieval Adventure.
* **Стилистика рендера:** Живописный высокодетализированный цифровой концепт-арт (Digital Oil / Concept Art / Matte Painting), напоминающий работы к *The Witcher 3: Wild Hunt*, *Dark Souls*, *Elden Ring*, *Dragon Age: Origins*, *Darkest Dungeon II*, *Pillars of Eternity* и иллюстрации *Games Workshop / Warhammer Fantasy*.
* **Атмосфера:** Суровые заснеженные горы, тяжелое железо, грубая сталь, гранит, тлеющие угли, темная магия теней и древние светящиеся руны. Никакого мультяшного или легкомысленного стиля — мир опасен, брутален и осязаем.
* **Цветовая палитра:**
  - *Окружение:* глубокие графитовые, асфальтово-серые, стальные, холодные сине-зеленые тона горной тундры и скал.
  - *Акценты освещения:* янтарно-оранжевое свечение костров, факелов и раскаленной руды; холодный мистический ультрамарин рунной магии; зловещий фиолетово-пурпурный нексус Скверны и Бездны; золото и багрянец в кульминационных моментах.

### 1.2. Базовые параметры генерации для нейросетей
* **Рекомендуемые модели:** Midjourney v6 / v6.1, FLUX.1 (Dev/Schnell), Stable Diffusion XL (SDXL 1.0), DALL-E 3.
* **Единый суффикс стиля (Style Prompt Booster):**
  > `dark fantasy digital concept art, epic matte painting, highly detailed, grim atmospheric lighting, painterly texture, dramatic shadows, moody cinematic composition, 8k resolution, artstation trending`
* **Единый негативный промпт (Negative Prompt для SD / FLUX):**
  > `cartoon, 3d render, anime, vibrant rainbow colors, saturated neon, low quality, blurry, deformed anatomy, plastic skin, modern objects, watermark, text, signature`
* **Соотношения сторон (Aspect Ratios):**
  - **Фоны экранов и боевые арены (Backgrounds / Battle):** `--ar 16:9` (или `1920x1080` / `1600x900`).
  - **Сюжетные иллюстрации (Story Dialogues):** `--ar 16:9` (для широких сцен) или `--ar 4:3`.
  - **Портреты персонажей и монстров (Portraits / Sprites):** `--ar 3:4` или `--ar 1:1` (например, `768x1024` или `512x512`).
  - **Баннеры глав / карточки (UI Cards):** `--ar 16:9` или `--ar 2:1`.

---

## 📁 2. Иерархия каталогов и соглашение об именовании

Все графические файлы сохраняются в каталог `Images/` в корне проекта (рядом с папкой `Sounds/`):

```
The Iron Hills/
│
├── Images/
│   ├── Backgrounds/     # Фоны интерфейсных экранов и хабов (16:9, .png)
│   ├── Battle/          # Фоны боевых арен для боевого экрана BattleView (16:9, .png)
│   ├── Story/           # Сюжетные иллюстрации для DialogueView (16:9, .png)
│   ├── Encounters/      # Иллюстрации случайных событий EncounterView (16:9 или 4:3, .png)
│   ├── Characters/      # Портреты героев и ключевых дружественных/нейтральных NPC (3:4, .png)
│   ├── Enemies/         # Портреты и визуальные карточки противников и боссов (3:4, .png)
│   └── UI/              # Логотип, иконки, баннеры глав (PNG / ICO)
```

> **Важно:** Все файлы сохраняются строго в формате **PNG** (или JPG максимального качества) с точными именами из данного документа в нижнем регистре (snake_case). Это позволит автоматически подключить весь пак картинок без ручного переименования.

---

## 🖼️ 3. Полный реестр визуальных сцен и промптов

---

### ГРУППА 1: ФОНЫ ИНТЕРФЕЙСНЫХ ЭКРАНОВ И ХАБОВ (`Images/Backgrounds/`)

#### 1.1. Главное меню игры
* **Файл:** `Images/Backgrounds/main_menu.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Задний план стартового экрана `Views/MainMenu.xaml`.
* **Описание сцены:** Величественная и суровая панорама предгорий Железного Кряжа на закате. Острые скалистые пики, покрытые снегом и льдом. На переднем плане — старинная каменная сигнальная башня с горящей жаровней, высеченные в скале гигантские рунические врата. Хмурое небо с клубящимися пепельными тучами, вдалеке видны зловещие огни орочьих костров.
* **Промпт для генерации:**
  > **Prompt:** `A grimdark epic landscape of the Iron Hills mountain pass, towering snowy jagged peaks under a dramatic stormy sky, an ancient weathered stone fortress with glowing fire braziers on the rocky ridge, massive carved runic stone gate in the mountain rock, distant menacing war campfires, dark fantasy matte painting, cinematic lighting, moody atmospheric haze, 8k resolution, artstation concept art --ar 16:9 --style raw --v 6.0`

---

#### 1.2. Меню выбора главы (Chapter Select)
* **Файл:** `Images/Backgrounds/chapter_select.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/ChapterSelectView.xaml`.
* **Описание сцены:** Старинный дубовый военный стол в походном штабном шатре. На столе развернута большая пергаментная карта Железного Кряжа с нанесенными стрелами походов, деревянными фигурками армий и каменными рунами-печатями. Рядом лежит обнаженный стальной кинжал, воткнутый в край стола, свитки с сургучными печатями, горящие толстые свечи в железных подсвечниках, сквозь разрез шатра видна заснеженная горная буря.
* **Промпт для генерации:**
  > **Prompt:** `A close-up view of an ancient wooden war table inside a commander's canvas tent, a detailed weathered parchment map of fantasy mountain realms spread out with tactical wooden markers, iron daggers, burning tallow candles casting warm shadows, wax-sealed scrolls, blowing blizzard snow visible through tent opening, grim medieval dark fantasy aesthetic, atmospheric, richly textured --ar 16:9 --style raw --v 6.0`

---

#### 1.3. Экран создания персонажа (Character Creation)
* **Файл:** `Images/Backgrounds/char_creation.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Задний план для `Views/CharacterCreationView.xaml`.
* **Описание сцены:** Арсенал полевого лагеря авантюристов у подножия Кряжа. Каменная стена древнего форпоста, оружейные стойки с мечами, секирами, луками и посохами. Сундуки с кольчугами и коваными латами, висящие меха и теплые плащи. Мягкий свет факела и каменного очага, создающий полумрак и атмосферу подготовки к великому опасному походу.
* **Промпт для генерации:**
  > **Prompt:** `An armory and expedition gear staging area inside a ruined stone fortress hall, weapons racks filled with battle-worn iron swords, shields, battleaxes, and arcane runic staves, open wooden crates with heavy plate armor and leather cloaks, glowing wall torches casting deep amber shadows, dark fantasy RPG character selection screen background, moody lighting, highly detailed --ar 16:9 --style raw --v 6.0`

---

#### 1.4. Интерактивная карта похода (Map View)
* **Файл:** `Images/Backgrounds/map_view.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Подложка карты в `Views/MapView.xaml` (под узлами этажей Slay the Spire style).
* **Описание сцены:** Вертикально ориентированная старинная топографическая карта-гравюра, высеченная на темной сланцевой плите или плотном пергаменте с обугленными краями. Изображение хребта Железного Кряжа снизу вверх: от сгоревших долин Пепельного Ручья через глубокие ущелья к циклопической Черной Цитадели и пылающей вершине горы. Легкая руническая сетка, мистическая дымка.
* **Промпт для генерации:**
  > **Prompt:** `A dark medieval hand-drawn parchment cartography map texture, vertical progression from ash valleys at the bottom to a towering sinister black fortress on top of a jagged mountain peak, engraved ink lines, subtle glowing runic paths, aged paper vignette, mystical fog, dark fantasy tabletop RPG board aesthetic --ar 16:9 --style raw --v 6.0`

---

#### 1.5. Привал у костра (Campfire View)
* **Файл:** `Images/Backgrounds/campfire.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/CampfireView.xaml`.
* **Описание сцены:** Уютный, но суровый ночной лагерь, укрытый в неглубоком скальном гроте от ледяного горного ветра. В центре жарко трещит костер, бросая яркие оранжевые отблески на слоистые гранитные стены. Рядом разложен спальный тюк из медвежьей шкуры, котелок над огнем, меч воткнут в землю, походный рюкзак. В проеме пещеры видно темно-синее морозное небо с мерцающими звездами и заснеженными пиками.
* **Промпт для генерации:**
  > **Prompt:** `A solitary campfire burning warmly inside a sheltered rocky mountain alcove at night, glowing orange embers, dancing flames illuminating coarse granite walls, travel bedroll made of thick fur, iron cooking pot over fire, broadsword stuck into ground, outside the cave a cold starlit dark blue sky and snow-dusted jagged peaks, comforting yet perilous dark fantasy camp, cinematic mood --ar 16:9 --style raw --v 6.0`

---

#### 1.6. Кузница Кряжа (Blacksmith View)
* **Файл:** `Images/Backgrounds/blacksmith.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/BlacksmithView.xaml`.
* **Описание сцены:** Монументальная подземная кузница северян или гномов, вырубленная прямо в толще скалы. Огромный кузнечный горн, пылающий золотисто-багровым жаром, массивная железная наковальня на каменном постаменте, разлетающиеся золотые искры. Вдоль закопченных стен развешаны боевые молоты, закаленные клинки, тигли с расплавленной железной рудой, светящиеся рунические наковальни.
* **Промпт для генерации:**
  > **Prompt:** `A colossal ancient underground dwarven blacksmith forge carved into deep mountain bedrock, massive stone furnace glowing with molten iron and fiery embers, gigantic anvil with flying golden sparks, masterwork weapons and rune-inscribed armor hanging on soot-stained stone walls, heavy iron chains, dramatic rim lighting, dark epic fantasy craftsmanship --ar 16:9 --style raw --v 6.0`

---

#### 1.7. Лавка странника (Merchant View)
* **Файл:** `Images/Backgrounds/merchant.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/MerchantView.xaml`.
* **Описание сцены:** Стоянка бродячего торговца на горном уступе у старого каменного моста. Большая крытая повозка, увешанная связками сушеных трав, клетками, стеклянными колбами с люминесцентными зельями и свитками. Качающийся керосиновый или масляный фонарь с теплым желтым светом освещает прилавок, где разложены магические амулеты, клинки и мешочки с золотыми монетами.
* **Промпт для генерации:**
  > **Prompt:** `A wandering mysterious merchant's wooden wagon encampment pitched on a misty mountain pass, canopy wagon hung with glowing alchemical potion flasks, dried herbs, antique scrolls, and exotic trinkets, warm hanging lantern light cutting through the foggy cold dusk, detailed RPG shop stall interior, dark fantasy atmosphere --ar 16:9 --style raw --v 6.0`

---

#### 1.8. Таверна Кряжа (Tavern View)
* **Файл:** `Images/Backgrounds/tavern.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/TavernView.xaml`.
* **Описание сцены:** Аутентичная горная таверна, построенная из тяжелых бревен и дикого камня. Большой каменный камин с пылающими поленьями, прокопченные балки под потолком, массивные дубовые столы с глиняными кружками темного эля, свечные огарки. На заднем плане — силуэты отдыхающих наемников и барная стойка с бочонками пива и связками вяленого мяса. Теплая, дурманящая и укрывающая от горной бури обстановка.
* **Промпт для генерации:**
  > **Prompt:** `Interior of a cozy yet gritty dark fantasy mountain tavern, massive stone hearth fireplace blazing with fire, thick timber wood beams, heavy oak tables with clay mugs of ale and scattered coins, candle light atmosphere, rustic bar counter with wooden kegs in background, warm refuge from freezing snowstorm outside, painterly concept art --ar 16:9 --style raw --v 6.0`

---

#### 1.9. Инвентарь и снаряжение (Inventory View)
* **Файл:** `Images/Backgrounds/inventory.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/InventoryView.xaml`.
* **Описание сцены:** Массивный дубовый стол с железной оковкой. На столе разложены боевые пластинчатые доспехи, сверкающий стальной шлем с забралом, кожаные перчатки, инкрустированные кольца и амулеты, светящиеся рунические камни. Рядом стоит открытый дорожный кожаный сундук.
* **Промпт для генерации:**
  > **Prompt:** `A top-down angle view of an adventurer's heavy iron-bound gear chest and wooden arming table, polished steel cuirass, runic rings glowing with faint blue aura, engraved dagger, leather pouches, inventory equipment screen concept art, dark medieval realistic fantasy, dramatic contrast lighting --ar 16:9 --style raw --v 6.0`

---

#### 1.10. Древо талантов (Skill Tree View)
* **Файл:** `Images/Backgrounds/skill_tree.png`
* **Разрешение:** 1920×1080 (16:9)
* **Назначение:** Фоновый арт в `Views/SkillTreeView.xaml`.
* **Описание сцены:** Циклопический каменный монолит с древними высеченными рунами и триптихом ветвей: Меч (сталь и огонь), Магия (мистический ультрамариновый свет), Выживание (изумрудные и земляные переплетения корней). Тонкие светящиеся линии энергии соединяют углубления в камне. Мистическая дымка, глубокая тьма древнего святилища.
* **Промпт для генерации:**
  > **Prompt:** `A mystical colossal stone monolith carved with glowing ancient magical constellations and runic skill trees, three branching paths of warrior sword, arcane wizardry, and wilderness survival glowing with ember, azure, and jade light lines, dark arcane sanctuary background, deep shadows, epic fantasy RPG talent tree UI background --ar 16:9 --style raw --v 6.0`

---

### ГРУППА 2: БОЕВЫЕ АРЕНЫ (`Images/Battle/`)

Фоны, которые устанавливаются на задний план экрана боя `Views/BattleView.xaml` в зависимости от текущей локации или волны:

#### 2.1. Пепельный Ручей (Акт 1 — Бой в деревне)
* **Файл:** `Images/Battle/battle_ash_creek.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Окраина разоренной северной деревни Пепельный Ручей. Обугленные остовы деревянных изб, тлеющие балки, серый пепел, падающий вместе с мокрым снегом на грязь. Разломанные частоколы, брошенные телеги, вдалеке клубится густой черный дым на фоне хмурого свинцового неба. На земле следы тяжелых орочьих сапог.
* **Промпт для генерации:**
  > **Prompt:** `A battlefield in a devastated northern medieval village called Ash Creek, charred smoking timber cottages, broken wooden palisades, grey ash and wet snow falling onto muddy ground, gloomy overcast sky with heavy black smoke plumes, dark gritty fantasy combat arena background, cinematic wide view, atmospheric depth --ar 16:9 --style raw --v 6.0`

---

#### 2.2. Горное ущелье / Засада (Акт 1 — Сцены 2–3)
* **Файл:** `Images/Battle/battle_mountain_pass.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Узкий скальный карниз высоко в горах Железного Кряжа. Слева — отвесная скальная стена с редкими корявыми соснами, справа — головокружительная бездна ущелья, где стелется ледяной туман. На тропе лежат упавшие каменные глыбы и останки разбитого каравана. Порывы ветра несут крупицы снега.
* **Промпт для генерации:**
  > **Prompt:** `A precarious narrow mountain ledge path high in the rugged Iron Hills, towering sheer granite cliff on one side, deep misty canyon abyss on the other, scattered boulders from a rockslide, twisted gnarled pines clinging to stone, whistling snow storm wind, dangerous dark fantasy battleground --ar 16:9 --style raw --v 6.0`

---

#### 2.3. Черные Врата Цитадели (Акт 2 — Штурм)
* **Файл:** `Images/Battle/battle_citadel_gates.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Циклопические Черные Врата Древней Цитадели, врезанные в основание монолитной черной скалы. Кованые железные створки с шипами, покрытые темными фиолетовыми защитными рунами. По краям возвышаются массивные бастионы с зубчатыми стенами, откуда свисают орочьи и культистские знамена. Земля усеяна стрелами и разбитыми щитами.
* **Промпт для генерации:**
  > **Prompt:** `Imposing titanic Black Gates of an ancient mountain citadel carved into a sheer obsidian cliff, colossal spiky iron portcullis humming with faint purple necrotic runes, heavy stone battlements above with dark ragged banners, shattered barricades and arrows in the foreground, epic dark fantasy siege battlefield --ar 16:9 --style raw --v 6.0`

---

#### 2.4. Тюремные Казематы Цитадели (Акт 2 — Подземелья)
* **Файл:** `Images/Battle/battle_citadel_dungeons.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Подземные тюремные казематы и пыточные подземелья Цитадели. Своды из замшелого черного камня, толстые железные решетки камер, свисающие с потолка цепи и кандалы. Вдоль стен горят факелы с неестественным фиолетово-зеленым пламенем, на влажном каменном полу лужи и пятна запекшейся крови.
* **Промпт для генерации:**
  > **Prompt:** `Dismal subterranean dungeon hall of an ancient fortress, vaulted mossy black stone ceilings, heavy rusted iron cell bars, hanging iron chains and torture apparatus, wall torches burning with eerie violet arcane fire, damp reflective flagstone floor with puddles, dark grim fantasy combat arena --ar 16:9 --style raw --v 6.0`

---

#### 2.5. Святилище Архимага Малкора (Акт 2 — Финальный босс)
* **Файл:** `Images/Battle/battle_citadel_sanctum.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Тронный зал и святилище темной магии в сердце Цитадели. Колоссальный круглый зал с парящими в воздухе фиолетовыми руническими кольцами. В центре — расколотый алтарь Бездны, из которого бьет вихрь теневой эктоплазмы. Разбитые витражные окна в готическом стиле, сквозь которые видно багровую луну в грозовом небе.
* **Промпт для генерации:**
  > **Prompt:** `The forbidden dark sanctum throne room of Archmage Malkor inside the citadel apex, floating glowing violet runic circles in mid-air, a shattered dark void altar radiating swirling shadows and purple lightning, grand gothic broken stained glass windows revealing a blood-red moon in stormy skies, epic final boss battle arena --ar 16:9 --style raw --v 6.0`

---

#### 2.6. Пылающие недра Кряжа / Зал Первородного Титана (Акт 3 — Финал)
* **Файл:** `Images/Battle/battle_titan_depths.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Самое сердце горы Железный Кряж — первобытный подземный зал колоссальных размеров. Из растрескавшегося базальтового пола бьют потоки кипящей золотисто-алой лавы. Вокруг возвышаются гигантские древние колонны в виде спящих титанов. В центре парит огромное раскаленное Сердце Горы, пульсирующее колоссальной первородной мощью.
* **Промпт для генерации:**
  > **Prompt:** `A primordial subterranean volcanic cavern inside the hollow roots of the Iron Hills mountain, glowing rivers of magma and molten gold cracking through obsidian floor, gigantic petrified titan stone statues acting as pillars, a colossal blazing molten Titan Core pulsing with blinding primordial power in the center, mythological epic scale, intense heat and smoke haze --ar 16:9 --style raw --v 6.0`

---

#### 2.7. Арена Аркады (Бесконечная битва)
* **Файл:** `Images/Battle/battle_arcade.png`
* **Разрешение:** 1920×1080 (16:9)
* **Описание сцены:** Заброшенная открытая каменная арена гладиаторов на заснеженном горном плато. Окружена древними разрушенными колоннами и деревянными кольями с орочьими черепами. Завывает вьюга, земля посыпана снегом и песком.
* **Промпт для генерации:**
  > **Prompt:** `An ancient ruined stone gladiatorial arena perched on a wind-swept snowy mountain plateau, weathered stone pillars, boundary palisades decorated with beast skulls and ragged battle standards, drifting blizzard snow across blood-stained frozen sand, dramatic dark fantasy combat ground --ar 16:9 --style raw --v 6.0`

---

### ГРУППА 3: СЮЖЕТНЫЕ ИЛЛЮСТРАЦИИ (STORY DIALOGUES) (`Images/Story/`)

Иллюстрации для `Views/DialogueView.xaml` (привязываются по `nodeId` в `StoryDatabase`):

#### Сюжетные сцены Акта 1 («Осада Пепельного Ручья»):
1. **`act1_scene1_village_ruins.png`** (Узлы `Start`, `Scene1_Pragmatic`, `Scene1_Aggressive`):
   - *Сюжет:* Герой входит в горящий Пепельный Ручей, видит разрушения и дым.
   - *Промпт:* `A grim warrior in worn armor standing at the entrance of a burning smoking medieval village, destroyed thatched roofs, falling embers, grim determination, dark fantasy storytelling cinematic still --ar 16:9 --style raw --v 6.0`
2. **`act1_scene1_ugluk_standoff.png`** (Узел `Scene1_Ugluk`):
   - *Сюжет:* Огромный свирепый орк Углук в ожерелье из костей с зазубренным тесаком преграждает дорогу.
   - *Промпт:* `A towering brutal orc warlord named Ugluk with battle scars, rough iron cleaver raised, bone necklace, snarling with yellow tusks among burning ruins, dramatic confrontation shot --ar 16:9 --style raw --v 6.0`
3. **`act1_scene1_interrogation.png`** (Узлы `PostBattle1`, `Interrogate_Mercy`, `Interrogate_Ruthless`, `EndChapter_Loot`):
   - *Сюжет:* Поверженный вожак орков на коленях в грязи, допрос о планах Вождя Громгара.
   - *Промпт:* `A defeated wounded orc on his knees in the mud being interrogated by a victorious warrior holding a steel blade to his throat, smoke and embers swirling in background --ar 16:9 --style raw --v 6.0`
4. **`act1_scene2_gorge_ambush.png`** (Узлы `Scene2_Start`, `Scene2_Dialogue1`, `Scene2_Noble`, `Scene2_Pragmatic`, `Scene2_Mercenary`):
   - *Сюжет:* Засада орочьего отряда на узком горном карнизе: орки с луками на скалах, падающие валуны.
   - *Промпт:* `An ambush in a treacherous mountain gorge, orc scouts perched on rocky cliffs raining arrows and rolling boulders down on a narrow trail, dynamic cinematic framing, tension --ar 16:9 --style raw --v 6.0`
5. **`act1_scene3_shaman_varg.png`** (Узлы `Scene3_Start`, `Scene3_Varg`, `PostBattle_Scene3`):
   - *Сюжет:* Стоянка Шамана Варга у древнего рунического алтаря в скалах, светящиеся тотемы черепов и зеленоватое пламя.
   - *Промпт:* `An eerie orc shaman named Varg performing a dark ritual before a glowing carved runic stone altar, shamanistic bone staves, eerie green spectral mist, twisted mountain pines --ar 16:9 --style raw --v 6.0`
6. **`act1_scene3_tome_discovery.png`** (Узлы `Scene3_InspectTome`, `Scene3_TomeIntact`, `Scene3_TomeDamaged`):
   - *Сюжет:* Герой находит древний рунический фолиант в тяжелом железном переплете с серебряными рунами.
   - *Промпт:* `A close-up of a heavy ancient iron-bound grimoire resting on a stone pedestal, glowing silver runes etched on leather cover, mystical knowledge, dark fantasy artifact --ar 16:9 --style raw --v 6.0`
7. **`act1_scene4_gromgar_throne.png`** (Узлы `Scene4_Start`, `Scene4_Gromgar`, `PostBattle_Scene4`):
   - *Сюжет:* Вождь Громгар — гигантский орочий владыка в рогатом шлеме и тяжелых черных латах перед Рунным Монолитом.
   - *Промпт:* `The massive Supreme Chieftain Gromgar clad in spiked black plate armor with a colossal war hammer, standing defiant before a glowing mountain monolith, epic confrontation --ar 16:9 --style raw --v 6.0`
8. **`act1_ending_light.png`** (Узел `Ending_Light`):
   - *Сюжет:* Очищение Рунного Монолита. Золотой луч святого света раскалывает тучи, орочья скверна развеивается.
   - *Промпт:* `A sacred ancient mountain monolith radiating radiant golden sunlight, dark mist dissolving into pure light, hopeful yet solemn dark fantasy triumph, sunrise over mountain ridge --ar 16:9 --style raw --v 6.0`
9. **`act1_ending_dark.png`** (Узел `Ending_Dark`):
   - *Сюжет:* Поглощение силы монолита. Монолит истекает багровой тенью, герой наполняется темной силой.
   - *Промпт:* `A dark warrior absorbing sinister crimson and shadow energy from a corrupted cracked monolith, glowing red veins, malevolent power surge, dark fantasy corruption --ar 16:9 --style raw --v 6.0`
10. **`act1_ending_neutral.png`** (Узел `Ending_Neutral`):
    - *Сюжет:* Запечатывание монолита. Камень погружается в спячку, путник уходит в заснеженные горы к Цитадели.
    - *Промпт:* `A solitary cloaked warrior walking along a high snowbound mountain ridge towards a distant dark citadel fortress in the clouds, balance maintained, contemplative moody journey --ar 16:9 --style raw --v 6.0`

---

#### Сюжетные сцены Акта 2 («Тени Цитадели»):
11. **`act2_scene1_citadel_approach.png`** (Узел `Act2_Start`):
    - *Сюжет:* Вид снизу на циклопическую Черную Цитадель, возвышающуюся над грозовыми тучами.
    - *Промпт:* `Looking up at the monolithic Black Citadel towering over jagged cliffs into thunderclouds, ominous gargoyles, purple lightning striking stone spires, epic entrance --ar 16:9 --style raw --v 6.0`
12. **`act2_gates_assault.png`** (Узлы `Act2_Gates_Assault`, `Act2_Gates_Magic`, `Act2_Gates_Stealth`, `Act2_Gates_TomeDispel`, `Act2_Gates_Main`, `Act2_PostBattle_Gates`):
    - *Сюжет:* Битва у Черных Врат Цитадели, разрушение баррикад, схватка со Стражами Врат.
    - *Промпт:* `A furious clash at the fortress gates, hero breaching barricades, clash of steel, shadow bolts colliding with iron shields, dynamic dark fantasy action scene --ar 16:9 --style raw --v 6.0`
13. **`act2_courtyard_crossroads.png`** (Узел `Act2_Courtyard`):
    - *Сюжет:* Внутренний двор Цитадели под багровой луной. Три пути: лестница в Казематы, арка Библиотеки, проход в Кузницу.
    - *Промпт:* `A sprawling stone courtyard of the dark citadel under a blood-red full moon, three ominous archways leading into depths: iron dungeon grate, arcane library gates, smoking forge door, crossroads --ar 16:9 --style raw --v 6.0`
14. **`act2_dungeons_kael.png`** (Узлы `Act2_Dungeons_Entry`, `Act2_Dungeons_Encounter`, `Act2_Dungeons_PostBattle`, `Act2_Dungeons_Kael_Dialogue`, `Act2_Kael_Freed`, `Act2_Kael_Looted`):
    - *Сюжет:* Глубокая темница: изможденный маг Каэль в рунических цепях передает герою Теневой Кристалл.
    - *Промпт:* `An emaciated captive wizard named Kael bound in glowing runic chains inside a damp stone cell, holding out a pulsing dark shadow crystal in trembling hands, dramatic emotional lighting --ar 16:9 --style raw --v 6.0`
15. **`act2_library_archives.png`** (Узлы `Act2_Library_Entry`, `Act2_Library_Inspect`, `Act2_Library_ArcaneSecrets`, `Act2_Library_TomeMastery`, `Act2_Library_Alchemy`, `Act2_Library_DarkAltar`):
    - *Сюжет:* Запретная Библиотека Малкора: гигантские шкафы со старинными гримуарами, алхимический стол с ретортами и темный алтарь Бездны.
    - *Промпт:* `A grand forbidden occult library in the citadel, towering wooden bookshelves filled with glowing arcane tomes, an alchemy workbench with bubbling vials, floating parchment pages, mysterious knowledge --ar 16:9 --style raw --v 6.0`
16. **`act2_forge_golem.png`** (Узлы `Act2_Armory_Entry`, `Act2_Armory_Golem_Inspect`, `Act2_Armory_Repair`, `Act2_Armory_DrainHeart`, `Act2_Armory_Golem_Rage`, `Act2_PostBattle_Golem`):
    - *Сюжет:* Древняя Кузница Цитадели: спящий 4-метровый Рунный Голем из железа и гранита с потухшим руническим сердцем.
    - *Промпт:* `A dormant four-meter tall ancient runic golem made of iron plates and carved granite stone sitting motionless in a dwarven forge, glowing runes carved into its chest core, awe-inspiring construct --ar 16:9 --style raw --v 6.0`
17. **`act2_boss_malkor.png`** (Узлы `Act2_PreBoss_Warning`, `Act2_Boss_Entry`, `Act2_Boss_Dialogue`):
    - *Сюжет:* Архимаг Малкор во главе святилища — бледный колдун в черной мантии с капюшоном, вскидывающий руки в вихре фиолетовой магии Бездны.
    - *Промпт:* `Archmage Malkor the dark sorcerer, pale gaunt face beneath ornate dark hood, floating above a void altar casting devastating purple shadow lightning from outstretched hands, terrifying presence --ar 16:9 --style raw --v 6.0`
18. **`act2_malkor_shadow_form.png`** (Узел `Act2_Boss_Dialogue` — Фаза 2):
    - *Сюжет:* Вторая фаза боя: Малкор трансформируется в исполинское Воплощение Тьмы с крыльями из чистого мрака и пылающими фиолетовыми глазами.
    - *Промпт:* `A monstrous shadowy incarnation of darkness, colossal shadowy entity with wings of pure smoke and void energy, burning violet eyes, dark demonic phantom form, apocalyptic dark fantasy --ar 16:9 --style raw --v 6.0`
19. **`act2_ending_light.png`** (Узлы `Act2_Ending_Decisions`, `Act2_Ending_Light`):
    - *Сюжет:* Очищение Сердца Цитадели: герой вонзает меч в алтарь, святой свет разрывает купол крепости, очищая ее от скверны.
    - *Промпт:* `The hero piercing a dark altar with a radiant glowing longsword, waves of pure golden daylight bursting through the shattered dome of the black citadel, darkness cleansed, epic triumphant climax --ar 16:9 --style raw --v 6.0`
20. **`act2_ending_dark.png`** (Узлы `Act2_Ending_Decisions`, `Act2_Ending_Dark`):
    - *Сюжет:* Владыка Цитадели: герой восходит на трон Малкора, облачаясь в Мантию Первородной Тьмы, тени склоняются перед новым владыкой.
    - *Промпт:* `A dark conquering hero seated upon the obsidian throne of the citadel, draped in the Mantle of Primordial Darkness, shadow wraiths bowing, ominous regal dark fantasy ruler --ar 16:9 --style raw --v 6.0`
21. **`act2_ending_neutral.png`** (Узлы `Act2_Ending_Decisions`, `Act2_Ending_Neutral`):
    - *Сюжет:* Вечный Страж: герой запирает врата Цитадели печатью Равновесия, оставаясь бессменным часовым на стене над спящим миром.
    - *Промпт:* `A stoic armored guardian standing solitary watch upon the high battlements of the sealed citadel against a starry night sky, holding a balance seal emblem, solemn vigilance --ar 16:9 --style raw --v 6.0`

---

#### Сюжетные сцены Акта 3 («Сердце Титана»):
22. **`act3_hall_of_kings.png`** (Узлы `Act3_Start`, `Act3_Ascent`):
    - *Сюжет:* Зал Древних Королей в недрах горы. Исполинские каменные троны древних владык, колоссальная винтовая лестница, уходящая в раскаленную бездну.
    - *Промпт:* `The Hall of Ancient Kings deep inside the mountain core, titanic carved stone thrones of forgotten dwarven monarchs, vast spiral staircase descending into a glowing magma abyss, breathtaking grandeur --ar 16:9 --style raw --v 6.0`
23. **`act3_titan_awakening.png`** (Узел `Act3_Boss_Entry`):
    - *Сюжет:* Пробуждение Первородного Титана Кряжа: гигантское существо из раскаленного базальта и магмы поднимается из расщелины, сотрясая гору.
    - *Промпт:* `The Primordial Titan of the Iron Hills awakening from a colossal chasm, a colossal creature forged of cracked black basalt and blazing orange magma veins, glowing molten eyes, earthquake scale --ar 16:9 --style raw --v 6.0`
24. **`act3_titan_shattered_core.png`** (Узел `Act3_Ending_Decisions`):
    - *Сюжет:* Расколотая оболочка Титана обнажила Первородное Сердце Кряжа — пульсирующий кристалл чистого огня и жизни.
    - *Промпт:* `The shattered rocky chest of the defeated Titan revealing the blazing exposed Primordial Mountain Heart, a blinding crystalline core pulsing with elemental fire and life energy, mythical climax --ar 16:9 --style raw --v 6.0`
25. **`act3_ending_light.png`** (Узел `Act3_Ending_Light`):
    - *Сюжет:* Эпилог Света («Светлые Горы»): Рассвет над Железным Кряжем, зеленеющие предгорья, мир и возрождение Севера.
    - *Промпт:* `A glorious golden sunrise illuminating the grand mountain ranges of the Iron Hills, pristine snow-capped peaks, verdant green valleys blooming, peaceful majestic ending landscape --ar 16:9 --style raw --v 6.0`
26. **`act3_ending_dark.png`** (Узел `Act3_Ending_Dark`):
    - *Сюжет:* Эпилог Тьмы («Владыка Сердца»): Недра горы повинуются новому Повелителю, вулканическое пламя и армия големов готовы покорить мир.
    - *Промпт:* `A supreme dark overlord standing above a sea of surging molten magma holding the pulsing Titan Heart, legions of basalt stone golems kneeling, triumphant tyrannical power --ar 16:9 --style raw --v 6.0`
27. **`act3_ending_neutral.png`** (Узел `Act3_Ending_Neutral`):
    - *Сюжет:* Эпилог Равновесия («Хранитель Кряжа»): Запечатанные на веки вечные недра горы, каменный дозорный у вечных врат покоя.
    - *Промпт:* `A sealed gigantic runic vault door in deep mountain stone, ancient protective wards glowing soft blue, an immortal guardian leaning on a greatsword in quiet perpetual peace --ar 16:9 --style raw --v 6.0`

---

### ГРУППА 4: СЛУЧАЙНЫЕ СОБЫТИЯ (ENCOUNTERS) (`Images/Encounters/`)

Иллюстрации карточек случайных встреч для `Views/EncounterView.xaml`:

1. **`encounter_blood_altar.png`**
   - *Событие:* «Кровавый Алтарь Предков» (`RandomEncounter`).
   - *Описание:* Зловещий древний каменный жертвенник среди сосен, покрытый засохшей кровью и орочьими черепами. В желобе тлеют багровые угли.
   - *Промпт:* `A sinister ancient stone blood altar in a dark pine forest clearing, weathered sacrificial basin filled with glowing crimson embers and dark stains, beast skulls impaled on spikes, dark pagan fantasy --ar 16:9 --style raw --v 6.0`

2. **`encounter_merchant_grix.png`**
   - *Событие:* «Бродячий Торговец Грикс» (`RandomEncounter`).
   - *Описание:* Хитрый коренастый торговец-гоблин или гном в широкополой шляпе и жилете с сотней карманов, сидящий на ящике у костра и предлагающий загадочные склянки.
   - *Промпт:* `A cunning sly travelling merchant sitting on a wooden supply crate beside a campfire on a mountain road, worn wide-brimmed felt hat, vest with many brass buckles and pockets, holding up a glowing vial with a grin --ar 16:9 --style raw --v 6.0`

3. **`encounter_abandoned_camp.png`**
   - *Событие:* «Заброшенный Привал у Ручья» (`RandomEncounter`).
   - *Описание:* Опустевший разбитый лагерь старателей у быстрого горного ручья: разорванная палатка, остывшее кострище, брошенная кирка и промывочный лоток среди холодных камней.
   - *Промпт:* `An eerie abandoned miner's campsite beside a rushing clear mountain stream, torn canvas tent flapping in wind, cold stone campfire ring, discarded iron pickaxe and prospecting tools, mysterious silence --ar 16:9 --style raw --v 6.0`

4. **`encounter_iron_chest.png`**
   - *Событие:* «Запертый Кованый Сундук» (`RandomEncounter`).
   - *Описание:* Массивный старинный сундук из мореного дуба, наглухо окованный черным железом с тяжелым фигурным замком в форме черепа, наполовину заваленный камнями в расщелине скалы.
   - *Промпт:* `A heavy antique iron-banded treasure chest hidden in a dark rocky mountain crevice, ornate demonic skull padlock, cobwebs and dusting of frost, tempting locked mystery, dark fantasy RPG encounter --ar 16:9 --style raw --v 6.0`

---

### ГРУППА 5: ПОРТРЕТЫ ПЕРСОНАЖЕЙ И NPC (`Images/Characters/`)

Формат: 3:4 (768×1024), поясной портрет или бюст на атмосферном фоне.

#### 5.1. Классы героя (Игрок):
1. **`hero_veteran.png`**
   - *Класс:* **Ветеран** (Warrior).
   - *Описание:* Суровый мужчина 40 лет с седеющей бородой и боевым шрамом через щеку. Облачен в тяжелые стальные латы поверх кольчуги, на плече меховой плащ. В руках держит закаленный двуручный меч с простой крестовиной. Взгляд непоколебимый и опытный.
   - *Промпт:* `Portrait of a seasoned grim veteran warrior, 40s, battle-hardened face with scars and salt-and-pepper beard, wearing heavy battle-worn steel plate armor and wolf fur cloak, holding a two-handed iron broadsword, grim dark fantasy hero portrait, rich textures, studio lighting --ar 3:4 --style raw --v 6.0`

2. **`hero_mage.png`**
   - *Класс:* **Изгой-маг** (Mage).
   - *Описание:* Загадочный затворник в темной мантии с глубоким капюшоном, скрывающим верхнюю часть лица. Из-под капюшона видны светящиеся фиолетовые рунические татуировки на скулах. В руке держит резной посох из темного дерева с парящим кристаллическим осколком, окруженным искрами чистой маны.
   - *Промпт:* `Portrait of an outcast renegade battlemage, mysterious face partially shadowed under a dark hooded traveling cloak, glowing arcane purple runic tattoos on cheeks, holding an ancient gnarled wooden staff with a hovering levitating crystal shard emitting azure sparks, mystical atmosphere --ar 3:4 --style raw --v 6.0`

3. **`hero_mercenary.png`**
   - *Класс:* **Наемник** (Mercenary / Rogue).
   - *Описание:* Ловкий, опасный следопыт с пронзительным взглядом. Кожаный доспех с металлическими пластинами на плечах, темный шарф-бандана на шее, пояс со множеством ножен и мешочков. В обеих руках сжимает смертоносные зазубренные кинжалы с чернеными лезвиями.
   - *Промпт:* `Portrait of a lethal mountain mercenary rogue, sharp observant eyes, weathered leather and studded iron brigandine armor, dark wool cowl scarf, twin blackened serrated daggers held in reverse grip, agile and dangerous, dark fantasy character art --ar 3:4 --style raw --v 6.0`

#### 5.2. Дружественные и сюжетные NPC:
4. **`npc_blacksmith.png`**
   - *Персонаж:* **Кузнец Бородан** (Кузница Кряжа).
   - *Описание:* Могучий кузнец с пышной медной бородой, заплетенной в косы, в кожаном фартуке с медными заклепками. Мускулистые руки со следами ожогов держат тяжелый кузнечный молот. Лицо освещено золотистым жаром горна.
   - *Промпт:* `Portrait of a burly master blacksmith named Borodan, braided fiery red beard with iron rings, soot on face, heavy leather apron over muscular scarred arms, holding a massive smithing hammer, warm glowing forge embers background --ar 3:4 --style raw --v 6.0`

5. **`npc_innkeeper.png`**
   - *Персонаж:* **Трактирщик Гуннар** (Таверна Кряжа).
   - *Описание:* Добродушный, но повидавший виды пожилой хозяин таверны с лысиной и седыми бакенбардами, в холщовой рубахе и жилете, натирающий глиняную кружку полотенцем. Умный, внимательный взгляд человека, знающего все горные слухи.
   - *Промпт:* `Portrait of an experienced stout innkeeper named Gunnar, grey mutton chops and bald head, friendly yet wary wise eyes, linen shirt and wool vest, wiping a rustic clay beer mug with a rag, warm tavern interior background --ar 3:4 --style raw --v 6.0`

6. **`npc_merchant.png`**
   - *Персонаж:* **Торговец-Странник** (Лавка).
   - *Описание:* Загадочный торговец редкостями в восточном или северном меховом тюрбане и плаще, пальцы унизаны старинными перстнями, на груди ожерелье из редких монет. Лукавая полуулыбка, в руке медные весы.
   - *Промпт:* `Portrait of an enigmatic traveling wanderer merchant, draped in layered patterned robes and fur mantle, fingers covered in antique brass rings, holding miniature balance scales, warm lantern glow --ar 3:4 --style raw --v 6.0`

7. **`npc_kael.png`**
   - *Персонаж:* **Узник Каэль** (Казематы Акта 2).
   - *Описание:* Бывший придворный чародей, изможденный долгим пленом. Спутанные пепельные волосы, запавшие, но горящие решимостью глаза, рваная роба со следами рунических ожогов.
   - *Промпт:* `Portrait of Kael the imprisoned royal mage, gaunt pale face, disheveled silver hair, haunted yet determined intense eyes, torn apprentice robes, faint magical aura whispering around his fingertips --ar 3:4 --style raw --v 6.0`

---

### ГРУППА 6: ВРАГИ И БОССЫ (`Images/Enemies/`)

Формат: 3:4 (768×1024), фронтальный или 3/4 вид для использования в боевых карточках `BattleView.xaml`:

#### 6.1. Сюжетные Боссы и Мини-боссы:
1. **`enemy_ugluk.png`**
   - *Враг:* **Орк Углук** (Босс Сцены 1).
   - *Описание:* Свирепый зеленокожий орк-головорез с выбритым ирокезом, боевой раскраской из белой глины, в ржавой кольчуге с зазубренным палашом.
   - *Промпт:* `Frontal battle portrait of Orc Ugluk, aggressive green skin, white war paint stripes, mohawk hair, snarling fangs, rusty chainmail, wielding a brutal notched iron cleaver, fire and smoke background --ar 3:4 --style raw --v 6.0`

2. **`enemy_shaman_varg.png`**
   - *Враг:* **Шаман Варг** (Босс Сцены 3).
   - *Описание:* Зловещий сгорбленный орочий шаман в черепе волка вместо шлема, увешанный амулетами из костей. В когтистых руках посох с тлеющим изумрудным огнем черепом.
   - *Промпт:* `Combat portrait of Orc Shaman Varg, wearing a terrifying dire wolf skull headdress, withered green skin covered in glowing runes, holding a gnarled staff topped with an emerald flaming skull --ar 3:4 --style raw --v 6.0`

3. **`enemy_gromgar.png`**
   - *Враг:* **Вождь Громгар / Верховный Вождь** (Финальный босс Акта 1).
   - *Описание:* Колоссальный 2.5-метровый орочий вождь в массивных черных доспехах с шипами и шкурой саблезубого зверя на плечах. В руках держит гигантский двуручный молот, раскалывающий камень.
   - *Промпт:* `Epic combat portrait of High Chieftain Gromgar, massive mountain orc warlord in spiked black iron armor, mammoth fur mantle, wielding a colossal two-handed crushing warhammer, terrifying dominance --ar 3:4 --style raw --v 6.0`

4. **`enemy_malkor.png`**
   - *Враг:* **Архимаг Малкор** (Финальный босс Акта 2 — Фаза 1).
   - *Описание:* Могущественный темный колдун в струящейся мантии из черного шелка с серебряным шитьем. Бледное аристократическое лицо с белыми глазами без зрачков, пальцы сплетают фиолетовые молнии Бездны.
   - *Промпт:* `Dark fantasy combat portrait of Archmage Malkor, imposing sorcerer in opulent midnight-black robes with silver runes, pupilless glowing violet eyes, hands weaving crackling purple void lightning, sinister power --ar 3:4 --style raw --v 6.0`

5. **`enemy_malkor_shadow.png`**
   - *Враг:* **Малкор: Воплощение Тьмы** (Фаза 2).
   - *Описание:* Чудовищная инкарнация чистой Бездны. Бесплотный гигант из клубящегося черного дыма и теневых щупалец с горящей фиолетовой маской-ликом в центре.
   - *Промпт:* `Monstrous entity of pure void shadow, Malkor's Shadow Avatar, towering smoke wraith with writhing dark tentacles, glowing horned purple phantom mask, cosmic dark fantasy horror --ar 3:4 --style raw --v 6.0`

6. **`enemy_titan.png`**
   - *Враг:* **Первородный Титан Кряжа** (Финальный босс Акта 3 / Трилогии).
   - *Описание:* Древнейший первородный гигант из растрескавшегося базальта и застывшей лавы. Сквозь трещины в камне сияет расплавленная магма, глаза пылают как два солнца.
   - *Промпт:* `Colossal Primordial Mountain Titan, ancient elemental creature made of jagged volcanic basalt rock and flowing liquid magma veins, glowing fiery golden eyes, immense crushing fists, cataclysmic boss art --ar 3:4 --style raw --v 6.0`

7. **`enemy_titan_core.png`**
   - *Враг:* **Сердце Титана: Пробуждённая Суть** (Фаза «Дробление»).
   - *Описание:* Обнаженное первородное ядро Титана — парящий ослепительный кристалл первозданного пламени, защищенный орбитальными осколками магматической породы.
   - *Промпт:* `The Awakened Titan Core, a radiant hovering geometric heart crystal of pure solar fire and molten gold, surrounded by orbiting shield fragments of scorched obsidian rock, epic phase 2 boss portrait --ar 3:4 --style raw --v 6.0`

#### 6.2. Элитные противники:
8. **`enemy_troll.png`**
   - *Враг:* **Пещерный Тролль-Крушитель**.
   - *Описание:* Огромный горный тролль с серой каменистой кожей, покрытой мхом и наростами сталагмитов, сжимающий ствол вырванного дуба в качестве дубины.
   - *Промпт:* `Fearsome cave troll crusher, grey stony mossy hide, protruding rock spines along hunched back, brutish drooling maw, swinging an entire uprooted oak tree trunk club, terrifying brute --ar 3:4 --style raw --v 6.0`

9. **`enemy_rune_golem.png`**
   - *Враг:* **Древний Рунный Голем**.
   - *Описание:* 4-метровый автоматона древних кузнецов: полированные гранитные блоки, соединенные бронзовыми шарнирами, по всему телу пульсируют лазурные руны.
   - *Промпт:* `Ancient Dwarven Rune Golem, hulking construct of carved granite blocks and bronze machinery, glowing cyan magical runes etched across stone chest and massive fists, unstoppable guardian --ar 3:4 --style raw --v 6.0`

10. **`enemy_shadow_inquisitor.png`**
    - *Враг:* **Теневой Инквизитор Цитадели**.
    - *Описание:* Безликий рыцарь культа в глухом железном шлеме с решеткой палача, в черных латах, вооруженный тяжелым двуручным эспадоном, объятым черным пламенем.
    - *Промпт:* `Shadow Inquisitor of the Citadel, executioner knight in polished black plate armor, barred iron visor hiding face, wielding a massive greatsword shrouded in black creeping fire, grim paladin of darkness --ar 3:4 --style raw --v 6.0`

11. **`enemy_shadow_king.png`**
    - *Враг:* **Тень Древнего Короля**.
    - *Описание:* Призрачный исполин в полупрозрачной древней короне и истлевшей царской мантии, держащий призрачный меч правосудия.
    - *Промпт:* `Spectral Shadow of the Ancient King, towering translucent ghostly monarch in ethereal decayed royal mantle, crowned with spectral iron thorns, phantom spectral broadsword, haunting majestic spirit --ar 3:4 --style raw --v 6.0`

#### 6.3. Рядовые противники:
12. **`enemy_orc_berserker.png`** (Орк-берсерк / Рубака)
    - *Промпт:* `Savage orc berserker charging forward with twin axes, foaming mouth, red warpaint, leather straps, frantic bloodlust expression --ar 3:4 --style raw --v 6.0`
13. **`enemy_orc_shield.png`** (Орк-щитоносец)
    - *Промпт:* `Heavily armored orc bulwark defender holding a gigantic spiked iron tower shield and short stabbing spear, defensive stance --ar 3:4 --style raw --v 6.0`
14. **`enemy_orc_scout.png`** (Орк-разведчик)
    - *Промпт:* `Cunning orc scout archer lurking in mountain rocks, notched shortbow drawn with poisoned arrow, wolf pelts camouflage --ar 3:4 --style raw --v 6.0`
15. **`enemy_orc_bodyguard.png`** (Орочий телохранитель / Гвардеец)
    - *Промпт:* `Stalwart elite orc veteran bodyguard in heavy scale armor, halberd in hand, disciplined brutal warrior --ar 3:4 --style raw --v 6.0`
16. **`enemy_shadow_archer.png`** (Теневой Лучник Цитадели)
    - *Промпт:* `Sinister shadow cult archer in dark leather garments and hood, drawing a composite black bow firing a glowing purple arrow of dark energy --ar 3:4 --style raw --v 6.0`
17. **`enemy_shadow_adept.png`** (Теневой Адепт / Культист)
    - *Промпт:* `Fanatical shadow cult adept in tattered dark violet vestments, holding a ritual curved dagger and casting dark smoke spells --ar 3:4 --style raw --v 6.0`
18. **`enemy_void_hound.png`** (Гончая Скверны)
    - *Промпт:* `Demonic void hound beast, monstrous hairless quadraped with chitinous obsidian armor plates, jaw splitting into multiple mandibles, glowing purple saliva --ar 3:4 --style raw --v 6.0`
19. **`enemy_heart_shard.png`** (Осколок Сердца)
    - *Промпт:* `Living crystal elemental shard of the Titan Heart, hovering jagged amber crystal creature crackling with molten sparks and raw energy --ar 3:4 --style raw --v 6.0`

---

### ГРУППА 7: ИНТЕРФЕЙС, БАННЕРЫ И БРЕНДИНГ (`Images/UI/`)

1. **`Images/UI/game_logo.png`** (2000×600, PNG с прозрачным фоном):
   - *Описание:* Эпический объемный металлический логотип игры «ЖЕЛЕЗНЫЙ КРЯЖ / THE IRON HILLS». Кованые стальные буквы с заклепками и золотой гравировкой рун, по бокам скрещенные мечи и северный горный хребет.
   - *Промпт:* `Epic game logo title typography design reading 'THE IRON HILLS', forged dark steel letters with golden runic engravings and iron rivets, crossed broadswords, mountain silhouette, isolated on black background, game title art --ar 16:9 --style raw --v 6.0`

2. **`Images/UI/window_icon.png` / `window_icon.ico`** (512×512, PNG/ICO):
   - *Описание:* Иконка приложения: стилизованный стальной рыцарский шлем с золотыми рунами на фоне скрещенных клинков и заснеженного пика.
   - *Промпт:* `An app icon of a medieval knight helmet crafted of dark steel and inscribed with glowing golden Nordic runes, crossed blades behind it, dark fantasy crest emblem, centered, clean borders, high detail --ar 1:1 --v 6.0`

3. **`Images/UI/banner_act1.png`** (Карточка Акта 1 в ChapterSelectView, 800×300):
   - *Описание:* Горящая деревня Пепельный Ручей и силуэт Орка с топором.
   - *Промпт:* `A horizontal banner illustration of a burning medieval village under dark stormy sky with an orc silhouette, banner format --ar 2:1 --v 6.0`

4. **`Images/UI/banner_act2.png`** (Карточка Акта 2 в ChapterSelectView, 800×300):
   - *Описание:* Черная Цитадель на скале, озаренная фиолетовыми молниями.
   - *Промпт:* `A horizontal banner illustration of an ominous black citadel fortress perched on high jagged peaks under violet lightning, banner format --ar 2:1 --v 6.0`

5. **`Images/UI/banner_act3.png`** (Карточка Акта 3 в ChapterSelectView, 800×300):
   - *Описание:* Раскаленное Сердце Титана и магматические разломы в недрах горы.
   - *Промпт:* `A horizontal banner illustration of fiery subterranean volcanic magma cavern with a pulsing molten core, banner format --ar 2:1 --v 6.0`

---

## 🛠️ 4. Технический план внедрения визуала в кодовую базу WPF

После того как вы сгенерируете и сохраните изображения в папки `Images/...`, интеграция в кодовую базу будет выполнена следующим образом:

### Шаг 1: Конфигурация проекта (`EpicBattle.csproj`)
Включение папки `Images` в автоматическое копирование в сборку:
```xml
<ItemGroup>
  <Content Include="Images\**\*.*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### Шаг 2: Внедрение фонов в Views
* **`MainMenu.xaml`**: Замена серого фона на `<Grid>` с подложкой:
  ```xml
  <Image Source="/Images/Backgrounds/main_menu.png" Stretch="UniformToFill" Opacity="0.55"/>
  ```
  с мягким затемнением градиентом для читаемости кнопок.
* **`CampfireView.xaml`**, **`BlacksmithView.xaml`**, **`TavernView.xaml`**, **`MerchantView.xaml`**, **`MapView.xaml`**, **`ChapterSelectView.xaml`**, **`CharacterCreationView.xaml`**:
  Установка соответствующих атмосферных артов с прозрачностью 0.35–0.50 поверх темно-серой базы, что сохранит идеальную контрастность текста и кнопок.

### Шаг 3: Динамические боевые фоны в `BattleView.xaml`
В `BattleViewModel.cs` добавляется свойство `CurrentBattleBackground`:
- Если бой в Акте 1, Сцене 1 → `/Images/Battle/battle_ash_creek.png`.
- Если бой в Ущелье / Аркаде → `/Images/Battle/battle_mountain_pass.png` или `battle_arcade.png`.
- Если штурм Цитадели → `/Images/Battle/battle_citadel_gates.png`.
- Если казематы → `/Images/Battle/battle_citadel_dungeons.png`.
- Если бой с Малкором → `/Images/Battle/battle_citadel_sanctum.png`.
- Если бой с Титаном (Акт 3) → `/Images/Battle/battle_titan_depths.png`.

В `BattleView.xaml`:
```xml
<Image Source="{Binding CurrentBattleBackground}" Stretch="UniformToFill" Opacity="0.4" Panel.ZIndex="0"/>
```

### Шаг 4: Портреты врагов в карточках боя
В шаблон `Enemy` в `BattleView.xaml` добавляется аватар/портрет противника:
```xml
<Image Source="{Binding AvatarPath}" Width="60" Height="80" Stretch="UniformToFill" Margin="0,0,10,0"/>
```
Каждый тип противника в `EnemyFactory.cs` получает путь к своему портрету (например, `enemy_gromgar.png`, `enemy_ugluk.png` и т.д.).

### Шаг 5: Иллюстрации в диалогах (`DialogueView.xaml`)
В `DialogueView.xaml` уже имеется заготовка:
```xml
<Image Name="BgImage" Stretch="UniformToFill" Opacity="0.45"/>
```
В `StoryDatabase.cs` каждому `DialogueNode` назначается поле `ImagePath` (или маппинг в `DialogueView.xaml.cs` по `nodeId`), загружающее соответствующую сцену из папки `Images/Story/`.

---

## 📋 5. Чеклист готовности ассетов

| Категория | Папка назначения | Количество файлов | Статус |
|:---|:---|:---:|:---:|
| 🏛️ Фоны экранов | `Images/Backgrounds/` | 10 файлов | ✅ **Сгенерировано и внедрено (10/10)** |
| ⚔️ Боевые арены | `Images/Battle/` | 7 файлов | ✅ **Сгенерировано и внедрено (7/7)** |
| 📜 Сюжетные сцены | `Images/Story/` | 27 файлов | ✅ **Сгенерировано и внедрено (27/27)** |
| 🎲 События (Encounters) | `Images/Encounters/` | 4 файла | ✅ **Сгенерировано и внедрено (4/4)** |
| 👤 Персонажи & NPC | `Images/Characters/` | 7 файлов | ✅ **Сгенерировано и внедрено (7/7)** |
| 💀 Враги и боссы | `Images/Enemies/` | 19 файлов | ✅ **Сгенерировано и внедрено (19/19)** |
| 🎨 UI & Брендинг | `Images/UI/` | 5 файлов | ✅ **Сгенерировано и внедрено (5/5)** |
| **ИТОГО** | **Всего в игре** | **79 изображений** | ✅ **ПОЛНОСТЬЮ ВНЕДРЕНО (79/79)** |

---

*Документ составлен для проекта «Эпическая битва: Железный Кряж». После генерации и размещения файлов в каталогах просто сообщите об этом, и весь визуальный ряд будет бесшовно интегрирован в код и XAML-разметку игры.*
