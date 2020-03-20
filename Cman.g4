grammar Cman;


/*
 * Parser Rules
 */

compileUnit
	:	(procStatement)*
	|	EOF
	;


//top level rules

/*
	Statements block
*/
bodyStatement
	:	(assignStatement | whileStatement | procCallStatement)
		NEWLINE*
	;

/*
	Procedure statement	
*/
procStatement
	:	PROC '(' IDENT varList ')' NEWLINE+
			(bodyStatement)*
		END_PROC
	;


/*
	While loop statement
*/
whileStatement
	:	WHILE '(' expr ')' NEWLINE+
			(bodyStatement)*
		END_CYC
	;


/*
	Assign statement
*/
assignStatement
	:	var '=' expr
	//|	var arrayItem '=' expr
	;

/*arrayItem
	:	'[' expr ']' ('[' expr ']')*
	;*/

procCallStatement
	:	IDENT '(' exprList? ')'
	;
 

varList
	:	(',' var)*
	;

exprList
	:	expr (',' expr)*
	;

expr
	:	TRUE
	|	FALSE
	|	NULL
	|	varOrExpr
	|	procCallStatement
	|	constString
	|	constNumber
	|	unarMinusOp expr
	|	expr mulOrDivOp expr
	|	expr addOrSubOp expr
	;



varOrExpr
    :	var
	|	'(' expr ')'
    ;



argList
	:	expr (',' expr)*
	;

var
	:	VARIABLE
	;

constString
	:	STRING
	;

constNumber
	:	INT
	//|	HEX
	//|	FLOAT
	;

/*
	OPERATORS
*/

orOp
	:	'||'
	;

andOp
	:	'&&'
	;

equalOp
	:	'=='
	;

notEqualOp
	:	'!='
	;

addOrSubOp
	:	'+'
	|	'-'
	;

mulOrDivOp
	:	'*'
	|	'/'
	;


unarMinusOp
	:	'-'
	;

unarNotOp
	:	'!'
	;

powOp
	:	'^'
	;

/*
 * Lexer Rules
 */


/*
	KEYWORDS
*/
NULL
	:	N U L L
	;

TRUE
	:	T R U E
	;

FALSE
	:	F A L S E
	;

FOR
	:	F O R
	;

WHILE
	:	W H I L E
	;

END_CYC
	:	E N D '_' C Y C
	;

PROC
	:	P R O C
	;

END_PROC
	:	E N D '_' P R O C
	;

RETURN
	:	R E T U R N
	;

/*
	LANG CONSTRUCTS
*/


VARIABLE
	:	'$' IDENT
	;

IDENT
	:	('a'..'z' | 'A'..'Z' | '_') ('a'..'z' | 'A'..'Z' | '_' | '0' .. '9')*//[a-zA-z_][a-zA-Z_0-9]*
	;


/*
	CONST LITERALS
*/
STRING
	:	'"' ( EscapeSeq | ~('\\'|'"') )* '"' 
    ;

FLOAT
    :	Digit+ '.' Digit* Exponent?
    |	'.' Digit+ Exponent?
    |	Digit+ Exponent
    ;

HEX
	:	'0'[xX]HexDigit+
	;

INT
	:	Digit+//[0-9][0-9]*//Digit+
	;


/*
	DELIMITERS
*/
WS:	(' '|'\t') -> channel(HIDDEN);
NEWLINE: ('\n'|'\r'|'\r\n');


/*
	LOW LEVEL SUBRULES
*/
fragment Exponent
	:	[eE] [+-]? Digit+
    ;

fragment HexDigit
	:	[0-9a-fA-F]
	;

fragment Digit
	:	[0-9]
	;

fragment EscapeSeq
	:	'\\' [abfnrtvz"'\\]
    |	'\\' '\r'? '\n'
	;

fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');