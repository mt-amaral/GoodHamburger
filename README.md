# Good Hamburger

Projeto com API em ASP.NET Core + frontend em Blazor WebAssembly

- API REST `.NET 9`
- `Blazor WebAssembly` front
- `MudBlazor` lib de componentes
- `PostgreSQL` como banco de dados
- `Response` padrão para as respostas da API
- `FluentValidation` validação nos requests (DTO)
- `services` centralizada com a regra de negócio
- `Entity Framework Core` ORM
- `seed` inicial do cardápio
- `ExceptionMiddleware` captura exceções e devolve no padrão da API
- testes automatizados para API, DTOs, entidades e front

## Estrutura

- `GoodHamburger.Api` API
- `GoodHamburger` frontend WASM
- `GoodHamburger.Shared` DTOs e objetos compartilhados
- `GoodHamburger.Test` para os testes automatizados

## Regras do pedido

- cada pedido pode ter 1 sandwich
- cada pedido pode ter 1 batata
- cada pedido pode ter 1 refrigerante
- sandwich + batata + refrigerante = 20% de desconto
- sandwich + refrigerante = 15% de desconto
- sandwich + batata = 10% de desconto
- sem combinação elegível = sem desconto

## Endpoints principais

- `GET /api/v1/menu/list`
- `GET /api/v1/menu/{id}`
- `GET /api/v1/order/list`
- `GET /api/v1/order/{id}`
- `POST /api/v1/order`
- `PUT /api/v1/order/{id}`
- `DELETE /api/v1/order/{id}`

## Padrão de retorno

A API trabalha com um objeto `Response<T>` simples:

- `data`
- `message`
- `errors`

Mantenha sempre esse padrão nas respostas da API.

## Seed inicial

Se você iniciar a API com `Staging` ou `FistStart`, ela aplica as migrations e popula o cardápio inicial automaticamente. (`Staging` para rodar no docker-compose, `FistStart` para rodar local: Desenvolvimento)

Itens iniciais:

- `x-burger`
- `x-egg`
- `x-bacon`
- `fries`
- `soft-drink`

## Rodar com docker

Garanta que o docker está em execução:

```bash
docker ps
```

Entre na pasta raiz do projeto e execute:

```bash
docker compose up --build -d
```

Isso sobe:

- `postgres` na porta `5432`
- API em `http://localhost:8080`
- front em `http://localhost:8081`

Observação:
No `docker-compose` a API roda em `Staging`, então o Swagger não fica habilitado.

## Rodar local

Se quiser rodar local sem subir tudo em container:

1. Suba o banco:
   ```bash
   docker compose up -d postgres
   ```

2. Rode a API com seed:
   ```bash
   dotnet run --project GoodHamburger.Api --launch-profile FistStart
   ```

3. Rode o front:
   ```bash
   dotnet run --project GoodHamburger
   ```

URLs locais:

- API: `https://localhost:7002`
- Swagger: `https://localhost:7002/swagger`
- front: `https://localhost:7078`

### GoodHamburger.Test (xUnit)

Os testes estão simples, mas cobrem os pontos principais:

- validação dos DTOs
- validação das entidades
- controllers
- services
- algumas partes do front

Rodar testes:

```bash
dotnet test
```
