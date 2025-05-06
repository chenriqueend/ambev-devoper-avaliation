# Documentação do Projeto

## Visão Geral

Este projeto é uma API REST desenvolvida em .NET 8 que implementa um sistema de vendas com funcionalidades de autenticação, gerenciamento de usuários, filiais, clientes e vendas.

## Arquitetura

O projeto segue os princípios da Arquitetura Limpa (Clean Architecture) e é organizado nas seguintes camadas:

### 1. WebApi
- Responsável pela exposição dos endpoints REST
- Implementa controllers e middlewares
- Gerencia autenticação e autorização
- Validação de requisições

### 2. Application
- Implementa a lógica de aplicação
- Gerencia casos de uso através de Commands e Queries
- Implementa validações de negócio
- Coordena operações entre diferentes entidades

### 3. Domain
- Contém as entidades de negócio
- Implementa regras de negócio
- Define interfaces para repositórios
- Gerencia eventos de domínio

### 4. ORM
- Implementa o acesso a dados
- Configura mapeamentos de entidades
- Gerencia migrações do banco de dados
- Implementa repositórios

## Endpoints da API

### Autenticação

#### POST /api/auth/login
- **Descrição**: Autentica um usuário e retorna um token JWT
- **Request Body**:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- **Response**: Token JWT e informações do usuário
- **Status Codes**:
  - 200: Sucesso
  - 400: Dados inválidos
  - 401: Credenciais inválidas

#### POST /api/auth/register
- **Descrição**: Registra um novo usuário
- **Request Body**:
  ```json
  {
    "name": "string",
    "email": "string",
    "password": "string"
  }
  ```
- **Response**: Dados do usuário criado
- **Status Codes**:
  - 201: Usuário criado
  - 400: Dados inválidos
  - 409: Email já existe

### Usuários

#### GET /api/users
- **Descrição**: Lista todos os usuários
- **Headers**: Authorization: Bearer {token}
- **Response**: Lista de usuários
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado

#### GET /api/users/{id}
- **Descrição**: Obtém detalhes de um usuário específico
- **Headers**: Authorization: Bearer {token}
- **Response**: Dados do usuário
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado
  - 404: Usuário não encontrado

### Filiais

#### GET /api/branches
- **Descrição**: Lista todas as filiais
- **Headers**: Authorization: Bearer {token}
- **Response**: Lista de filiais
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado

#### POST /api/branches
- **Descrição**: Cria uma nova filial
- **Headers**: Authorization: Bearer {token}
- **Request Body**:
  ```json
  {
    "name": "string",
    "address": "string"
  }
  ```
- **Response**: Dados da filial criada
- **Status Codes**:
  - 201: Filial criada
  - 400: Dados inválidos
  - 401: Não autorizado

### Clientes

#### GET /api/customers
- **Descrição**: Lista todos os clientes
- **Headers**: Authorization: Bearer {token}
- **Response**: Lista de clientes
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado

#### POST /api/customers
- **Descrição**: Cria um novo cliente
- **Headers**: Authorization: Bearer {token}
- **Request Body**:
  ```json
  {
    "name": "string",
    "email": "string",
    "phone": "string"
  }
  ```
- **Response**: Dados do cliente criado
- **Status Codes**:
  - 201: Cliente criado
  - 400: Dados inválidos
  - 401: Não autorizado

### Vendas

#### GET /api/sales
- **Descrição**: Lista todas as vendas
- **Headers**: Authorization: Bearer {token}
- **Response**: Lista de vendas
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado

#### POST /api/sales
- **Descrição**: Cria uma nova venda
- **Headers**: Authorization: Bearer {token}
- **Request Body**:
  ```json
  {
    "saleNumber": "string",
    "customerId": "guid",
    "branchId": "guid",
    "items": [
      {
        "productId": "guid",
        "quantity": "integer",
        "unitPrice": "decimal"
      }
    ]
  }
  ```
- **Response**: Dados da venda criada
- **Status Codes**:
  - 201: Venda criada
  - 400: Dados inválidos
  - 401: Não autorizado

#### GET /api/sales/{id}
- **Descrição**: Obtém detalhes de uma venda específica
- **Headers**: Authorization: Bearer {token}
- **Response**: Dados da venda
- **Status Codes**:
  - 200: Sucesso
  - 401: Não autorizado
  - 404: Venda não encontrada

#### POST /api/sales/{id}/cancel
- **Descrição**: Cancela uma venda
- **Headers**: Authorization: Bearer {token}
- **Response**: Dados da venda cancelada
- **Status Codes**:
  - 200: Venda cancelada
  - 401: Não autorizado
  - 404: Venda não encontrada

## Regras de Negócio

### Vendas
1. Uma venda deve ter pelo menos um item
2. O número da venda deve ser único
3. Uma venda cancelada não pode ser modificada
4. Descontos são aplicados com base na quantidade de itens:
   - 4+ itens: 10% de desconto
   - 10+ itens: 20% de desconto

### Usuários
1. Email deve ser único
2. Senha deve ter no mínimo 6 caracteres
3. Nome não pode estar vazio

### Filiais
1. Nome deve ser único
2. Nome não pode estar vazio

### Clientes
1. Email deve ser único
2. Nome não pode estar vazio

## Banco de Dados

O projeto utiliza SQLite como banco de dados. As principais tabelas são:

- Users: Armazena dados dos usuários
- Branches: Armazena dados das filiais
- Customers: Armazena dados dos clientes
- Sales: Armazena dados das vendas
- SaleItems: Armazena itens de cada venda

## Segurança

- Autenticação via JWT
- Senhas são armazenadas com hash
- Validação de tokens em todas as requisições
- Proteção contra CSRF
- Validação de entrada em todos os endpoints 