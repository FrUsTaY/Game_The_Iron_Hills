# 🎬 Видео-концепция и база промптов для игры «Эпическая битва: Железный Кряж» (The Iron Hills)

Данный документ содержит готовые промпты, тайминги, тексты русской озвучки и визуальные ориентиры для генерации двух ключевых видеороликов при запуске игры через современные видео-нейросети (Kling AI, Runway Gen-3, Sora, Minimax Hailuo, Luma Dream Machine, Hedra, Pika).

---

## 🧭 Общие рекомендации по генерации

1. **Ограничение длины видео:** 9–10 секунд на одно поколение.
2. **Формат кадра (Aspect Ratio):** `--ar 16:9` (1920×1080) — идеально подходит для WPF-интерфейса и полноэкранного воспроизведения.
3. **Голосовая русская озвучка прямо в видео:**
   - Для нейросетей, поддерживающих одновременную генерацию голоса и звука (Kling Audio, Minimax, Sora, Hedra), в промпте явно указывается блок `[VOICEOVER: Russian]` с точной репликой и тембром.
   - Текст озвучки строго оптимизирован: не более 15–18 слов на отрезок (темп 2 слова/сек), чтобы диктор успел спокойно, эпично и без спешки произнести фразу за 7–8 секунд до плавного затухания ролика.
4. **Склейка второго видео:** Ролик 2 генерируется в виде 4 последовательных фрагментов по 10 секунд (всего 40 секунд). После генерации их достаточно объединить в любом видеоредакторе (CapCut, Premiere, DaVinci или через FFmpeg).

---

# 📀 ВИДЕО 1: Интро Студии — «FrUsTaY PRESENTS»

* **Хронометраж:** 8–10 секунд
* **Назначение:** Первое видео при старте игры до главного меню. Создает премиальный брендинг автора/разработчика.
* **Стиль:** Эпический дарк-фэнтези логотип, ковка стали, древние руны, раскаленный металл, кинематографичный свет.

### 📋 Промпт для нейросети (скопируйте целиком в строку ввода):
```text
Dark fantasy cinematic studio logo reveal. Complete darkness fades in to reveal the glowing 3D embossed metallic text "FrUsTaY PRESENTS" forged from heavy dark iron and incandescent glowing molten gold. Fiery sparks, floating ember particles and hot steam drift across the screen. Intricate glowing magical Nordic runes subtly pulse on the rough dark stone slab background. Smooth slow cinematic camera push-in. Professional 8k CGI movie opener, volumetric rim lighting, dramatic smoke, smooth fade-in from black at 0s, peak glow at 5s, smooth cinematic fade-out to black at 9s.
Audio: deep resonant cinematic anvil hammer strike, heavy sub-bass braam boom, crackling fire embers, mysterious low choir swell.
```

### 🗣️ Параметры звука (Sound & SFX):
* **Озвучка:** Без закадрового голоса (только эпичный кинематографический саунд-дизайн).
* **Звуковые эффекты:** Мощный глухой удар кузнечного молота по наковальне в начале (`0.5s`), нарастающий низкий бас (`sub-bass braam`), потрескивание раскаленных углей и шепот древнего мужского хора на заднем плане, затухание в тишину к 9–10 секунде.

### 🎯 Что должно получиться в итоге (Чек-лист для проверки):
1. **0–2 секунды:** Из глубокой тьмы плавно проявляется массивная текстура темного гранита, усыпанного искрами.
2. **3–6 секунд:** В центре экрана ярко разгораются массивные рельефные металлические буквы **«FrUsTaY PRESENTS»**. Края букв светятся раскаленной золотистой лавой или расплавленной сталью. Камера медленно наплывает вперед.
3. **7–10 секунд:** Жар плавно остывает до благородного холодного стального блеска, дым рассеивается, и картинка плавно уходит в черный экран (Fade to Black).

---

# 📜 ВИДЕО 2: Пролог и Лор Игры (Предыстория Железного Кряжа)

Видеоролик рассказывает о происхождении мира, древнем Первородном Титане, восстании культа теней и начале вторжения. Разбито на 4 этапа по 9–10 секунд.

---

### 🔹 ЭТАП 1: Заточение Первородного Титана (00:00 — 00:10)
* **Тема:** Древняя эпоха, исполин в недрах гор, рунические цепи.
* **Локация:** Пылающие магматические пещеры глубоко под Железным Кряжем.

#### 📋 Промпт для нейросети:
```text
Cinematic dark fantasy establishing shot inside the colossal subterranean abyss of the Iron Hills. A gigantic ancient Primordial Titan made of cracked obsidian stone and blazing white magma veins sits slumbering, bound by massive glowing blue runic iron chains. Slow drifting downward camera pan showing the scale of the colossal beast. Volumetric steam, drifting ash, eerie magical cyan glow contrasting with fiery orange lava river below. Hyper-realistic matte painting, 8k resolution, Peter Jackson epic fantasy tone.
[VOICEOVER: Deep dramatic Russian male narrator, epic grave voice]:
"В ледяных недрах Железного Кряжа веками спал Первородный Титан. Короли сковали его пламя древними рунами... но ничто не длится вечно."
```

#### 🗣️ Текст русской озвучки:
> *«В ледяных недрах Железного Кряжа веками спал Первородный Титан. Короли сковали его пламя древними рунами... но ничто не длится вечно.»*  
> *(Длина: 19 слов, ~7.5 секунд, спокойный, низкий, повествовательный тон мудрого летописца).*

#### 🎯 Что должно получиться в итоге:
* Камера медленно опускается сверху вниз по циклопической подземной пещере.
* На заднем плане виден силуэт скованного цепями каменного исполина, сквозь трещины которого сочится магма.
* Цепи покрыты светящимися синими рунами, в воздухе летают пепел и искры.
* Диктор звучит глухо и величественно, под стать древней легенде.

---

### 🔹 ЭТАП 2: Черная Цитадель и Теневой Культ Малкора (00:10 — 00:20)
* **Тема:** Пробуждение зла, архимаг Малкор, угроза Сердцу Кряжа.
* **Локация:** Базальтовая Черная Цитадель на острой скале под грозовым небом.

#### 📋 Промпт для нейросети:
```text
Epic wide cinematic tracking shot of the Black Citadel perched atop razor-sharp jagged snowy mountain peaks. Menacing dark gothic stone towers, eerie purple arcane lightning swirling in storm clouds overhead. Ominous shadowy corruption energy leaking from the fortress gates. Figure of dark archmage in black robes standing on balcony channeling necrotic violet magic. Dark fantasy grim atmosphere, Dark Souls style, hyper-detailed, slow majestic camera dolly forward.
[VOICEOVER: Dramatic Russian male narrator, foreboding sinister tone]:
"В чертогах Древней Цитадели восстал темный культ. Архимаг Малкор призвал Бездну, желая подчинить пылающее Сердце Кряжа."
```

#### 🗣️ Текст русской озвучки:
> *«В чертогах Древней Цитадели восстал темный культ. Архимаг Малкор призвал Бездну, желая подчинить пылающее Сердце Кряжа.»*  
> *(Длина: 16 слов, ~7 секунд, тревожная, зловещая интонация).*

#### 🎯 Что должно получиться в итоге:
* Панорама мрачной монолитной крепости среди заснеженных скал.
* В небе над шпилями закручивается фиолетово-грозовой вихрь темной магии Бездны.
* Из бойниц и ворот струится теневое свечение. Атмосфера надвигающейся катастрофы.
* Голос диктора передает ощущение раскрывающейся тайны и смертельной угрозы.

---

### 🔹 ЭТАП 3: Нашествие Орды и Осада Пепельного Ручья (00:20 — 00:30)
* **Тема:** Война, горящая деревня, вождь Громгар, похищение реликвии.
* **Локация:** Деревня Пепельный Ручей в огне, воины орды.

#### 📋 Промпт для нейросети:
```text
Dark fantasy battlefield scene at night with pouring cold rain mixing with blazing fire. The wooden medieval village of Ash Creek burning to the ground, collapsing timber roof, black smoke billowing. Brutal terrifying orc warriors clad in spiked rusted iron armor carrying torches and bloodied cleavers marching through the mud. A monstrous orc warlord brandishing a glowing stolen ancient parchment scroll. Intense dramatic camera tracking forward through embers. Grim, violent, cinematic lighting.
[VOICEOVER: Intense Russian male narrator, gritty dramatic tone]:
"Орда вождя Громгара обрушилась на людские земли. Пепельный Ручей сгорел дотла, а священный фолиант был похищен."
```

#### 🗣️ Текст русской озвучки:
> *«Орда вождя Громгара обрушилась на людские земли. Пепельный Ручей сгорел дотла, а священный фолиант был похищен.»*  
> *(Длина: 16 слов, ~7 секунд, жесткий, напряженный военный темп).*

#### 🎯 Что должно получиться в итоге:
* Ночной дождь, падающий на охваченные яростным пламенем бревенчатые избы пограничной деревни.
* По грязным улицам сквозь дым шествуют тяжеловооруженные свирепые орки с факелами и секирами.
* Один из предводителей держит в руках светящийся рунический свиток (фолиант).
* В звуке различимы треск горящих балок, глухие барабаны войны и решительный голос рассказчика.

---

### 🔹 ЭТАП 4: Восхождение Героя (00:30 — 00:40)
* **Тема:** Одинокий герой на пепелище, обнажение клинка, выбор судьбы.
* **Локация:** Центральная площадь Пепельного Ручья, взгляд на заснеженные вершины Кряжа.

#### 📋 Промпт для нейросети:
```text
Epic low-angle hero shot. Amidst the smoking ashes and charred ruins of the burned village, a lone weathered hero in a tattered traveler's cloak and battle-scarred steel armor steps forward. In slow motion, the hero draws a gleaming broadsword, the cold steel reflecting amber firelight and faint blue runic enchantments. The hero looks up towards the menacing snowy mountain pass of the Iron Hills. Raindrops splash on the blade. Heroic determined atmosphere, cinematic backlighting, smooth fade to black.
[VOICEOVER: Epic Russian male narrator, powerful inspiring finale]:
"Но среди пепла встает тот, кто сломает оковы судьбы. Твоя сталь решит — спасешь ли ты Кряж, или сгоришь в его огне!"
```

#### 🗣️ Текст русской озвучки:
> *«Но среди пепла встает тот, кто сломает оковы судьбы. Твоя сталь решит — спасешь ли ты Кряж, или сгоришь в его огне!»*  
> *(Длина: 18 слов, ~7.5 секунд, мощный, воодушевляющий призыв к началу игры).*

#### 🎯 Что должно получиться в итоге:
* Низкий ракурс камеры позади героя.
* В кадре видны плащ, латные наплечники и обнажение сияющего стального меча с каплями дождя.
* Герой поднимает взгляд на вздымающиеся вдали горные вершины Железного Кряжа, куда ведет дорога мести.
* Завершающая фраза звучит как вызов лично игроку, после чего кадр мягко уходит в темноту, подготавливая появление Главного Меню.

---

## 🛠️ Как объединить сгенерированные видео в игре

1. **Видео 1 (Логотип):** Сохраните результат под именем `intro_logo.mp4` в папку `Videos/` проекта.
2. **Видео 2 (Сюжетный пролог):** 
   - 4 кусочка (`stage1.mp4`, `stage2.mp4`, `stage3.mp4`, `stage4.mp4`) можно моментально объединить без потери качества бесплатной утилитой **FFmpeg** или в любом видеоредакторе (CapCut / Clipchamp на Windows):
     ```bash
     # Команда склейки через ffmpeg (если установлен):
     ffmpeg -f concat -safe 0 -i list.txt -c copy intro_story.mp4
     ```
   - Сохраните итоговый файл как `Videos/intro_story.mp4`.
3. **Воспроизведение в WPF (`The Iron Hills`):**
   - В WPF для видео используется стандартный элемент `<MediaElement>`. Мы сможем добавить полноэкранный проигрыватель перед показом `MainMenu` с возможностью пропустить видео по нажатию клавиши `Space` или `Esc`.
