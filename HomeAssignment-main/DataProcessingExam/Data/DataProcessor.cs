using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace DataProcessingExam
{
    public class DataProcessorService
    {
        private eServerState m_state;
        private Stopwatch stopwatch;
        private readonly ILogger<DataProcessorService> logger;
        private readonly FileConfiguration config;
        private List<WordAnalysis> wordAnalyses;
        public ConcurrentDictionary<char,int> LetterCounter = new ConcurrentDictionary<char,int>();
        public DataProcessorService(ILogger<DataProcessorService> logger, IOptions<FileConfiguration> config)
        {
            this.logger = logger;
            this.config = config.Value;
             
        }

        public event EventHandler stateHasChanged;

        public async Task<List<WordAnalysis>> ProcessData()
        {
            if (m_state != eServerState.Completed)
            {
                FillDataWithLetters();

                m_state = eServerState.Running;
                stopwatch = Stopwatch.StartNew();

                wordAnalyses = await ProcessFile();

                stopwatch.Stop();
                m_state = eServerState.Completed;
            }
            return wordAnalyses;
        }

        private void FillDataWithLetters()
        {
            LetterCounter.TryAdd('a', 0);
            LetterCounter.TryAdd('b', 0);
            LetterCounter.TryAdd('c', 0);
            LetterCounter.TryAdd('d', 0);
            LetterCounter.TryAdd('e', 0);
            LetterCounter.TryAdd('f', 0);
            LetterCounter.TryAdd('g', 0);
            LetterCounter.TryAdd('h', 0);
            LetterCounter.TryAdd('i', 0);
            LetterCounter.TryAdd('j', 0);
            LetterCounter.TryAdd('k', 0);
            LetterCounter.TryAdd('l', 0);
            LetterCounter.TryAdd('m', 0);
            LetterCounter.TryAdd('n', 0);
            LetterCounter.TryAdd('o', 0);
            LetterCounter.TryAdd('p', 0);
            LetterCounter.TryAdd('q', 0);
            LetterCounter.TryAdd('r', 0);
            LetterCounter.TryAdd('s', 0);
            LetterCounter.TryAdd('t', 0);
            LetterCounter.TryAdd('u', 0);
            LetterCounter.TryAdd('v', 0);
            LetterCounter.TryAdd('w', 0);
            LetterCounter.TryAdd('x', 0);
            LetterCounter.TryAdd('y', 0);
            LetterCounter.TryAdd('z', 0);
            LetterCounter.OrderBy(lc => lc.Key);
        }

        public List<WordAnalysis> GetLastResult => wordAnalyses;
       
        private async Task<List<WordAnalysis>> ProcessFile()
        {
            config.UseTestFile = true;
            logger.LogInformation($"Initiating process {config.GetFileForProcessing()}");
            //TODO: Implement!
            
            //await Task.Delay(TimeSpan.FromSeconds(5)); //Simulates the process time

            logger.LogInformation($"Process completed {config.GetFileForProcessing()}. Execution time {stopwatch.Elapsed}");

            return await AnalysesFile(config.GetFileForProcessing());

            
            //return new List<WordAnalysis> {
            //    new WordAnalysis { Letter = 'B', NumberOfOccurrences = 22 },
            //    new WordAnalysis { Letter = 'A', NumberOfOccurrences = 50 }
            //};
        }

        public ServerState GeteServerState() => new ServerState
        {
            ProcessTime = stopwatch?.Elapsed ?? TimeSpan.Zero,
            State = m_state,
        };

        public async Task<List<WordAnalysis>> AnalysesFile(string file)
        {
            List<WordAnalysis> wa = new List<WordAnalysis>();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                int counter = 0;
                using (StreamReader sr = new StreamReader(file))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {

                        AnalysesLine(line);
                        counter++;
                        if (counter >= 10000)
                        {
                            stateHasChanged?.Invoke(this,null);
                            counter = 0;
                        }

                    }
                }
            }
            
            return wa;
        }       

        private void AnalysesLine(string line)
        {           
            foreach (var c in line)
            {
                if (char.IsLetter(c))
                {
                    var lowerLetter = char.ToLower(c);
                    if (!LetterCounter.ContainsKey(lowerLetter))
                    {
                        LetterCounter.TryAdd(lowerLetter, 1);
                    }
                    else
                    {
                        var counter = LetterCounter[lowerLetter];
                        LetterCounter[lowerLetter] = counter + 1;
                    }
                }
            }          
        }
    
    }
}
