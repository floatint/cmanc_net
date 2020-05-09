# Clickermann .NET compiler

Компилятор подмножества скриптового языка [Clickermann](https://ru.wikipedia.org/wiki/Clickermann).

Основная отличительная черта - по сравнению с оригиналом было решено сделать язык процедурным. 
Возможно, это своеобразный инструмент создания 'хранимых' процедур. Которые сохраняются в сборки EXE или DLL
и готовы к выполнению как самостоятельные программы или через оболочку Clickermann. 

Остальной список отличий не точен и может дополняться(надеюсь, сокращаться) по мере развития.

Компиляция производится в CIL. Для парсинга использует ANTLR 4.

## Примеры на языке
### Пример 1

Простая печать аргументов коммандной строки

```
sub(main, $args)
	$argc = arrsize($args)
	for($i = 0, $i < $argc)
		print($args[$i])
	end_cyc
end_sub
```

### Пример 2

Складывает при помощи подпрограммы два числа и на основании результата выводит на экран

```
sub(my_sub, $a, $b)
	return $a + $b
end_sub


sub(main, $args)
	if (my_sub($args[1],$args[2]) < 24)
		print("< 24")
	else
		print(">= 24")
	end_if
end_sub
```


### TODO list:
 - [ ] AST Parser
    - [x] Compilation unit
    - [x] Procedure statement
        - [x] Arguments list
        - [x] Return statement
    - [x] Body statement
    - [ ] Loops
        - [ ] Break statement
        - [x] While loop
        - [x] For loop
	- [x] Flow control
		- [x] If statement
			- [x] Else statement
    - [x] Assign statement
    - [x] Expressions
    - [x] Procedure call's
	- [ ] Binary operators
		- [x] Additional
		- [x] Substraction
		- [x] Multiply
		- [x] Division
		- [x] Less
		- [x] Greater
		- [x] Equal
		- [ ] LessOrEqual
		- [ ] GreaterOrEqual
		- [ ] Not equal
	- [x] Unary operators
		- [x] Not
		- [x] Minus
		- [x] Indexing
 - [x] AST Processors
	- [x] Symbol table builder
	- [x] Semantic checker
		- [x] Undefined variables
		- [x] Undefined subroutines
		- [x] Subroutine call arguments count check
		- [x] Subroutine call return check
		- [x] Assignment to rvalue check
		- [x] Dead code detection
		- [x] Implicit casts detection
 - [ ] Codegen
	- [ ] Subroutines
		- [ ] Return statement
	- [x] Expressions
		- [ ] Binary
			- [x] Additional
			- [x] Substraction
			- [x] Multiply
			- [x] Division
			- [x] Equal
			- [x] Less
			- [x] Greater
			- [ ] LessOrEqual
			- [ ] GreaterOrEqual
			- [ ] Not equal
		- [x] Unary
		- [x] Subroutine calls
	- [ ] Loops
		- [x] For loop
		- [x] While loop
		- [ ] Break statement
	- [x] Flow control
		- [x] If statement
			- [x] Else statement

## AST parser I/O example

### Input (Examples/example_1.txt)

```
sub(test)
$a = $c[0][2][7]
end_sub

sub(add, $a, $b)
	
	$c = $a + $b

end_sub

sub(main, $args)
	$argCnt = arrsize($args)
	$num  = add(4,4)
end_sub
```

### Output

```
ASTCompileUnitNode(1,1): [example_1]
	ASTSubStatementNode(1,1): [test]
		ASTBodyStatementNode(2,1)
			ASTAssignStatementNode(2,1)
				ASTVariableNode(2,1): [a]
				ASTIndexOpNode(2,14)
					ASTIndexOpNode(2,11)
						ASTIndexOpNode(2,8)
							ASTVariableNode(2,6): [c]
							ASTNumberLiteralNode(2,9): [0]
						ASTNumberLiteralNode(2,12): [2]
					ASTNumberLiteralNode(2,15): [7]
	ASTSubStatementNode(5,1): [add]
		ASTArgListNode(5,8)
			ASTVariableNode(5,10): [a]
			ASTVariableNode(5,14): [b]
		ASTBodyStatementNode(7,2)
			ASTAssignStatementNode(7,2)
				ASTVariableNode(7,2): [c]
				ASTAddOpNode(7,10)
					ASTVariableNode(7,7): [a]
					ASTVariableNode(7,12): [b]
	ASTSubStatementNode(11,1): [main]
		ASTArgListNode(11,9)
			ASTVariableNode(11,11): [args]
		ASTBodyStatementNode(12,2)
			ASTAssignStatementNode(13,2)
				ASTVariableNode(13,2): [num]
				ASTCallStatementNode(13,10): [add]
					ASTExprListNode(13,14)
						ASTNumberLiteralNode(13,14): [4]
						ASTNumberLiteralNode(13,16): [4]
			ASTAssignStatementNode(12,2)
				ASTVariableNode(12,2): [argCnt]
				ASTCallStatementNode(12,12): [arrsize]
					ASTExprListNode(12,20)
						ASTVariableNode(12,20): [args]
```