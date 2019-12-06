using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Unicode;
using McMaster.Extensions.CommandLineUtils;

namespace ESAnimalRecognition
{
    class Program
    {
        static Rules db;
        static void Main(string[] args)
        {
            #region 配置CLI程序基础信息和选项
            var app = new CommandLineApplication
            {
                Name = "ESAnimalRecognition",
                FullName = "简单的专家系统：动物识别",
                Description = "用产生式系统设计的一个简单的专家系统，根据输入事实得出最接近的推论"
            };
            app.HelpOption("-h|--help");
            var ruleOption= app.Option("-r|--rule", "指定所使用的规则库的路径（默认使用同路径下rules.json文件）", CommandOptionType.SingleValue);            
            var deduceOption=app.Option("-d|--deduce", "使用指定的规则库对后方的事实进行推理（以全角逗号作为分隔符）", CommandOptionType.SingleValue);
            var addOption=app.Option("-a|--add", "添加新规则到指定的规则（最后一个参数为结论，倒数第二个参数用true/false标明是否为最终结论，以全角逗号作为分隔符）", CommandOptionType.SingleValue);
            #endregion
            #region CLI程序真正执行的内容
            app.OnExecute(() =>
            {
                //数据库路径及其读取
                var path = "rules.json";
                if (ruleOption.HasValue())
                    path = ruleOption.Value();
                var text = File.ReadAllText(path);
                db = JsonSerializer.Deserialize<Rules>(text);
                #region 添加规则
                if (addOption.HasValue())
                {
                    var input = addOption.Value();
                    var values = input.Split('，');
                    var conclusion = values[^1];
                    var isFinal = values[^2] == "true";
                    if (!isFinal)
                        if (values[^2] != "false")
                            throw new ArgumentException("请确保倒数第个参数为true或false");//直接抛异常了
                    var newRule = new Rule
                    {
                        features = new HashSet<int>(),
                        isFinal = isFinal
                    };
                    for (int i = 0; i < values.Length - 2; i++)
                    {
                        if (!db.allFeatures.Contains(values[i]))
                            db.allFeatures.Add(values[i]);
                        newRule.features.Add(db.allFeatures.IndexOf(values[i]));
                    }
                    if (newRule.isFinal)
                    {
                        if (!db.finalConclusion.Contains(conclusion))
                            db.finalConclusion.Add(conclusion);
                        newRule.conclusion = db.finalConclusion.IndexOf(conclusion);
                    }
                    else
                    {
                        if (!db.allFeatures.Contains(conclusion))
                            db.allFeatures.Add(conclusion);
                        newRule.conclusion = db.allFeatures.IndexOf(conclusion);
                    }
                    if (db.rules.Add(newRule))//只有不重复的规则才会被加入数据库并写回数据文件
                    {
                        using (var writer = new StreamWriter(path, false))
                        {
                            var options = new JsonSerializerOptions
                            {
                                WriteIndented = true,
                                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
                            };
                            var jsonStr = JsonSerializer.Serialize(db, options);
                            writer.Write(jsonStr);
                        }
                        Console.WriteLine("已将规则写入规则库文件");
                    }
                    else Console.WriteLine("规则库中已存在该规则，请勿重复添加");
                }
                #endregion
                #region 按照规则进行推理
                if (deduceOption.HasValue())
                {
                    var input = deduceOption.Value();
                    var values = input.Split('，');
                    HashSet<int> facts = new HashSet<int>();
                    string conclusion = "";
                    foreach (var item in values)
                    {
                        int temp = db.allFeatures.IndexOf(item);
                        if (temp >= 0)
                            facts.Add(temp);
                        else
                            Console.WriteLine("未能在规则库中找到符合输入内容“" + item + "”的规则");
                    }
                    while (true)
                    {
                        bool breakloop = true;
                        foreach (var rule in db.rules)
                        {
                            if (facts.IsSupersetOf(rule.features))
                            {
                                breakloop = false;
                                if (rule.isFinal)
                                {
                                    conclusion = db.finalConclusion[rule.conclusion];
                                    breakloop = true;
                                    break;
                                }
                                else
                                {
                                    facts.Add(rule.conclusion);
                                    if (conclusion.Length == 0)
                                        conclusion = db.allFeatures[rule.conclusion];
                                }
                            }
                            else breakloop = true;
                        }                        
                       if (breakloop)
                           break;
                    }
                    if (conclusion.Length != 0)
                        Console.WriteLine("可能的结果：动物" + conclusion);
                    else Console.WriteLine("未能推理出结果");
                }
                #endregion
                return 0;
            });
            #endregion
            if (args.Length==0)
                app.ShowHelp();          
            else try
                {
                    app.Execute(args);
                }
               catch (Exception e)//异常处理，不仔细分了，Exception全都打出来
               {
                    Console.WriteLine(e.Message);
                }       
            app.Dispose();
        }
    }
}
