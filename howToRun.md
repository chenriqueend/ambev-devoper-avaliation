# Como Executar o Projeto

Este guia fornece instruções passo a passo para configurar, compilar, executar e testar o projeto.

## Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html)
- Um editor de código (recomendado: [Visual Studio Code](https://code.visualstudio.com/) ou [Visual Studio](https://visualstudio.microsoft.com/))

## Estrutura do Projeto

O projeto está organizado em várias camadas:
- `Ambev.DeveloperEvaluation.WebApi`: API REST
- `Ambev.DeveloperEvaluation.Application`: Lógica de aplicação
- `Ambev.DeveloperEvaluation.Domain`: Entidades e regras de negócio
- `Ambev.DeveloperEvaluation.ORM`: Mapeamento e acesso a dados
- `Ambev.DeveloperEvaluation.Common`: Utilitários e classes compartilhadas
- `Ambev.DeveloperEvaluation.IoC`: Injeção de dependências

## Passos para Execução

### 1. Clone o Repositório

```bash
git clone https://github.com/chenriqueend/Ambev-DeveloperEvaluation-WebApi.git
```

### 2. Navegue até o Diretório do Projeto

```bash
cd template/backend
```

### 3. Restaure as Dependências

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
```

### 4. Compile o Projeto

```bash
dotnet build Ambev.DeveloperEvaluation.sln
```

### 5. Execute os Testes

```bash
dotnet test Ambev.DeveloperEvaluation.sln
```

### 6. Execute a API

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

A API estará disponível em `https://localhost:5119` ou `http://localhost:5000`.

## Comandos Úteis

### Compilar um Projeto Específico

```bash
dotnet build src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj
```

### Executar Testes de um Projeto Específico

```bash
dotnet test tests/Ambev.DeveloperEvaluation.Domain.Tests/Ambev.DeveloperEvaluation.Domain.Tests.csproj
```

### Limpar a Solução

```bash
dotnet clean Ambev.DeveloperEvaluation.sln
```

## Estrutura de Testes

O projeto possui diferentes tipos de testes:
- Testes Unitários (`Ambev.DeveloperEvaluation.Unit`)
- Testes de Domínio (`Ambev.DeveloperEvaluation.Domain.Tests`)
- Testes de Integração (`Ambev.DeveloperEvaluation.Integration`)
- Testes Funcionais (`Ambev.DeveloperEvaluation.Functional`)

## Solução de Problemas

### Erro de Compilação

Se encontrar erros de compilação:
1. Verifique se todas as dependências estão instaladas
2. Execute `dotnet restore` novamente
3. Limpe a solução com `dotnet clean` e tente compilar novamente

### Erro de Banco de Dados

Se encontrar erros relacionados ao banco de dados:
1. Verifique se o SQLite está instalado corretamente
2. Verifique se o arquivo de banco de dados tem as permissões corretas
3. Verifique se as migrações foram aplicadas

## Contribuindo

1. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
2. Faça commit das suas alterações (`git commit -m 'Adiciona nova feature'`)
3. Faça push para a branch (`git push origin feature/nova-feature`)
4. Abra um Pull Request 

## Owner

Desenvolvido por https://github.com/chenriqueend

## Executando o Projeto

1. Navegue até o diretório do projeto:
   ```bash
   cd template/backend
   ```

2. Execute o projeto:
   ```bash
   dotnet run
   ```

3. Acesse a API através do Swagger em: `http://localhost:5119/swagger`

## Inicializando o Banco de Dados

Antes de usar a API, é necessário inicializar o banco de dados SQLite. Siga os passos abaixo:

1. Navegue até o diretório do projeto WebApi:
   ```bash
   cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
   ```

2. Execute o comando de migração do banco de dados:
   ```bash
   dotnet ef database update
   ```

Este comando irá:
- Criar o banco de dados SQLite se ele não existir
- Aplicar todas as migrações pendentes
- Criar todas as tabelas necessárias com suas respectivas estruturas

Após a execução bem-sucedida, o banco de dados estará pronto para uso e a API poderá ser utilizada normalmente.

## Testando a API