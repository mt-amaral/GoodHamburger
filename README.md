# Good Hamburger

Projeto com API em ASP.NET Core + frontend em Blazor WebAssembly

- API REST `.NET 9`
- `Blazor WebAssembly` front 
- `Mudblazor` lib de componentes
- `PostgreSQL` como banco de dados
- `Response` padrão para as respostas da API
- `FluentValidation` validação nos requests (DTO)
- `services` centralizada com a regra de negocio
- `Entity Framework Core` ORM
- `seed` inicial do cardapio
- `ExceptionMiddleware` capturar exceçõesa e devolve no padrão da API
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
- sem combinacao elegivel = sem desconto

## Endpoints principais

- `GET /api/v1/menu/list`
- `GET /api/v1/menu/{id}`
- `GET /api/v1/order/list`
- `GET /api/v1/order/{id}`
- `POST /api/v1/order`
- `PUT /api/v1/order/{id}`
- `DELETE /api/v1/order/{id}`

## Padrao de retorno

A API trabalha com um objeto `Response<T>` simples:

- `data`
- `message`
- `errors`

Mantenha sempre esse padrao nas respostas da API.

## Seed inicial

Se voce iniciar a API com `Staging` ou `FistStart`, ela aplica as migrations e popula o cardapio inicial automaticamente.(staging para rodar no docker-compose, fistStart para rodar local: Desenvolvimento)

Itens iniciais:

- `x-burger`
- `x-egg`
- `x-bacon`
- `fries`
- `soft-drink`

## Rodar com docker

Granta que o docker está em execução
```
docker ps
```
entre na pasta raiz do projeto e execute:
```
docker compose up --build -d
```


Isso sobe:

- `postgres` na porta `5432`
- API em `http://localhost:8080`
- front em `http://localhost:8081`

Observacao:
No `docker-compose` a API roda em `Staging`, entao o Swagger nao fica habilitado.

## Rodar local

Se quiser rodar local sem subir tudo em container:

1. Suba o banco:
   `docker compose up -d postgres`
2. Rode a API com seed:
   `dotnet run --project GoodHamburger.Api --launch-profile FistStart`
3. Rode o front:
   `dotnet run --project GoodHamburger`

Urls locais:

- API: `https://localhost:7002`
- Swagger: `https://localhost:7002/swagger`
- front: `https://localhost:7078`

### GoodHamburger.Test (xUnit)

Os testes estao simples, mas cobrem os pontos principais:

- validacao dos DTOs
- validacao das entidades
- controllers
- services
- algumas partes do front

Rodar testes:
**dotnet test**
