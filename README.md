# Pascal to x86-Asm compiler
Компилятор обрабатывает Pascal-подобный код, написанный в 'project_name'/bin/Debug/net5.0/Text.txt \n
В консоль выводятся сообщения об ошибках/успешном компилировании \n
Сгенерированный asm-файл можно будет найти здесь же: 'project_name'/bin/Debug/net5.0 

# Запуск сгенерированного TASM-кода в DosBox:
  1. На хост-ОС копируем сгенерированный code.asm в папку с пакетом TASM
  2. запускаем DosBox и выполняем следующие команды:
  3. tasm code.asm - ассемблирование (создается code.obj)
  4. tlink code.obj – линковка (создание code.exe)
  5. td code – запуск code.exe в отладчике
