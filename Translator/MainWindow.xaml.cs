using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Globalization;
using System.Threading;
using Translator.Properties;

namespace Translator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static StringDictionary LanguageNamesToShortCodes = new StringDictionary()
        {
            {"Afrikaans","af"},
            {"Albanian","sq"},
            {"Arabic","ar"},
            {"Belarusian","be"},
            {"Bulgarian","bg"},
            {"Catalan","ca"},
            {"Chinese","zh-CN"},
            {"Chinese (Simplified)","zh-CN"},
            {"Simplified Chinese","zh-CN"},
            {"Chinese (Traditional)","zh-TW"},
            {"Traditional Chinese","zh-TW"},
            {"Croatian","hr"},
            {"Czech","cs"},
            {"Danish","da"},
            {"Dutch","nl"},
            {"English","en"},
            {"Estonian","et"},
            {"Filipino","tl"},
            {"Finnish","fi"},
            {"French","fr"},
            {"Galician","gl"},
            {"German","de"},           
            {"Greek","el"},
            {"Hebrew","iw"},
            {"Hindi","hi"},
            {"Hungarian","hu"},
            {"Icelandic","is"},
            {"Indonesian","id"},
            {"Irish","ga"},
            {"Italian","it"},
            {"Japanese","ja"},
            {"Korean","ko"},
            {"Latvian","lv"},
            {"Lithuanian","lt"},
            {"Macedonian","mk"},
            {"Malay","ms"},
            {"Maltese","mt"},
            {"Persian","fa"},
            {"Polish","pl"},
            {"Portugese","pt"},
            {"Romanian","ro"},
            {"Russian","ru"},
            {"Serbian","sr"},
            {"Slovak","sk"},
            {"Slovenian","sl"},
            {"Spanish","es"},
            {"Swahili","sw"},
            {"Swedish","sv"},
            {"Thai","th"},
            {"Turkish","tr"},
            {"Ukranian","uk"},
            {"Vietnamese","vi"},
            {"Welsh","cy"},
            {"Yiddish","yi"}
        };

        public MainWindow()
        {
            InitializeComponent();
            List<String> LanguageNames = new List<string>();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
				

            foreach (String LanguageName in LanguageNamesToShortCodes.Keys)
            {
                LanguageNames.Add(textInfo.ToTitleCase(LanguageName));
            }
            LanguageNames.Sort();
            foreach (String LanguageName in LanguageNames)
            {
                CmbBoxFrom.Items.Add(LanguageName);
                CmbBoxTo.Items.Add(LanguageName);
            }

            CmbBoxFrom.SelectedItem = Settings.Default.LangFrom;
            CmbBoxTo.SelectedItem = Settings.Default.LangTo;
        }

        private void BtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            String translationString = String.Format("{0}|{1}|{2}", TxtBoxFrom.Text, CmbBoxTo.SelectedItem.ToString(), CmbBoxFrom.SelectedItem.ToString());
            String translation = GetTranslation(translationString);
            translation = translation.Replace("\"", "");
            TxtBoxTo.Text = translation.Trim();
        }

        static string GetTranslation(String input)
        {
            String baseUrl = "http://ajax.googleapis.com/ajax/services/language/translate?";
            String dataToBeTranslated = "";
            String sourceLanguage = "";
            String destinationLanguage = "";


            if (input.Contains('|'))
            {
                String[] splitInput = input.Split('|');
                dataToBeTranslated = splitInput[0].Trim();
                destinationLanguage = LanguageNamesToShortCodes[splitInput[1].Trim()];
                sourceLanguage = splitInput.Length == 3 ? splitInput[2].Trim() : "";
                if (sourceLanguage != "") sourceLanguage = LanguageNamesToShortCodes[destinationLanguage];

            }



            String langpair = String.Format("{0}|{1}", sourceLanguage, destinationLanguage);
            UTF8Encoding utf8encoding = new UTF8Encoding();
            UnicodeEncoding utf32encoding = new UnicodeEncoding();
            String data = MakeQueryString(new Dictionary<string, string>()
            {
                {"v","1.0"},
                {"q",dataToBeTranslated},
                {"ie","UTF8"},
                {"langpair",langpair}
            });
            String url = baseUrl + data;
            String json = GetWebContents(url);


            JObject o = JObject.Parse(json);
            String s = o["responseData"]["translatedText"].ToString();

            String utf8DecodedString = Encoding.UTF8.GetString(Encoding.Default.GetBytes(s));


            return utf8DecodedString;
        }

        static String GetWebContents(String url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            return client.DownloadString(url);
        }

        //Create a query string from a dictionary
        static public string MakeQueryString(Dictionary<string, string> args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string name in args.Keys)
            {
                sb.Append(HttpUtility.UrlEncode(name));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(args[name]));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.LangFrom = CmbBoxFrom.SelectedItem.ToString();
            Settings.Default.LangTo = CmbBoxTo.SelectedItem.ToString();
            Settings.Default.Save();
        }

        private void BtnSwitch_Click(object sender, RoutedEventArgs e)
        {
            String temp = TxtBoxTo.Text;
            TxtBoxTo.Text = TxtBoxFrom.Text;
            TxtBoxFrom.Text = temp;

            String tempLang = CmbBoxTo.SelectedItem.ToString();
            CmbBoxTo.SelectedItem = CmbBoxFrom.SelectedItem.ToString();
            CmbBoxFrom.SelectedItem = tempLang;
            
        }

        private void ChkBoxOnTop_Checked(object sender, RoutedEventArgs e)
        {
            SetValue(MainWindow.TopmostProperty,ChkBoxOnTop.IsChecked);
            
        }

      
    }
}
