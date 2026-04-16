using System.Diagnostics;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
namespace Project;


public class Program
{
    public static void Main(string[] args)
    {
        PSDict.PopulateDict();
        while (true)
        {
            if(Globals.op_stack.Count == 0)
            {
                Console.Write("REPL<>"); // show empty stack prompt
            }
            else
            {
                Console.Write($"REPL<{Globals.op_stack.Count}>");
            }
            string input = Console.ReadLine();
            List<string> tokens = Tokenizer.Tokenize(input);
            foreach (string token in tokens)
            {
                if (token.ToLower() == "quit")
                {
                    Console.WriteLine("Exiting REPL...");
                    return;
                }
                else if (token.ToLower() == "static")
                {
                    Globals.static_scoping = true;
                    Console.WriteLine("Switched to static scoping mode");
                    continue;
                }
                else if (token.ToLower() == "dynamic")
                {
                    Globals.static_scoping = false;
                    Console.WriteLine("Switched to dynamic scoping mode");
                    continue;
                }
                InputProcessor.ProcessInput(token);

                Debug.WriteLine($"Current op stack: {string.Join(" ", Globals.op_stack.Select(DebugFormatter.Format))}");
            }
        }
    }
}

public class Globals
{
    public static List<object> op_stack = new List<object>();
    public static List<DictNode> dict_stack = new List<DictNode>();
    public static bool static_scoping = false;
    public static DictNode currentLexicalScope = null;
}

public class DictNode
{
    public Dictionary<string, Action> Data = new();
    public DictNode Parent;
    public DictNode(DictNode parent = null)
    {
        Parent = parent;
    }
    public void SetParent(DictNode parent)
    {
        Parent = parent;
    }
    public void Add(string key, Action value)
    {
        Data[key] = value;
    }
}

public static class PSDict
{
    public static void PopulateDict()
    {
        Globals.dict_stack.Add(new DictNode());
        Globals.dict_stack[0].Add("add", () => Add_Operation());
        Globals.dict_stack[0].Add("def", () => Def_Operation());
        Globals.dict_stack[0].Add("dict", () => Dict_Operation());
        Globals.dict_stack[0].Add("begin", () => Begin_Operation());
        Globals.dict_stack[0].Add("end", () => End_Operation());
        Globals.dict_stack[0].Add("exch", () => Exch_Operation());
        Globals.dict_stack[0].Add("=", () => EqualSignPrint_Operation());
        Globals.dict_stack[0].Add("copy", () => Copy_Operation());
        Globals.dict_stack[0].Add("dup", () => Dup_Operation());
        Globals.dict_stack[0].Add("clear", () => Clear_Operation());
        Globals.dict_stack[0].Add("count", () => Count_Operation());
        Globals.dict_stack[0].Add("sub", () => Sub_Operation());
        Globals.dict_stack[0].Add("mul", () => Mul_Operation());
        Globals.dict_stack[0].Add("div", () => Div_Operation());
        Globals.dict_stack[0].Add("idiv", () => Idiv_Operation());
        Globals.dict_stack[0].Add("mod", () => Mod_Operation());
        Globals.dict_stack[0].Add("abs", () => Abs_Operation());
        Globals.dict_stack[0].Add("neg", () => Neg_Operation());
        Globals.dict_stack[0].Add("ceiling", () => Ceiling_Operation());
        Globals.dict_stack[0].Add("round", () => Round_Operation());
        Globals.dict_stack[0].Add("sqrt", () => Sqrt_Operation());
        Globals.dict_stack[0].Add("floor", () => Floor_Operation());
        Globals.dict_stack[0].Add("length", () => Length_Operation());
        Globals.dict_stack[0].Add("maxLength", () => MaxLength_Operation());
        Globals.dict_stack[0].Add("get", () => Get_Operation());
        Globals.dict_stack[0].Add("getinterval", () => GetInterval_Operation());
        Globals.dict_stack[0].Add("putinterval", () => PutInterval_Operation());
        Globals.dict_stack[0].Add("eq", () => Eq_Operation());
        Globals.dict_stack[0].Add("ne", () => Ne_Operation());
        Globals.dict_stack[0].Add("ge", () => Ge_Operation());
        Globals.dict_stack[0].Add("gt", () => Gt_Operation());
        Globals.dict_stack[0].Add("le", () => Le_Operation());
        Globals.dict_stack[0].Add("lt", () => Lt_Operation());
        Globals.dict_stack[0].Add("and", () => And_Operation());
        Globals.dict_stack[0].Add("or", () => Or_Operation());
        Globals.dict_stack[0].Add("not", () => Not_Operation());
        Globals.dict_stack[0].Add("true", () => True_Operation());
        Globals.dict_stack[0].Add("false", () => False_Operation());
        Globals.dict_stack[0].Add("if", () => If_Operation());
        Globals.dict_stack[0].Add("ifelse", () => IfElse_Operation());
        Globals.dict_stack[0].Add("for", () => For_Operation());
        Globals.dict_stack[0].Add("repeat", () => Repeat_Operation());
        Globals.dict_stack[0].Add("print", () => Print_Operation());
        Globals.dict_stack[0].Add("==", () => PostScriptPrint_Operation());
    }

    public static void Add_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            if ((a is int || a is float || a is double) && (b is int || b is float || b is double))
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                double da = Convert.ToDouble(a);
                double db = Convert.ToDouble(b);

                Globals.op_stack.Add(da + db);
            }
            else
            {
                Console.WriteLine("Add operation requires two numbers");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to add operation");
        }
    }

    public static void Def_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object value = Globals.op_stack[Globals.op_stack.Count - 1];
            object name = Globals.op_stack[Globals.op_stack.Count - 2];
            if (name is string sname && sname.StartsWith("/"))
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                string key = sname.Substring(1); // remove leading '/'
                if (value is List<string> codeBlock)
                {
                    Globals.dict_stack[Globals.dict_stack.Count - 1].Add(key, () => InputProcessor.ExecuteBlock(codeBlock));
                }
                else
                {
                    Globals.dict_stack[Globals.dict_stack.Count - 1].Add(key, () => Globals.op_stack.Add(value));
                }
            }
            else
            {
                Console.WriteLine("Def operation requires a name starting with '/' and a value");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to def operation");
        }
    }
    public static void Dict_Operation()
    {
        DictNode new_dict = new DictNode();
        if (Globals.static_scoping)
        {
            new_dict.SetParent(Globals.dict_stack[Globals.dict_stack.Count - 1]);

        }
        Globals.op_stack.Add(new_dict);

    }
    public static void Begin_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object dict = Globals.op_stack[Globals.op_stack.Count - 1];
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            if (dict is DictNode user_dict)
            {
                Globals.dict_stack.Add(user_dict);
            }
            else
            {
                Console.WriteLine("Begin operation requires a dictionary on top of stack");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void End_Operation()
    {
        if (Globals.dict_stack.Count > 1)
        {
            Globals.dict_stack.RemoveAt(Globals.dict_stack.Count - 1);
        }
        else
        {
            Console.WriteLine("Cannot pop default dictionary");
        }
    }
    public static void Exch_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            object b = Globals.op_stack[Globals.op_stack.Count - 2];
            Globals.op_stack[Globals.op_stack.Count - 1] = b;
            Globals.op_stack[Globals.op_stack.Count - 2] = a;
        }
        else
        {
            Console.WriteLine("Not enough items on stack to exch operation");
        }
    }
    public static void EqualSignPrint_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object top = Globals.op_stack[Globals.op_stack.Count - 1];
            Console.WriteLine(top);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Copy_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object top = Globals.op_stack[Globals.op_stack.Count - 1];
            if (top is int n && n >= 0 && Globals.op_stack.Count >= n + 1)
            {
                var itemsToCopy = Globals.op_stack.Skip(Globals.op_stack.Count - n - 1).Take(n).ToList();
                Globals.op_stack.AddRange(itemsToCopy);
            }
            else
            {
                Console.WriteLine("Copy operation requires a non-negative integer on top of stack and enough items to copy");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Dup_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object top = Globals.op_stack[Globals.op_stack.Count - 1];
            Globals.op_stack.Add(top);
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Clear_Operation()
    {
        Globals.op_stack.Clear();
    }
    public static void Count_Operation()
    {
        Globals.op_stack.Add(Globals.op_stack.Count);
    }
    public static void Sub_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            if ((a is int || a is float || a is double) && (b is int || b is float || b is double))
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                double da = Convert.ToDouble(a);
                double db = Convert.ToDouble(b);

                Globals.op_stack.Add(da - db);
            }
            else
            {
                Console.WriteLine("Sub operation requires two integers or two floats");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to sub operation");
        }
    }
    public static void Mul_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            if ((a is int || a is float || a is double) && (b is int || b is float || b is double))
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                double da = Convert.ToDouble(a);
                double db = Convert.ToDouble(b);

                Globals.op_stack.Add(da * db);
            }
            else
            {
                Console.WriteLine("Mul operation requires two integers or two floats");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to mul operation");
        }
    }
    public static void Div_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            if ((a is int || a is float || a is double) && (b is int || b is float || b is double))
            {

                double ai = a switch
                {
                    int i => i,
                    float f => f,
                    double d => d,
                    _ => throw new Exception("Invalid number")
                };

                double bi = b switch
                {
                    int i => i,
                    float f => f,
                    double d => d,
                    _ => throw new Exception("Invalid number")
                };

                if (bi == 0 || ai == 0)
                {
                    Console.WriteLine("Division by zero");
                    return;
                }


                Globals.op_stack.Add(ai / bi);
            }
            else
            {
                Console.WriteLine("Division requires numeric types");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to div operation");
        }
    }
    public static void Idiv_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            if (a is int ai && b is int bi)
            {
                if (bi == 0)
                {
                    Console.WriteLine("Division by zero");
                    return;
                }
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.Add(ai / bi);
            }
            else
            {
                Console.WriteLine("Idiv operation requires two integers");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to idiv operation");
        }
    }
    public static void Mod_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            if (a is int ai && b is int bi)
            {
                if (bi == 0)
                {
                    Console.WriteLine("Division by zero");
                    return;
                }

                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.Add(ai % bi);
            }
            else
            {
                Console.WriteLine("Mod operation requires two integers");
            }
        }
    }
    public static void Abs_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Abs(ai);
            }
            else if (a is float af)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Abs(af);
            }
            else if (a is double ad)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Abs(ad);
            }
            else
            {
                Console.WriteLine("Abs operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Neg_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = -ai;
            }
            else if (a is float af)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = -af;
            }
            else if (a is double ad)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = -ad;
            }
            else
            {
                Console.WriteLine("Neg operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Ceiling_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = ai; // ceiling of int is itself
            }
            else if (a is float af)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Ceiling(af);
            }
            else if (a is double ad)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Ceiling(ad);
            }
            else
            {
                Console.WriteLine("Ceiling operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Round_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = ai; // round of int is itself
            }
            else if (a is float af)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Round(af);
            }
            else if (a is double ad)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Round(ad);
            }
            else
            {
                Console.WriteLine("Round operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Sqrt_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                if (ai < 0)
                {
                    Console.WriteLine("Cannot take square root of negative number");
                    return;
                }

                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Sqrt(ai);
            }
            else if (a is float af)
            {
                if (af < 0)
                {
                    Console.WriteLine("Cannot take square root of negative number");
                    return;
                }
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Sqrt(af);
            }
            else if (a is double ad)
            {
                if (ad < 0)
                {
                    Console.WriteLine("Cannot take square root of negative number");
                    return;
                }
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Sqrt(ad);
            }
            else
            {
                Console.WriteLine("Sqrt operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Floor_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object a = Globals.op_stack[Globals.op_stack.Count - 1];
            if (a is int ai)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = ai; // floor of int is itself
            }
            else if (a is float af)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Floor(af);
            }
            else if (a is double ad)
            {
                Globals.op_stack[Globals.op_stack.Count - 1] = Math.Floor(ad);
            }
            else
            {
                Console.WriteLine("Round operation requires a numeric type");
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void Length_Operation()
    {

        if (Globals.op_stack.Count >= 1)
        {
            if (Globals.op_stack[Globals.op_stack.Count - 1] is string)
            {
                string s = (string)Globals.op_stack[Globals.op_stack.Count - 1];
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.Add(s.Length);
                return;
            }
            else
            {

                if (Globals.dict_stack.Count > 1)
                {
                    int count = 0;
                    foreach (Action key in Globals.dict_stack[Globals.dict_stack.Count - 1].Data.Values)
                    {
                        count++;
                    }
                    Globals.op_stack.Add(count);
                }
                else
                {
                    Console.WriteLine("Invalid input, create a dictionary or call this command with a string!");
                    return;
                }
            }
        }
        

    }
    public static void MaxLength_Operation()
    {
        if (Globals.dict_stack.Count > 1)
        {

            Globals.op_stack.Add(Int32.MaxValue);
        }
        else
        {
            Console.WriteLine("There are no user defined dictionaries!");
            return;
        }
    }
    public static void Get_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object index = Globals.op_stack[Globals.op_stack.Count - 1];
            object str = Globals.op_stack[Globals.op_stack.Count - 2];
            if (index is int)
            {
                if (str is string)
                {
                    //we also need to check the string bounds for the index
                    if ((int)index > ((string)str).Length - 1 || (int)index < 0)
                    {
                        Console.WriteLine("Index out of bounds for get operation");
                        return;
                    }
                    else
                    {
                        char c = ((string)str)[(int)index];
                        Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                        Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                        Globals.op_stack.Add((int)c);
                        return;
                    }


                }
                else
                {
                    Console.WriteLine("Get operation requires a string below the index on stack");
                }
            }
            else
            {
                Console.WriteLine("Get operation requires an integer index on top of stack");
            }
        }
        else
        {
            Console.WriteLine("get requires a string and an index!");
        }
    }
    public static void GetInterval_Operation()
    {
        if (Globals.op_stack.Count >= 3)
        {
            object count = Globals.op_stack[Globals.op_stack.Count - 1];
            object index = Globals.op_stack[Globals.op_stack.Count - 2];
            object str = Globals.op_stack[Globals.op_stack.Count - 3];
            if (count is int)
            {
                if (index is int)
                {
                    if (str is string)
                    {
                        //we also need to check the string bounds for the index and count
                        if ((int)index > ((string)str).Length - 1 || (int)index < 0 || (int)count < 0 || (int)index + (int)count > ((string)str).Length)
                        {
                            Console.WriteLine("Index or count out of bounds for getinterval operation");
                            return;
                        }
                        else
                        {
                            string substr = ((string)str).Substring((int)index, (int)count);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.Add(substr);
                            return;
                        }
                    }
                }
            }
        }
    }
    public static void PutInterval_Operation()
    {
        if (Globals.op_stack.Count >= 3)
        {
            object substr = Globals.op_stack[Globals.op_stack.Count - 1];
            object index = Globals.op_stack[Globals.op_stack.Count - 2];
            object str = Globals.op_stack[Globals.op_stack.Count - 3];
            if (substr is string)
            {
                if (index is int)
                {
                    if (str is string)
                    {
                        //we also need to check the string bounds for the index and substr length
                        if ((int)index > ((string)str).Length - 1 || (int)index < 0 || (int)index + ((string)substr).Length > ((string)str).Length)
                        {
                            Console.WriteLine("Index or substring length out of bounds for putinterval operation");
                            return;
                        }
                        else
                        {
                            string result = ((string)str).Remove((int)index, ((string)substr).Length).Insert((int)index, (string)substr);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                            Globals.op_stack.Add(result);
                            return;
                        }
                    }
                }
            }
        }
    }
    public static void Eq_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(a.Equals(b));
        }
        else
        {
            Console.WriteLine("Not enough items on stack to eq operation");
        }
    }
    public static void Ne_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object b = Globals.op_stack[Globals.op_stack.Count - 1];
            object a = Globals.op_stack[Globals.op_stack.Count - 2];
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(!a.Equals(b));
        }
        else
        {
            Console.WriteLine("Not enough items on stack to ne operation");
        }
    }

    //helper function i just slapped in here 
    private static bool IsNumber(object obj)
    {
        return obj is int || obj is float || obj is double;
    }

    public static void Ge_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for ge");
            return;
        }

        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];

        // Numbers
        if (IsNumber(a) && IsNumber(b))
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);

            Globals.op_stack.Add(da >= db);
            return;
        }

        // Strings
        if (a is string sa && b is string sb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            // Lexicographic comparison (PostScript style)
            Globals.op_stack.Add(string.Compare(sa, sb) >= 0);
            return;
        }

        Console.WriteLine("ge requires two numbers or two strings");
    }
    public static void Gt_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for ge");
            return;
        }

        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];

        // Numbers
        if (IsNumber(a) && IsNumber(b))
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);

            Globals.op_stack.Add(da > db);
            return;
        }

        // Strings
        if (a is string sa && b is string sb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            // Lexicographic comparison (PostScript style)
            Globals.op_stack.Add(string.Compare(sa, sb) < 0);
            return;
        }

        Console.WriteLine("gt requires two numbers or two strings");
    }
    public static void Le_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for ge");
            return;
        }

        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];

        // Numbers
        if (IsNumber(a) && IsNumber(b))
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);

            Globals.op_stack.Add(da <= db);
            return;
        }

        // Strings
        if (a is string sa && b is string sb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);

            // Lexicographic comparison (PostScript style)
            Globals.op_stack.Add(string.Compare(sa, sb) >= 0);
            return;
        }

        Console.WriteLine("le requires two numbers or two strings");
    }
    public static void Lt_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for ge");
            return;
        }
        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];
        // Numbers
        if (IsNumber(a) && IsNumber(b))
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);
            Globals.op_stack.Add(da < db);
            return;
        }
        // Strings
        if (a is string sa && b is string sb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            // Lexicographic comparison (PostScript style)
            Globals.op_stack.Add(string.Compare(sa, sb) < 0);
            return;
        }
        Console.WriteLine("lt requires two numbers or two strings");
    }
    public static void And_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for and");
            return;
        }

        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];

        // Boolean AND
        if (a is bool ba && b is bool bb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(ba && bb);
            return;
        }

        // Integer bitwise AND
        if (a is int ai && b is int bi)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(ai & bi);
            return;
        }

        Console.WriteLine("and requires two booleans or two integers");
    }
    public static void Or_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items on stack for or");
            return;
        }
        object b = Globals.op_stack[^1];
        object a = Globals.op_stack[^2];
        // Boolean OR
        if (a is bool ba && b is bool bb)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(ba || bb);
            return;
        }
        // Integer bitwise OR
        if (a is int ai && b is int bi)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(ai | bi);
            return;
        }
        Console.WriteLine("or requires two booleans or two integers");
    }
    public static void Not_Operation()
    {
        if (Globals.op_stack.Count < 1)
        {
            Console.WriteLine("Not enough items on stack for not");
            return;
        }
        object a = Globals.op_stack[^1];
        // Boolean NOT
        if (a is bool ba)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(!ba);
            return;
        }
        // Integer bitwise NOT
        if (a is int ai)
        {
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
            Globals.op_stack.Add(~ai);
            return;
        }
        Console.WriteLine("not requires a boolean or an integer");
    }
    public static void True_Operation()
    {
        Globals.op_stack.Add(true);
    }
    public static void False_Operation()
    {
        Globals.op_stack.Add(false);
    }
    public static void If_Operation()
    {
        if (Globals.op_stack.Count >= 2)
        {
            object codeBlock = Globals.op_stack[Globals.op_stack.Count - 1];
            object condition = Globals.op_stack[Globals.op_stack.Count - 2];
            if (codeBlock is List<string> && condition is bool)
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                if ((bool)condition)
                {
                    InputProcessor.ExecuteBlock((List<string>)codeBlock);
                }
            }
            else
            {
                Console.WriteLine("if operation requires a code block on top of stack and a boolean below it");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to if operation");
        }
    }
    public static void IfElse_Operation()
    {
        if (Globals.op_stack.Count >= 3)
        {
            object elseBlock = Globals.op_stack[Globals.op_stack.Count - 1];
            object ifBlock = Globals.op_stack[Globals.op_stack.Count - 2];
            object condition = Globals.op_stack[Globals.op_stack.Count - 3];
            if (elseBlock is List<string> && ifBlock is List<string> && condition is bool)
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                if ((bool)condition)
                {
                    InputProcessor.ExecuteBlock((List<string>)ifBlock);
                }
                else
                {
                    InputProcessor.ExecuteBlock((List<string>)elseBlock);
                }
            }
            else
            {
                Console.WriteLine("ifelse operation requires two code blocks on top of stack and a boolean below them");
            }
        }
        else
        {
            Console.WriteLine("Not enough items on stack to ifelse operation");
        }
    }
    public static void For_Operation()
    {
        if (Globals.op_stack.Count < 4)
        {
            Console.WriteLine("Not enough items for for loop");
            return;
        }

        object codeBlock = Globals.op_stack[^1];
        object end = Globals.op_stack[^2];
        object increment = Globals.op_stack[^3];
        object start = Globals.op_stack[^4];

        Globals.op_stack.RemoveRange(Globals.op_stack.Count - 4, 4);

        if (codeBlock is List<string> block &&
            increment is int inc &&
            end is int e &&
            start is int s)
        {
            if (inc == 0)
            {
                Console.WriteLine("Increment cannot be zero");
                return;
            }

            if (inc > 0)
            {
                for (int i = s; i <= e; i += inc)
                {
                    Globals.op_stack.Add(i);
                    InputProcessor.ExecuteBlock(block);
                }
            }
            else
            {
                for (int i = s; i >= e; i += inc)
                {
                    Globals.op_stack.Add(i);
                    InputProcessor.ExecuteBlock(block);
                }
            }

            return;
        }

        Console.WriteLine("Invalid types for for loop");
    }
    public static void Repeat_Operation()
    {
        if (Globals.op_stack.Count < 2)
        {
            Console.WriteLine("Not enough items for repeat loop");
            return;
        }
        object codeBlock = Globals.op_stack[^1];
        object count = Globals.op_stack[^2];
        if (codeBlock is List<string> block && count is int c)
        {
            if (c < 0)
            {
                Console.WriteLine("Count cannot be negative");
                return;
            }
            Globals.op_stack.RemoveRange(Globals.op_stack.Count - 2, 2);
            for (int i = 0; i < c; i++)
            {
                InputProcessor.ExecuteBlock(block);
            }
            return;
        }
        Console.WriteLine("Invalid types for repeat loop");
    }
    public static void Print_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object top = Globals.op_stack[Globals.op_stack.Count - 1];
            if (top is string s)
            {
                Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                Console.WriteLine(s);
            }
            else
            {
                Console.WriteLine("print can only be used on strings");
                return;
            }
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
    public static void PostScriptPrint_Operation()
    {
        if (Globals.op_stack.Count >= 1)
        {
            object top = Globals.op_stack[Globals.op_stack.Count - 1];
            Console.WriteLine(DebugFormatter.PostScriptFormat(top));
            Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
        }
        else
        {
            Console.WriteLine("Stack is empty!");
        }
    }
}
public static class InputProcessor
{
    private static Parsers parser = new Parsers(new IParser[] { new BoolParser(), new NumParser(), new NameParser(), new ArrayParser(), new StringParser(), new CodeBlockParser() });

    //Helper function for get variables when trying to resolve array vars
    public static bool Get_Var(string token)
    {
        if (!Globals.static_scoping)
        {
            foreach (DictNode dict in Globals.dict_stack.AsEnumerable().Reverse())
            {
                if (dict.Data.ContainsKey(token))
                {
                    dict.Data[token]();
                    return true;
                }
            }
            return false;
        }
        else
        {
            DictNode currentScope = Globals.dict_stack[Globals.dict_stack.Count - 1];
            while (currentScope != null)
            {
                if (currentScope.Data.ContainsKey(token))
                {
                    currentScope.Data[token]();
                    return true;
                }
                currentScope = currentScope.Parent;
            }
            return false; ;
        }
    }
    public static void Dictionary_Lookup(string token)
    {
        if (!Globals.static_scoping)
        {
            foreach (DictNode dict in Globals.dict_stack.AsEnumerable().Reverse())
            {
                if (dict.Data.ContainsKey(token))
                {
                    dict.Data[token]();
                    Debug.WriteLine("Found operation in dictionary");
                    return;
                }
            }
            Debug.WriteLine("Couldn't find operation in dict");
            Console.WriteLine("Unknown command");
        }
        else
        {
            DictNode currentScope = Globals.dict_stack[Globals.dict_stack.Count - 1];
            while (currentScope != null)
            {
                if (currentScope.Data.ContainsKey(token))
                {
                    currentScope.Data[token]();
                    Debug.WriteLine("Found operation in dictionary");
                    return;
                }
                currentScope = currentScope.Parent;
            }
            Debug.WriteLine("Couldn't find operation in dict");
            Console.WriteLine("Unknown command");
        }
    }
    public static void ProcessInput(string token)
    {

        //try to parse as constatn
        object res = parser.Parse(token);
        if (res != null)
        {
            Globals.op_stack.Add(res);
        }
        else
        {
            Dictionary_Lookup(token);
        }
    }
    public static void ExecuteBlock(List<string> tokens)
    {
        foreach (string token in tokens)
        {
            InputProcessor.ProcessInput(token);
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
    public static string PostScriptFormat(object obj)
    {
        if (obj == null) return "null";

        
        if (obj is string s)
            return s;

        if (obj is IEnumerable enumerable)
        {
            var items = enumerable.Cast<object>().ToList();

            // Check if this is a list of strings
            bool isStringList = items.All(x => x is string);

            var formattedItems = items.Select(PostScriptFormat);

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
        return null;
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
        if (token.ToLower()[0] != '/')
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
        token = token.Trim();

        if (!token.StartsWith("[") || !token.EndsWith("]"))
        {
            result = null;
            return false;
        }

        string inner = token.Substring(1, token.Length - 2).Trim();

        if (string.IsNullOrWhiteSpace(inner))
        {
            result = new List<object>();
            return true;
        }

        string[] parts = inner.Split(
            new[] { ' ' },
            StringSplitOptions.RemoveEmptyEntries
        );

        List<object> list = new List<object>();

        foreach (var part in parts)
        {
            if (int.TryParse(part, out int i))
                list.Add(i);

            else if (double.TryParse(part, out double d))
                list.Add(d);

            else if (bool.TryParse(part, out bool b))
                list.Add(b);

            else
            {
                //we basically need to check if the input is a variable in the dictionary
                //Get_Var will call the def operation and automatically add the value to the stack.
                if(InputProcessor.Get_Var(part))
                {
                    //add the newly added value
                    list.Add(Globals.op_stack[Globals.op_stack.Count - 1]);
                    //remove it from the stack
                    Globals.op_stack.RemoveAt(Globals.op_stack.Count - 1);
                }
                else
                {
                    list.Add(part);
                }

            }
                
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
        token = token.Trim();

        if (!token.StartsWith("{") || !token.EndsWith("}"))
        {
            result = null;
            return false;
        }

        string inner = token.Substring(1, token.Length - 2).Trim();

        result = ParseBlock(inner);
        return true;
    }

    private List<string> ParseBlock(string input)
    {
        List<string> tokens = new();
        int i = 0;

        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                i++;
                continue;
            }

            if (input[i] == '{')
            {
                int start = i;
                int depth = 1;
                i++;

                while (i < input.Length && depth > 0)
                {
                    if (input[i] == '{') depth++;
                    else if (input[i] == '}') depth--;
                    i++;
                }

                // keep it as a STRING (important for your model)
                tokens.Add(input.Substring(start, i - start));
            }
            else
            {
                int start = i;

                while (i < input.Length &&
                       !char.IsWhiteSpace(input[i]) &&
                       input[i] != '{' &&
                       input[i] != '}')
                {
                    i++;
                }

                tokens.Add(input.Substring(start, i - start));
            }
        }

        return tokens;
    }
}

public interface IParser
{
    bool TryParse(string token, out object result);
}
