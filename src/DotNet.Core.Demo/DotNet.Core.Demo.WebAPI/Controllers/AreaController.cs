using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Core.Demo.IServices;
using DotNet.Core.Demo.Models;
using DotNet.Standard.NParsing.Factory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DotNet.Core.Demo.WebAPI.Controllers
{
    public class AreaController : BaseController<IAreaService, AreaInfo>
    {

        [HttpGet]
        public object Down()
        { 
            _= Down(null, 1, "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021/index.html");
            return new
            {
                Status = "ok"
            };
            /*var jsonString = await new HttpClient
            {
                DefaultRequestHeaders =
                {
                    {"Authorization", "APPCODE 994bc4c9d396452eb4e36a1b45187a84"}
                }
            }.GetStringAsync("http://jisuarea.market.alicloudapi.com/area/all");
            //var jsonString =
            //    "{\"status\":0,\"msg\":\"ok\",\"result\":[{\"id\":1,\"name\":\"北京\",\"parentid\":0,\"parentname\":\"\",\"districtcode\":\"11\",\"areacode\":\"010\",\"zipcode\":\"100000\",\"population\":\"1300万人\",\"totalarea\":\"16412平方千米\",\"depth\":1}]}";
            var jo = JObject.Parse(jsonString);
            if (jo.Value<string>("status") == "0")
            {
                var list = jo["result"].Children<JObject>();
                foreach (var item in list)
                {
                    var area = new AreaInfo().Of();
                    area.AreaId = item.Value<int>("id");
                    area.Name = item.Value<string>("name");
                    area.AreaCode = item.Value<string>("areacode");
                    area.ParentId = item.Value<int>("parentid");
                    area.ParentName = item.Value<string>("parentname");
                    area.DistrictCode = item.Value<string>("districtcode");
                    area.ZipCode = item.Value<string>("zipcode");
                    area.Depth = item.Value<int>("depth");
                    await Service.Add(area);
                }
            }
            return jo;*/
        }

        private async Task<Dictionary<string, string>> Down(string parentId, int depth, string url)
        {
            var rurl = url.Substring(0, url.LastIndexOf('/')+1);
            var dic = new Dictionary<string, string>();
            await Task.Delay(1000);
            var html = await new HttpClient().GetStringAsync(url);
            if (html.Contains("<table class=\"provincetable\"")) //省
            {
                var matchs = Regex.Matches(html, "<td><a href=\"(([^.]*).html)\">([^<]*)<br /></a></td>");
                foreach (Match match in matchs)
                {
                    dic.Add(match.Groups[2].Value, match.Groups[3].Value);
                    var model = new AreaInfo().Of();
                    model.ParentId = parentId;
                    model.AreaId = match.Groups[2].Value + "0000000000";
                    model.Name = match.Groups[3].Value;
                    model.Depth = depth;
                    await Service.Add(model);
                }
                depth++;
                foreach (Match match in matchs)
                {
                    var dic2 = await Down(match.Groups[2].Value + "0000000000", depth, rurl + match.Groups[1].Value);
                    foreach (var (key, value) in dic2)
                    {
                        dic.Add(key, value);
                    }
                }
            }
            else if (html.Contains("<table class=\"citytable\"") //市
                     || html.Contains("<table class=\"countytable\"") //区、县
                     || html.Contains("<table class=\"towntable\"") //街道、乡镇
                     || html.Contains("<table class=\"villagetable\"") //社区、村
                     )
            {
                var matchs = Regex.Matches(html, "<td><a href=\"([^/]*/([^.]*).html)\">([^<]*)</a></td>");
                for (var i = 0; i < matchs.Count; i++)
                {
                    var match = matchs[i++];
                    var match2 = matchs[i];
                    dic.Add(match.Groups[3].Value, match2.Groups[3].Value);
                    var model = new AreaInfo().Of();
                    model.ParentId = parentId;
                    model.AreaId = match.Groups[3].Value;
                    model.Name = match2.Groups[3].Value;
                    model.Depth = depth;
                    await Service.Add(model);
                }
                depth++;
                if (!html.Contains("<table class=\"villagetable\""))
                {
                    //不是最后一级
                    for (var i = 0; i < matchs.Count; i += 2)
                    {
                        var match = matchs[i];
                        var dic2 = await Down(match.Groups[3].Value, depth, rurl + match.Groups[1].Value);
                        foreach (var (key, value) in dic2)
                        {
                            dic.Add(key, value);
                        }
                    }
                }
            }
            /*else if (html.Contains("<table class=\"countytable\""))
            {
                //区县
                var matchs = Regex.Matches(html, "<td><a href=\"([^/]#1#([^.]*).html)\">([^<]*)</a></td>");
                for (var i = 0; i < matchs.Count; i++)
                {
                    var match = matchs[i++];
                    var match2 = matchs[i];
                    dic.Add(match.Groups[3].Value, match2.Groups[3].Value);
                    var dic2 = await Down(rurl + match.Groups[1].Value);
                    foreach (var (key, value) in dic2)
                    {
                        dic.Add(key, value);
                    }
                }
            }
            else if (html.Contains("<table class=\"towntable\""))
            {
                //街道、镇、乡
                var matchs = Regex.Matches(html, "<td><a href=\"([^/]#1#([^.]*).html)\">([^<]*)</a></td>");
                for (var i = 0; i < matchs.Count; i++)
                {
                    var match = matchs[i++];
                    var match2 = matchs[i];
                    dic.Add(match.Groups[3].Value, match2.Groups[3].Value);
                    var dic2 = await Down(rurl + match.Groups[1].Value);
                    foreach (var (key, value) in dic2)
                    {
                        dic.Add(key, value);
                    }
                }
            }
            else if (html.Contains("<table class=\"villagetable\""))
            {
                //社区、村
                var matchs = Regex.Matches(html, "<td><a href=\"([^/]#1#([^.]*).html)\">([^<]*)</a></td>");
                for (var i = 0; i < matchs.Count; i++)
                {
                    var match = matchs[i++];
                    var match2 = matchs[i];
                    dic.Add(match.Groups[3].Value, match2.Groups[3].Value);
                }
            }*/
            return dic;
        }
    }
}
