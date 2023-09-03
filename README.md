# Pascal to x86-asm compiler

1. Компилятор обрабатывает Pascal-подобный код, написанный в CompilerV2/bin/Debug/net5.0/Text.txt
2. В консоль выводятся сообщения об ошибках/успешном компилировании
3. Сгенерированный asm-файл можно будет найти здесь же: CompilerV2/bin/Debug/net5.0

Запуск assembly-кода:
  1. mount d: /Users/your_user/Asm (Ваш путь к пакету ассемблера, туда же перемещаем сгенерированный .asm)
  2. d: 
  3. tasm code.asm - ассемблирование
  4. tlink code.obj – создание .exe
  5. td code – его запуск в отладчике
