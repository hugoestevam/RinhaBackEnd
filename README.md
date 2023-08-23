# RinhaBackend
Projeto em c# com dotnet 7 utilizando minimal api, objetivo foi ser o mais simples possível para um CRUD, sem Cache e/ou Mensagem.
Tentei escrever para participar mas com a mudança de data não deu tempo. 
Mais detalhes no repositório [rinha-de-backend-2023-q3](https://github.com/zanfranceschi/rinha-de-backend-2023-q3)

### Tecnologias
- [C# 11](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Docker](https://www.docker.com/) / [Docker compose](https://docs.docker.com/compose/) (container orchestrator)
- [Nginx](https://www.nginx.com/) (Load balancer)
- [Postgres](https://www.postgresql.org/) (Database)

### Rodar o projeto
Para rodar o projeto, siga as instruções abaixo:
1. No terminal, navegue até o diretório raiz do projeto.
2. Execute o seguinte comando:
```
build.ps1
```
3. Logo após o comando:
```
run.ps1
```
- A **API** estará disponínvel na URL `http://localhost:9999`

### Instruções e Documentação
As regras e documentação pode ser encontrada nas [instruções](https://github.com/zanfranceschi/rinha-de-backend-2023-q3/blob/main/INSTRUCOES.md).