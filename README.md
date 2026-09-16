# Projeto Desafio Hypesoft — Sistema de Gestão de Produtos

> Status: em desenvolvimento (Dia 1 — infraestrutura e esqueleto).

Sistema de gestão de produtos com autenticação via Keycloak, backend em .NET 9 (Clean Architecture + CQRS) e frontend em React + Vite.

## Stack
- **Backend**: .NET 9, Clean Architecture, CQRS + MediatR, MongoDB.Driver, FluentValidation, Serilog
- **Frontend**: React 18 + TypeScript, Vite, TailwindCSS + shadcn/ui, React Query, React Hook Form + Zod
- **Infra**: MongoDB, Keycloak, Docker Compose

## Como rodar
Pré-requisitos: Docker Desktop, .NET 9 SDK (para desenvolvimento local fora do container), Node 18+.

```bash
docker-compose up -d --build
docker-compose ps
```

URLs:
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Mongo Express: http://localhost:8081
- Keycloak: http://localhost:8080 (admin/admin)

Usuário de teste (já importado no realm): `igor` / `igor123` (roles: admin, user).

## Decisões de arquitetura
Ver [docs/decisions.md](docs/decisions.md).

## Estrutura
Ver árvore de pastas em `backend/src` (Domain/Application/Infrastructure/API) e `frontend/`.
