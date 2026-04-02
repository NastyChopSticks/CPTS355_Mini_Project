using System.Diagnostics;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
namespace Project;


public class Program
{
    public static void Main(string[] args)
    {
        
        List<object> op_stack = new List<object>();
        Parsers parser = new Parsers(new IParser[] { new BoolParser(), new NumParser(), new NameParser(), new ArrayParser(), new StringParser(), new CodeBlockParser() });
        while (true)
        {
            Console.Write("REPL>");
            string input = Console.ReadLine();
            List<string> tokens = Tokenizer.Tokenize(input);
            foreach (string token in tokens)
            {
                if (token.ToLower() == "quit")
                {
                    Console.WriteLine("Exiting REPL...");
                    return;
                }
                object res = parser.Parse(token);
                op_stack.Add(res);
                Debug.WriteLine($"Parsed token: {token} -> {res}");
                Debug.WriteLine($"Current op stack: {string.Join(" ", op_stack.Select(DebugFormatter.Format))}");
                Console.WriteLine(token.ToLower());
            }
        }
    }
}

public static class DebugFormatter
{
    public static string Format(object obj)
    {
        if (obj == null) return "null";

        // Strings should not be treated as IEnumerable
        if (obj is string s)
            return $"\"{s}\"";

        if (obj is IEnumerable enumerable)
        {
            var items = enumerable.Cast<object>().ToList();

            // Check if this is a list of strings
            bool isStringList = items.All(x => x is string);

            var formattedItems = items.Select(Format);

            string inner = string.Join(" ", formattedItems);

            if (isStringList)
                return "{" + inner + "}";   // string list
            else
                return "[" + inner + "]";   // other lists
        }

        return obj.ToString();
    }
}

public class Parsers
{
    private readonly IEnumerable<IParser> _parsers;

    public Parsers(IEnumerable<IParser> parsers)
    {
        _parsers = parsers;
    }
    public object Parse(string token)
    {
        
        foreach (var parser in _parsers)
        {
            if (parser.TryParse(token, out object result))
            {
                return result;
            }
        }
        throw new Exception($"Unable to parse token: {token}");
    }
}
    public static class Tokenizer
{
    public static char Matching(char open)
    {
        return open switch
        {
            '[' => ']',
            '{' => '}',
            '(' => ')',
            _ => throw new ArgumentException("Invalid opening delimiter")
        };
    }
    public static List<string> Tokenize(string input)
    {
        List<string> tokens = new List<string>();
        int i = 0;

        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                i++;
                continue;
            }

            char c = input[i];

            // Handle grouped tokens
            if (c == '[' || c == '{' || c == '(')
            {
                char open = c;
                char close = Matching(c);

                int start = i;
                int depth = 1;
                i++;

                while (i < input.Length && depth > 0)
                {
                    if (input[i] == open) depth++;
                    else if (input[i] == close) depth--;
                    i++;
                }

                string token = input.Substring(start, i - start);
                tokens.Add(token);
            }
            else
            {
                // Normal token
                int start = i;

                while (i < input.Length && !char.IsWhiteSpace(input[i]))
                    i++;

                tokens.Add(input.Substring(start, i - start));
            }
        }

        return tokens;
    }
}
    public class BoolParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to bool parse token: " + token);
        if (token.ToLower() == "true")
        {
            result = true;
            return true;
        }
        else if (token.ToLower() == "false")
        {
            result = false;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }
}

public class NumParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to number parse token: " + token);

        // Try int first
        if (int.TryParse(token, out int intValue))
        {
            result = intValue;
            return true;
        }

        // Then try double
        if (double.TryParse(token, out double doubleValue))
        {
            result = doubleValue;
            return true;
        }

        result = null;
        return false;
    }
}

public class NameParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to parse constant token: " + token);
        if (token.ToLower()[0] != '\\')
        {
            result = null;
            return false;
        }
        result = token.ToLower();
        return true;
    }
}

public class ArrayParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to array parse token: " + token);

        token = token.Trim();

        if (!token.StartsWith("[") || !token.EndsWith("]"))
        {
            result = null;
            return false;
        }

        // Remove [ and ]
        string inner = token.Substring(1, token.Length - 2).Trim();

        // Handle empty array
        if (string.IsNullOrWhiteSpace(inner))
        {
            result = new List<object>();
            return true;
        }

        // Split by whitespace
        string[] parts = inner.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        List<object> list = new List<object>();

        foreach (var part in parts)
        {
            list.Add(part); // store raw tokens for now
        }

        result = list;
        return true;
    }
}

public class StringParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to string parse token: " + token);

        token = token.Trim();

        if (!token.StartsWith("(") || !token.EndsWith(")"))
        {
            result = null;
            return false;
        }

        // Remove ( and )
        string inner = token.Substring(1, token.Length - 2);

        result = inner;
        return true;
    }
}

public class CodeBlockParser : IParser
{
    public bool TryParse(string token, out object result)
    {
        Debug.WriteLine("Attempting to code block parse token: " + token);

        token = token.Trim();

        if (!token.StartsWith("{") || !token.EndsWith("}"))
        {
            result = null;
            return false;
        }

        // Remove { and }
        string inner = token.Substring(1, token.Length - 2).Trim();

        // Handle empty code block
        if (string.IsNullOrWhiteSpace(inner))
        {
            result = new List<string>();
            return true;
        }

        // Split into tokens
        string[] parts = inner.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        result = new List<string>(parts);
        return true;
    }
}


public interface IParser
{
    bool TryParse(string token, out object result);
}
