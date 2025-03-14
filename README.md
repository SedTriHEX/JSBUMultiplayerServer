# JSB Ultimate Multiplayer Server

Это проект для запуска сервера для JSB Ultimate (JSB Port) для игры в мультиплеер.

## Содержание

- [Установка](#установка)
  - [Android](#android)
  - [PC](#pc)
- [Запуск сервера](#запуск-сервера)
- [Настройка конфига игры](#настройка-конфига-игры)
- [Подключение к серверу через локальный сервер](#подключение-к-серверу-через-локальный-сервер)

## Установка

### Android

1. Скачайте и установите приложение Termux из [Google Play](https://play.google.com/store/apps/details?id=com.termux) или [F-Droid](https://f-droid.org/packages/com.termux/).
2. Установите необходимые пакеты в Termux, выполнив следующие команды:

   ```bash
   pkg update && pkg upgrade
   pkg install mono
   pkg install curl
   ```

3. Теперь у вас есть выбор: либо скачивайте файлы сервера прямо на устройство, либо скачивайте прямо из Termux zip-файл через команду `curl`.

   - **Скачивание на устройство:**
     - Скачайте из последнего релиза zip-файл с файлами сервера (не исходный код!) и распакуйте все файлы в удобную для вас директорию.
     - В Termux выполните следующую команду (для доступа к файлам устройства):

       ```bash
       termux-setup-storage
       ```

   - **Скачивание в Termux:**
     - Установите пакет `curl` и `unzip`, затем скачайте и распакуйте архив (в третьей команде замените `{VERSION}` на последнюю версию):

       ```bash
       pkg install curl
       pkg install unzip
       curl -L -o JSBUMultiplayerServer_dotnet31.zip https://github.com/SedTriHEX/JSBUMultiplayerServer/releases/download/{VERSION}/JSBUMultiplayerServer_dotnet31.zip
       mkdir ~/jsbuserver
       cd ~/jsbuserver
       unzip ~/JSBUMultiplayerServer_dotnet31.zip
       ```

### PC

1. Скачайте и установите .NET Core 3.1 с [официального сайта Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/3.1).
2. Скачайте zip-файл из последнего релиза (не исходный код!) и распакуйте все файлы в удобную для вас папку.

## Запуск сервера

- **Windows:** Просто запустите `JustShapesBeatsMultiplayerServer.exe`.
- **Linux:** Запустите сервер через терминал командой:

  ```bash
  dotnet JustShapesBeatsMultiplayerServer.dll
  ```

- **Android:** Запустите сервер через Termux командой:

  ```bash
  mono {полный путь к JustShapesBeatsMultiplayerServer.dll}
  ```

  - Если вы выбрали скачивание прямо в Termux, путь будет:

    ```bash
    mono ~/jsbuserver/JustShapesBeatsMultiplayerServer.dll
    ```

  - Если вы выбрали скачивание на устройство, путь будет:

    ```bash
    mono ~/storage/shared/{путь к папке}/JustShapesBeatsMultiplayerServer.dll
    ```

## Настройка конфига игры

- **Android:** Конфиг файл находится в `/android/data/com.RayNick.JSABMobile/files/config.txt`.
- **Windows:** Конфиг файл находится в `папка с игрой/JSB Ultimate_Data/gameData/config.txt`.

По умолчанию мультиплеер отключен. Для включения мультиплеера установите значение `enable-custom-levels` на `0`:

```plaintext
enable-custom-levels=0
```

Также по умолчанию IP для мультиплеера установлен на `127.0.0.1`, а порт на `25545`. Если нужно изменить IP или порт, установите значения:

```plaintext
multiplayer-ip-address={Айпи}
multiplayer-port={Порт}
```

## Подключение к серверу через локальный сервер

- **Для хоста:** Обычно ничего менять не нужно. Просто запустите сервер и зайдите в игру.
- **Для Android:**
  - Включите точку доступа и подключите друзей к ней.
  - В Termux, при включенном сервере, выполните команду `gameip`. Сервер выдаст все доступные IP-адреса.
  - Выберите любой из списка и введите его в `config.txt` в значение `multiplayer-ip-address`.
  - Если подключение не удается, попробуйте другой IP из списка.
- **Для Windows:**
  - Если сервер запущен на Windows, а играть хотите через Android, отключите брандмауэр Windows, чтобы можно было подключиться к серверу.
  - IP также нужно будет изменить в `config.txt`, узнав его через команду `gameip`.
