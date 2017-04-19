using JobSearch.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace JobSearch.Converters
{
    public class HightlightTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var descObj = value as HighlightControl;
            int fontSize;
            if (!int.TryParse(parameter as string, out fontSize))
            {
                // set Default
                fontSize = 10;
            }
            if (descObj != null)
            {
                string input = descObj.Value;
                string[] highlights = descObj.HighlightWords;
                var replacementIndexes = new List<Tuple<int, int>>();

                if (input != null)
                {
                    var textBlock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = fontSize
                    };
                    if (highlights != null && highlights.Length > 0)
                    {
                        foreach (var hightlight in highlights)
                        {
                            var indexes = input.AllIndexesOf(hightlight);
                            foreach (var i in indexes)
                            {
                                replacementIndexes.Add(Tuple.Create(i, hightlight.Length));
                            }
                        }

                        var lastIndex = 0;
                        if (replacementIndexes.Count > 0)
                        {
                            var orderedreplacements = replacementIndexes.OrderBy(x => x.Item1);
                            foreach (var replacement in orderedreplacements)
                            {
                                var part = input.Substring(lastIndex, (replacement.Item1 - lastIndex));
                                textBlock.Inlines.Add(new Run() { Text = part });
                                part = input.Substring(replacement.Item1, replacement.Item2);
                                textBlock.Inlines.Add(new Run() { Text = part, FontWeight = FontWeights.Bold });
                                lastIndex = replacement.Item1 + replacement.Item2;
                            }
                        }
                        // add the remaining
                        var lastPart = input.Substring(lastIndex);
                        textBlock.Inlines.Add(new Run() { Text = lastPart });
                    }
                    else
                    {
                        textBlock.Inlines.Add(new Run() { Text = input });
                    }
                    return textBlock;
                }
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }
    }


    public static class StringExtensions
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    return indexes;
                }
                indexes.Add(index);
            }
        }
    }
}
