# Задача:
Реализовать систему сохранения и просмотра изображений. Пользователи загружают изображения, могут их просматривать, добавлять друзей, которые могут просматривать изображения пользователя.

# Функциональные требования:
- Наличие регистрации пользователя
- Пользователь должен иметь возможность загрузить изображение
- Пользователь должен иметь возможность просмотреть свои изображения
- Пользователь должен иметь возможность просмотреть изображения другого пользователя, если он является его другом
- Пользователь должен иметь возможность добавить другого пользователя в друзья (пользователь А может просматривать изображения пользователя B, если пользователь B добавил пользователя А в друзья, но пользователь B не может просматривать изображения пользователя А, если тот не ответил взаимностью)

# Не функциональные требования:
- Приложение должно быть написано на .NET 7, C# 11, ASP.NET Core
- Любой вид авторизации (ASP.NET Identity, можно хранить в базе логин+пароль использовать basic auth)
- Для реализации должен быть использован EntityFrameworkCore, богатая доменная модель
- В сущности пользователя, набор картинок должен быть приватным полем, наружу должно быть доступно только свойство с IReadOnlyCollection (необходимо настроить конфигурацию EF чтобы это работало)
- Сами изображения должны храниться на локальном хранилище, путь должен быть настраиваемым через appsettings.json, при переносе локации хранилища и самих изображений, приложение должно работать корректно (не хранить в базе абсолютные пути)
- Отношение пользователь-друзья (many-to-many на пользователях) должно быть реализовано через конфигурацию EF
- Приложение должно иметь restful API, возвращать корректные коды ошибок (404, 401, 403 итд)
- Приложение должно иметь swagger, с возможностью конфигурировать выбранный способ авторизации через UI
- Приложение должно иметь все эндпоинты для реализации функциональных требований
- Фронт не обязательно, swagger приветствуется.

# Endpoints

Примечание. Для удобства описания эндпоинтов далее будут использоваться пометки.
- Пометка [Auth] - означает что метод доступен только авторизованным пользователям. В случае, если пользователь не авторизован, метод вернет код 401 Unauthorized.
- Пометка [Admin] - означает что метод доступен только администраторам. В случае, если пользователь не имеет роли администратора, метод вернет код 403 Forbidden
- Пометка [Required] - означает что параметр является обязательным.В случае его отсутствия, метод вернет код 400 BadRequest

## UsersController

### GET /api/User/getall - [Admin]

Получение всех существующих пользователей.

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/getall' --header 'Authorization: Basic dXNlcjpwYXNz'
```
Пример ответа:
```
[
    {
        "id": "2738f277-9fe5-440a-a98d-3bb317f8508c",
        "username": "user",
        "role": "Admin"
    },
    ...
    {
        "id": "9b382b49-8a7f-4aec-8803-e9be99f5603d",
        "username": "user3",
        "role": "User"
    }
]
```

### GET /api/User/getuser - [Admin]
Получение конкретного пользователя. Доступ имеется только у администраторов.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ  400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Тело ответа 200OK
  - Информация о пользователе

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/getuser?username=user' \
--header 'Authorization: Basic dXNlcjpwYXNz' \
--data ''
```
Пример ответа:
```
{
    "id": "2738f277-9fe5-440a-a98d-3bb317f8508c",
    "username": "user",
    "role": "Admin"
}
```

### POST /api/User/createuser

Создание пользователя.

- Тело запроса
  - username - Username пользователя - [Required]
  - password - пароль пользователя - [Required]
- Ответ  400BadRequest
  - Один из параметров не указан или пользователь с таким Username уже существует
- Тело ответа 200OK
  - Информация о созданном пользователе

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/createuser' \
--header 'Content-Type: application/json' \
--data '{
  "username": "Username",
  "password": "password"
}'
```
Пример ответа:
```
{
    "id": "7404bfd7-74c0-4c0f-8a4a-f265ae953954",
    "username": "Username",
    "role": "User"
}
```

### PUT /api/User/updateuser - [Auth]

Обновление информации о пользоваетеле.

- Тело запроса
  - id - Id пользователя - [Required]
  - username - Username пользователя - [Required]
  - password - пароль пользователя - [Required]
  - role - роль пользователя - [Required]
- Ответ  403 Forbidden:
  - Пользователь не является администратором, а параметр role имеет значение "Admin"
- Ответ  400BadRequest
  - Пользователь с таким Username уже существует
- Тело ответа 200OK
  - Информация об обновленном пользователе

Пример запроса:
```
curl --location --request PUT 'http://localhost:5065/api/User/updateuser' \
--header 'Content-Type: application/json' \
--header 'Authorization: Basic dXNlcjpwYXNz' \
--data '{
  "id": "7404bfd7-74c0-4c0f-8a4a-f265ae953954",
  "username": "NewUsername",
  "password": "NewPassword",
  "role": "User"
}'
```
Пример ответа:
```
{
    "id": "7404bfd7-74c0-4c0f-8a4a-f265ae953954",
    "username": "NewUsername",
    "role": "User"
}
```

### DELETE /api/User/deleteuser - [Auth]

Удаление пользователя.

- Тело запроса
  - id - Id пользователя - [Required]
- Тело ответа 200OK
  - Информация об удаленном пользователе

Пример запроса:
```
curl --location --request DELETE 'http://localhost:5065/api/User/deleteuser?id=7404bfd7-74c0-4c0f-8a4a-f265ae953954' \
--header 'Authorization: Basic TmV3VXNlcm5hbWU6TmV3UGFzc3dvcmQ='
```
Пример ответа:
```
{
    "id": "7404bfd7-74c0-4c0f-8a4a-f265ae953954",
    "username": "NewUsername",
    "role": "User"
}
```

### POST /api/User/friend - [Auth]

Добавление пользователя в друзья.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ 400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Тело ответа 200OK
  - Пустое тело

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/friend' \
--header 'Content-Type: application/json' \
--header 'Authorization: Basic dXNlcjI6cGFzcw==' \
--data '{
    "username":"user"
}'
```

### PUT /api/User/unfriend - [Auth]

Удаление пользователя из друзей.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ 400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Тело ответа 200OK
  - Пустое тело

Пример запроса:
```
curl --location --request PUT 'http://localhost:5065/api/User/unfriend' \
--header 'Content-Type: application/json' \
--header 'Authorization: Basic dXNlcjI6cGFzcw==' \
--data '{
    "username":"user"
}'
```

### GET /api/User/getusersfriends - [Auth]

Получение списка пользователей, которых авторизованный пользователь добавил в друзья.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ 400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Ответ 403Forbidden
  - Пользователь, не имеющий роли администратора, пытается получить информацию не о себе
- Тело ответа 200OK
  - Список пользователей

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/getusersfriends?username=user' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
[
    {
        "id": "9b382b49-8a7f-4aec-8803-e9be99f5603d",
        "username": "user3",
        "role": "User"
    }
]
```

### GET /api/User/getusersfriendof - [Auth]

Получение списка пользователей, котрые добавили авторизованного пользователя в друзья.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ 400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Ответ 403Forbidden
  - Пользователь, не имеющий роли администратора, пытается получить информацию не о себе
- Тело ответа 200OK
  - Список пользователей

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/getusersfriendof?username=user3' \
--header 'Authorization: Basic dXNlcjM6cGFzcw=='
```

Пример ответа:
```
[
    {
        "id": "2738f277-9fe5-440a-a98d-3bb317f8508c",
        "username": "user",
        "role": "Admin"
    }
]
```

### GET /api/User/getimages - [Auth]

Получение изображений пользователя.

- Тело запроса
  - id - Id пользователя
  - username - Username пользователя
- Ответ 400BadRequest
  - Пользователь не найден или оба параметра id и username не указаны
- Ответ 403Forbidden
  - Пользователь, не имеющий роли администратора, пытается получить информацию не о себе
- Тело ответа 200OK
  - Список изображений

Пример запроса:
```
curl --location 'http://localhost:5065/api/User/getimages?username=user' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
[
    {
        "id": "30bc8716-bdba-4e8c-96a5-ce8a86b9d82d",
        "fileName": "9YkMmdQNl0g.jpg",
        "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
    }
]
```

## ImageController

### GET /api/Image/getall - [Admin]

Получение всех изображений.

- Тело ответа 200OK
  - Список информации об изображениях

Пример запроса:
```
curl --location 'http://localhost:5065/api/Image/getall' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
[
    {
        "id": "30bc8716-bdba-4e8c-96a5-ce8a86b9d82d",
        "fileName": "9YkMmdQNl0g.jpg",
        "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
    }
]
```

### GET /api/Image/info - [Auth]

Получениие информации о изображении.

- Тело запроса
  - id - Id изображения - [Required]
- Ответ 400BadRequest
  - Изображение не найдено
- Ответ 403Forbidden
  - Пользователь, не имеющий роли администратора, пытается получить информацию не о своем изображении
- Тело ответа 200OK
  - Информация об изображении

Пример запроса:
```
curl --location 'http://localhost:5065/api/Image/info?id=30bc8716-bdba-4e8c-96a5-ce8a86b9d82d' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
{
    "id": "30bc8716-bdba-4e8c-96a5-ce8a86b9d82d",
    "fileName": "9YkMmdQNl0g.jpg",
    "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
}
```

### GET /api/Image/download - [Auth]

Загрузка изображения из локального хранилища.

- Тело запроса
  - id - Id изображения - [Required]
- Ответ 400BadRequest
  - Изображение не найдено
- Ответ 403Forbidden
  - Пользователь, не имеющий роли администратора, пытается получить недоступное ему изображение
- Тело ответа 200OK
  - Файл изображения

Пример запроса:
```
curl --location 'http://localhost:5065/api/Image/info?id=30bc8716-bdba-4e8c-96a5-ce8a86b9d82d' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
{
    "id": "30bc8716-bdba-4e8c-96a5-ce8a86b9d82d",
    "fileName": "9YkMmdQNl0g.jpg",
    "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
}
```

### POST /api/Image/upload - [Auth]

Загрузка изображений в локальное хранилище.

- Тело запроса
  - images - Список файлов с изображениями - [Required]
- Ответ 500InternalServerError
  - Ошибка во время сохранения файлов в локальном хранилище
- Тело ответа 200OK
  - Список информации о загруженных изображениях

Пример запроса:
```
curl --location 'http://localhost:5065/api/Image/upload' \
--header 'Authorization: Basic dXNlcjpwYXNz' \
--form 'images=@"path/to/image.jpg"'
```

Пример ответа:
```
[
    {
        "id": "333e9722-ed55-4e90-9804-bbbeed75aea8",
        "fileName": "ba5c3c5ac34807e6b21c8a3eeb62ed62-image.jpg",
        "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
    }
]
```

### DELETE /api/Image/delete - [Auth]

Удаление изображения.

- Тело запроса
  - id - Id изображения - [Required]
- Ответ 400BadRequest
  - Изображение не найдено
- Ответ 403Forbidden
  - Попытка удалить изображение, непринадлежащее пользователю
- Ответ 500InternalServerError
  - Ошибка во время удаления файла в локальном хранилище
- Тело ответа 200OK
  - Информации об изображении

Пример запроса:
```
curl --location --request DELETE 'http://localhost:5065/api/Image/delete?id=8a0c57ba-52a7-4c79-a28a-ef49d5a1044d' \
--header 'Authorization: Basic dXNlcjpwYXNz'
```

Пример ответа:
```
{
    "id": "8a0c57ba-52a7-4c79-a28a-ef49d5a1044d",
    "fileName": "ba5c3c5ac34807e6b21c8a3eeb62ed62--emoji-decorations-decoration-crafts.jpg",
    "userId": "2738f277-9fe5-440a-a98d-3bb317f8508c"
}
```