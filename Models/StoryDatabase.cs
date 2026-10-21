using System;
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
            // =================================================================
            // --- АКТ 1: ОСАДА ПЕПЕЛЬНОГО РУЧЬЯ (SIEGE OF ASH CREEK) ---
            // =================================================================

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
                    Text = "Ха! Благородный дурак... Ладно! Вождь ищет святилище в ущелье. Шаман Варг проводит там ритуал пробуждения камня. Отпусти меня, и я не стану вставать у тебя на пути.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Уходи, пока я не передумал. (Отпустить орка)",
                            NextNodeId = "EndChapter"
                        },
                        new DialogueChoice {
                            Text = "(Оглушить рукоятью меча и забрать припасы)",
                            NextNodeId = "EndChapter_Loot",
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
                    Text = "А-а-агрх! Стой! Хватит! Я все скажу! У них карта засады в сосновом лесу... Шаман Варг ждет подкрепления у алтаря... Не убивай!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Забрать карту и добить орка без лишнего шума.",
                            NextNodeId = "EndChapter_Loot",
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
                    Text = "Обыскав тело надзирателя, вы находите ценные трофеи и сведения о передвижениях орков.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Продолжить путь к холмам", NextNodeId = "EndChapter" }
                    }
                }
            },
            {
                "EndChapter", new DialogueNode
                {
                    Id = "EndChapter",
                    SpeakerName = "Автор",
                    Text = "Дождь усиливается, смывая пепел с руин деревни. Впереди высятся Железные Холмы, где сокрыто древнее святилище...",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Перейти к Сцене 2: Ратуша Старосты", NextNodeId = "Scene2_Start" }
                    }
                }
            },

            // --- Сцена 2: Встреча со старостой Бранном ---
            {
                "Scene2_Start", new DialogueNode
                {
                    Id = "Scene2_Start",
                    SpeakerName = "Автор",
                    Text = "Среди дымящихся руин вы находите уцелевший подвал Ратуши. Из темноты с охотничьим арбалетом наперевес выходит седой мужчина в порванном плаще — староста Бранн.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Опустить оружие и заговорить со старостой", NextNodeId = "Scene2_Dialogue1" }
                    }
                }
            },
            {
                "Scene2_Dialogue1", new DialogueNode
                {
                    Id = "Scene2_Dialogue1",
                    SpeakerName = "Староста Бранн",
                    Text = "(Откашливаясь от дыма) Элрик! Хвала богам, ты жив... Они пришли на рассвете. Нам нечем было защищаться! Орки искали не еду и не золото. Они перерыли мой дом и забрали Рунический Фолиант — древнюю рукопись наших предков!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Ветеран] Я пришел защитить этих людей. Клянусь честью, фолиант вернется в деревню!",
                            NextNodeId = "Scene2_Noble",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice {
                            Text = "[Изгой-маг] Какая сила скрыта в этой рукописи? Зачем она шаманам?",
                            NextNodeId = "Scene2_Pragmatic",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice {
                            Text = "[Наемник] Вернуть реликвию будет стоить дорого. Что ты можешь предложить за риск?",
                            NextNodeId = "Scene2_Mercenary",
                            RequiredFlag = "Class_Наемник"
                        },
                        new DialogueChoice {
                            Text = "[Прагматик] Куда они унесли рукопись?",
                            NextNodeId = "Scene2_Pragmatic"
                        },
                        new DialogueChoice {
                            Text = "[Благородство] Я остановлю орду и спасу наш край!",
                            NextNodeId = "Scene2_Noble"
                        }
                    }
                }
            },
            {
                "Scene2_Noble", new DialogueNode
                {
                    Id = "Scene2_Noble",
                    SpeakerName = "Староста Бранн",
                    Text = "Твое благородство вселяет надежду в сердца выживших. Возьми это семейное защитное кольцо и последнее целебное зелье. Они понесли фолиант через горное ущелье к лагерю Шамана Варга.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Принять дар и выдвигаться в ущелье",
                            NextNodeId = "EndScene2",
                            Action = "Brann_Noble"
                        }
                    }
                }
            },
            {
                "Scene2_Pragmatic", new DialogueNode
                {
                    Id = "Scene2_Pragmatic",
                    SpeakerName = "Староста Бранн",
                    Text = "В фолианте описаны печати древней Цитадели в Железных Кряжах. Если Варг сломает руны, он сможет открыть врата цитадели! Они устроили засаду на горном тракте, но я знаю обходную тропу.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Изучить карту засады и подготовить контрудар",
                            NextNodeId = "EndScene2",
                            Action = "Brann_Pragmatic"
                        }
                    }
                }
            },
            {
                "Scene2_Mercenary", new DialogueNode
                {
                    Id = "Scene2_Mercenary",
                    SpeakerName = "Староста Бранн",
                    Text = "(Тяжело вздыхает) Всё, что осталось в казне ратуши — 50 золотых монет. Забирай всё, наемник, только верни книгу и спаси тех, кого угнали в плен!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Взять задаток и отправиться по следам орков",
                            NextNodeId = "EndScene2",
                            Action = "Brann_Mercenary"
                        }
                    }
                }
            },
            {
                "EndScene2", new DialogueNode
                {
                    Id = "EndScene2",
                    SpeakerName = "Автор",
                    Text = "Покинув обугленные стены Пепельного Ручья, вы вступаете под своды мрачного ущелья. В воздухе пахнет озоном и едким серным дымом шаманских костров.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Перейти к Сцене 3: Лагерь Шамана Варга", NextNodeId = "Scene3_Start" }
                    }
                }
            },

            // --- Сцена 3: Лагерь Шамана Варга ---
            {
                "Scene3_Start", new DialogueNode
                {
                    Id = "Scene3_Start",
                    SpeakerName = "Автор",
                    Text = "Горное плато. В центре возвышается базальтовый алтарь, окутанный фиолетовыми всполохами. Шаман Варг в маске из черепа вепря чертит руны над древним фолиантом, окруженный стражей.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Окликнуть шамана", NextNodeId = "Scene3_Varg" }
                    }
                }
            },
            {
                "Scene3_Varg", new DialogueNode
                {
                    Id = "Scene3_Varg",
                    SpeakerName = "Шаман Варг",
                    Text = "Глупый смертный! Ты опоздал! Кровь защитников деревни уже напитала руны. Скоро древние врата Цитадели распахнутся перед Ордой Громгара!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Ветеран] (Грозно обнажить клинок) Брось книгу, шаман, или я пригвоздю тебя к алтарю!",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "Varg_Honorable",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice {
                            Text = "[Изгой-маг] (Использовать тайные знания, чтобы сбить ритуал)",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "SkipTurn",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice {
                            Text = "[Засада на тракте] Напасть из укрытия со спины охраны",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = "Ambush",
                            RequiredFlag = "UnlockedAmbush"
                        },
                        new DialogueChoice {
                            Text = "Атаковать Варга и его стражей",
                            NextNodeId = "PostBattle_Scene3",
                            Action = "StartBattle",
                            ActionParam = ""
                        }
                    }
                }
            },
            {
                "PostBattle_Scene3", new DialogueNode
                {
                    Id = "PostBattle_Scene3",
                    SpeakerName = "Автор",
                    Text = "Шаман Варг повержен. Его посох с треском раскалывается, а стражи разбегаются в панике. На алтаре лежит Рунический Фолиант.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Осмотреть фолиант и алтарь", NextNodeId = "Scene3_InspectTome" }
                    }
                }
            },
            {
                "Scene3_InspectTome", new DialogueNode
                {
                    Id = "Scene3_InspectTome",
                    SpeakerName = "Автор",
                    Text = "Ритуальный огонь едва не сжег древние страницы. Удалось ли сохранить знания предков в целости?",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "Проверить состояние фолианта",
                            Action = "CheckTomeIntact"
                        }
                    }
                }
            },
            {
                "Scene3_TomeIntact", new DialogueNode
                {
                    Id = "Scene3_TomeIntact",
                    SpeakerName = "Автор",
                    Text = "Благодаря быстрой атаке рукопись не пострадала! На ее страницах вы находите полное описание слабостей вождя Громгара и карту его ставки на вершине Кряжа.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Выдвинуться к Святилищу Вождя Громгара", NextNodeId = "Scene4_Start" }
                    }
                }
            },
            {
                "Scene3_TomeDamaged", new DialogueNode
                {
                    Id = "Scene3_TomeDamaged",
                    SpeakerName = "Автор",
                    Text = "Часть страниц сгорела, но вы успеваете забрать защитный оберег шамана и узнаете, что Громгар уже ждет вас у священного Рунного Камня на вершине.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Подняться на вершину к Святилищу Громгара", NextNodeId = "Scene4_Start" }
                    }
                }
            },

            // --- Сцена 4: Битва с Вождем Громгаром ---
            {
                "Scene4_Start", new DialogueNode
                {
                    Id = "Scene4_Start",
                    SpeakerName = "Автор",
                    Text = "Вершина Железного Кряжа. В центре ритуального круга пульсирует исполинский Рунный Камень. Перед ним возвышается сам Вождь Громгар, закованный в шипастые латы.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice { Text = "Вступить в диалог с верховным вождем", NextNodeId = "Scene4_Gromgar" }
                    }
                }
            },
            {
                "Scene4_Gromgar", new DialogueNode
                {
                    Id = "Scene4_Gromgar",
                    SpeakerName = "Вождь Громгар",
                    Text = "Ты прошел через моих лучших воинов, человек. Но здесь твой путь окончен! Этот Камень даст Орде силу сокрушить любые королевства!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Ветеран] Поединок чести! Пусть боги решат судьбу этой земли один на один!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Honor",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice {
                            Text = "[Фолиант предков] Прочесть контрзаклинание и дестабилизировать Камень!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Lore",
                            RequiredFlag = "HasTomeIntact"
                        },
                        new DialogueChoice {
                            Text = "[Беспощадность] Ты сдохнешь так же, как и твои шаманы!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = "Gromgar_Ruthless",
                            RequiredFlag = "HasTraitRuthless"
                        },
                        new DialogueChoice {
                            Text = "Обнажить оружие и вступить в финальную битву!",
                            NextNodeId = "PostBattle_Scene4",
                            Action = "StartBattle",
                            ActionParam = ""
                        }
                    }
                }
            },
            {
                "PostBattle_Scene4", new DialogueNode
                {
                    Id = "PostBattle_Scene4",
                    SpeakerName = "Автор",
                    Text = "Громгар повержен и падает на колени. Ритуальный Рунный Камень гудит, выплескивая потоки древней энергии. Судьба региона в ваших руках!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "[Светлый финал] Уничтожить Рунный Камень и спасти край от скверны",
                            NextNodeId = "Ending_Light"
                        },
                        new DialogueChoice {
                            Text = "[Темный финал] Вобрать силу Камня в себя и подчинить остатки Орды",
                            NextNodeId = "Ending_Dark"
                        },
                        new DialogueChoice {
                            Text = "[Нейтральный финал] Предложить Громгару мирный союз в обмен на уход в горы",
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
                    SpeakerName = "Светлый финал Акта 1",
                    Text = "Рунный Камень раскалывается на сотни чистых кристаллов. Осада с Пепельного Ручья снята, но перед смертью Громгар шепчет: «Не я задумал этот штурм... Тот, кто восседает в Цитадели Теней, уже пробудил древнее Сердце Кряжа!»",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "➡️ Направиться к Древней Цитадели (Перейти к Акту 2)",
                            NextNodeId = "Act2_Start",
                            Action = "StartAct2"
                        },
                        new DialogueChoice {
                            Text = "🏠 Завершить и выйти в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Ending_Dark", new DialogueNode
                {
                    Id = "Ending_Dark",
                    SpeakerName = "Темный финал Акта 1",
                    Text = "Элрик поглощает первородную магию Камня. Тьма и мощь наполняют его тело. Взор героя устремляется на север — к неприступным Черным Вратам Древней Цитадели, где сокрыты еще более великие тайны.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "➡️ Выдвинуться на штурм Цитадели (Перейти к Акту 2)",
                            NextNodeId = "Act2_Start",
                            Action = "StartAct2"
                        },
                        new DialogueChoice {
                            Text = "🏠 Завершить и выйти в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Ending_Neutral", new DialogueNode
                {
                    Id = "Ending_Neutral",
                    SpeakerName = "Нейтральный финал Акта 1",
                    Text = "Хрупкий союз с вождем заключен. Громгар признает силу Элрика и предупреждает: «Теневой культ Архимага Малкора в глубинах Цитадели готовит гибель для всего живого. Ты должен остановить его!»",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice {
                            Text = "➡️ Принять вызов и идти в Цитадель (Перейти к Акту 2)",
                            NextNodeId = "Act2_Start",
                            Action = "StartAct2"
                        },
                        new DialogueChoice {
                            Text = "🏠 Завершить и выйти в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },

            // =================================================================
            // --- АКТ 2: ТЕНИ ЦИТАДЕЛИ (CITADEL OF SHADOWS) ---
            // =================================================================

            // --- Пролог Акта 2: Черные Врата Цитадели ---
            {
                "Act2_Start", new DialogueNode
                {
                    Id = "Act2_Start",
                    SpeakerName = "Автор",
                    Text = "Перед Элриком возвышаются гигантские базальтовые стены Древней Цитадели. Небо над твердыней затянуто пепельными облаками, а из щелей массивных Черных Врат сочится зловещее фиолетовое свечение теневого культа.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Ветеран] Осмотреть фортификацию и нанести удар по часовым",
                            NextNodeId = "Act2_Gates_Assault",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice
                        {
                            Text = "[Изгой-маг] Почувствовать резонанс барьера и выкачать теневую ману",
                            NextNodeId = "Act2_Gates_Magic",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice
                        {
                            Text = "[Наемник] Проникнуть через заброшенный водосток и ударить со спины",
                            NextNodeId = "Act2_Gates_Stealth",
                            RequiredFlag = "Class_Наемник"
                        },
                        new DialogueChoice
                        {
                            Text = "[Древний Фолиант] Прочесть формулу рассеивания и разрушить защитный щит Врат",
                            NextNodeId = "Act2_Gates_TomeDispel",
                            RequiredFlag = "HasTomeIntact"
                        },
                        new DialogueChoice
                        {
                            Text = "Подойти к главным вратам и вызвать стражу на бой",
                            NextNodeId = "Act2_Gates_Main"
                        }
                    }
                }
            },
            {
                "Act2_Gates_Assault", new DialogueNode
                {
                    Id = "Act2_Gates_Assault",
                    SpeakerName = "Автор",
                    Text = "Опытный взгляд воина мгновенно находит слабую опору в левом заграждении. Вы опрокидываете баррикаду, сбивая с ног теневых стрелков!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Сразиться со Стражем Врат",
                            NextNodeId = "Act2_PostBattle_Gates",
                            Action = "StartBattle",
                            ActionParam = "Act2_Gates_Assault"
                        }
                    }
                }
            },
            {
                "Act2_Gates_Magic", new DialogueNode
                {
                    Id = "Act2_Gates_Magic",
                    SpeakerName = "Автор",
                    Text = "Вы смыкаете ладони, вплетая свою ману в структуру теневого щита. Защитные руны взрываются обратной вспышкой, оглушая защитников врат!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вступить в бой с дезориентированным Стражем",
                            NextNodeId = "Act2_PostBattle_Gates",
                            Action = "StartBattle",
                            ActionParam = "Act2_Gates_Magic"
                        }
                    }
                }
            },
            {
                "Act2_Gates_Stealth", new DialogueNode
                {
                    Id = "Act2_Gates_Stealth",
                    SpeakerName = "Автор",
                    Text = "Бесшумно проскользнув через сливной желоб, вы оказываетесь за спинами стражников и мгновенно нейтрализуете теневого стрелка!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Нанести внезапный удар Стражу Врат",
                            NextNodeId = "Act2_PostBattle_Gates",
                            Action = "StartBattle",
                            ActionParam = "Act2_Gates_Stealth"
                        }
                    }
                }
            },
            {
                "Act2_Gates_TomeDispel", new DialogueNode
                {
                    Id = "Act2_Gates_TomeDispel",
                    SpeakerName = "Автор",
                    Text = "Слова древнего фолианта звучат на языке первородных стихий. Запечатывающие руны Врат рассыпаются сверкающим пеплом, лишая врагов магической защиты!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Сокрушить ослабленного Стража Врат",
                            NextNodeId = "Act2_PostBattle_Gates",
                            Action = "StartBattle",
                            ActionParam = "Act2_Gates_Dispel"
                        }
                    }
                }
            },
            {
                "Act2_Gates_Main", new DialogueNode
                {
                    Id = "Act2_Gates_Main",
                    SpeakerName = "Страж Врат",
                    Text = "Кто посмел потревожить владения Архимага Малкора?! Твоя плоть напитает Сердце Кряжа!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Обнажить оружие и вступить в бой",
                            NextNodeId = "Act2_PostBattle_Gates",
                            Action = "StartBattle",
                            ActionParam = ""
                        }
                    }
                }
            },
            {
                "Act2_PostBattle_Gates", new DialogueNode
                {
                    Id = "Act2_PostBattle_Gates",
                    SpeakerName = "Автор",
                    Text = "Массивный Страж Врат повержен. С его пояса вы снимаете тяжелый железный ключ с рунической гравировкой — Ключ от Внутренней Цитадели. Тяжелые створки со скрежетом отворяются.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Забрать ключ и войти во Внутренний Двор Цитадели",
                            NextNodeId = "Act2_Courtyard",
                            Action = "Act2_Gates_Loot"
                        }
                    }
                }
            },

            // --- Локация 1: Внутренний Двор и Развилка Путей ---
            {
                "Act2_Courtyard", new DialogueNode
                {
                    Id = "Act2_Courtyard",
                    SpeakerName = "Автор",
                    Text = "Внутренний Двор Цитадели окружен высокими стрельчатыми галереями. Отсюда открываются три пути: в Тюремные Казематы, в Запретную Библиотеку и в Древнюю Кузницу. Над всем доминирует башня Тронного Зала Архимага.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Путь 1] Спуститься в Тюремные Казематы (Поиск выживших и пленников)",
                            NextNodeId = "Act2_Dungeons_Entry"
                        },
                        new DialogueChoice
                        {
                            Text = "[Путь 2] Направиться в Запретную Библиотеку (Поиск тайн ритуала и слабостей Малкора)",
                            NextNodeId = "Act2_Library_Entry"
                        },
                        new DialogueChoice
                        {
                            Text = "[Путь 3] Пройти в Древнюю Кузницу (Осмотреть замершего Рунного Голема)",
                            NextNodeId = "Act2_Armory_Entry"
                        },
                        new DialogueChoice
                        {
                            Text = "[Штурм] Подняться по центральной лестнице к Тронному Залу Архимага",
                            NextNodeId = "Act2_PreBoss_Warning"
                        }
                    }
                }
            },

            // --- Ветка 1: Тюремные Казематы ---
            {
                "Act2_Dungeons_Entry", new DialogueNode
                {
                    Id = "Act2_Dungeons_Entry",
                    SpeakerName = "Автор",
                    Text = "В подземельях стоит удушливый запах серы и сырости. Из глубины доносится звон кандалов и приглушенный голос палача.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Продвинуться вглубь казематов",
                            NextNodeId = "Act2_Dungeons_Encounter"
                        }
                    }
                }
            },
            {
                "Act2_Dungeons_Encounter", new DialogueNode
                {
                    Id = "Act2_Dungeons_Encounter",
                    SpeakerName = "Теневой Инквизитор",
                    Text = "Чужак в казематах?! Ты станешь идеальным подношением для наших гончих скверны! Взять его!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Ветеран] Врезать щитом в Инквизитора и начать бой",
                            NextNodeId = "Act2_Dungeons_PostBattle",
                            Action = "StartBattle",
                            ActionParam = "Act2_Dungeon_Assault",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice
                        {
                            Text = "[Наемник] Метнуть дымовую шашку и атаковать из тени",
                            NextNodeId = "Act2_Dungeons_PostBattle",
                            Action = "StartBattle",
                            ActionParam = "Ambush",
                            RequiredFlag = "Class_Наемник"
                        },
                        new DialogueChoice
                        {
                            Text = "Вступить в бой с Инквизитором и гончими",
                            NextNodeId = "Act2_Dungeons_PostBattle",
                            Action = "StartBattle",
                            ActionParam = ""
                        }
                    }
                }
            },
            {
                "Act2_Dungeons_PostBattle", new DialogueNode
                {
                    Id = "Act2_Dungeons_PostBattle",
                    SpeakerName = "Автор",
                    Text = "Инквизитор повержен. В центральной камере вы находите измученного узника в разорванной мантии ученика магии. Это Каэль — бывший помощник архимага Малкора.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Поговорить с узником",
                            NextNodeId = "Act2_Dungeons_Kael_Dialogue"
                        }
                    }
                }
            },
            {
                "Act2_Dungeons_Kael_Dialogue", new DialogueNode
                {
                    Id = "Act2_Dungeons_Kael_Dialogue",
                    SpeakerName = "Каэль, бывший ученик",
                    Text = "(Дрожащим голосом) Не убивай меня! Я помог Малкору расшифровать руны, но когда понял, что он хочет выкачать жизнь из всей долины — он бросил меня на растерзание инквизиторам! У меня есть Теневой Кристалл, который питает его барьер. Возьми его, и щит Малкора рухнет!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Милосердие] Освободить Каэля, перевязать раны и забрать Кристалл",
                            NextNodeId = "Act2_Kael_Freed",
                            Action = "Act2_Kael_Mercy"
                        },
                        new DialogueChoice
                        {
                            Text = "[Беспощадный] Вырвать Кристалл и забрать кошель Каэля, оставив его в камере",
                            NextNodeId = "Act2_Kael_Looted",
                            Action = "Act2_Kael_Ruthless",
                            RequiredFlag = "HasTraitRuthless"
                        },
                        new DialogueChoice
                        {
                            Text = "Забрать Кристалл и приказать Каэлю бежать из Цитадели",
                            NextNodeId = "Act2_Kael_Freed",
                            Action = "Act2_Kael_Mercy"
                        }
                    }
                }
            },
            {
                "Act2_Kael_Freed", new DialogueNode
                {
                    Id = "Act2_Kael_Freed",
                    SpeakerName = "Каэль, бывший ученик",
                    Text = "Спасибо тебе, путник! Помни: когда начнется бой с Малкором, разбей Теневой Кристалл об пол — это разорвет его связь с Сердцем Кряжа и ослабит его темные чары!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Kael_Looted", new DialogueNode
                {
                    Id = "Act2_Kael_Looted",
                    SpeakerName = "Автор",
                    Text = "Вы безжалостно забираете Теневой Кристалл, золото и записи заклинаний Каэля. Теперь у вас есть мощный артефакт против Архимага.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },

            // --- Ветка 2: Запретная Библиотека ---
            {
                "Act2_Library_Entry", new DialogueNode
                {
                    Id = "Act2_Library_Entry",
                    SpeakerName = "Автор",
                    Text = "Своды Запретной Библиотеки уходят ввысь. В воздухе левитируют древние гримуары, а на алхимических столах кипят фиолетовые растворы. На центральном постаменте лежит рабочий дневник Малкора.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Осмотреть библиотеку и алхимические столы",
                            NextNodeId = "Act2_Library_Inspect"
                        }
                    }
                }
            },
            {
                "Act2_Library_Inspect", new DialogueNode
                {
                    Id = "Act2_Library_Inspect",
                    SpeakerName = "Автор",
                    Text = "Перед вами раскрываются тайны многолетних исследований теневого культа.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Изгой-маг] Расшифровать запретные формулы в дневнике Малкора",
                            NextNodeId = "Act2_Library_ArcaneSecrets",
                            Action = "Act2_Library_Research",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice
                        {
                            Text = "[Древний Фолиант] Сопоставить древние руны с записями библиотеки",
                            NextNodeId = "Act2_Library_TomeMastery",
                            Action = "Act2_Library_Research",
                            RequiredFlag = "HasTomeIntact"
                        },
                        new DialogueChoice
                        {
                            Text = "Обыскать алхимическую лабораторию и сварить Эликсир Очищения",
                            NextNodeId = "Act2_Library_Alchemy",
                            Action = "Act2_Library_Elixir"
                        },
                        new DialogueChoice
                        {
                            Text = "[Опасный ритуал] Впитать темную эссенцию с алтаря Бездны (-20 HP -> +2 Очка Характеристик)",
                            NextNodeId = "Act2_Library_DarkAltar",
                            Action = "Act2_Library_Dark"
                        },
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Library_ArcaneSecrets", new DialogueNode
                {
                    Id = "Act2_Library_ArcaneSecrets",
                    SpeakerName = "Автор",
                    Text = "Ваш магический разум с легкостью расшифровывает записи. Малкор уязвим к священному огню, а его заклинания пожирают колоссальное количество маны при контратаках (+1 Очко Навыков, раскрыта слабость босса!).",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Library_TomeMastery", new DialogueNode
                {
                    Id = "Act2_Library_TomeMastery",
                    SpeakerName = "Автор",
                    Text = "Фолиант старосты резонирует с книжными полками, открывая тайный тайник в стене! Внутри вы находите Перстень Тайной Ярости и свиток с печатью снятия барьеров.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Забрать кольцо и вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Library_Alchemy", new DialogueNode
                {
                    Id = "Act2_Library_Alchemy",
                    SpeakerName = "Автор",
                    Text = "Смешав редкие горные экстракты и солнечную пыльцу, вы получаете Эликсир Очищения Скверны, а также пополняете все свои зелья здоровья и маны!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Спрятать эликсир и вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Library_DarkAltar", new DialogueNode
                {
                    Id = "Act2_Library_DarkAltar",
                    SpeakerName = "Автор",
                    Text = "Темное пламя обжигает вашу плоть, но вливает в мышцы и вены дикую, неистовую силу Бездны (-20 HP, получено +2 Очка Характеристик!).",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },

            // --- Ветка 3: Древняя Кузница и Рунный Голем ---
            {
                "Act2_Armory_Entry", new DialogueNode
                {
                    Id = "Act2_Armory_Entry",
                    SpeakerName = "Автор",
                    Text = "Огромная подземная кузница древних гномов. По желобам течет раскаленная лава. Посреди зала на цепях застыл колоссальный 4-метровый Рунный Голем из черного гранита.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Подойти к застывшему Голему",
                            NextNodeId = "Act2_Armory_Golem_Inspect"
                        }
                    }
                }
            },
            {
                "Act2_Armory_Golem_Inspect", new DialogueNode
                {
                    Id = "Act2_Armory_Golem_Inspect",
                    SpeakerName = "Автор",
                    Text = "В груди голема мерцает поврежденное руническое ядро. Механизм заблокирован теневыми цепями.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Ветеран] Починить заклинивший привод управления и настроить боевой блок",
                            NextNodeId = "Act2_Armory_Repair",
                            Action = "Act2_Golem_Repair",
                            RequiredFlag = "Class_Ветеран"
                        },
                        new DialogueChoice
                        {
                            Text = "[Ключ/Инструменты из деревни] Использовать мастерские ключи для починки ядра",
                            NextNodeId = "Act2_Armory_Repair",
                            Action = "Act2_Golem_Repair",
                            RequiredFlag = "HasVillageChestKey"
                        },
                        new DialogueChoice
                        {
                            Text = "[Изгой-маг] Извлечь Сердце Голема как эпический амулет",
                            NextNodeId = "Act2_Armory_DrainHeart",
                            Action = "Act2_Golem_Drain",
                            RequiredFlag = "Class_Изгой-маг"
                        },
                        new DialogueChoice
                        {
                            Text = "Попытаться грубо выломать цепи (Пробудить Голема в ярости)",
                            NextNodeId = "Act2_Armory_Golem_Rage"
                        },
                        new DialogueChoice
                        {
                            Text = "Оставить Голема и вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Armory_Repair", new DialogueNode
                {
                    Id = "Act2_Armory_Repair",
                    SpeakerName = "Древний Рунный Голем",
                    Text = "(Глухой рокот камня) ПРИКАЗ ПРИНЯТ... ПРОТОКОЛ ОЧИЩЕНИЯ ЦИТАДЕЛИ АКТИВИРОВАН. Я СОКРУШУ СЛУГ ТЬМЫ ПО ТВОЕМУ ПРИКАЗУ, КОМАНДИР!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Отлично! Жди моего сигнала перед Тронным Залом!",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Armory_DrainHeart", new DialogueNode
                {
                    Id = "Act2_Armory_DrainHeart",
                    SpeakerName = "Автор",
                    Text = "Вы вытягиваете ядро из груди исполина. Каменный остов рассыпается в прах, а в ваших руках остается эпический амулет — Сердце Рунного Голема (+8 Броня, +60 HP, +5 Выносливость)!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },
            {
                "Act2_Armory_Golem_Rage", new DialogueNode
                {
                    Id = "Act2_Armory_Golem_Rage",
                    SpeakerName = "Древний Рунный Голем",
                    Text = "ТРЕВОГА! НЕСАНКЦИОНИРОВАННЫЙ ДОСТУП! УНИЧТОЖИТЬ НАРУШИТЕЛЯ!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Вступить в бой с взбесившимся Големом",
                            NextNodeId = "Act2_PostBattle_Golem",
                            Action = "StartBattle",
                            ActionParam = ""
                        }
                    }
                }
            },
            {
                "Act2_PostBattle_Golem", new DialogueNode
                {
                    Id = "Act2_PostBattle_Golem",
                    SpeakerName = "Автор",
                    Text = "Колоссальный голем с грохотом раскалывается на куски. Среди обломков вы находите тяжелое оружие — Рассекатель Голема и 150 золотых монет.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Забрать трофеи и вернуться во Внутренний Двор",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },

            // --- Подход к Тронному Залу ---
            {
                "Act2_PreBoss_Warning", new DialogueNode
                {
                    Id = "Act2_PreBoss_Warning",
                    SpeakerName = "Автор",
                    Text = "Перед парадной лестницей в Тронный Зал воздух вибрирует от колоссального скопления темной энергии. Уверены ли вы, что достаточно подготовились и исследовали Цитадель?",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Я готов. Начать штурм Тронного Зала Архимага!",
                            NextNodeId = "Act2_Boss_Entry"
                        },
                        new DialogueChoice
                        {
                            Text = "Вернуться во Внутренний Двор (доисследовать Казематы/Библиотеку/Кузницу)",
                            NextNodeId = "Act2_Courtyard"
                        }
                    }
                }
            },

            // --- Финальный Зал: Архимаг Малкор ---
            {
                "Act2_Boss_Entry", new DialogueNode
                {
                    Id = "Act2_Boss_Entry",
                    SpeakerName = "Автор",
                    Text = "Величественный Тронный Зал Цитадели. Под куполом парит гигантский первородный кристалл — Сердце Кряжа. Возле него, окутанный вихрем багровых молний, стоит Теневой Архимаг Малкор в короне древних королей.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Выйти вперед и бросить вызов Архимагу",
                            NextNodeId = "Act2_Boss_Dialogue"
                        }
                    }
                }
            },
            {
                "Act2_Boss_Dialogue", new DialogueNode
                {
                    Id = "Act2_Boss_Dialogue",
                    SpeakerName = "Теневой Архимаг Малкор",
                    Text = "(Громогласный смех) Наконец-то! Мелкий червь, возомнивший себя героем! Орки Громгара были лишь слепым тараном, чтобы отвлечь королевство. Сила Сердца Кряжа уже подчинена мне! Ты опоздал!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Теневой Кристалл Каэля] Расколоть кристалл и сжечь магический щит Малкора!",
                            NextNodeId = "Act2_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act2_Malkor_Weakened",
                            RequiredFlag = "HasShadowGem"
                        },
                        new DialogueChoice
                        {
                            Text = "[Союзный Голем] Приказать Голему протаранить ритуальный барьер!",
                            NextNodeId = "Act2_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act2_Golem_Assault",
                            RequiredFlag = "GolemAllied"
                        },
                        new DialogueChoice
                        {
                            Text = "[Эликсир Очищения] Выпить эликсир и атаковать сквозь теневой огонь!",
                            NextNodeId = "Act2_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act2_Purification",
                            RequiredFlag = "HasPurificationElixir"
                        },
                        new DialogueChoice
                        {
                            Text = "[Клятва Громгара] Протрубить в рог северных племен — удар союзных орков!",
                            NextNodeId = "Act2_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act2_Orc_Reinforce",
                            RequiredFlag = "HonorGromgar"
                        },
                        new DialogueChoice
                        {
                            Text = "«Твое безумие заканчивается здесь, Малкор!» (Вступить в бой в лоб)",
                            NextNodeId = "Act2_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Boss_Malkor"
                        }
                    }
                }
            },

            // --- Развязка и Финальные Решения Акта 2 ---
            {
                "Act2_Ending_Decisions", new DialogueNode
                {
                    Id = "Act2_Ending_Decisions",
                    SpeakerName = "Автор",
                    Text = "Теневой Архимаг Малкор с истошным криком рассыпается черным прахом. Вихрь тьмы рассеивается. Перед Элриком парит первородное Сердце Кряжа — источник невероятной мощи. Судьба всего Севера теперь зависит от вашего окончательного выбора!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[Светлый Путь] Очистить Сердце Кряжа святым светом и вернуть покой в горы",
                            NextNodeId = "Act2_Ending_Light",
                            Action = "Act2_Ending_Purify"
                        },
                        new DialogueChoice
                        {
                            Text = "[Темный Путь] Подчинить силу Сердца себе и взойти на Трон Цитадели Теней",
                            NextNodeId = "Act2_Ending_Dark",
                            Action = "Act2_Ending_Domination"
                        },
                        new DialogueChoice
                        {
                            Text = "[Путь Равновесия] Запечатать Цитадель и Сердце вечной клятвой хранителя",
                            NextNodeId = "Act2_Ending_Neutral",
                            Action = "Act2_Ending_Balance"
                        }
                    }
                }
            },
            {
                "Act2_Ending_Light", new DialogueNode
                {
                    Id = "Act2_Ending_Light",
                    SpeakerName = "Светлый финал: Спаситель Железного Кряжа",
                    Text = "Золотые лучи солнца пробивают вековые тучи над Цитаделью. Сердце Кряжа исцелено от скверны и дарует плодородие и защиту всем землям предгорий. Жители Пепельного Ручья и всего Севера слагают легенды об Элрике — величайшем Защитнике королевства!\n\nПолучен легендарный меч: Клинок Солнечного Очищения!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🌋 Продолжить кампанию: Акт 3 «Сердце Титана»",
                            NextNodeId = "Act3_Start",
                            Action = "SetupAct3"
                        },
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить кампанию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Act2_Ending_Dark", new DialogueNode
                {
                    Id = "Act2_Ending_Dark",
                    SpeakerName = "Темный финал: Владыка Цитадели Теней",
                    Text = "Вы возлагаете руки на Сердце Кряжа. Бескрайняя мощь Бездны наполняет ваш разум. Черные знамена вздымаются над башнями. Отныне никто не посмеет бросить вызов новому Владыке Железного Кряжа!\n\nПолучено легендарное одеяние: Мантия Первородной Тьмы!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🌋 Продолжить кампанию: Акт 3 «Сердце Титана»",
                            NextNodeId = "Act3_Start",
                            Action = "SetupAct3"
                        },
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить кампанию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Act2_Ending_Neutral", new DialogueNode
                {
                    Id = "Act2_Ending_Neutral",
                    SpeakerName = "Финал Равновесия: Вечный Страж Севера",
                    Text = "Понимая, что подобная сила не должна принадлежать ни одному смертному, вы запечатываете врата Цитадели древней печатью титанов. Ни люди, ни орки больше не потревожат святилище. Вы остаетесь единственным безмолвным Стражем горного покоя.\n\nПолучен легендарный амулет: Эмблема Солнечного Очищения!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🌋 Продолжить кампанию: Акт 3 «Сердце Титана»",
                            NextNodeId = "Act3_Start",
                            Action = "SetupAct3"
                        },
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить кампанию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },

            // ==========================================
            // === ЭТАП 3.4: АКТ 3 «СЕРДЦЕ ТИТАНА» ===
            // ==========================================
            {
                "Act3_Start", new DialogueNode
                {
                    Id = "Act3_Start",
                    SpeakerName = "Автор",
                    Text = "Минула луна после гибели Архимага Малкора. Земля Железного Кряжа дрожит без перерыва: трескаются перевалы, из кратеров поднимается пепел древних костров. Старики шепчут, что Сердце Кряжа было лишь засовом — а за ним веками спал ПЕРВОРОДНЫЙ ТИТАН, первый король гор. И теперь засов сорван вашей победой.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Собрать отряд и спуститься в недра Кряжа",
                            NextNodeId = "Act3_Ascent"
                        }
                    }
                }
            },
            {
                "Act3_Ascent", new DialogueNode
                {
                    Id = "Act3_Ascent",
                    SpeakerName = "Голос Кряжа",
                    Text = "Тропа вниз проходит через Зал Древних Королей — колоннаду черных базальтовых статуй правителей, когда-то заковавших Титана. На постаментах тлеют руны последней печати. Чтобы удержать Титана во сне, нужен новый якорь — живое сердце с родством к камню, тени или свету.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[СВЕТ] «Я очищу Сердце вторично — пусть Титан уснет вечным спокойным сном камня!»",
                            NextNodeId = "Act3_Boss_Entry",
                            RequiredFlag = "Act2LightEnding"
                        },
                        new DialogueChoice
                        {
                            Text = "[ТЕНЬ] «Корона Древних Королей моя по праву силы. Я подчиню Титана, как подчинил Тень!»",
                            NextNodeId = "Act3_Boss_Entry",
                            RequiredFlag = "Act2DarkEnding"
                        },
                        new DialogueChoice
                        {
                            Text = "[РАВНОВЕСИЕ] «Я дал слово Стража. Закую Титана в новую печать — и останусь его тюремщиком навечно».",
                            NextNodeId = "Act3_Boss_Entry",
                            RequiredFlag = "Act2NeutralEnding"
                        },
                        new DialogueChoice
                        {
                            Text = "«Что бы там ни спало — мой меч решит, проснется ли оно» (спускаться без клятвы)",
                            NextNodeId = "Act3_Boss_Entry"
                        }
                    }
                }
            },
            {
                "Act3_Boss_Entry", new DialogueNode
                {
                    Id = "Act3_Boss_Entry",
                    SpeakerName = "Автор",
                    Text = "Дно мира. Озеро магмы озаряет купол пещеры, равный небу. В центре, опутанный ржавыми цепями печатей, поднимается коленопреклоненный исполин — Первородный Титан Кряжа. Камень его кожи лопается, из трещин брызжет белый жар. «СМЕРТНЫЕ... ВЫ СНОВА ПРИШЛИ БУДИТЬ МЕНЯ?»",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "[ЩИТ КОРОЛЕЙ] Восстать против Титана под защитой рун Зала Древних Королей!",
                            NextNodeId = "Act3_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act3_Titan_Shield"
                        },
                        new DialogueChoice
                        {
                            Text = "[СЕРДЦЕ ГОЛЕМА] Ударить, питая снаряжение кристаллом Рунного Голема!",
                            NextNodeId = "Act3_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act3_Crystal_Empowered",
                            RequiredFlag = "GolemAllied"
                        },
                        new DialogueChoice
                        {
                            Text = "«Вставай, древний. Мы закончим то, что начали твои короли!» (Честный бой)",
                            NextNodeId = "Act3_Ending_Decisions",
                            Action = "StartBattle",
                            ActionParam = "Act3_Titan"
                        }
                    }
                }
            },
            {
                "Act3_Ending_Decisions", new DialogueNode
                {
                    Id = "Act3_Ending_Decisions",
                    SpeakerName = "Автор",
                    Text = "Грохот падшего Титана сотрясает основание мира. Перед Элриком парит обнажённое Сердце — пульсирующий узел всех судеб Кряжа. Три дороги расходятся от него, как три акта вашей истории. Финальный выбор свершится сейчас.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🌅 [СВЕТЛЫЕ ГОРЫ] Усыпить Сердце песней рассвета — горы свободны от всякой власти",
                            NextNodeId = "Act3_Ending_Light",
                            Action = "Act3_Ending_Dawn"
                        },
                        new DialogueChoice
                        {
                            Text = "🌑 [ВЛАДЫКА СЕРДЦА] Поглотить силу Титана и взойти на Трон Древних Королей",
                            NextNodeId = "Act3_Ending_Dark",
                            Action = "Act3_Ending_Heart"
                        },
                        new DialogueChoice
                        {
                            Text = "⚖️ [ХРАНИТЕЛЬ] Запечатать Сердце печатью трех актов и остаться его стражем",
                            NextNodeId = "Act3_Ending_Neutral",
                            Action = "Act3_Ending_Seal"
                        }
                    }
                }
            },
            {
                "Act3_Ending_Light", new DialogueNode
                {
                    Id = "Act3_Ending_Light",
                    SpeakerName = "🌅 Эпилог Светлых Гор",
                    Text = "Первый луч солнца входит в подземный мир, и Сердце, словно услышав колыбельную, гаснет мягким янтарным светом. Титан засыпает спокойным сном камня — и горы впервые за тысячелетие замолкают. Песни о Легенде Железного Кряжа будут звучать в тавернах Севера вечно.\n\nПолучен легендарный меч: Рассеятель Утра!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить трилогию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Act3_Ending_Dark", new DialogueNode
                {
                    Id = "Act3_Ending_Dark",
                    SpeakerName = "🌑 Эпилог Владыки Сердца",
                    Text = "Вы сжимаете Сердце в кулаке. Раскалённая сила Титана втекает в жилы — и вы становитесь тем, кем были Древние Короли: властелином камня, лавы и самих гор. С вершин Железного Кряжа видны все королевства, склонившие знамена.\n\nПолучен легендарный амулет: Сердце Титана в Дланях!",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить трилогию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            },
            {
                "Act3_Ending_Neutral", new DialogueNode
                {
                    Id = "Act3_Ending_Neutral",
                    SpeakerName = "⚖️ Эпилог Хранителя",
                    Text = "Печать трех актов — крови орочьих кланов, света архимагов и слова Стража — ложится на Сердце. Врата недрам затворяются навсегда. Кто-то должен остаться по эту сторону. Это будете вы — вечный хранитель равновесия Кряжа, о котором помнят только горы.",
                    Choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "🏆 Завершить трилогию и вернуться в Главное Меню",
                            Action = "MainMenu"
                        }
                    }
                }
            }

        };
    }
}
