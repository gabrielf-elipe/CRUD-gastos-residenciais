# 🏠Sistema de Controle de Gastos Residenciais

API RESTful simples desenvolvida em .NET para controle de gastos residenciais, com CRUD de pessoas e transações, regras de negócio por perfil de usuário e cálculo de totais.

## Tecnologias utilizadas

**Back-end**
- .NET / C#
- ASP.NET Core Web API
- Entity Framework Core
- SQLite 

## Como executar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado
- Ferramenta `dotnet-ef` instalada:
  ```
  dotnet tool install --global dotnet-ef
  ```

### Back-end

1. Acesse a pasta do back-end:
   ```
   cd backend
   ```
2. Restaure as dependências:
   ```
   dotnet restore
   ```
3. Aplique as migrations para criar o banco de dados:
   ```
   dotnet ef database update
   ```
4. Execute a aplicação:
   ```
   dotnet run
   ```
5. A API provavelmente estará disponível em `http://localhost:5237` (ou em outra porta exibida no terminal). A documentação interativa dos endpoints pode ser acessada via Scalar.

P.S: Durante umas semanas eu estava utilizando o Swagger, mas descobri que ele já é considerado ""datado"", então migrei pro Scalar, mas utilizando uma interface parecida, pois me acostumei com a do Swagger.


## Funcionalidades

### Cadastro de Pessoas
- Criação, listagem, consulta específica e remoção de pessoas.
- Cada pessoa possui: identificador único (gerado automaticamente), nome e idade.
- Ao remover uma pessoa, todas as transações vinculadas a ela são removidas automaticamente.

### Cadastro de Transações
- Criação e listagem de transações.
- Cada transação possui: identificador único (também gerado automaticamente), descrição, valor, tipo (receita/despesa) e um id de pessoa vinculada.
- Regra de negócio: pessoas menores de 18 anos só podem ter transações do tipo despesa.
- A pessoa informada na transação precisa existir previamente no cadastro, se não a transação não será criada.

### Consulta de Totais
- Exibe para cada pessoa cadastrada o total de receitas, total de despesas e o saldo (receitas - despesas)
- Exibe, ao final, o total geral de receitas, despesas e saldo líquido de todas as pessoas

## Endpoints principais da API

| Método | Rota                     | Descrição                                   |
|--------|---------------------------|----------------------------------------------|
| POST   | /api/Pessoas               | Cadastra uma nova pessoa                      |
| GET    | /api/Pessoas               | Lista todas as pessoas                        |
| GET    | /api/Pessoas/busca-por-nome               | Lista todas as pessoas com o nome especificado                       |
| GET    | /api/Pessoas/pessoas/totais         | Retorna os totais por pessoa e o total geral  |
| GET    | /api/Pessoas/{id}               | Consulta pessoa específica pelo id                       |
| PUT    | /api/Pessoas/{id}        | atualiza informações da pessoa com o id especificado  |
| DELETE | /api/Pessoas/{id}          | Remove a pessoacom o id especificado e suas transações           |
| POST   | /api/Transacoes               | Cadastra uma nova transação                      |
| GET    | /api/Transacoes            | Lista todas as transações                     |
| POST   | /api/Transacoes/{id}            | Consulta uma transação específica pelo id                   |

## ☀️ Possíveis futuros
Desejo tornar esse projeto algo mais próximo de um sistema real, portanto trabalharei em:
- frontend responsivo 
- Migrar do SQLite para bancos como PostgreSQL, SQL Server...
- Autenticação e autorização