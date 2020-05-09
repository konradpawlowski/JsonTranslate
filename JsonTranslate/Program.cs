using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace JsonTranslate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Translate");
            Console.WriteLine("====================");
            try
            {
                List<string> lang = new List<string> { "da", "sv" };

                Program prog = new Program();
                prog.TranslateFile("en", @"c:\Temp\trans\", lang);


            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

        }

        //$env:GOOGLE_APPLICATION_CREDENTIALS = "$env:d:\Downloads\JsonTranslate-40e35c034065.json"

        public Dictionary<string, string> ReadFile(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

        }



        public void TranslateFile(string fromLanguage, string path, List<string> languages)
        {
            TranslationClient client = TranslationClient.Create();
            List<string> fileList = Directory.GetFiles(path, $"*.json").ToList();
            foreach (string language in languages)
            {
                foreach (string fileName in fileList)
                {
                    Dictionary<string, string> toTranslaDictionary = ReadFile(fileName);
                    Dictionary<string, string> dic2 = new Dictionary<string, string>();
                    string fileNameOutput = fileName.Replace($".{fromLanguage}.", $".{language}.");
                    foreach (KeyValuePair<string, string> pair in toTranslaDictionary)
                    {
                        var response = client.TranslateText(
                            text: pair.Value,
                            targetLanguage: language,  // Russian
                            sourceLanguage: fromLanguage);  // English
                        dic2.Add(pair.Key, response.TranslatedText);
                        Console.WriteLine($"{pair.Value} - {response.TranslatedText}");
                    }

                    string json2 = JsonConvert.SerializeObject(dic2, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(fileNameOutput, json2, Encoding.UTF8);
                        Console.WriteLine($"File save {fileNameOutput}");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        var r = new Random();
                        fileNameOutput = $"{r.Next(100000).ToString()}_{fileNameOutput}";
                        File.WriteAllText(fileNameOutput, json2, Encoding.UTF8);
                        Console.WriteLine($"File save {fileNameOutput}");
                    }
                }



            }




        }



    }
}
