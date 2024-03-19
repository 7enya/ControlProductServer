using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Enums
{
    public enum OpCode : byte
    {
        CHECK_CONNECTION, // проверка подключения
        START_AUTH, // запуск процесса аутентификации устройства (ожидание логина и пароля)
        ACCESS_DENIED, // если устройство не прошло аутентификацию
        PRODUCT_LIST, // отправка необходимого списка продуктов из json-файла от сервера к клиентам
        JOB_DONE, // сигнал серверу от клиента, что все штрих-коды просканированы без ошибок
        JOB_FAILED // сигнал серверу от клиента, что при сканировании штрих-кодов возникла ошибка
    }
}
