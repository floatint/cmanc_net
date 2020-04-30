parser grammar CmanParser;

options {tokenVocab = CmanLexer;}

compileUnit
	:	(NEWLINE* subStatement)*
	|	NEWLINE*
	|	EOF
	;


subStatement
	:	SUB LR_BRACKET name argListDecl? RR_BRACKET NEWLINE+
			(bodyStatement)?
		END_SUB
	;

bodyStatement
	:	((assignStatement | whileStatement | ifStatement | forStatement | subCallStatement | returnStatement) NEWLINE+)+
	;

assignStatement
	:	expr ASSIGN expr
	;

returnStatement
	:	RETURN varOrExpr?
	;

ifStatement
	:	IF LR_BRACKET expr RR_BRACKET NEWLINE+
			(bodyStatement)?
			(elseStatement)?
		END_IF
	;

elseStatement
	:	ELSE NEWLINE+
			(bodyStatement)?
	;


whileStatement
	:	WHILE LR_BRACKET expr RR_BRACKET NEWLINE+
			(bodyStatement)?
		END_CYC
	;

forStatement
	:	FOR LR_BRACKET counterDecl COMMA expr stepDecl? RR_BRACKET NEWLINE+
			(bodyStatement)?
		END_CYC
	;

counterDecl
	:	var
	|	assignStatement
	;

stepDecl
	:	COMMA expr
	;

subCallStatement
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
	|	subCallStatement
	|	stringLiteral
	|	numberLiteral
	
	|	expr indexOp

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


indexOp
	:	LS_BRACKET expr RS_BRACKET
	;

name
	:	IDENT
	;


compOp
	:	EQUAL
	|	NOT_EQUAL
	|	LESS
	|	GREATER
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