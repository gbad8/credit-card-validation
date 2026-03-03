# 💳 Validação de Cartão – Frontend React

Interface moderna desenvolvida em **React**, responsável por:

- 🎯 **Interface Intuitiva** - Formulário limpo para validação de cartões
- 🔍 **Validação Instantânea** - Feedback imediato sobre cartões válidos/inválidos  
- 🏷️ **Detecção Visual** - Exibe ícones das bandeiras identificadas
- 📱 **Design Responsivo** - Funciona em desktop e mobile
- 💾 **Histórico Local** - Mantém registros das últimas validações (dados mascarados)
- 🔒 **Segurança** - Não armazena números completos de cartão

A aplicação é otimizada para produção com **Nginx** e totalmente containerizada.

---

## 🚀 Tecnologias Utilizadas

- **React 19** - Biblioteca de interface
- **JavaScript ES6+** - Linguagem moderna  
- **Bootstrap 5** - Framework CSS responsivo
- **React Icons** - Ícones das bandeiras
- **Nginx** - Servidor web de produção
- **Docker** - Containerização

---

## 📂 Estrutura do Projeto

```
validacao-frontend/
├── src/
│   ├── components/        # Componentes React reutilizáveis
│   │   ├── CardValidator/ # Componente principal de validação
│   │   ├── CardHistory/   # Histórico de validações
│   │   └── BrandIcons/    # Ícones das bandeiras
│   ├── services/          # Comunicação com API
│   │   ├── creditCardService.js  # Chamadas para backend
│   │   └── storageService.js     # localStorage utilitário
│   ├── App.js            # Componente raiz
│   ├── index.js          # Entry point
│   └── index.css         # Estilos globais
├── public/               # Assets estáticos
│   ├── index.html        # Template HTML
│   └── favicon.ico       # Ícone da aplicação
├── package.json          # Dependências e scripts
├── Dockerfile           # Build de produção
└── README.md
```

---

## ⚙️ Pré-requisitos

- **Node.js 18+** (para desenvolvimento local)
- **npm** ou **yarn** (para gerenciamento de pacotes)
- **Docker** (para execução containerizada)

---

## 🚀 Execução

### 🔹 Desenvolvimento Local

```bash
# Instalar dependências
npm install

# Executar servidor de desenvolvimento
npm start
# Aplicação: http://localhost:3000

# Executar testes
npm test

# Build para produção
npm run build
```

### 🔹 Docker (Produção)

```bash
# Build da imagem
docker build -t validacao-frontend .

# Executar container
docker run -p 3000:80 validacao-frontend
# Aplicação: http://localhost:3000
```

### 🔹 Docker Compose (Ambiente Completo)

```bash
# Na raiz do projeto (com backend)
docker compose up
# Frontend: http://localhost:3000
# Backend: http://localhost:8001
```

---

## 🌐 Comunicação com Backend

### Configuração da API

A URL da API é configurável via variável de ambiente:

```bash
# Desenvolvimento
REACT_APP_API_URL=http://localhost:8001

# Produção
REACT_APP_API_URL=https://api.validacao-cartao.com
```

**Configuração padrão**: `http://localhost:8001`

### Endpoints Utilizados

- **POST** `/api/creditcard/validate` - Validação de cartão

```javascript
// Exemplo de chamada
const response = await validateCreditCard("5513 9459 0874 2906");
// Retorna: { cardNumber: "************2906", isValid: true, brand: "MasterCard" }
```

---

## 🎨 Funcionalidades da Interface

### 🔹 Validação de Cartão
- Campo de entrada com máscara automática
- Limpeza de caracteres especiais
- Validação em tempo real
- Feedback visual (cores e ícones)

### 🔹 Detecção de Bandeiras
- Ícones das principais bandeiras
- Identificação automática durante digitação
- Suporte a: Visa, MasterCard, Amex, Discover, Elo, Hipercard

### 🔹 Histórico Local
- Últimas 10 validações armazenadas
- Apenas últimos 4 dígitos visíveis  
- Data/hora de cada validação
- Botão para limpar histórico

### 🔹 Tratamento de Erros
- Mensagens amigáveis para usuário
- Fallbacks para problemas de rede
- Estados de loading durante requisições

---

## 🧪 Testes

### Scripts Disponíveis

```bash
# Testes unitários
npm test

# Testes em modo watch
npm test -- --watch

# Coverage de testes
npm test -- --coverage

# Lint do código
npm run lint
```

### Estrutura de Testes

```
src/
├── __tests__/
│   ├── App.test.js              # Testes do componente principal
│   ├── services/
│   │   ├── creditCardService.test.js  # Testes de API
│   │   └── storageService.test.js     # Testes de localStorage
│   └── components/
│       └── CardValidator.test.js      # Testes de componentes
```

---

## 🔒 Segurança e Privacidade

### Proteção de Dados
- **Nunca armazena números completos** de cartão
- **Mascaramento automático** (apenas últimos 4 dígitos)
- **Limpeza automática** de formulários após envio
- **Histórico limitado** (máximo 10 itens)

### Validação Client-Side
- Pré-validação de formato antes de enviar
- Sanitização de entrada
- Prevenção de XSS básica

### Comunicação Segura
- Requisições HTTPS em produção
- Headers de segurança configurados
- CORS adequadamente configurado

---

## 🎨 Personalização

### Temas e Estilos
```css
/* Variáveis CSS customizáveis */
:root {
  --primary-color: #007bff;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --border-radius: 0.5rem;
}
```

### Componentes Reutilizáveis
- `CardValidator` - Formulário principal
- `BrandIcon` - Ícones de bandeiras
- `ValidationResult` - Exibição de resultados
- `HistoryItem` - Item do histórico

---

## 📱 Responsividade

### Breakpoints Suportados
- **Mobile**: < 768px
- **Tablet**: 768px - 1024px  
- **Desktop**: > 1024px

### Funcionalidades Mobile
- Touch-friendly inputs
- Teclado numérico para cartões
- Gestos de swipe no histórico
- Layout otimizado para telas pequenas

---

## 🚀 Deploy e Produção

### Build Otimizado
```bash
npm run build
# Gera pasta build/ com assets otimizados
```

### Docker Multi-Stage
- **Stage 1**: Build da aplicação React
- **Stage 2**: Serve com Nginx otimizado
- **Tamanho final**: ~15MB

### Nginx Configuration
```nginx
# Configuração automática para SPA
location / {
  try_files $uri $uri/ /index.html;
}

# Cache de assets estáticos
location /static/ {
  expires 1y;
  add_header Cache-Control "public, immutable";
}
```

---

## 🧩 Integração com Backend

### Variáveis de Ambiente
```bash
# .env.development
REACT_APP_API_URL=http://localhost:8001
REACT_APP_ENVIRONMENT=development

# .env.production  
REACT_APP_API_URL=https://api.example.com
REACT_APP_ENVIRONMENT=production
```

### Service Workers (Futuro)
- Cache offline básico
- Notificações push
- Sincronização em background

---

## 📊 Performance

### Métricas Atuais
- **First Paint**: < 1.5s
- **Time to Interactive**: < 2s
- **Bundle Size**: < 500KB
- **Lighthouse Score**: 90+

### Otimizações Implementadas
- Code splitting por rotas
- Lazy loading de componentes
- Compressão de assets
- Service worker para cache

---

## 🤝 Contribuição

### Setup de Desenvolvimento
```bash
git clone <repository-url>
cd validacao-cartao/validacao-frontend
npm install
npm start
```

### Padrões de Código
- **ESLint** - Linting automático
- **Prettier** - Formatação consistente  
- **Conventional Commits** - Mensagens padronizadas
- **Component-First** - Componentes reutilizáveis

---

## 🐛 Troubleshooting

### Problemas Comuns

**Erro de CORS**
```bash
# Verificar se backend está rodando
curl http://localhost:8001/api/creditcard/validate

# Verificar variável de ambiente
echo $REACT_APP_API_URL
```

**Build Failing**
```bash
# Limpar cache do npm
npm clean-install

# Verificar versão do Node
node --version  # Deve ser 18+
```

**Testes Falhando**
```bash
# Executar com verbose
npm test -- --verbose

# Atualizar snapshots
npm test -- --updateSnapshot
```

---

## 📝 Changelog

### v2.0.0 (Atual)
- ✅ Refatoração completa da arquitetura
- ✅ Implementação de testes unitários
- ✅ Mascaramento seguro de dados
- ✅ Melhoria na experiência do usuário
- ✅ Containerização otimizada

### v1.0.0
- ✅ Implementação inicial
- ✅ Validação básica de cartões
- ✅ Interface responsiva
- ✅ Integração com backend

---

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](../LICENSE) para detalhes.
