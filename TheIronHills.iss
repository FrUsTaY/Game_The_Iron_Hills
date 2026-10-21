; -----------------------------------------------------------------------------
; Скрипт сборки инсталлятора Inno Setup 7 для игры "The Iron Hills"
; Разработчик и издатель: FRUSTAY
; -----------------------------------------------------------------------------

#define MyAppName "The Iron Hills"
#define MyAppVersion "2.0.0"
#define MyAppPublisher "FRUSTAY"
#define MyAppExeName "EpicBattle.exe"
#define MyAppAssocName MyAppName + " Game"
#define MyAppAssocExt ".tih"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; Уникальный идентификатор приложения (GUID)
AppId={{8B196C1F-02D4-49E8-9CA7-195B48B47C5E}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright=© 2026 {#MyAppPublisher}. Все права защищены.

; Пути установки
DefaultDirName={autopf}\FRUSTAY\{#MyAppName}
DefaultGroupName=FRUSTAY\{#MyAppName}
AllowNoIcons=yes

; Документ согласия и требований (Обязательное согласие пользователя)
LicenseFile=installer_assets\requirements.txt

; Выходной файл
OutputDir=installer_output
OutputBaseFilename=TheIronHills_Setup_v2.0
SetupIconFile=app_icon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} ({#MyAppPublisher})

; Современный визуальный стиль и брендированные баннеры (светлая и тёмная темы Windows)
WizardStyle=modern dynamic
WizardImageFile=installer_assets\wizard_large.bmp
WizardImageFileDynamicDark=installer_assets\wizard_large.bmp
WizardSmallImageFile=installer_assets\wizard_small.bmp
WizardSmallImageFileDynamicDark=installer_assets\wizard_small.bmp

; Сжатие дистрибутива
Compression=lzma2/max
SolidCompression=yes

; Архитектура x64 Windows 10/11
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Права администратора (обязательно для записи в Program Files и предотвращения "Ошибки 5: Отказано в доступе")
PrivilegesRequired=admin

; Защита от конфликтов запущенной игры
CloseApplications=yes
CloseApplicationsFilter=*{#MyAppExeName}*
AppMutex=TheIronHills_EpicBattle_Mutex

; Включение всех информативных страниц
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=no
DisableReadyPage=no
DisableFinishedPage=no

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
; Кастомизация русского языка с атмосферой игры и явным подтверждением
russian.WizardLicense=Системные требования и подтверждение установки
russian.LicenseLabel=Пожалуйста, ознакомьтесь с системными требованиями и составом устанавливаемых компонентов:
russian.LicenseLabel3=Игра «{#MyAppName}» разработана и издаётся студией {#MyAppPublisher}. Подтвердите готовность к распаковке:
russian.LicenseAccepted=Я &согласен с условиями, компоненты понятны — продолжить установку
russian.LicenseNotAccepted=Я &не согласен (отмена установки)
russian.WelcomeLabel1=Добро пожаловать в установку The Iron Hills!
russian.WelcomeLabel2=Вас приветствует студия {#MyAppPublisher}!%n%nМастер поможет вам установить тактическую ролевую игру «The Iron Hills: Epic Battle» на ваш компьютер.%n%nПеред началом установки рекомендуется закрыть другие работающие приложения.
russian.ClickNext=Нажмите «Далее», чтобы перейти к ознакомлению с компонентами и требованиями.
russian.FinishedHeadingLabel=Установка «The Iron Hills» успешно завершена!
russian.FinishedLabel=Игра The Iron Hills и все сопутствующие библиотеки готовы к первому запуску.%n%nСтудия {#MyAppPublisher} желает вам славных побед в битвах Железных Холмов!
russian.FinishedLabelNoIcons=Игра The Iron Hills и все сопутствующие библиотеки готовы к первому запуску.%n%nСтудия {#MyAppPublisher} желает вам славных побед в битвах Железных Холмов!

english.WizardLicense=System Requirements & Installation Confirmation
english.LicenseLabel=Please review the system requirements and game components before continuing:
english.LicenseLabel3=«{#MyAppName}» is developed and published by {#MyAppPublisher}. Please confirm readiness to install:
english.LicenseAccepted=I &agree with requirements and confirm installation
english.LicenseNotAccepted=I &do not agree (cancel installation)
english.WelcomeLabel1=Welcome to The Iron Hills Setup!
english.WelcomeLabel2=Welcome from studio {#MyAppPublisher}!%n%nThis wizard will install the tactical RPG «The Iron Hills: Epic Battle» on your computer.%n%nIt is recommended that you close all other applications before continuing.
english.ClickNext=Click Next to proceed to system requirements.
english.FinishedHeadingLabel=Completing The Iron Hills Setup!
english.FinishedLabel=The Iron Hills and all runtime dependencies are ready for your first journey.%n%nStudio {#MyAppPublisher} wishes you glory and victory!

[Tasks]
Name: "desktopicon"; Description: "Создать ярлык на Рабочем столе"; GroupDescription: "Дополнительные ярлыки:"; Flags: checkedonce
Name: "startmenuicon"; Description: "Создать папку и ярлык в меню «Пуск»"; GroupDescription: "Дополнительные ярлыки:"; Flags: checkedonce

[Files]
; Исполняемый файл игры (.NET 8 Self-Contained)
Source: "dist\game\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "dist\game\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "dist\game\app_icon.ico"; DestDir: "{app}"; Flags: ignoreversion

; Медиа-ресурсы: Images, Sounds, Videos
Source: "dist\game\Images\*"; DestDir: "{app}\Images"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "dist\game\Sounds\*"; DestDir: "{app}\Sounds"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "dist\game\Videos\*"; DestDir: "{app}\Videos"; Flags: ignoreversion recursesubdirs createallsubdirs

; Документация, патч-ноут и системное соглашение
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "UPDATE_2.0.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "installer_assets\requirements.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: startmenuicon
Name: "{group}\Руководство к игре (README)"; Filename: "{app}\README.md"; Tasks: startmenuicon
Name: "{group}\Список обновлений (Patch Notes v2.0)"; Filename: "{app}\UPDATE_2.0.md"; Tasks: startmenuicon
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"; Tasks: startmenuicon
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Запустить The Iron Hills прямо сейчас"; Flags: postinstall nowait skipifsilent
Filename: "{app}\UPDATE_2.0.md"; Description: "Посмотреть список изменений (Changelog v2.0)"; Flags: postinstall shellexec skipifsilent unchecked
Filename: "{app}"; Description: "Открыть папку с установленной игрой"; Flags: postinstall shellexec skipifsilent unchecked

[UninstallDelete]
Type: filesandordirs; Name: "{app}\logs"
Type: filesandordirs; Name: "{app}\crashdumps"
