using System;
using System.Text.Json;
using System.Threading;

namespace HomeBroker
{
  class Program
  {
    static void Main(string[] args)
    {
      string stock = args[0];
      double max = convertPrice(args[1]);
      double min = convertPrice(args[2]);

      while (true)
      {
        string price = getPriceStockAPI(stock);
        Thread.Sleep(15000);
      }
    }

    static double convertPrice(string price)
    {
      double value = Convert.ToDouble(price.Replace('.', ','));
      return value;
    }

    static string getPriceStockAPI(string stock)
    {
      try
      {
        string APIKey = GetSettingValue("APIKey");

        string url = $"https://api.hgbrasil.com/finance/stock_price?key={APIKey}&symbol={stock}";

        using (HttpClient client = new HttpClient())
        {
          var response = client.GetAsync(url).Result;
          string price = "";

          if (response.IsSuccessStatusCode)
          {
            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonDocument.Parse(json).RootElement.GetProperty("results").GetProperty(stock);
            price = result.GetProperty("price").ToString();

            Console.WriteLine($"Ativo {stock}, consultado {price}");
          }
          else
          {
            Console.WriteLine($"Erro em consultar ativo {stock}");
            return "";
          }
          return price;
        }
      }
      catch (Exception error)
      {
        Console.WriteLine(error.Message);
        throw;
      }
    }

    static string GetSettingValue(string paramName)
    {
      try
      {
        string json = System.IO.File.ReadAllText(@".\configFile.json");
        var configs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        if (configs == null)
        {
          Console.WriteLine("Parâmetro não encontrado no arquivo de configuração");
          return "";
        }
        return configs[paramName];
      }
      catch (FileNotFoundException error)
      {
        Console.WriteLine(error.Message);
        throw;
      }
    }
  }
}