using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace LoeApp.Services;

public class HtmlParseService
{
    public List<DataRow> GetStatesPerStreet()
    {
        var client = new HttpClient();
        var content = client.GetStringAsync(@"https://poweroff.loe.lviv.ua/search_off?csrfmiddlewaretoken=S3nXwVXUie4HvyLehiZ4odjh2i6fnGWx0sKuX3TPRl0gJo2K9NbBzi70rvNh1smQ&city=%D0%9B%D1%8C%D0%B2%D1%96%D0%B2&street=&otg=&q=%D0%9F%D0%BE%D1%88%D1%83%D0%BA").Result;
        HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(content);
       
        var tableNode = document.DocumentNode.Descendants("table").Last(); //.FirstOrDefault(); //.Attributes["<td"].Value;
        var tableContentNodes = tableNode.SelectNodes("//tbody/tr/td").ToList();

        var list = new List<DataRow>();

        var counter = 0;

        DataRow tempRow = null;

        foreach (var htmlNode in tableContentNodes)
        {
            if (counter > 0)
            {
                switch (counter)
                {
                    case 6: tempRow.Street = htmlNode.InnerText; break;
                    case 5: tempRow.Building = htmlNode.InnerText; break;
                    case 4: tempRow.TurnOffType = htmlNode.InnerText; break;
                    case 3: tempRow.Reason = htmlNode.InnerText; break;
                    case 2: tempRow.TurnOffTime = htmlNode.InnerText; break;
                    case 1: tempRow.ExpectedTurnOnTime = htmlNode.InnerText; break;
                }
                counter--;
            }

            if (counter == 0 && tempRow != null)
            {
                list.Add(tempRow);
                tempRow = null;
            }

            if (htmlNode.InnerText == "Львів")
            {
                tempRow = new DataRow();
                counter = 6;
                tempRow.Settlement = htmlNode.InnerText;
            }
        }

        return list;
    }
}

public class DataRow
{
    public string Settlement { get; set; }
    public string Street { get; set; }
    public string Building { get; set; }
    public string TurnOffType { get; set; }
    public string Reason { get; set; }
    public string TurnOffTime { get; set; }
    public string ExpectedTurnOnTime { get; set; }
}