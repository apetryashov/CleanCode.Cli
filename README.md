# clean-code cli

## Основные ссылки

git: [https://github.com/apetryashov/CleanCode.Cli](https://github.com/apetryashov/CleanCode.Cli)

Установочный файл: [https://apetryashov.github.io/utils/clean-code/clean-code.installer.exe](https://apetryashov.github.io/utils/clean-code/clean-code.installer.exe)

Последний релиз: [1.2](https://github.com/apetryashov/CleanCode.Cli/releases/tag/1.2.0.0)

## Текущий функционал:

- Установка одним exe файлов
- Автообновление утилиты на основе github releases
- Автообновление resarper-clt
- Возможность использовать как глобальные .DotSettings файл, так и кастомный файл
- Проверка происходит только для измененных файлов.
    - Для новых проектов в первый запуск будут проверены все файл
    - После обновления resharper-clt, кеш очищается
    - Можно делать фулл проверку при помощи ключа **-f\—forse**
- Формирование как консольного так и html отчета о работе для функции code-inspections
- ...

## Описание:

Данная утилита позволяет из консоли запустить статический анализ .net проекта и провести реформат кода в соответствии с описанными в файле .DotSettings файле правил. 

По сути, утилита являет оберткой над консольной утилитой resharper-clt от Jetbrains. Данная утилита используется в Resharper и Rider.

Для чего это нужно? 

1. Для многих cli утилиты являются более удобными в использовании
2. Возможность встраивания в процесс ci.
Дело в том, что мы используем teamCity для прогона тестов. На основе этой утилиты можно легко сделать шаг, проверяющий code-style проекта. Это выступает гарантом единообразия кода для всех контрибьюторов.
3. Гарантия того, что всегда используется последняя версия как утилиты, так и инспекций.

## Поддерживаемые команды:

```
Info:
  cleanup             Start ReSharper cleanup tool for given directory
  code-inspections    Start ReSharper code-inspection tool for given directory
  update              Update resharper-clt tool
  help                Display more information on a specific command.

Copyright (C) 2020 @apetryashov v1.2.0.0
```
