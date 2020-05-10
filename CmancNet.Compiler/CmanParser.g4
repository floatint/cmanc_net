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
	:	((assignStatement | whileStatement | ifStatement | forStatement | subCallStatement | returnStatement | breakStatement) NEWLINE+)+
	;

assignStatement
	:	expr ASSIGN expr
	;

returnStatement
	:	RETURN expr?
	;

breakStatement
	:	BREAK
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
	:	subCallStatement #callOp
	|	expr LS_BRACKET expr RS_BRACKET #indexOp
	|	<assoc=right> (MINUS|NOT) expr #unarOp
	|	expr (MUL|DIV) expr #mulOrDivOp
	|	expr (PLUS|MINUS) expr #addOrSubOp
	|	expr (LESS|GREATER|LESS_OR_EQUAL|GREATER_OR_EQUAL) expr #compOp
	|	expr (EQUAL|NOT_EQUAL) expr #equalsOp
	|	expr LOGIC_AND expr #logicAnd
	|	expr LOGIC_OR expr #logicOr
	|	LR_BRACKET expr RR_BRACKET #parenExpr
	//atoms
	|	var #varLiteral
	|	NULL #null
	|	(TRUE|FALSE) #boolLiteral
	|	STRING #stringLiteral
	|	(INT|HEX|FLOAT) #numberLiteral
	;


var
	:	DOLLAR name
	;


name
	:	IDENT
	;