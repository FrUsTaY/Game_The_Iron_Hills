using System.Collections.Generic;

namespace EpicBattle.Models
{
    public class DialogueNode
    {
        public string Id { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public List<DialogueChoice> Choices { get; set; } = new List<DialogueChoice>();
    }

    public class DialogueChoice
    {
        public string Text { get; set; } = string.Empty;
        public string NextNodeId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // "StartBattle", "AddLoot", etc.
        public string ActionParam { get; set; } = string.Empty; // Modifiers for battle or specific loot
        public string RequiredFlag { get; set; } = string.Empty; // Optional flag required to show this choice
    }

    public static class StoryDatabase
    {
        public static Dictionary<string, DialogueNode> Nodes = new Dictionary<string, DialogueNode>
        {
            // --- Сцена 1: Встреча на пепелище ---
            {
                "Start", new DialogueNode
                {
                    Id = "Start",
                    SpeakerName = "Автор",
                    Text = "Разрушенная деревня Пепельный Ручей. Идет мелкий дождь, над сгоревшими домами поднимается дым. Герой выходит на центральную площадь и видит Углука — орочьего надзирателя.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Подойти к орку", NextNodeId = "Scene1_Ugluk" }
                    }
                }
            },
            {
                "Scene1_Ugluk", new DialogueNode
                {
                    Id = "Scene1_Ugluk",
                    SpeakerName = "Орк Углук",
                    Text = "Р-р-рах! Ещё один человечишка вернулся на гарь! Эта земля теперь принадлежит Орде Громгара. Сложи оружие, и, может быть, я убью тебя быстро!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Ветеран] Ты пожалеешь об этих словах, орк. Мой клинок видел сотни таких как ты.",
                            NextNodeId = "Scene1_Aggressive",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice {
                            Text = "[Изгой-маг] (Сгустить ману в ладони) Глупец. Твой топор не спасет тебя от тайного огня.",
                            NextNodeId = "Scene1_Pragmatic",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice {
                            Text = "[Наемник] Сколько Громгар тебе платит? Я убью тебя бесплатно.",
                            NextNodeId = "Scene1_Aggressive",
                            RequiredFlag = "Class_Наемник"
                        },
                        new DialogueChoice {
                            Text = "[Прагматик] Зачем вы жжёте деревни? Что вам нужно?",
                            NextNodeId = "Scene1_Pragmatic"
                        },
                        new DialogueChoice {
                            Text = "[Агрессия] Твои сородичи уже заплатили кровью. Ты следующий!",
                            NextNodeId = "Scene1_Aggressive"
                        },
                        new DialogueChoice {
                            Text = "[Внезапный удар] (Молча использовать Руну Огня)",
                            NextNodeId = "PostBattle1",
                            Action = "StartBattle",
                            ActionParam = "Surprise"
                        }
                    }
                }
            },
            {
                "Scene1_Pragmatic", new DialogueNode
                {
                    Id = "Scene1_Pragmatic",
                    SpeakerName = "Орк Углук",
                    Text = "Хе-хе... Нам нужны не ваши вшивые дома, а то, что скрыто под ними! Вождь ищет Рунный Камень! А теперь умри!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Вступить в бой",
                            NextNodeId = "PostBattle1",
                            Action = "StartBattle",
                            ActionParam = "Pragmatic"
                        }
                    }
                }
            },
            {
                "Scene1_Aggressive", new DialogueNode
                {
                    Id = "Scene1_Aggressive",
                    SpeakerName = "Орк Углук",
                    Text = "Ты смеешь рычать на меня, слизень?! Я вырву твое сердце!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Вступить в бой",
                            NextNodeId = "PostBattle1",
                            Action = "StartBattle",
                            ActionParam = "Aggressive"
                        }
                    }
                }
            },

            // --- Сцена 1.5: После боя — Допрос Углука ---
            {
                "PostBattle1", new DialogueNode
                {
                    Id = "PostBattle1",
                    SpeakerName = "Автор",
                    Text = "Углук тяжело ранен, его топор отброшен в сторону. Он сидит, прижавшись спиной к обгоревшему столбу, и сплевывает кровь. Воин стоит над ним с обнаженным мечом.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Послушать орка", NextNodeId = "PostBattle1_Dialogue" }
                    }
                }
            },
            {
                "PostBattle1_Dialogue", new DialogueNode
                {
                    Id = "PostBattle1_Dialogue",
                    SpeakerName = "Орк Углук",
                    Text = "Хр-р-р... Твоя сталь оказалась острее, человечишка. Ну давай, добей! Громгар все равно сожжет эту землю дотла, а из твоей черепушки сделает кубок!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Милосердие] Ты будешь жить, если скажешь зачем Громгару Рунный Камень.",
                            NextNodeId = "Interrogate_Mercy"
                        },
                        new DialogueChoice {
                            Text = "[Жестокий допрос] (Надавить сапогом на рану) Говори сейчас же!",
                            NextNodeId = "Interrogate_Ruthless"
                        },
                        new DialogueChoice {
                            Text = "[Расправа] Ты ответишь за всех, кого сожгла твоя Орда. (Вонзить меч)",
                            NextNodeId = "EndChapter_Loot",
                            Action = "ExecuteOrc"
                        }
                    }
                }
            },
            {
                "Interrogate_Mercy", new DialogueNode
                {
                    Id = "Interrogate_Mercy",
                    SpeakerName = "Орк Углук",
                    Text = "Ха! Думаешь, я предатель? Камень... он в подземельях старой Цитадели. Вождь хочет пробудить его силу. Я сказал все! Теперь отпусти или убей!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Отпустить орка",
                            NextNodeId = "EndChapter"
                        },
                        new DialogueChoice {
                            Text = "Оглушить и забрать его вещи",
                            NextNodeId = "EndChapter",
                            Action = "StunAndLoot"
                        }
                    }
                }
            },
            {
                "Interrogate_Ruthless", new DialogueNode
                {
                    Id = "Interrogate_Ruthless",
                    SpeakerName = "Орк Углук",
                    Text = "А-а-грх! Проклятый слизень! Ладно, ладно! Громгар везет карту к старой Цитадели! У передового отряда в лесу есть копия!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Добить орка и отправиться в лес",
                            NextNodeId = "EndChapter",
                            Action = "Ruthless"
                        }
                    }
                }
            },
            {
                "EndChapter_Loot", new DialogueNode
                {
                    Id = "EndChapter_Loot",
                    SpeakerName = "Автор",
                    Text = "Элрик молча добивает орка. Обыскав труп, он находит Карту засады в лесу и Ключ от сундука в деревне.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Подойти к развалинам ратуши", NextNodeId = "Scene2_Start" }
                    }
                }
            },
            {
                "EndChapter", new DialogueNode
                {
                    Id = "EndChapter",
                    SpeakerName = "Автор",
                    Text = "Оставив орка позади, Элрик осматривает пепелище своей деревни.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Подойти к развалинам ратуши", NextNodeId = "Scene2_Start" }
                    }
                }
            },

            // --- Сцена 2: Диалог со Старейшиной Бранном ---
            {
                "Scene2_Start", new DialogueNode
                {
                    Id = "Scene2_Start",
                    SpeakerName = "Автор",
                    Text = "Элрик подходит к развалинам ратуши и освобождает связанного Старейшину Бранна. Тот тяжело дышит, его одежда в копоти, но в глазах горит надежда, когда он узнает ветерана.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Выслушать старейшину", NextNodeId = "Scene2_Dialogue1" }
                    }
                }
            },
            {
                "Scene2_Dialogue1", new DialogueNode
                {
                    Id = "Scene2_Dialogue1",
                    SpeakerName = "Старейшина Бранн",
                    Text = "(Откашливаясь от дыма) Элрик! Хвала богам, ты жив... Они пришли на рассвете. Нам нечем было защищаться! Орки искали не еду и не золото. Они перерыли мой дом и забрали Рунический Фолиант — древнюю рукопись наших предков!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Благородный] Главное, что вы живы. Я верну этот фолиант и заставлю их заплатить.",
                            NextNodeId = "Scene2_Noble",
                            Action = "Brann_Noble"
                        },
                        new DialogueChoice {
                            Text = "[Прагматик] Зачем оркам книга? Что в ней такого ценного?",
                            NextNodeId = "Scene2_Pragmatic",
                            Action = "Brann_Pragmatic"
                        },
                        new DialogueChoice {
                            Text = "[Наемник] Я больше не на службе. Что я получу взамен, если ринусь за ними?",
                            NextNodeId = "Scene2_Mercenary",
                            Action = "Brann_Mercenary"
                        }
                    }
                }
            },
            {
                "Scene2_Noble", new DialogueNode
                {
                    Id = "Scene2_Noble",
                    SpeakerName = "Старейшина Бранн",
                    Text = "Твое сердце все так же благородно, Элрик. Возьми это зелье здоровья — оно тебе пригодится. Отряд с фолиантом ушел в сторону Темного Леса к их лагерю.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Отправиться в Темный Лес", NextNodeId = "EndScene2" }
                    }
                }
            },
            {
                "Scene2_Pragmatic", new DialogueNode
                {
                    Id = "Scene2_Pragmatic",
                    SpeakerName = "Старейшина Бранн",
                    Text = "В ней описаны печати Старой Цитадели! Если Громгар расшифрует их, он сможет открыть врата и призвать древнее горное пламя. Мы должны перехватить их до того, как они доберутся до Главного Лагеря!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Устроить засаду на тракте", NextNodeId = "EndScene2" }
                    }
                }
            },
            {
                "Scene2_Mercenary", new DialogueNode
                {
                    Id = "Scene2_Mercenary",
                    SpeakerName = "Старейшина Бранн",
                    Text = "(Вздыхает с горечью) Время никому не идет на пользу, Элрик... Хорошо. В подвале ратуши спрятан тайник моего рода. Там лежит старинная Руна Защиты. Забери её, если спасешь наш народ.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Принять сделку и отправиться в путь", NextNodeId = "EndScene2" }
                    }
                }
            },
            {
                "EndScene2", new DialogueNode
                {
                    Id = "EndScene2",
                    SpeakerName = "Автор",
                    Text = "Получен Квест: «Украденные тайны». \nЭлрик оставляет сожженную деревню позади, направляясь в Темный Лес. Впереди его ждут новые битвы.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Войти в Темный Лес", NextNodeId = "Scene3_Start" }
                    }
                }
            },

            // --- Сцена 3: Засада в Тёмном Лесу ---
            {
                "Scene3_Start", new DialogueNode
                {
                    Id = "Scene3_Start",
                    SpeakerName = "Автор",
                    Text = "Густой, туманный Тёмный Лес. Вековые деревья закрывают солнце. Элрик нагоняет отряд орков у заброшенного путеводного камня.\n\nОтряд состоит из двух рядовых орков и их командира — Шамана Варга. Шаман держит в руках светящийся Рунический Фолиант и пытается прочесть заклинание, пока воины отдыхают у костра.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Подойти ближе", NextNodeId = "Scene3_Varg" }
                    }
                }
            },
            {
                "Scene3_Varg", new DialogueNode
                {
                    Id = "Scene3_Varg",
                    SpeakerName = "Шаман Варг",
                    Text = "(Хриплым, дребезжащим голосом) Остановитесь! Запах чешуи и старого металла... Воин из деревни пришел за своей книжицей? Ты опоздал, человечишка! Страницы уже шепчут мне свои тайны!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Тактика] (Молча использовать «Руну Огня» по костру)",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "Varg_Tactical"
                        },
                        new DialogueChoice {
                            Text = "[Хитрость] Ты держишь книгу вверх ногами, шаман. Орки слишком тупы для рун.",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "Varg_Provocative"
                        },
                        new DialogueChoice {
                            Text = "[Прямой вызов] Положи фолиант и обнажи оружие. Посмотрим, так ли ты силен!",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "Varg_Honorable"
                        }
                    }
                }
            },

            // --- Сцена 3: После боя ---
            {
                "PostBattle_Scene3", new DialogueNode
                {
                    Id = "PostBattle_Scene3",
                    SpeakerName = "Автор",
                    Text = "Последний из отряда падает замертво. Элрик подходит к телу Шамана Варга и забирает Рунический Фолиант.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Осмотреть Фолиант", NextNodeId = "Scene3_InspectTome" }
                    }
                }
            },
            {
                "Scene3_InspectTome", new DialogueNode
                {
                    Id = "Scene3_InspectTome",
                    SpeakerName = "Автор",
                    Text = "Вы осматриваете книгу. (Состояние книги зависит от скорости победы над шаманом).",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Оценить ущерб", NextNodeId = "Scene3_TomeResult", Action = "CheckTomeIntact" }
                    }
                }
            },
            {
                "Scene3_TomeIntact", new DialogueNode
                {
                    Id = "Scene3_TomeIntact",
                    SpeakerName = "Элрик",
                    Text = "Книга цела. Страницы не тронуты пламенем, а переплет невредим. Бранн сможет быстро расшифровать эти печати.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Продолжить путь", NextNodeId = "Scene4_Start" }
                    }
                }
            },
            {
                "Scene3_TomeDamaged", new DialogueNode
                {
                    Id = "Scene3_TomeDamaged",
                    SpeakerName = "Элрик",
                    Text = "Проклятье... Шаман успел сжечь часть страниц перед смертью. Книга сильно повреждена. Придется искать дополнительные расшифровки в Старой Цитадели.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Продолжить путь", NextNodeId = "Scene4_Start" }
                    }
                }
            },

            // --- Сцена 4: Кульминация — Врата Старой Цитадели ---
            {
                "Scene4_Start", new DialogueNode
                {
                    Id = "Scene4_Start",
                    SpeakerName = "Автор",
                    Text = "Главный зал заброшенной горной Цитадели. В центре — гигантская древняя руническая арка (Врата Горного Пламени), пульсирующая огненно-багровым светом. Вождь Громгар — гигантский орк в тяжелых доспехах из черного железа, с двуручным молотом, стоит у алтаря и запечатывает ритуал. Вокруг лежат поверженные защитники цитадели. Элрик вбегает в зал. Громгар медленно поворачивается, его глаза горят магическим пламенем.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Сделать шаг вперед", NextNodeId = "Scene4_Gromgar" }
                    }
                }
            },
            {
                "Scene4_Gromgar", new DialogueNode
                {
                    Id = "Scene4_Gromgar",
                    SpeakerName = "Вождь Громгар",
                    Text = "(Голос грохочет, как обвал в горах) Ты... Человеческий червь, который уничтожил моих лучших воинов в лесу и на тракте. Ты опоздал, воин! Печати взломаны. Сила Горного Пламени почти принадлежит Орде!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Идеология / Честь воина] Эта сила погубит тебя самого, Громгар! Власть над огнем не сделает твой народ великим, она сделает вас рабами древнего зла!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Honor"
                        },
                        new DialogueChoice {
                            Text = "[Использование знаний / Взлом ритуала] (Произнести противоположную руну из Фолианта) Аз-Кхар Тему-Ра! Твой ритуал незавершен, Громгар!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Lore",
                            RequiredFlag = "HasTomeIntact"
                        },
                        new DialogueChoice {
                            Text = "[Угроза / Беспощадный мечник] Мне плевать на твою Орду и твой ритуал. Ты сожег мою деревню. Сегодня твоя голова украсит ворота Пепельного Ручья.",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Ruthless"
                        }
                    }
                }
            },

            // --- Сцена 4: После победы над Громгаром ---
            {
                "PostBattle_Scene4", new DialogueNode
                {
                    Id = "PostBattle_Scene4",
                    SpeakerName = "Автор",
                    Text = "Громгар повержен и тяжело дыша падает на колени. Врата Горного Пламени начинают нестабильно пульсировать, выходя из-под контроля. Решение, которое вы примете сейчас, изменит судьбу этих земель.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Светлый финал] Запечатать Врата своей магией и спасти королевство.",
                            NextNodeId = "Ending_Light"
                        },
                        new DialogueChoice {
                            Text = "[Темный финал] Добить Громгара и забрать силу Врат себе.",
                            NextNodeId = "Ending_Dark"
                        },
                        new DialogueChoice {
                            Text = "[Нейтральный финал] Пощадить Громгара и заключить союз. Орда отступает.",
                            NextNodeId = "Ending_Neutral",
                            RequiredFlag = "HonorGromgar"
                        }
                    }
                }
            },
            {
                "Ending_Light", new DialogueNode
                {
                    Id = "Ending_Light",
                    SpeakerName = "Светлый финал",
                    Text = "Элрик использует остатки своей магии и энергии рун, чтобы навсегда уничтожить Врата. Цитадель рушится, погребая под собой планы Орды. Герой спасает земли людей и возвращается домой легендарным защитником.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Завершить игру", Action = "Ending_Light" }
                    }
                }
            },
            {
                "Ending_Dark", new DialogueNode
                {
                    Id = "Ending_Dark",
                    SpeakerName = "Темный финал",
                    Text = "Жажда силы берет верх. Элрик добивает Громгара и поглощает мощь Горного Пламени. Обретя нечеловеческое могущество, он становится новым Владыкой Цитадели, перед которым преклоняются как остатки орков, так и люди.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Завершить игру", Action = "Ending_Dark" }
                    }
                }
            },
            {
                "Ending_Neutral", new DialogueNode
                {
                    Id = "Ending_Neutral",
                    SpeakerName = "Нейтральный финал",
                    Text = "Элрик предлагает Громгару остановить ритуал и увести Орду назад в горы, заключив новый мирный договор. Хрупкий мир сохранен. Громгар отступает, уважая силу и мудрость Воина.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Завершить игру", Action = "Ending_Neutral" }
                    }
                }
            }
        };
    }
}