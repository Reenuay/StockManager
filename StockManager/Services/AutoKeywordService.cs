using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using NLog;
using StockManager.Models;
using System.Data.Entity.Validation;

namespace StockManager.Services {
    public static class AutoKeywordService {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static float Progress {
            get;
            private set;
        }

        public static event EventHandler ProgressChanged;

        private static void ChangeProgress(float progress) {
            Progress = progress;
            ProgressChanged?.Invoke(null, new EventArgs());
        }

        public static async Task<bool> DoWork(IEnumerable<Icon> icons) {
            try {
                var client = new HttpClient();
                var parser = new HtmlParser();
                var count = icons.Count();
                var counter = 0.0f;

                ChangeProgress(counter / count);

                using (var context = new Context()) {

                    foreach (var icon in icons) {
                        var rgx = new Regex("[^a-zA-Z]");
                        var name = rgx.Replace(
                            Path.GetFileNameWithoutExtension(icon.FullPath),
                            " "
                        ).Trim();

                        var response = await client.GetStringAsync(
                            "https://www.mykeyworder.com/keywords?tags=" + name
                        );

                        var document = await parser.ParseAsync(response);

                        var inputs = document.QuerySelectorAll(
                            "input[name='keywordselect[]'][checked]"
                        );

                        foreach (var input in inputs) {
                            var key = input.GetAttribute("value");

                            if (string.IsNullOrEmpty(key))
                                continue;

                            var keyword = context
                                .Keywords
                                .FirstOrDefault(k => k.Name == key);

                            if (keyword == null) {
                                keyword = new Keyword {
                                    Name = key
                                };

                                context.Keywords.Add(keyword);
                            }

                            if (!keyword.Icons.Any(i => i.Id == icon.Id))
                                keyword.Icons.Add(context.Icons.Find(icon.Id));
                        }

                        try {
                            await context.SaveChangesAsync();
                        }
                        catch (DbEntityValidationException e) {
                            foreach (var eve in e.EntityValidationErrors) {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors) {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                    counter++;
                        ChangeProgress(counter / count);
                    }
                }
            }
            catch (Exception ex) {
                logger.Error(ex);
                return false;
            }

            return true;
        }
    }
}
