using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;

namespace Sentithink
{
    public static class HandleSnippets
    {
        static Dictionary<string, bool> stopwords = getStopwords();

        public static Dictionary<string, bool> getStopwords()
        {
            string[] lines = new string[]
            {
                "a", "able", "about", "above", "according", "accordingly", "across", "actually", "after", "afterwards", "again", "against", "aint", "all", "allow", "allows", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "an", "and", "another", "any", "anybody", "anyhow", "anyone", "anything", "anyway", "anyways", "anywhere", "apart", "appear", "appreciate", "appropriate", "are", "arent", "around", "as", "aside", "ask", "asking", "associated", "at", "available", "away", "awfully", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "believe", "below", "beside", "besides", "best", "better", "between", "beyond", "both", "brief", "but", "by", "came", "can", "cannot", "cant", "cause", "causes", "certain", "certainly", "changes", "clearly", "cmon", "co", "com", "come", "comes", "concerning", "consequently", "consider", "considering", "contain", "containing", "contains", "corresponding", "could", "couldnt", "course", "cs", "currently", "definitely", "described", "despite", "did", "didnt", "different", "do", "does", "doesnt", "doing", "done", "dont", "down", "downwards", "during", "each", "edu", "eg", "eight", "either", "else", "elsewhere", "enough", "entirely", "especially", "et", "etc", "even", "ever", "every", "everybody", "everyone", "everything", "everywhere", "ex", "exactly", "example", "except", "far", "few", "fifth", "first", "five", "followed", "following", "follows", "for", "former", "formerly", "forth", "four", "from", "further", "furthermore", "get", "gets", "getting", "given", "gives", "go", "goes", "going", "gone", "got", "gotten", "greetings", "had", "hadnt", "happens", "hardly", "has", "hasnt", "have", "havent", "having", "he", "hello", "help", "hence", "her", "here", "hereafter", "hereby", "herein", "heres", "hereupon", "hers", "herself", "hes", "hi", "him", "himself", "his", "hither", "hopefully", "how", "howbeit", "however", "i", "id", "ie", "if", "ignored", "ill", "im", "immediate", "in", "inasmuch", "inc", "indeed", "indicate", "indicated", "indicates", "inner", "insofar", "instead", "into", "inward", "is", "isnt", "it", "itd", "itll", "its", "itself", "ive", "just", "keep", "keeps", "kept", "know", "known", "knows", "last", "lately", "later", "latter", "latterly", "least", "less", "lest", "let", "lets", "like", "liked", "likely", "little", "look", "looking", "looks", "ltd", "mainly", "many", "may", "maybe", "me", "mean", "meanwhile", "merely", "might", "more", "moreover", "most", "mostly", "much", "must", "my", "myself", "name", "namely", "nd", "near", "nearly", "necessary", "need", "needs", "neither", "never", "nevertheless", "new", "next", "nine", "no", "nobody", "non", "none", "noone", "nor", "normally", "not", "nothing", "novel", "now", "nowhere", "obviously", "of", "off", "often", "oh", "ok", "okay", "old", "on", "once", "one", "ones", "only", "onto", "or", "other", "others", "otherwise", "ought", "our", "ours", "ourselves", "out", "outside", "over", "overall", "own", "particular", "particularly", "per", "perhaps", "placed", "please", "plus", "possible", "presumably", "probably", "provides", "que", "quite", "qv", "rather", "rd", "re", "really", "reasonably", "regarding", "regardless", "regards", "relatively", "respectively", "right", "said", "same", "saw", "say", "saying", "says", "second", "secondly", "see", "seeing", "seem", "seemed", "seeming", "seems", "seen", "self", "selves", "sensible", "sent", "serious", "seriously", "seven", "several", "shall", "she", "should", "shouldnt", "since", "six", "so", "some", "somebody", "somehow", "someone", "something", "sometime", "sometimes", "somewhat", "somewhere", "soon", "sorry", "specified", "specify", "specifying", "still", "sub", "such", "sup", "sure", "take", "taken", "tell", "tends", "th", "than", "thank", "thanks", "thanx", "that", "thats", "the", "their", "theirs", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "theres", "thereupon", "these", "they", "theyd", "theyll", "theyre", "theyve", "think", "third", "this", "thorough", "thoroughly", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "took", "toward", "towards", "tried", "tries", "truly", "try", "trying", "ts", "twice", "two", "un", "under", "unfortunately", "unless", "unlikely", "until", "unto", "up", "upon", "us", "use", "used", "useful", "uses", "using", "usually", "value", "various", "very", "via", "viz", "vs", "want", "wants", "was", "wasnt", "way", "we", "wed", "welcome", "we'll", "went", "were", "werent", "weve", "what", "whatever", "whats", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "wheres", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whos", "whose", "why", "will", "willing", "wish", "with", "within", "without", "wonder", "wont", "would", "wouldnt", "yes", "yet", "you", "youd", "youll", "your", "youre", "yours", "yourself", "yourselves", "youve", "zero"
            };
            Dictionary<string, bool> stpwrds = new Dictionary<string, bool>();
            foreach (var line in lines)
            {
                stpwrds.Add(line, true);
            }

            return stpwrds;
        }

        [FunctionName("RecordSnippet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            float x = float.Parse(req.Query["X"]);
            float y = float.Parse(req.Query["Y"]);
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IEnumerable<string> wordsTranscribed = new List<string>();

            //requestBody = @"{""job"":{""id"":""V4gui5hWSUVH"",""created_on"":""2019 - 09 - 14T23: 50:52.11Z"",""completed_on"":""2019 - 09 - 14T23: 51:07.976Z"",""name"":""CUsersJonDownloadsspeech.mp3"",""callback_url"":""https://sentithinkfunction.azurewebsites.net/api/RecordSnippet?code=cai3FEcCtUChUkje59jabACIf9N2/Ea0G6UMvj7kS6xX9TBHim0mPg==&X=5.0&Y=3.2&name=TextLock2"",""status"":""transcribed"",""duration_seconds"":2.19,""type"":""async""}}";
            log.LogInformation($"SnippetProcessed from location {name} ({x},{y})");
            foreach (Match match in Regex.Matches(requestBody, @"""id""\:""([a-zA-Z0-9]+)"""))
            {
                for (int groupCtr = 1; groupCtr < match.Groups.Count; groupCtr++)
                {
                    Group group = match.Groups[groupCtr];
                    var jobId = group.Value;
                    var client = new WebClient();
                    client.Headers["Authorization"] = "Bearer 02g01w-kZhbjrtz5qXsiT71gchuHv7SyFFqnRTOAjYYTrJlxV7KA9DkYehOZofxVvOZl_qfQD10LlsiMsu6G--WT7ai2I";
                    client.Headers["Accept"] = "text/plain";
                    var textReturn = client.DownloadString($"https://api.rev.ai/speechtotext/v1/jobs/{jobId}/transcript");
                    wordsTranscribed = wordsTranscribed.Concat(processTranscriptions(textReturn));
                }
            }

            using (var connection = new SqlConnection("Server=tcp:sentithink-server.database.windows.net,1433;Initial Catalog=SentiThinkDB;Persist Security Info=False;User ID=WhoDat;Password=Iggy1994;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                foreach (var word in wordsTranscribed)
                {
                    if (!stopwords.ContainsKey(word) && word != "\n"  && word != "")
                    {
                        var orderDetails = connection.QueryAsync<WORD_SPOKEN>(makeQuery(x,y,name,word)).Result;
                        log.LogInformation(makeQuery(x, y, name, word));
                    }
                }
            }

            return new OkResult();
        }

        [FunctionName("GetLocationWordFrequiencies")]
        public static async Task<ActionResult> Return(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            float x = float.Parse(req.Query["X"]);
            float y = float.Parse(req.Query["Y"]);
            string name = req.Query["name"];

            log.LogInformation($"Getting frequencies from location {name} ({x},{y})");

            IEnumerable<WORD_SPOKEN> orderDetails = new List<WORD_SPOKEN>();
            using (var connection = new SqlConnection("Server=tcp:sentithink-server.database.windows.net,1433;Initial Catalog=SentiThinkDB;Persist Security Info=False;User ID=WhoDat;Password=Iggy1994;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                var sqlQuery = $@"SELECT * FROM WORDS_SPOKEN
                                WHERE (X = {x} AND Y = {y}) OR SPEAKER_NAME = '{name}'
                                ORDER BY FREQUENCY DESC";
                orderDetails = connection.QueryAsync<WORD_SPOKEN>(sqlQuery).Result;
            }

            return new OkObjectResult(JsonConvert.SerializeObject(orderDetails));
        }

        public static IEnumerable<string> processTranscriptions(string textReturned)
        {
            IEnumerable<string> allWords = new List<string>();

            //textReturned = "Speaker 0    00:02    Also <inaudible> really? The applicants. Yeah. They don't even teach you how to do the sequences. Very true. Just was like the desks, like Mars. ";
            textReturned = textReturned.ToLower();
            textReturned = new string(textReturned.Where(c => !char.IsPunctuation(c)).ToArray());
            textReturned = textReturned.Replace("<inaudible>", "");
            foreach (Match match in Regex.Matches(textReturned, @"speaker.+\:*\d+\s+([\s\w]+)[\s*\n]*"))
            {
                for (int groupCtr = 1; groupCtr < match.Groups.Count; groupCtr++)
                {
                    Group group = match.Groups[groupCtr];
                    var transcription = group.Value;
                    var words = transcription.Split(" ");
                    allWords = allWords.Concat(words);
                }
            }

            return allWords;
        }

        public static string makeQuery(float x, float y, string speaker, string keyword)
        {
            return $@"IF EXISTS(SELECT * FROM WORDS_SPOKEN WHERE X={x} AND Y={y} AND (SPEAKER_NAME='' OR SPEAKER_NAME='{speaker}') AND KEYWORD='{keyword}')
                       UPDATE WORDS_SPOKEN SET FREQUENCY = FREQUENCY + 1 WHERE X = {x} AND Y = {y} AND(SPEAKER_NAME = '' OR SPEAKER_NAME = '{speaker}' AND KEYWORD = '{keyword}')
                    ELSE
                       INSERT INTO WORDS_SPOKEN values({x}, {y}, '{speaker}', '{keyword}', 0, 1); ";
        }
    }
}
