using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Project;


namespace mini_project_tests
{
    public class ParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        //BOOL PARSER TESTS
        [Test]
        public void BoolParser_ValidTrue()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new BoolParser();
            bool success = parser.TryParse("true", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BoolParser_ValidFalse()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new BoolParser();
            bool success = parser.TryParse("false", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void BoolParser_Invalid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new BoolParser();
            bool success = parser.TryParse("hello", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //BOOL PARSER TESTS


        //NUM PARSER TESTS
        [Test]
        public void NumParser_Int()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new NumParser();
            bool success = parser.TryParse("42", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void NumParser_Double()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new NumParser();
            bool success = parser.TryParse("3.14", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(3.14, result);
        }

        [Test]
        public void NumParser_Invalid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new NumParser();
            bool success = parser.TryParse("abc", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [Test]
        public void NameParser_Valid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new NameParser();
            bool success = parser.TryParse("/hello", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual("/hello", result);
        }

        [Test]
        public void NameParser_Invalid_NoBackslash()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new NameParser();
            bool success = parser.TryParse("hello", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //NUM PARSER TESTS


        //STRING PARSER TESTS
        [Test]
        public void StringParser_Valid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new StringParser();
            bool success = parser.TryParse("(Hello World)", currentScope, out object result);

            Assert.IsTrue(success);
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void StringParser_Invalid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new StringParser();
            bool success = parser.TryParse("Hello World", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //STRING PARSER TESTS

        //ARRAY PARSER TESTS
        [Test]
        public void ArrayParser_Valid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new ArrayParser();
            bool success = parser.TryParse("[1 2 3]", currentScope, out object result);

            Assert.IsTrue(success);

            var list = result as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        [Test]
        public void ArrayParser_Empty()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new ArrayParser();
            bool success = parser.TryParse("[]", currentScope, out object result);

            Assert.IsTrue(success);

            var list = result as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ArrayParser_Invalid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new ArrayParser();
            bool success = parser.TryParse("1 2 3", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //ARRAY PARSER TESTS

        //CODE BLOCK PARSER TESTS
        [Test]
        public void CodeBlockParser_Valid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new CodeBlockParser();
            bool success = parser.TryParse("{1 2 add}", currentScope, out object result);

            Assert.IsTrue(success);

            var list = result as List<string>;
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("1", list[0]);
            Assert.AreEqual("2", list[1]);
            Assert.AreEqual("add", list[2]);
        }

        [Test]
        public void CodeBlockParser_Invalid()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            var parser = new CodeBlockParser();
            bool success = parser.TryParse("1 2 add", currentScope, out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //CODE BLOCK PARSER TESTS
    }

    public class TokenizerTests
    {
        private void AssertTokens(string input, List<string> expected)
        {
            var result = Tokenizer.Tokenize(input);
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void SimpleTokens()
        {
            AssertTokens("1 2 add", new List<string>
            {
                "1", "2", "add"
            });
        }

        [Test]
        public void ArrayToken()
        {
            AssertTokens("[1 2 3]", new List<string>
            {
                "[1 2 3]"
            });
        }

        [Test]
        public void ProcedureToken()
        {
            AssertTokens("{ 1 2 add }", new List<string>
            {
                "{ 1 2 add }"
            });
        }

        [Test]
        public void StringToken()
        {
            AssertTokens("(Hello, world)", new List<string>
            {
                "(Hello, world)"
            });
        }

        [Test]
        public void MixedTokens()
        {
            AssertTokens("1 [2 3] { add } (hi)", new List<string>
            {
                "1", "[2 3]", "{ add }", "(hi)"
            });
        }

        [Test]
        public void NestedArray()
        {
            AssertTokens("[1 [2 3] 4]", new List<string>
            {
                "[1 [2 3] 4]"
            });
        }

        [Test]
        public void NestedProcedure()
        {
            AssertTokens("{1 {2 add} 3}", new List<string>
            {
                "{1 {2 add} 3}"
            });
        }
        [Test]
        public void ExtraWhitespace()
        {
            AssertTokens("   1    2     add   ", new List<string>
            {
                "1", "2", "add"
            });
        }

        [Test]
        public void EmptyInput()
        {
            AssertTokens("", new List<string>());
        }

        [Test]
        public void SingleToken()
        {
            AssertTokens("add", new List<string>
            {
                "add"
            });
        }

        [Test]
        public void StringWithSpaces()
        {
            AssertTokens("(Hello world test)", new List<string>
            {
                "(Hello world test)"
            });
        }

    }


    public class OperationTests
    {
        private StringWriter consoleOutput;
        private TextWriter originalOutput;

        [SetUp]
        public void Setup()
        {
            Globals.op_stack = new List<object>();
            Globals.dict_stack = new List<DictNode>();

            PSDict.PopulateDict();

            originalOutput = Console.Out;
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalOutput);
            consoleOutput.Dispose();
        }

        // ================= ADD =================

        [Test]
        public void Add_Ints_PushesSum()
        {
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(3);

            PSDict.Add_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(5, Globals.op_stack[0]);
        }

        [Test]
        public void Add_Floats_PushesSum()
        {
            Globals.op_stack.Add(2.5f);
            Globals.op_stack.Add(1.5f);

            PSDict.Add_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(4f, Globals.op_stack[0]);
        }

        [Test]
        public void Add_TooFewItems_LeavesStackUnchanged()
        {
            Globals.op_stack.Add(1);

            PSDict.Add_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(1, Globals.op_stack[0]);
        }

        [Test]
        public void Add_MixedTypes()
        {
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2.5f);

            PSDict.Add_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(3.5, Globals.op_stack[0]);
        }

        // ================= DEF =================

        [Test]
        public void Def_ValidDefinition_AddsValue()
        {
            Globals.op_stack.Add("/x");
            Globals.op_stack.Add(42);

            PSDict.Def_Operation(Globals.dict_stack[^1]);

            Assert.IsEmpty(Globals.op_stack);
            Assert.IsTrue(Globals.dict_stack[^1].Data.ContainsKey("x"));
        }

        [Test]
        public void Def_FunctionPushesValue()
        {
            Globals.op_stack.Add("/x");
            Globals.op_stack.Add(new PSValue(99));

            PSDict.Def_Operation(Globals.dict_stack[^1]);

            var stored = (PSValue)Globals.dict_stack[^1].Data["x"];
            

            Assert.AreEqual(99, stored.Value);
            Assert.AreEqual(0, Globals.op_stack.Count);
        }

        [Test]
        public void Def_NameWithoutBackslash()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            InputProcessor.ProcessInput("x", currentScope);

            Globals.op_stack.Add(10);
            PSDict.Def_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(10, Globals.op_stack[0]);
        }

        [Test]
        public void Def_TooFewItems()
        {
            Globals.op_stack.Add("/x");

            PSDict.Def_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual("/x", Globals.op_stack[0]);
        }

        // ================= DICT =================

        [Test]
        public void Dict_Operation_StaticScoping_AddsParent()
        {
            DictNode parent = new DictNode();
            Globals.dict_stack.Add(parent);

            Globals.static_scoping = true;

            PSDict.Dict_Operation(Globals.dict_stack[^1]);

            var result = Globals.op_stack[0] as DictNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(parent, result.Parent);
        }

        [Test]
        public void Dict_Operation_DynamicScoping_DoesNotSetParent()
        {
            DictNode parent = new DictNode();
            Globals.dict_stack.Add(parent);

            Globals.static_scoping = false;

            PSDict.Dict_Operation(Globals.dict_stack[^1]);

            var result = Globals.op_stack[0] as DictNode;

            Assert.IsNotNull(result);
            Assert.IsNull(result.Parent);
        }

        [Test]
        public void Dict_Operation_AlwaysPushesNewDictNode()
        {
            Globals.static_scoping = true;

            int before = Globals.op_stack.Count;

            PSDict.Dict_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(before + 1, Globals.op_stack.Count);
            Assert.IsInstanceOf<DictNode>(Globals.op_stack[^1]);
        }

        [Test]
        public void Dict_Operation_UsesTopOfDictStackAsParent()
        {
            DictNode parent1 = new DictNode();
            DictNode parent2 = new DictNode();

            Globals.dict_stack.Add(parent1);
            Globals.dict_stack.Add(parent2);

            Globals.static_scoping = true;

            PSDict.Dict_Operation(Globals.dict_stack[^1]);

            var child = Globals.op_stack[0] as DictNode;

            Assert.AreEqual(parent2, child.Parent);
        }

        // ================= BEGIN =================

        [Test]
        public void Begin_Operation_EmptyStack_PrintsError()
        {
            Globals.op_stack.Clear();

            PSDict.Begin_Operation(Globals.dict_stack[^1]);

            Assert.IsTrue(consoleOutput.ToString().Contains("Stack is empty!"));
            Assert.AreEqual(1, Globals.dict_stack.Count);
        }

        [Test]
        public void Begin_Operation_TopIsDictNode_MovesToDictStack()
        {
            DictNode dict = new DictNode();
            Globals.op_stack.Add(dict);

            int before = Globals.dict_stack.Count;

            PSDict.Begin_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(0, Globals.op_stack.Count);
            Assert.AreEqual(before + 1, Globals.dict_stack.Count);
            Assert.AreEqual(dict, Globals.dict_stack[^1]);
        }

        [Test]
        public void Begin_Operation_TopIsNotDictNode_PrintsError()
        {
            Globals.op_stack.Add(123);

            int before = Globals.dict_stack.Count;

            PSDict.Begin_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(0, Globals.op_stack.Count);
            Assert.AreEqual(before, Globals.dict_stack.Count);

            Assert.IsTrue(consoleOutput.ToString()
                .Contains("Begin operation requires a dictionary on top of stack"));
        }

        [Test]
        public void Begin_Operation_RemovesOnlyTopItem()
        {
            Globals.op_stack.Add("not a dict");
            Globals.op_stack.Add(new DictNode());

            int before = Globals.dict_stack.Count;

            PSDict.Begin_Operation(Globals.dict_stack[^1]);

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(before + 1, Globals.dict_stack.Count);
        }
    }

}