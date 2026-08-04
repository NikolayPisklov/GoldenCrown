# GoldenCrown

## Описание проекта

GoldenCrown - это ASP.NET Core Web API для простой финансовой системы с пользователями, счетами, сессиями и переводами.

Приложение позволяет:

- регистрировать пользователей;
- входить в систему и получать токен сессии;
- смотреть баланс авторизованного пользователя;
- пополнять свой счет;
- переводить деньги другому пользователю по логину;
- получать историю входящих и исходящих транзакций.

Для хранения данных используется Microsoft SQL Server и Entity Framework Core. В проекте есть миграции для создания структуры базы данных и начального наполнения таблицы пользователей.

## Инструкция по запуску

### Требования

- .NET SDK 10.0
- Microsoft SQL Server
- Entity Framework Core CLI

Если `dotnet ef` не установлен, его можно установить командой:

```bash
dotnet tool install --global dotnet-ef
```

### Настройка подключения к базе данных

Приложение ожидает строку подключения с именем `GoldenCrownDbConnection`.

Пример для PowerShell:

```powershell
$env:ConnectionStrings__GoldenCrownDbConnection="Server=localhost;Database=GoldenCrown;Trusted_Connection=True;TrustServerCertificate=True"
```

Также строку подключения можно добавить через user-secrets:

```bash
cd GoldenCrown
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:GoldenCrownDbConnection" "Server=localhost;Database=GoldenCrown;Trusted_Connection=True;TrustServerCertificate=True"
```

### Применение миграций

```bash
cd GoldenCrown
dotnet ef database update
```

### Запуск приложения

```bash
cd GoldenCrown
dotnet run
```

По умолчанию приложение запускается на:

- `http://localhost:5036`
- `https://localhost:7196`

В режиме Development доступен Swagger UI:

```text
https://localhost:7196/swagger
```

## Примеры API запросов

В примерах используется базовый адрес:

```text
http://localhost:5036
```

### Регистрация пользователя

```bash
curl -X POST "http://localhost:5036/register" \
  -H "Content-Type: application/json" \
  -d '{
    "login": "user1",
    "name": "User One",
    "password": "Password1"
  }'
```

Пароль должен содержать минимум 6 символов, хотя бы одну строчную букву, одну заглавную букву и одну цифру.

### Авторизация

```bash
curl -X POST "http://localhost:5036/login" \
  -H "Content-Type: application/json" \
  -d '{
    "login": "user1",
    "password": "Password1"
  }'
```

Пример ответа:

```json
{
  "token": "c0d2d83d-7cc3-48d5-9c85-5c4b8f2d27e2"
}
```

Токен действует 1 час. Для защищенных финансовых методов его нужно передавать в заголовке `Authorization`.

### Получение баланса

```bash
curl -X GET "http://localhost:5036/api/Finance/balance" \
  -H "Authorization: c0d2d83d-7cc3-48d5-9c85-5c4b8f2d27e2"
```

Пример ответа:

```json
{
  "balance": 100.00
}
```

### Пополнение счета

```bash
curl -X POST "http://localhost:5036/api/Finance/deposit" \
  -H "Authorization: c0d2d83d-7cc3-48d5-9c85-5c4b8f2d27e2" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 250.50
  }'
```

Пример ответа:

```json
{
  "balance": 250.50
}
```

### Перевод другому пользователю

Перед переводом у получателя должен существовать аккаунт. Аккаунт создается автоматически при регистрации пользователя.

```bash
curl -X POST "http://localhost:5036/api/Finance/transfer" \
  -H "Authorization: c0d2d83d-7cc3-48d5-9c85-5c4b8f2d27e2" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverLogin": "user2",
    "amount": 100.00
  }'
```

Пример ответа:

```json
{
  "balance": 150.50
}
```

### История транзакций

```bash
curl -X GET "http://localhost:5036/api/Finance/get-history?from=2026-08-01T00:00:00Z&to=2026-08-31T23:59:59Z&limit=10&offset=0" \
  -H "Authorization: c0d2d83d-7cc3-48d5-9c85-5c4b8f2d27e2"
```

Пример ответа:

```json
{
  "transactions": [
    {
      "isSender": false,
      "senderName": "User One",
      "receiverName": "User One",
      "amount": 250.50,
      "date": "2026-08-04T12:00:00Z"
    }
  ]
}
```

Параметры истории:

- `from` - дата начала периода, необязательный параметр;
- `to` - дата конца периода, необязательный параметр;
- `limit` - количество записей, должно быть больше 0;
- `offset` - смещение, не может быть отрицательным.

## Структура базы данных

### Users

Таблица пользователей.

| Поле | Тип | Описание |
| --- | --- | --- |
| `Id` | `int` | Первичный ключ, identity |
| `Login` | `nvarchar(450)` | Логин пользователя, уникальный индекс |
| `Name` | `nvarchar(max)` | Имя пользователя |
| `Password` | `nvarchar(max)` | Пароль пользователя |

Начальные пользователи из миграций:

| Login | Password | Name |
| --- | --- | --- |
| `admin` | `admin123` | `Administrator` |
| `ivan` | `ivan123` | `Ivan Petrov` |
| `maria` | `maria123` | `Maria Smirnova` |
| `alex` | `alex123` | `Alex Kuznetsov` |
| `elena` | `elena123` | `Elena Sokolova` |

### Accounts

Таблица счетов пользователей.

| Поле | Тип | Описание |
| --- | --- | --- |
| `Id` | `int` | Первичный ключ, identity |
| `UserId` | `int` | Внешний ключ на `Users.Id`, уникальный индекс |
| `Balance` | `decimal(18,2)` | Баланс счета |

Ограничения:

- `CK_Account_Balance_NonNegative` - баланс не может быть меньше 0.
- Один пользователь связан с одним счетом.

### Sessions

Таблица активных сессий пользователей.

| Поле | Тип | Описание |
| --- | --- | --- |
| `UserId` | `int` | Первичный ключ и внешний ключ на `Users.Id` |
| `Token` | `nvarchar(max)` | Токен авторизации |
| `ExpiresAt` | `datetime2` | Дата и время истечения сессии |

Ограничения:

- Один пользователь может иметь одну активную сессию.
- При повторном входе старая сессия пользователя удаляется и создается новая.

### Transactions

Таблица финансовых операций.

| Поле | Тип | Описание |
| --- | --- | --- |
| `Id` | `int` | Первичный ключ, identity |
| `SenderAccountId` | `int` | Внешний ключ на счет отправителя |
| `ReceiverAccountId` | `int` | Внешний ключ на счет получателя |
| `Date` | `datetime2` | Дата и время операции |
| `Amount` | `decimal(18,2)` | Сумма операции |

Ограничения:

- `CK_Transaction_Amount_GreaterThanZero` - сумма операции должна быть больше 0.
- При пополнении счета отправителем и получателем является один и тот же счет.
