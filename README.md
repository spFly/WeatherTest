Задание:

Используя Яндекс.API погода([1]https://yandex.ru/dev/weather/) сделать контроллер на net core5, который по GET-запросу из браузера с параметром city возвращал бы JSON-объект, содержащий фактические параметры погоды :-Температура (°C)-Ощущаемая температура (°C).-Температура воды (°C).Если в городе нет водоема, вернуть null-Код расшифровки погодного описания(в кириллице)-Скорость ветра (в м/с).-Скорость порывов ветра (в м/с).-Направление ветра(в кириллице)-Давление (в мм рт. ст.).-Тип осадков.(в кириллице)Набор городов органичен следующими значениями:-Krasnodar-Moscow-Orengurg-St.Peretburg-Kaliningrad.Историю запросов хранить в базе данных.
Контроллер должен быть спроектирован таким образом, чтобы мог обрабатывать до 100 000 запросов в секунду. предусмотреть условие, что задержка исполнения запроса API Yandex может составлять до 100мс.

Так как основным условием задания была возможность обработки 100 000 запросов в секунду, разделил получение данных и сохранение логов в базу на разные сервисы которые можно горизонтально маштабировать и упровлять нагрузкой через LoadBalancer, передача сообщений между сервисами в режиме fire and forget через шину событий (MassTransit). Так же раз в условии задачи явно ограничен набор доступных городов, для которых можно получить информацию, целесообразно кэшировать данные о погоде в inMemoryStorage, поэтому реализовал сохранение в кэш через Service Worker по расписанию. Так же в кэш хранится список доступных городов, для быстрой проверки при получении запроса.
Для получения списка доступных городов реализован метод, возвращающий статические данные, но при необходимости можно подменить реализацию на любую другую (Запрос из БД, запрос из другого сервиса, получение из конфига и т.д.).

Архитектура системы выглядит следующим образом:

![d1](https://user-images.githubusercontent.com/51089027/130180159-6a2f619c-5861-43ad-a86f-52939c336fde.png)

Вся система запускается в docker-compose и включает в себя
Сервис WebAPI WeatherTest
Сервис WebAPI RequestLogAPI
Сервер RabbitMQ
Сервер redis

Для запуска:

В WeatherTest appsettings.json

"YandexAPIKey" – указать действующий яндекс API ключ

В RequestLogAPI appsettings.json

"DefaultConnection" указать параметры подключения к БД

Настоить запуск с docker-compose
