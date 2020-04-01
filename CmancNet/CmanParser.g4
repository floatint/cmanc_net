parser grammar CmanParser;

options {tokenVocab = CmanLexer;}

compileUnit
	:	(procStatement NEWLINE*)*
	|	EOF
	;


procStatement
	:	PROC LR_BRACKET name argListDecl? RR_BRACKET NEWLINE+
			(bodyStatement)?
		END_PROC
	;

bodyStatement
	:	((assignStatement | whileStatement | procCallStatement) NEWLINE+)+
	;

assignStatement
	:	var ASSIGN expr
	;

returnStatement
	:	RETURN expr
	;

whileStatement
	:	WHILE LR_BRACKET expr RR_BRACKET NEWLINE+
			(bodyStatement)?
		END_CYC
	;

procCallStatement
	:	name LR_BRACKET exprList? RR_BRACKET
	;


argListDecl
	:	(COMMA var)+
	;

exprList
	:	expr (COMMA expr)*
	;

expr
	:	null
	|	true
	|	false
	|	varOrExpr
	|	procCallStatement
	|	stringLiteral
	|	numberLiteral
	
	|	expr indexStatement

	|	unarOp expr
	|	expr mulOrDivOp expr
	|	expr addOrSubOp expr
	|	expr compOp expr
	;

varOrExpr
	:	var
	|	LR_BRACKET expr RR_BRACKET
	;

var
	:	DOLLAR name
	;
//?
indexStatement
	:	LS_BRACKET expr RS_BRACKET
	;

name
	:	IDENT
	;


compOp
	:	EQUAL
	|	NOT_EQUAL
	|	LESS_OR_EQUAL
	|	GREATER_OR_EQUAL
	;

mulOrDivOp
	:	MUL
	|	DIV
	;

addOrSubOp
	:	PLUS
	|	MINUS
	;

unarOp
	:	NOT
	|	MINUS
	;

null
	:	NULL
	;

true
	:	TRUE
	;

false
	:	FALSE
	;

numberLiteral
	:	INT
	|	HEX
	|	FLOAT
	;

stringLiteral
	:	STRING
	;