using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

int update_id = 0;
string messagFronId = "";
string messageText = "";
string firstName = "";
string Path = @"D:\repos\WeatherAppToken.txt";
string token = System.IO.File.ReadAllText(Path);

WebClient webClient = new WebClient();

string startUrl = $"https://api.telegram.org/bot{token}";

while (true)
{
    string url = $"{startUrl}/getUpdates?offset={update_id + 1}";
    string response = webClient.DownloadString(url);

    var Messages = JObject.Parse(response)["result"].ToArray();

    foreach (var currentMessage in Messages)
    {
        update_id = Convert.ToInt32(currentMessage["update_id"]);
        try
        {

            firstName = currentMessage["message"]["from"]["first_name"].ToString();
            messagFronId = currentMessage["message"]["from"]["id"].ToString();
            messageText = currentMessage["message"]["text"].ToString();

            Console.WriteLine($"{firstName} {messagFronId} {messageText}");

            messageText = GetWeather(messageText);

            url = $"{startUrl}/sendMessage?chat_id={messagFronId}&text={messageText}";
            webClient.DownloadString(url);
        }

        catch { }
    }
    Thread.Sleep(100); //Задрежка 0,1 сек.
}

static string GetWeather(string name)
{
    string path = @"D:\repos\WeatherApp\ApiKey.txt";
    string apiKey = System.IO.File.ReadAllText(path);

    //Получение URL странички на которой отображается информация о погоде в формате XML
    string url =
        $"http://api.openweathermap.org/data/2.5/weather?q={name}&lang=ru&appid={apiKey}&units=metric&mode=xml";

    Console.WriteLine(url);

    //Создание объекта класса WebClient и загрузка информации с URL
    string xmlData = new WebClient().DownloadString(url);

    //Парсинг XML
    var xmlColItem = XDocument.Parse(xmlData).Descendants("current").ToArray();

    string text = string.Empty;

    foreach (var item in xmlColItem)
    {
        text += $"Погода в городе {item.Element("city").Attribute("name").Value} сегодня, " +
                $"{item.Element("weather").Attribute("value").Value}, " +
                $"температура от {item.Element("temperature").Attribute("min").Value} до {item.Element("temperature").Attribute("max").Value} градусов С, " +
                $"чувствуется как {item.Element("feels_like").Attribute("value").Value}, " +
                $"влажность {item.Element("humidity").Attribute("value").Value} %, ";
    }
    return(text);
}
