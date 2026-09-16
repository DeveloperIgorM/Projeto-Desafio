# Decisões de arquitetura

Registro simplificado de decisões (estilo ADR), justificando escolhas feitas para viabilizar a entrega em prazo curto sem abrir mão dos fundamentos pedidos no desafio.

## Escopo cortado deliberadamente
- Sem GraphQL, SignalR/real-time, PWA, i18n, exportação em PDF, roles avançadas no Keycloak (Admin/Manager/User) — fora do prazo disponível.
- Testes: foco em regras de negócio de maior risco (cálculo de valor de estoque, detecção de estoque baixo), sem perseguir cobertura mínima de 85%.
- Documentação de arquitetura: este arquivo substitui ADRs extensos e C4 Model formal.

## Vite em vez de Next.js
Next.js agrega SSR/roteamento de servidor que não é exigido pelos critérios do desafio. Vite entrega o mesmo React 18 + TS com setup e build mais simples.

## MongoDB.Driver em vez do provider EF Core para MongoDB
Driver oficial é mais maduro, mais documentado e mais previsível para quem está começando com MongoDB, evitando armadilhas do provider EF (que trata Mongo como se fosse relacional em vários pontos).

## Keycloak com import automático de realm
O `docker-compose` sobe o Keycloak já com `keycloak/realm-export.json` importado (`--import-realm`), evitando reconfiguração manual a cada `docker-compose up` — decisão sobretudo pensada para a fase de testes finais e gravação do vídeo de demonstração.

## Modelagem no MongoDB
`Product` referencia `CategoryId` (sem "join"); quando o nome da categoria for necessário na listagem, é feito um lookup/agregação específico em vez de embutir o documento completo da categoria.
