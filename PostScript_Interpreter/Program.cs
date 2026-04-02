using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
namespace Project;
public class Program
{
    public static void Main(string[] args)
    {
        while(true)
        {
            Console.Write("REPL>");
            string input = Console.ReadLine();
            string[] tokens = input.Split(' ');
            foreach (string token in tokens)
            {
                if (token.ToLower() == "quit")
                {
                    Console.WriteLine("Exiting REPL...");
                    return;
                }
                Console.WriteLine(token.ToLower());
            }
        }
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
