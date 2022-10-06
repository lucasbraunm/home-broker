using System;
using System.Text.Json;
using System.Threading;
using System.Net.Mail;
using System.Net;

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
        string priceStr = getPriceStockAPI(stock);
        if (priceStr == "")
        {
          Console.WriteLine($"Não foi possível adquirir preço do ativo {stock}");
          return;
        }
        double price = convertPrice(priceStr);

        string recommendation = StockRecommendation(max, min, price);
        MailWithRecommendation(recommendation, stock, price);

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

    static string StockRecommendation(double max, double min, double price)
    {
      if (price > max)
      {
        return "Vender";
      }
      else if (price < min)
      {
        return "Comprar";
      }
      else
      {
        return "Manter";
      }
    }

    static void MailWithRecommendation(string recommendation, string stock, double price)
    {
      try
      {
        MailMessage message = GetMailMessage(recommendation, stock, price);

        if (message.Subject != "")
        {
          Console.WriteLine(message.Subject);
          string server = GetSettingValue("SMTPServer");
          int host = Convert.ToInt32(GetSettingValue("SMTPHost"));
          string user = GetSettingValue("SMTPUser");
          string password = GetSettingValue("SMTPPassword");

          SmtpClient smtp = new SmtpClient(server, host)
          {
            Credentials = new NetworkCredential(user, password),
            EnableSsl = true
          };
          smtp.Send(message);
          Console.WriteLine("Email enviado");
        }
        else
        {
          Console.WriteLine("Email não enviado");
        }
      }
      catch (Exception error)
      {
        Console.WriteLine(error.Message);
        throw;
      }
    }

    static MailMessage GetMailMessage(string recommendation, string stock, double price)
    {
      MailAddress from = new MailAddress(GetSettingValue("emailFrom"));
      MailAddress to = new MailAddress(GetSettingValue("emailTo"));
      MailMessage message = new MailMessage(from, to);

      if (recommendation == "Vender")
      {
        message.Subject = recommendation;
        message.Body = $"É recomendada a venda do ativo {stock} no preço {price}";
      }
      else if (recommendation == "Comprar")
      {
        message.Subject = recommendation;
        message.Body = $"É recomendada a compra do ativo {stock} no preço {price}";
      }
      else
      {
        message.Subject = "";
        message.Body = "";
      }

      return message;
    }

    private static string GetSettingValue(string paramName)
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