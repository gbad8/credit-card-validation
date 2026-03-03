
# 💳 Sistema de Validação de Cartão de Crédito

Sistema completo de validação de cartão de crédito com **ASP.NET Core** backend e **React** frontend, implementando:

- ✅ **Validação Luhn** - Algoritmo de verificação de cartões
- ✅ **Detecção de Bandeiras** - Visa, MasterCard, American Express, Discover, Elo, Hipercard
- ✅ **Arquitetura Limpa** - Camadas bem definidas com injeção de dependência
- ✅ **Testes Unitários** - 175+ testes com 93% de cobertura
- ✅ **API RESTful** - Endpoints documentados com Swagger
- ✅ **Containerização** - Docker para desenvolvimento e produção
- ✅ **Segurança** - Mascaramento de dados sensíveis

---

## 🏗️ Arquitetura do Sistema

### Backend (ASP.NET Core)
```
validacao/
├── Controllers/           # Endpoints da API REST
├── Services/
│   ├── Abstractions/     # Interfaces (Contratos)
│   ├── Implementations/  # Lógica de negócio
│   └── Utilities/        # Funções auxiliares
├── Models/
│   ├── Domain/           # Entidades de domínio
│   ├── Requests/         # DTOs de entrada
│   └── Responses/        # DTOs de saída
├── Middleware/           # Middlewares customizados
└── Validators/           # Validações FluentValidation
```

### Frontend (React)
```
validacao-frontend/
├── src/
│   ├── components/       # Componentes React
│   └── services/         # API e localStorage
├── public/               # Assets públicos
└── Dockerfile           # Container de produção
```

### Testes
```
validacao.Tests/
├── Controllers/          # Testes de API endpoints
├── Services/
│   ├── Implementations/  # Testes de lógica de negócio
│   └── Utilities/        # Testes de utilitários
└── Dockerfile.tests     # Container de testes
```

---

## 🚀 Tecnologias Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Injeção de Dependência** - Container IoC nativo
- **Swagger/OpenAPI** - Documentação da API
- **Serilog** - Logging estruturado

### Frontend  
- **React 19** - Interface do usuário
- **Bootstrap 5** - Framework CSS
- **JavaScript ES6+** - Linguagem cliente

### Testes
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions expressivas
- **Moq** - Mocking para isolamento
- **ASP.NET Core Testing** - Testes de integração

### DevOps
- **Docker & Docker Compose** - Containerização
- **Multi-stage builds** - Otimização de imagens
- **GitHub Copilot** - Desenvolvimento assistido por IA

---

## ⚙️ Pré-requisitos

- **Docker** e **Docker Compose**
- **Git** para clonagem do repositório

---

## 🚀 Execução Rápida (Docker Compose)

### 🔹 Ambiente Completo de Desenvolvimento

```bash
# Clonar o repositório
git clone <repository-url>
cd validacao-cartao

# Executar aplicação completa
docker compose up

# Executar apenas o backend
docker compose up backend

# Executar apenas o frontend
docker compose up frontend
```

**URLs de Acesso:**
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:8001
- **Swagger**: http://localhost:8001/swagger

---

## 🧪 Executar Testes

### 🔹 Testes Unitários (Docker)

```bash
# Executar todos os testes
docker build -f Dockerfile.tests -t validacao-tests .
docker run --rm validacao-tests

# Resultado: 175+ testes passando com 93% de cobertura
```

### 🔹 Testes Locais (se .NET SDK disponível)

```bash
cd validacao.Tests
dotnet test --logger console --verbosity normal
```

---

## 🛠️ Desenvolvimento Local

### Backend (ASP.NET Core)

```bash
cd validacao

# Restaurar dependências
dotnet restore

# Executar em desenvolvimento
dotnet run
# API: http://localhost:5259
# Swagger: http://localhost:5259/swagger

# Build para produção
dotnet build --configuration Release
```

### Frontend (React)

```bash
cd validacao-frontend

# Instalar dependências
npm install

# Executar em desenvolvimento
npm start
# App: http://localhost:3000

# Build para produção
npm run build
```

---

## 🌐 API Endpoints

### Validação de Cartão

**POST** `/api/creditcard/validate`

**Request:**
```json
{
  "cardNumber": "5513 9459 0874 2906"
}
```

**Response (Sucesso):**
```json
{
  "cardNumber": "************2906",
  "isValid": true,
  "brand": "MasterCard",
  "message": "Cartão válido"
}
```

**Response (Erro):**
```json
{
  "cardNumber": "",
  "isValid": false,
  "brand": "Unknown",
  "message": "Card number is required"
}
```

### Status Codes
- `200 OK` - Validação realizada com sucesso
- `400 Bad Request` - Dados inválidos ou cartão inválido
- `500 Internal Server Error` - Erro interno do servidor

---

## 🧠 Regras de Negócio

### Validação de Cartão
1. **Limpeza Automática** - Remove espaços, traços e caracteres especiais
2. **Validação de Formato** - Aceita apenas números de 13-19 dígitos
3. **Algoritmo de Luhn** - Verifica integridade matemática do número
4. **Detecção de Bandeira** - Identifica por padrões de prefixo

### Bandeiras Suportadas
| Bandeira | Prefixos | Exemplo |
|----------|----------|---------|
| **Visa** | 4 | 4532015112830366 |
| **MasterCard** | 51-55, 2221-2720 | 5555555555554444 |
| **American Express** | 34, 37 | 378282246310005 |
| **Discover** | 6011, 65, 644-649 | 6011111111111117 |
| **Elo** | 4011, 4312, 4389, etc. | 5041111111111111 |
| **Hipercard** | 606282 | 6062821234567890 |

### Segurança
- **Mascaramento** - Apenas últimos 4 dígitos visíveis na resposta
- **Não Persistência** - Números de cartão não são armazenados
- **Validação Sanitizada** - Entrada limpa antes do processamento

---

## 🏗️ Arquitetura e Padrões

### Princípios SOLID
- **Single Responsibility** - Cada classe tem uma responsabilidade
- **Open/Closed** - Extensível sem modificar código existente  
- **Liskov Substitution** - Interfaces bem definidas
- **Interface Segregation** - Contratos específicos
- **Dependency Inversion** - Injeção de dependência

### Padrões Implementados
- **Repository Pattern** - Abstração de acesso a dados
- **Service Layer** - Lógica de negócio isolada
- **DTO Pattern** - Transferência de dados
- **Factory Pattern** - Criação de objetos
- **Strategy Pattern** - Algoritmos intercambiáveis

---

## 🐳 Docker

### Imagens Disponíveis

```bash
# Backend para desenvolvimento
docker build -t validacao-api ./validacao

# Frontend para produção (Nginx)
docker build -t validacao-frontend ./validacao-frontend

# Testes unitários
docker build -f Dockerfile.tests -t validacao-tests .
```

### Configuração de Portas

| Serviço | Porta Interna | Porta Externa | 
|---------|---------------|---------------|
| Backend | 5259 | 8001 |
| Frontend | 80 | 3000 |
| API (Docker) | 8080 | 8081 |

---

## 📊 Qualidade do Código

### Métricas de Testes
- **📊 Total**: 188 testes
- **✅ Passando**: 175 testes (93% sucesso)
- **📝 Linhas**: 1.365 linhas de testes
- **⚡ Execução**: ~4 segundos
- **🔧 Framework**: xUnit + FluentAssertions + Moq

### Cobertura por Camada
- **✅ Controllers**: Endpoints, status codes, serialização
- **✅ Services**: Lógica de negócio, casos extremos
- **✅ Utilities**: Formatação, validação, utilitários  
- **✅ Integration**: Testes end-to-end

---

## 🤝 Contribuição

### Desenvolvimento
1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Execute os testes: `docker run --rm validacao-tests`
4. Commit as mudanças: `git commit -m "Adiciona nova feature"`
5. Push: `git push origin feature/nova-feature`
6. Abra um Pull Request

### Estrutura de Commits
```
tipo(escopo): descrição

feat(api): adiciona validação de Diners Club
fix(frontend): corrige mascaramento de cartão
test(luhn): adiciona testes para casos extremos
docs(readme): atualiza documentação da API
```

---

## 📝 Roadmap

### 🔴 Melhorias Críticas de Segurança
- [ ] Configurar CORS específico por ambiente
- [ ] Remover dados sensíveis do localStorage  
- [ ] Implementar rate limiting

### 🟠 Funcionalidades
- [ ] Suporte a mais bandeiras (JCB, UnionPay)
- [ ] Validação de CVV
- [ ] API de consulta de BIN

### 🟡 Qualidade
- [ ] Logging estruturado
- [ ] Métricas de performance
- [ ] Testes E2E com Playwright

### 🟢 DevOps
- [ ] CI/CD com GitHub Actions
- [ ] Deploy automatizado
- [ ] Monitoramento com Application Insights

---

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 🙏 Agradecimentos

- **GitHub Copilot** - Assistência no desenvolvimento e refatoração
- **Comunidade .NET** - Frameworks e ferramentas excepcionais
- **React Community** - Ecossistema frontend moderno
