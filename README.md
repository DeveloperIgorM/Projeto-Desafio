# Hypesoft — Sistema de Gestão de Produtos

Desafio técnico da Hypesoft: um sistema de gestão de produtos com CRUD de produtos e categorias, controle de estoque, dashboard e autenticação via Keycloak. Backend em .NET 9 (Clean Architecture + CQRS) e frontend em React + Vite.

## Stack

**Backend**: .NET 9, Clean Architecture (Domain/Application/Infrastructure/API), CQRS + MediatR, MongoDB.Driver, FluentValidation, Serilog, xUnit + FluentAssertions.

**Frontend**: React 19 + TypeScript, Vite, TailwindCSS v4, componentes no padrão shadcn/ui (montados manualmente, ver `docs/decisions.md`), React Query, React Hook Form + Zod, Recharts, keycloak-js.

**Infra**: MongoDB, Keycloak (realm importado automaticamente), Docker Compose.

## Como rodar

Pré-requisito: Docker Desktop.

```bash
git clone https://github.com/DeveloperIgorM/Projeto-Desafio.git
cd Projeto-Desafio
docker-compose up -d --build
```

Aguarde alguns segundos pros containers subirem (o Keycloak demora um pouco mais que os outros) e confira com `docker-compose ps`.

| Serviço | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |
| Mongo Express | http://localhost:8081 |
| Keycloak | http://localhost:8080 (admin console: `admin` / `admin`) |

### Usuários de teste

O realm do Keycloak já sobe importado com dois usuários, pra dar pra testar a autorização por role sem precisar configurar nada:

| Usuário | Senha | Roles |
|---|---|---|
| `igor` | `igor123` | `admin`, `user` |
| `usuario` | `usuario123` | `user` |

No frontend, quem loga como `usuario` consegue ver produtos, categorias e dashboard normalmente, mas não vê os botões de criar/editar/excluir nem consegue ajustar estoque — essas ações exigem a role `admin`.

### Rodando fora do Docker (desenvolvimento local)

Backend:
```bash
cd backend
dotnet restore
dotnet run --project src/Hypesoft.API
```

Frontend:
```bash
cd frontend
npm install
npm run dev
```

Nesse caso o Mongo e o Keycloak ainda precisam estar rodando (via `docker-compose up -d mongodb keycloak`), e as variáveis de ambiente do frontend (`VITE_API_URL`, `VITE_KEYCLOAK_URL`, etc.) precisam ir num `.env` — veja `frontend/.env.example`.

## Testes

```bash
cd backend
dotnet test
```

Os testes cobrem as regras de negócio (`Product.IsLowStock()` e os cálculos de `StockCalculator` — valor total em estoque e filtro de estoque baixo). Não persegui uma porcentagem de cobertura; priorizei testar o que tem lógica de verdade, em vez de testar getters/setters e DTOs.

## Funcionalidades

- CRUD de produtos (nome, descrição, preço, categoria, estoque) com validação via FluentValidation.
- CRUD de categorias, com bloqueio de exclusão se ainda houver produto usando ela.
- Busca de produtos por nome, filtro por categoria, paginação.
- Controle de estoque: atualização manual e listagem de produtos com estoque abaixo de 10 unidades.
- Dashboard: total de produtos, valor total em estoque, produtos com estoque baixo e gráfico de produtos por categoria.
- Autenticação via Keycloak (OAuth2/OIDC), com o frontend inteiro protegido atrás de login e autorização por role nas ações de escrita.
- Rate limiting por IP e headers de segurança básicos na API.
- Health check em `/health`.

## Decisões de arquitetura

O racional por trás de boa parte das escolhas — inclusive os cortes de escopo, os trade-offs de prazo e um bug real que encontrei no caminho — está em [`docs/decisions.md`](docs/decisions.md). Vale a leitura pra entender o "porquê" além do "o quê".

## Estrutura

```
backend/
  src/
    Hypesoft.Domain/          # Entidades e interfaces de repositório, sem dependência de infra
    Hypesoft.Application/     # Commands/Queries (CQRS) + MediatR, DTOs, validação
    Hypesoft.Infrastructure/  # Repositórios MongoDB.Driver, configuração de DI
    Hypesoft.API/             # Controllers, middlewares, Program.cs
  tests/
    Hypesoft.Tests/           # xUnit + FluentAssertions

frontend/
  src/
    components/                # UI (padrão shadcn) e layout
    contexts/                  # AuthContext (Keycloak)
    hooks/                     # Hooks de React Query (produtos, categorias, dashboard)
    lib/                       # Cliente Keycloak, cliente axios, utilitário cn()
    pages/                     # Dashboard, Produtos, Categorias

keycloak/
  realm-export.json           # Realm importado automaticamente no docker-compose up
```

## Vídeo

Link do vídeo de demonstração: https://www.loom.com/share/204f087bf81f4fb8b21d572dadfd7f5e
