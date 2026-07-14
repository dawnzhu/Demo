using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Core.Demo.Services;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NSmart.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Core.Demo.ConsoleApp
{
    internal class Program
    {
        private static IAreaService Service;
        private static HttpClient httpClient;
        static  void Main(string[] args)
        {
            httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                DefaultRequestHeaders =
                {
                    {"KeepAlive", "false"},
                    {"UserAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11"}
                },
                BaseAddress = new Uri("http://www.stats.gov.cn"),
                Timeout = new TimeSpan(0,0,0,30)
            };
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true).Build();
            var services = new ServiceCollection();
            services.AddNSmart(options =>
            {
                options.Use(configuration);
            });
            Service = new AreaService();
            _ = Down(null, "", 1, "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021/index.html");
            Console.Read();
        }

        private static async Task<Dictionary<string, string>> Down(string parentId, string parentName, int depth, string url)
        {
            Console.WriteLine(url);
            var rurl = url.Substring(0, url.LastIndexOf('/') + 1);
            var dic = new Dictionary<string, string>();
            await Task.Delay(1000);
            var count = 6;
            var html = "";
            do
            {
                try
                {
                    html = await httpClient.GetStringAsync(url);
                    count = 0;
                }
                catch (Exception er)
                {
                    count--;
                    Console.WriteLine(er.Message);
                    await Task.Delay(1000);
                }
            } while (count > 0);
            if (html.Contains("<table class=\"provincetable\"")) //省
            {
                var matchs = Regex.Matches(html, "<td><a href=\"(([^.]*).html)\">([^<]*)<br /></a></td>");
                foreach (Match match in matchs)
                {
                    dic.Add(match.Groups[2].Value, match.Groups[3].Value);
                    var model = new AreaInfo().Of();
                    model.ParentId = parentId;
                    model.AreaId = match.Groups[2].Value + "0000";
                    model.Name = match.Groups[3].Value.TrimEnd('市');
                    model.FullName = model.Name;
                    model.Depth = depth;
                    await Service.Add(model);
                    var dic2 = await Down(match.Groups[2].Value + "0000", model.FullName + " ", depth+1, rurl + match.Groups[1].Value);
                    foreach (var (key, value) in dic2)
                    {
                        dic.Add(key, value);
                    }
                }
            }
            else if (html.Contains("<table class=\"citytable\"") //市
                     || html.Contains("<table class=\"countytable\"") //区、县
                     //|| html.Contains("<table class=\"towntable\"") //街道、乡镇
                    )
            {
                if (html.Contains("<table class=\"countytable\"") //区、县
                    && !Regex.IsMatch(html, "<td>([^<]*)</td>[^<]*<td>市辖区</td>"))
                {
                    var model = new AreaInfo().Of();
                    model.ParentId = parentId;
                    model.AreaId = parentId.TrimEnd('0') + "1";
                    model.Name = parentName.Split(' ')[1].TrimEnd();
                    model.FullName = parentName.Replace("省直辖县级行政区划", model.Name).Replace("自治区直辖县级行政区划", model.Name) + model.Name;
                    model.Depth = depth;
                    await Service.Add(model);
                }
                else
                {
                    var matchs = Regex.Matches(html, "<td><a href=\"([^.]*.html)\">([^<]*)</a></td>[^<]*<td><a[^>]*>([^<]*)</a></td>");
                    foreach (Match match in matchs)
                    {
                        dic.Add(match.Groups[2].Value, match.Groups[3].Value);
                        var model = new AreaInfo().Of();
                        model.ParentId = parentId;
                        model.AreaId = match.Groups[2].Value.Substring(0, 6);
                        model.Name = match.Groups[3].Value;
                        if (model.Name == "市辖区")
                        {
                            model.Name = parentName.TrimEnd() + "市";
                        }
                        //parentName = parentName.Replace("省直辖县级行政区划", model.Name).Replace("自治区直辖县级行政区划", model.Name);
                        model.FullName = parentName.Replace("省直辖县级行政区划", model.Name).Replace("自治区直辖县级行政区划", model.Name) + model.Name;
                        model.Depth = depth;
                        await Service.Add(model);
                        if (!html.Contains("<table class=\"countytable\""))
                        {
                            var dic2 = await Down(match.Groups[2].Value.Substring(0, 6), model.FullName + " ", depth + 1, rurl + match.Groups[1].Value);
                            foreach (var (key, value) in dic2)
                            {
                                dic.Add(key, value);
                            }
                        }
                    }
                }
            }
            /*else if (html.Contains("<table class=\"villagetable\"")) //社区、村
            {
                var matchs = Regex.Matches(html, "<td>([^<]*)</td><td>[^<]*</td><td>([^<]*)</td>");
                foreach (Match match in matchs)
                {
                    dic.Add(match.Groups[1].Value, match.Groups[2].Value);
                    var model = new AreaInfo().Of();
                    model.ParentId = parentId;
                    model.AreaId = match.Groups[1].Value;
                    model.Name = match.Groups[2].Value;
                    model.Depth = depth;
                    await Service.Add(model);
                }
            }*/
            return dic;
        }
    }
}
