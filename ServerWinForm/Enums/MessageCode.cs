using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Enums
{
    public enum MessageCode : byte
    {
        START_AUTH, // запуск процесса аутентификации устройства (ожидание логина и пароля или mac-адреса)
        RECONNECTION, // в случае если было потеряно соединение с устройством во время выполнения заявки
        SEND_PROPOSAL, // отправка необходимого списка продуктов из json-файла от сервера к клиентам
        ACCESS_GRANTED, // устройство прошло авторизацию
        ACCESS_DENIED, // устройство не прошло аутентификацию
        PROPOSAL_ACCEPTED, // клиент принял заявку от сервера
        PROPOSAL_REJECTED, // клиент отклонил заявку от сервера
        JOB_DONE, // клиент выполнил заявку (штрих-коды просканированы без ошибок)
    }
}
