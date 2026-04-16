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
            var parser = new BoolParser();
            bool success = parser.TryParse("true", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BoolParser_ValidFalse()
        {
            var parser = new BoolParser();
            bool success = parser.TryParse("false", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void BoolParser_Invalid()
        {
            var parser = new BoolParser();
            bool success = parser.TryParse("hello", out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //BOOL PARSER TESTS


        //NUM PARSER TESTS
        [Test]
        public void NumParser_Int()
        {
            var parser = new NumParser();
            bool success = parser.TryParse("42", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void NumParser_Double()
        {
            var parser = new NumParser();
            bool success = parser.TryParse("3.14", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual(3.14, result);
        }

        [Test]
        public void NumParser_Invalid()
        {
            var parser = new NumParser();
            bool success = parser.TryParse("abc", out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [Test]
        public void NameParser_Valid()
        {
            var parser = new NameParser();
            bool success = parser.TryParse("/hello", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual("/hello", result);
        }

        [Test]
        public void NameParser_Invalid_NoBackslash()
        {
            var parser = new NameParser();
            bool success = parser.TryParse("hello", out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //NUM PARSER TESTS


        //STRING PARSER TESTS
        [Test]
        public void StringParser_Valid()
        {
            var parser = new StringParser();
            bool success = parser.TryParse("(Hello World)", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void StringParser_Invalid()
        {
            var parser = new StringParser();
            bool success = parser.TryParse("Hello World", out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //STRING PARSER TESTS

        //ARRAY PARSER TESTS
        [Test]
        public void ArrayParser_Valid()
        {
            var parser = new ArrayParser();
            bool success = parser.TryParse("[1 2 3]", out object result);

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
            var parser = new ArrayParser();
            bool success = parser.TryParse("[]", out object result);

            Assert.IsTrue(success);

            var list = result as List<object>;
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ArrayParser_Invalid()
        {
            var parser = new ArrayParser();
            bool success = parser.TryParse("1 2 3", out object result);

            Assert.IsFalse(success);
            Assert.IsNull(result);
        }
        //ARRAY PARSER TESTS

        //CODE BLOCK PARSER TESTS
        [Test]
        public void CodeBlockParser_Valid()
        {
            var parser = new CodeBlockParser();
            bool success = parser.TryParse("{1 2 add}", out object result);

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
            var parser = new CodeBlockParser();
            bool success = parser.TryParse("1 2 add", out object result);

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
            Console.SetOut(originalOutput);  // RESTORE first
            consoleOutput.Dispose();         // THEN dispose
        }
        [Test]
        public void Add_Ints_PushesSum()
        {
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(3);

            PSDict.Add_Operation();

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(5, Globals.op_stack[0]);
        }
        [Test]
        public void Add_Floats_PushesSum()
        {
            Globals.op_stack.Add(2.5f);
            Globals.op_stack.Add(1.5f);

            PSDict.Add_Operation();

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(4f, Globals.op_stack[0]);
        }
        [Test]
        public void Add_TooFewItems_Throws()
        {
            Globals.op_stack.Add(1);
            PSDict.Add_Operation();
            Assert.AreEqual(1, Globals.op_stack[0]);
        }
        [Test]
        public void Add_MixedTypes()
        {
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2.5f);
            PSDict.Add_Operation();
            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(3.5, Globals.op_stack[0]);
        }
        [Test]
        public void Def_ValidDefinition_AddsFunction()
        {
            Globals.op_stack.Add("/x");
            Globals.op_stack.Add(42);

            PSDict.Def_Operation();

            Assert.IsEmpty(Globals.op_stack);
            Assert.IsTrue(Globals.dict_stack[0].Data.ContainsKey("x"));
        }
        [Test]
        public void Def_FunctionPushesValue()
        {
            Globals.op_stack.Add("/x");
            Globals.op_stack.Add(99);

            PSDict.Def_Operation();

            Globals.dict_stack[0].Data["x"]();

            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(99, Globals.op_stack[0]);
        }
        [Test]
        public void Def_NameWithoutBackslash()
        {

            InputProcessor.ProcessInput("x");
            Globals.op_stack.Add(10);
            PSDict.Def_Operation();
            Assert.AreEqual(10, Globals.op_stack[0]);
            Assert.AreEqual(1, Globals.op_stack.Count);
        }
        [Test]
        public void Def_TooFewItems()
        {
            Globals.op_stack.Add("/x");

            PSDict.Def_Operation();
            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual("/x", Globals.op_stack[0]);

        }

        [Test]
        public void Dict_Operation_StaticScoping_AddsParent()
        {
            // Arrange
            DictNode parent = new DictNode();
            Globals.dict_stack.Add(parent);

            Globals.static_scoping = true;

            // Act
            PSDict.Dict_Operation();

            // Assert
            Assert.AreEqual(1, Globals.op_stack.Count);

            var result = Globals.op_stack[0] as DictNode;
            Assert.IsNotNull(result);

            Assert.AreEqual(parent, result.Parent);
        }
        [Test]
        public void Dict_Operation_DynamicScoping_DoesNotSetParent()
        {
            // Arrange
            DictNode parent = new DictNode();
            Globals.dict_stack.Add(parent);

            Globals.static_scoping = false;

            // Act
            PSDict.Dict_Operation();

            // Assert
            var result = Globals.op_stack[0] as DictNode;
            Assert.IsNotNull(result);

            Assert.IsNull(result.Parent);
        }
        [Test]
        public void Dict_Operation_AlwaysPushesNewDictNode()
        {
            // Arrange
            Globals.static_scoping = true;

            int before = Globals.op_stack.Count;

            // Act
            PSDict.Dict_Operation();

            // Assert
            Assert.AreEqual(before + 1, Globals.op_stack.Count);
            Assert.IsInstanceOf<DictNode>(Globals.op_stack[Globals.op_stack.Count - 1]);
        }
        [Test]
        public void Dict_Operation_UsesTopOfDictStackAsParent()
        {
            // Arrange
            DictNode parent1 = new DictNode();
            DictNode parent2 = new DictNode();

            Globals.dict_stack.Add(parent1);
            Globals.dict_stack.Add(parent2);

            Globals.static_scoping = true;

            // Act
            PSDict.Dict_Operation();

            // Assert
            var child = Globals.op_stack[0] as DictNode;
            Assert.AreEqual(parent2, child.Parent);
        }
        [Test]
        public void Begin_Operation_EmptyStack_PrintsError()
        {
            // Arrange
            Globals.op_stack.Clear();

            // Act
            PSDict.Begin_Operation();

            // Assert
            Assert.IsTrue(consoleOutput.ToString().Contains("Stack is empty!"));

            // root dict must still exist (from Populate)
            Assert.AreEqual(1, Globals.dict_stack.Count);
        }
        [Test]
        public void Begin_Operation_TopIsDictNode_MovesToDictStack()
        {
            // Arrange
            DictNode dict = new DictNode();
            Globals.op_stack.Add(dict);

            int before = Globals.dict_stack.Count;

            // Act
            PSDict.Begin_Operation();

            // Assert
            Assert.AreEqual(0, Globals.op_stack.Count);
            Assert.AreEqual(before + 1, Globals.dict_stack.Count);
            Assert.AreEqual(dict, Globals.dict_stack[Globals.dict_stack.Count - 1]);
        }
        [Test]
        public void Begin_Operation_TopIsNotDictNode_PrintsErrorAndRemovesItem()
        {
            // Arrange
            Globals.op_stack.Add(123);

            int beforeDict = Globals.dict_stack.Count;

            // Act
            PSDict.Begin_Operation();

            // Assert
            Assert.AreEqual(0, Globals.op_stack.Count);
            Assert.AreEqual(beforeDict, Globals.dict_stack.Count);

            Assert.IsTrue(
                consoleOutput.ToString().Contains("Begin operation requires a dictionary on top of stack")
            );
        }
        [Test]
        public void Begin_Operation_RemovesOnlyTopItem()
        {
            // Arrange
            Globals.op_stack.Add("not a dict");
            Globals.op_stack.Add(new DictNode());

            int before = Globals.dict_stack.Count;

            // Act
            PSDict.Begin_Operation();

            // Assert
            Assert.AreEqual(1, Globals.op_stack.Count);
            Assert.AreEqual(before + 1, Globals.dict_stack.Count);
        }
    }
    
}