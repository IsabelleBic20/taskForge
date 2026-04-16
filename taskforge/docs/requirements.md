# 📄 Documento de Requisitos de Software (DRS)

## Sistema: TaskForge

---

## 1. 📌 Visão Geral

### 1.1 Objetivo

O **TaskForge** é um sistema de gerenciamento de tarefas focado em produtividade, permitindo que usuários organizem, acompanhem e priorizem atividades e projetos.

### 1.2 Escopo

O sistema permitirá:

* Gerenciamento de tarefas
* Organização por projetos
* Definição de prioridades e prazos
* Acompanhamento de progresso

---

## 2. 👥 Stakeholders

* Usuário final
* Equipes de desenvolvimento
* Gerentes de projeto
* Administradores

---

## 3. 🧩 Definições

| Termo      | Descrição            |
| ---------- | -------------------- |
| Tarefa     | Unidade de trabalho  |
| Projeto    | Grupo de tarefas     |
| Prioridade | Nível de importância |
| Status     | Estado da tarefa     |

---

## 4. ⚙️ Requisitos Funcionais

### 4.1 Usuários

* RF01: Cadastro de usuários
* RF02: Login e logout
* RF03: Edição de perfil

### 4.2 Tarefas

* RF04: Criar tarefa
* RF05: Editar tarefa
* RF06: Excluir tarefa
* RF07: Listar tarefas
* RF08: Alterar status
* RF09: Definir prioridade
* RF10: Definir prazo

### 4.3 Projetos

* RF11: Criar projetos
* RF12: Associar tarefas a projetos
* RF13: Listar tarefas por projeto

### 4.4 Filtros

* RF14: Filtrar por status
* RF15: Filtrar por prioridade
* RF16: Ordenar por prazo

---

## 5. 🚫 Requisitos Não Funcionais

* RNF01: Tempo de resposta até 2s
* RNF02: Senhas criptografadas
* RNF03: Autenticação segura
* RNF04: Interface responsiva
* RNF05: Sistema escalável
* RNF06: Arquitetura em camadas

---

## 6. 🗄️ Regras de Negócio

* RN01: Tarefa deve ter título
* RN02: Tarefa pertence a um usuário
* RN03: Status inicial = pendente
* RN04: Prazo não pode ser no passado

---

## 7. 🧱 Modelo de Dados

### User

* Id
* Nome
* Email
* Senha

### Task

* Id
* Título
* Descrição
* Status
* Prioridade
* DataCriacao
* DataVencimento
* UserId

### Project

* Id
* Nome
* Descrição
* UserId

---

## 8. 📊 Casos de Uso

### Criar Tarefa

1. Usuário acessa sistema
2. Clica em nova tarefa
3. Preenche dados
4. Salva

### Concluir Tarefa

1. Usuário seleciona tarefa
2. Marca como concluída

---

## 9. 🚀 Backlog Futuro

* Notificações
* Integração com calendário
* Kanban board
* Dashboard
* Tags

---

## 10. 🏗️ Arquitetura

* Backend: .NET
* Frontend: React (ou similar)
* API REST
* Banco: SQL Server

---

## 11. 📌 Observações

Este documento pode evoluir conforme o sistema cresce.
