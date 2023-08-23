CREATE TABLE public.Pessoas (
	id UUID PRIMARY KEY NOT NULL,
	apelido VARCHAR(32) UNIQUE NOT NULL,
	nome VARCHAR(100) NOT NULL,
	nascimento DATE NOT NULL,
	stack TEXT[] NULL
);

CREATE INDEX pessas_apelido_index ON pessoas(apelido);
CREATE INDEX pessoas_array_text_stack ON pessoas USING GIN(stack);