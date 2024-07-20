using System.Text.RegularExpressions;
using WordCounter.Domain.WordSource;

namespace WordCounter.Implementation.WordSource;

public class FileWordSource : IWordSource
{
    private readonly FileWordSourceOption _wordSourceOption;
    private readonly StreamReader _streamReader;
    private readonly FileStream _fileStream;
    private readonly Regex _regex = new(@"\b[a-zA-Zа-яА-ЯёЁ]+\b");

    private string _lastWord = "";
    
    public FileWordSource(FileWordSourceOption wordSourceOption, FileStream fileStream)
    {
        _wordSourceOption = wordSourceOption;
        _fileStream = fileStream;
        _streamReader = new StreamReader(_fileStream);
    }
    
    public async Task<IEnumerable<string>> GetNextBatch(CancellationToken cancellationToken)
    {
        var wordCollection = new List<string>();
        
        // Если в строке последний символ не разделитель
        // То мы не знаем, прочитано ли слово полностью
        // Поэтому последнее слово хранится в отдельном буфере
        // И добавляется из буфера в список слов только тогда, когда мы уверены,
        // что считали его полностью 
        
        while (!_streamReader.EndOfStream)
        {
            var wordLines = _lastWord + ReadBuffer();
            var lastSymbolIsSeparator = wordLines.Last() is ' ' or '\n' or '\r';
            
            var words = SplitWords(wordLines);

            if (!words.Any())
                break;
            
            _lastWord = RemoveAndReturnLastWord(words);
            if (lastSymbolIsSeparator)
            {
                AddIfNotNull(wordCollection, _lastWord);
                _lastWord = "";
            }
            
            // Фактически, мы можем вернуть не BatchSize, а больше,
            // если у нас маленькие слова и большой BufferSize
            // но мне не кажется это проблемой,
            // т.к. при адекватном значении BufferSize мы не будем потреблять сильно много памяти 
            wordCollection.AddRange(words);
            if (wordCollection.Count >= BatchSize)
                return wordCollection;
        }

        
        AddIfNotNull(wordCollection, _lastWord);
        return wordCollection;
    }

    private string ReadBuffer()
    {
        var buffer = new char[BufferSize];
        _streamReader.ReadBlock(buffer);
        return new string(buffer);
    }

    private List<string> SplitWords(string wordLines)
    {
        return _regex.Matches(wordLines).Select(match => match.Value).ToList();
    }

    private string RemoveAndReturnLastWord(List<string> words)
    {
        var lastWord = words.Last();
        words.RemoveAt(words.Count - 1);
        return lastWord;
    }

    private void AddIfNotNull(List<string> words, string word)
    {
        if(string.IsNullOrWhiteSpace(word))
            return;
        
        words.Add(_lastWord);
    }
    
    private int BufferSize => _wordSourceOption.CountSymbolRead;

    private int BatchSize => _wordSourceOption.WordsInBatch;
    
    public void Dispose()
    {
        _streamReader.Dispose();
        _fileStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _streamReader.Dispose();
        await _fileStream.DisposeAsync();
    }
}