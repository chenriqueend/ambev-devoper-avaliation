# Deploy na Render - Ambev Developer Evaluation

Este guia mostra como fazer o deploy deste projeto .NET 8 na plataforma [Render](https://render.com/), utilizando o Dockerfile já presente no repositório.

---

## Pré-requisitos
- Conta no [Render](https://render.com/)
- Projeto hospedado em um repositório GitHub, GitLab ou Bitbucket

---

## Passos para Deploy

### 1. Suba o projeto para um repositório Git
Se ainda não estiver, faça o push do projeto para um repositório remoto (GitHub, GitLab ou Bitbucket).

### 2. Crie um novo serviço Web na Render
1. Acesse o [dashboard da Render](https://dashboard.render.com/)
2. Clique em **New +** > **Web Service**
3. Escolha **Deploy an existing Dockerfile**
4. Conecte seu repositório e selecione a branch desejada
5. Em **Root Directory**, coloque:
   ```
   template/backend
   ```
6. Clique em **Create Web Service**

### 3. Configuração de variáveis de ambiente (opcional, mas recomendado)
- Para produção, recomenda-se definir a chave JWT como variável de ambiente.
- Exemplo:
  - **Key:** `Jwt__SecretKey`
  - **Value:** `uma-chave-secreta-bem-grande`
- Para adicionar variáveis:
  - Vá em **Environment** > **Add Environment Variable**

### 4. Banco de Dados
- Por padrão, o projeto usa SQLite, que será criado dentro do container.
- Para persistência entre deploys, utilize um volume externo ou migre para um banco gerenciado (ex: PostgreSQL).

### 5. Acesso à API
- Após o deploy, a Render fornecerá uma URL pública para sua API.
- Os endpoints estarão disponíveis conforme documentado em `documentation.md`.

---

## Dicas
- Para logs, utilize o painel de logs da Render.
- Para rodar migrações manualmente, utilize o shell da Render ou configure um script de inicialização.
- O Dockerfile já expõe a porta 80, compatível com a Render.

---

## Referências
- [Documentação Render - Deploy com Docker](https://render.com/docs/deploy-docker)
- [Documentação oficial .NET no Render](https://render.com/docs/deploy-dotnet)

---

Dúvidas? Consulte o arquivo `howToRun.md` para instruções locais ou abra uma issue no repositório. 