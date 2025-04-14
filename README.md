# MVCConsultaMedica

### Passos para corrigir o erro `The specified LocalDB instance does not exist`:

#### 1. **Verifique se o LocalDB está instalado**

Abra o terminal (cmd) e execute:

```bash
sqllocaldb i
```

Se aparecer algo como `MSSQLLocalDB` ou `ProjectsV13`, o LocalDB está instalado.  
Se aparecer erro ou lista vazia, você precisa **instalar o LocalDB**:

#### 2. **Verifique se a instância está criada**

Se seu erro fala da instância:

```text
(localdb)\ProjectModels
```

Verifique se ela existe:

```bash
sqllocaldb i ProjectModels
```

Se der erro, é porque **essa instância não existe**.

---

#### 3. **Crie a instância correta ou use outra existente**

Você pode:

- **Criar a instância `ProjectModels`** (se ela for necessária pro seu projeto):

```bash
sqllocaldb create ProjectModels
sqllocaldb start ProjectModels
```

ou

- **Mudar a string de conexão** do seu projeto para apontar para `MSSQLLocalDB`, que é a instância padrão do LocalDB:

No `appsettings.json`, altere isso:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=sistemaConsultasDB;Trusted_Connection=True;"
}
```
