# Decisões e anotações do projeto

Esse arquivo é basicamente um caderno de bordo com as escolhas que fiz e os porquês. Vim de PHP + Angular + MySQL, então parte disso aqui é literalmente eu registrando o motivo de ter escolhido um caminho mais simples em vez de "fazer certo" nos mínimos detalhes — o prazo é curto e prefiro entregar algo coerente e funcionando do que começar a complicar em cima de tecnologia que ainda tô aprendendo.

## O que decidi não fazer
GraphQL, SignalR, PWA, i18n, exportação em PDF, roles avançadas no Keycloak (Admin/Manager/User) — não entrou. Não é falta de saber que existe, é questão de prazo mesmo: prefiro ter o CRUD, o dashboard e a autenticação bem feitos do que espalhar esforço em extra.

Nos testes, fui direto nas regras de negócio que realmente importam (cálculo de valor de estoque, detecção de estoque baixo) em vez de perseguir uma porcentagem de cobertura.

## Vite em vez de Next.js
Não precisava de SSR pra esse projeto, então fui de Vite mesmo — setup mais rápido e eu já tinha mais familiaridade com o modelo "SPA" vindo do Angular.

## MongoDB.Driver em vez do provider EF Core pra Mongo
Como eu nunca tinha mexido com MongoDB antes (vim de MySQL), preferi o driver oficial. É mais direto, tem mais exemplo na documentação, e evita que eu tropece em alguma abstração do EF tentando fazer Mongo parecer banco relacional.

## Realm do Keycloak importado automático no docker-compose
Configurar o Keycloak na mão toda hora que eu resetasse o ambiente ia consumir tempo que eu não tenho — então o realm já sobe pronto (`keycloak/realm-export.json`) via `--import-realm`.

## Modelagem no Mongo
`Product` guarda só o `CategoryId`, sem embutir a categoria inteira. Quando precisar do nome da categoria numa listagem, isso é resolvido com uma consulta/lookup separada — nada de tentar simular JOIN.

## Bug real que caí: Authority x Issuer do Keycloak
Isso aqui vale registrar porque não é óbvio de primeira. O backend, rodando dentro do Docker, busca as chaves públicas do Keycloak usando o nome interno do container (`http://keycloak:8080`). Só que quando eu logo de verdade (via curl, ou depois pelo frontend no navegador), estou acessando o Keycloak por `http://localhost:8080` — e é esse endereço que fica gravado no token como `iss` (quem emitiu). Configurei os dois iguais no começo e a validação simplesmente falhava sem erro nenhum aparecer (401 sem corpo, bem difícil de debugar de primeira). A solução foi separar: `Keycloak:Authority` continua interno (pra buscar as chaves), e `Keycloak:ValidIssuer` é o externo (pra bater com o token). Achei que valia registrar porque é o tipo de coisa que qualquer setup Docker + Keycloak vai esbarrar.

## Dashboard calculado em memória, sem aggregation pipeline
Pra montar o resumo do dashboard (total de produtos, valor total em estoque, produtos por categoria) eu simplesmente busco todos os produtos e categorias e calculo em C# (`StockCalculator` + um `GroupBy`), em vez de montar uma aggregation pipeline no Mongo. Pro volume de dados desse desafio isso não faz diferença nenhuma de performance, e o código fica bem mais fácil de ler e de testar. Se o catálogo crescesse pra dezenas de milhares de produtos, aí sim valeria migrar isso pra uma pipeline no banco.

## shadcn/ui montado na mão, sem o CLI
Em vez de rodar `npx shadcn init`/`add` (que é interativo e não ia funcionar direito no meu fluxo de terminal remoto), copiei manualmente os componentes que precisava (Button, Input, Label, Card) seguindo o mesmo padrão que o CLI geraria. Junto com isso, não instalei os pacotes do Radix UI: os únicos componentes "complexos" que eu precisava (modal de criar/editar produto, modal de ajuste de estoque) resolvi com um componente de modal simples feito na mão, então não fazia sentido trazer mais uma dependência só pra isso.

## Autorização por role no frontend
Os dois usuários do realm (`igor` com admin+user, `usuario` só com user) dão pra mostrar isso na prática: quem não tem a role `admin` visualiza produtos e categorias normalmente, mas não vê os botões de criar/editar/excluir nem consegue ajustar estoque. É simples, mas cobre o requisito de "autorização baseada em roles do Keycloak" sem eu precisar montar uma tela de gestão de permissões completa.

## Login forçado (onLoad: 'login-required')
Preferi deixar o Keycloak barrar o acesso à aplicação inteira antes dela renderizar, em vez de deixar entrar e só bloquear rotas específicas depois. Pra um painel interno como esse faz mais sentido e simplifica a lógica de proteção de rota — não precisei de uma tela de "login" própria, o próprio Keycloak assume esse trabalho.

## Revisão final: fechando pontas soltas de segurança
Antes de fechar o projeto, dei uma passada geral procurando TODO e requisito que tivesse ficado pra trás. Encontrei três coisas:

1. **`ValidateAudience` estava desligado** desde o Dia 1 (deixei um TODO no código). Tentei ligar de volta assumindo que o `aud` do token viria como `"account"` (comportamento padrão em muitos setups de Keycloak) — mas ao testar quebrou todos os endpoints autenticados (criar categoria, criar produto). O token desse client especificamente não carrega esse audience, então reverti pra `ValidateAudience = false` e deixei anotado no código pra eu conferir o conteúdo real do token antes de tentar de novo, em vez de chutar. Fica como aprendizado: não vale a pena mexer em validação de token sem antes decodificar um token de verdade pra ver o que tem lá dentro.
2. **Rate limiting não existia** — o próprio README do desafio pede isso na seção de segurança e eu tinha deixado passar. Adicionei um limiter simples por IP (100 requisições/minuto) usando o `RateLimiter` nativo do ASP.NET Core, sem precisar de Redis ou infra extra.
3. **Sem nenhum header de segurança** — adicionei os básicos (`X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`). Não é uma lista exaustiva, mas cobre os problemas mais óbvios (sniffing de MIME type, clickjacking, vazamento de referrer).

Também limpei do frontend uns arquivos que sobraram do scaffold inicial do Vite (`App.css`, `src/assets`, `public/icons.svg`) que não eram mais usados por nada.
