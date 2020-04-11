lexer grammar CmanLexer;

/*
	Top level lexems
*/

SUB
	:	S U B
	;

END_SUB
	:	E N D '_' S U B
	;

RETURN
	:	R E T U R N
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




IDENT
	:	('a'..'z' | 'A'..'Z' | '_') ('a'..'z' | 'A'..'Z' | '_' | '0' .. '9')*//[a-zA-z_][a-zA-Z_0-9]*
	;

NULL
	:	'null'
	;

TRUE
	:	'true'
	;

FALSE
	:	'false'
	;

STRING
	:	'"' ( EscapeSeq | ~('\\'|'"') )* '"' 
    ;

FLOAT
	:	Digit+ '.' Digit* Exponent?
    |	'.' Digit+ Exponent?
    |	Digit+ Exponent
	;

HEX
	:	'0' [xX] HexDigit+
	;

INT
	:	Digit+
	;

ASSIGN
	:	'='
	;

EQUAL
	:	'=='
	;

NOT_EQUAL
	:	'!='
	;

LESS
	:	'<'
	;

GREATER
	:	'>'
	;

LESS_OR_EQUAL
	:	'<='
	;

GREATER_OR_EQUAL
	:	'>='
	;

NOT
	:	'!'
	;

MUL
	:	'*'
	;

DIV
	:	'/'
	;

PLUS
	:	'+'
	;

MINUS
	:	'-'
	;

LS_BRACKET
	:	'['
	;

RS_BRACKET
	:	']'
	;

LR_BRACKET
	:	'('
	;

RR_BRACKET
	:	')'
	;

COMMA
	:	','
	;

DOLLAR
	:	'$'
	;

WS
	:	(' '| '\t') -> channel(HIDDEN)
	;

NEWLINE
	:	('\n'|'\r'|'\r\n')
	;
//
BLOCK_COMMENT
    :	'/*' .*? '*/' -> skip
;

LINE_COMMENT
    :	'//' ~[\r\n]* -> skip
	;


fragment Exponent
	:	('e'|'E') ('+'|'-')? Digit+//[eE] [+-]? Digit+
    ;

fragment HexDigit
	:	('0'..'9' | 'a'..'f' | 'A'..'F')//[0-9a-fA-F]
	;

fragment Digit
	:	'0'..'9'//[0-9]
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
