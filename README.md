# Clickermann .NET compiler

Компилятор подмножества скриптового языка [Clickermann](https://ru.wikipedia.org/wiki/Clickermann).

Основная отличительная черта - по сравнениею с оригиналом было решено сделать язык процедурным. 
Возможно, это своеобразный инструмент создания 'хранимых' процедур. Которые сохраняются в сборки EXE или DLL
и готовы к выполнению как самостоятельные программы или через оболочку Clickermann. 

Остальной список отличий не точен и может дополняться(надеюсь, сокращаться) по мере развития.

Компиляция производится в CIL. Для парсинга использует ANTLR 4.

### TODO list:
 - [ ] AST Parser
    - [x] Compilation unit
    - [x] Procedure statement
        - [x] Arguments list
        - [ ] Return statement
    - [x] Body statement
    - [ ] Loops
        - [ ] Break statement
        - [ ] While loop
        - [ ] For loop
    - [x] Assign statement
    - [x] Expressions
    - [x] Procedure call's
    - [x] Arithmatic operation (+, -, *, /)
 - AST Processors
 - Codegen
 - Assemblies builder

## Примеры

### Пример 1

Простая печать аргументов коммандной строки

```
proc(main, $args)
	$argc = arrsize($args)
	for($i = 0, $i < $argc)
		print($args[$i])
	end_cyc
end_proc
```

### Пример 2

Складывает при помощи подпрограммы два числа и на основании результата выводит на экран

```
proc(my_sub, $a, $b)
	return $a + $b
end_proc


proc(main, $args)
	if (my_sub($args[1],$args[2]) < 24)
		print("< 24")
	else
		print(">= 24")
	end_if
end_proc
```