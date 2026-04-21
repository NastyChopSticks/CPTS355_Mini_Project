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

    [TestFixture]
    public class BeginOperationTests
    {
        private DictNode CreateDict() => new DictNode();

        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            Globals.dict_stack.Clear();
        }

        [Test]
        public void BeginOperation_MovesDict_FromOpStackToDictStack()
        {
            // Arrange
            var dict = CreateDict();
            Globals.op_stack.Add(dict);

            int initialDictStackSize = Globals.dict_stack.Count;

            // Act
            PSDict.Begin_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(initialDictStackSize + 1));
            Assert.That(Globals.dict_stack[^1], Is.EqualTo(dict));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void BeginOperation_DoesNotModifyDictStack_WhenTopIsNotDict()
        {
            // Arrange
            Globals.op_stack.Add(123); // not a DictNode

            int before = Globals.dict_stack.Count;

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Begin_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(before));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Begin operation requires a dictionary on top of stack"));
        }

        [Test]
        public void BeginOperation_PrintsError_WhenOpStackEmpty()
        {
            // Arrange
            int before = Globals.dict_stack.Count;

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Begin_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(before));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
        }

        [Test]
        public void BeginOperation_RemovesOnlyTopOfOpStack()
        {
            // Arrange
            var dict1 = CreateDict();
            var dict2 = CreateDict();

            Globals.op_stack.Add(dict1);
            Globals.op_stack.Add(dict2);

            // Act
            PSDict.Begin_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(dict1));
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.dict_stack[^1], Is.EqualTo(dict2));
        }
    }
    [TestFixture]
    public class EndOperationTests
    {
        private DictNode CreateDict() => new DictNode();

        [SetUp]
        public void Setup()
        {
            Globals.dict_stack.Clear();
        }

        [Test]
        public void EndOperation_RemovesTop_WhenMoreThanOneDictExists()
        {
            // Arrange
            Globals.dict_stack.Add(CreateDict());
            Globals.dict_stack.Add(CreateDict());
            Globals.dict_stack.Add(CreateDict());

            int before = Globals.dict_stack.Count;

            // Act
            PSDict.End_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void EndOperation_DoesNotRemove_WhenOnlyDefaultDictionaryExists()
        {
            // Arrange
            Globals.dict_stack.Add(CreateDict());

            int before = Globals.dict_stack.Count;

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.End_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void EndOperation_PrintsError_WhenOnlyOneDictionaryExists()
        {
            // Arrange
            Globals.dict_stack.Add(CreateDict());

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.End_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Cannot pop default dictionary"));
        }

        [Test]
        public void EndOperation_RemovesOnlyTopDictionary()
        {
            // Arrange
            var d1 = CreateDict();
            var d2 = CreateDict();
            var d3 = CreateDict();

            Globals.dict_stack.Add(d1);
            Globals.dict_stack.Add(d2);
            Globals.dict_stack.Add(d3);

            // Act
            PSDict.End_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.dict_stack[^1], Is.EqualTo(d2));
        }

    }
    [TestFixture]
    public class ExchOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void ExchOperation_SwapsTopTwoItems()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Exch_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[0], Is.EqualTo(2));
            Assert.That(Globals.op_stack[1], Is.EqualTo(1));
        }

        [Test]
        public void ExchOperation_DoesNothing_WhenOnlyOneItem()
        {
            // Arrange
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Exch_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[0], Is.EqualTo(1));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Not enough items on stack to exch operation"));
        }

        [Test]
        public void ExchOperation_DoesNothing_WhenStackIsEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Exch_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Not enough items on stack to exch operation"));
        }

        [Test]
        public void ExchOperation_SwapsDifferentTypesCorrectly()
        {
            // Arrange
            var dict = new DictNode();

            Globals.op_stack.Add(dict);
            Globals.op_stack.Add(42);

            // Act
            PSDict.Exch_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[0], Is.EqualTo(42));
            Assert.That(Globals.op_stack[1], Is.EqualTo(dict));
        }

        [Test]
        public void ExchOperation_DoesNotChangeStackSize()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            int before = Globals.op_stack.Count;

            // Act
            PSDict.Exch_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(before));
        }
    }
    [TestFixture]
    public class EqualSignPrintOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void EqualSignPrint_PrintsAndRemovesTopItem()
        {
            // Arrange
            Globals.op_stack.Add(42);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.EqualSignPrint_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("42"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void EqualSignPrint_DoesNothing_WhenStackIsEmptyExceptErrorMessage()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.EqualSignPrint_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void EqualSignPrint_PreservesCorrectValueBeforePop()
        {
            // Arrange
            Globals.op_stack.Add("hello");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.EqualSignPrint_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("hello"));
        }

        [Test]
        public void EqualSignPrint_RemovesOnlyTopElement()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(3);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.EqualSignPrint_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class CopyOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void CopyOperation_CopiesNItemsCorrectly()
        {
            // Arrange
            // stack: [1, 2, 3, 2]
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(2); // n = 2

            // Act
            PSDict.Copy_Operation(null);

            // Expected:
            // copy last 2 items before n: [2,3]
            // result: [1,2,3,2,2,3]

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(6));
            Assert.That(Globals.op_stack[^2], Is.EqualTo(2));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3));
        }

        [Test]
        public void CopyOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Copy_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
        }

        [Test]
        public void CopyOperation_DoesNothing_WhenTopIsNotInteger()
        {
            // Arrange
            Globals.op_stack.Add("not an int");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Copy_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Copy operation requires a non-negative integer on top of stack and enough items to copy"));
        }

        [Test]
        public void CopyOperation_DoesNothing_WhenIntegerIsNegative()
        {
            // Arrange
            Globals.op_stack.Add(-3);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Copy_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Copy operation requires a non-negative integer on top of stack and enough items to copy"));
        }

        [Test]
        public void CopyOperation_DoesNothing_WhenNotEnoughItemsToCopy()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(5); // tries to copy 5 items but only 1 exists

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Copy_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Copy operation requires a non-negative integer on top of stack and enough items to copy"));
        }

        [Test]
        public void CopyOperation_DoesNotRemoveTopElement()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(20);
            Globals.op_stack.Add(1);

            // Act
            PSDict.Copy_Operation(null);

            // Assert
            // original top (1) should still be there
            Assert.That(Globals.op_stack.Contains(1), Is.True);
        }
    }
    [TestFixture]
    public class DupOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void DupOperation_DuplicatesTopElement()
        {
            // Arrange
            Globals.op_stack.Add(42);

            // Act
            PSDict.Dup_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(42));
            Assert.That(Globals.op_stack[^2], Is.EqualTo(42));
        }

        [Test]
        public void DupOperation_DoesNothing_WhenStackIsEmptyExceptError()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Dup_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
        }

        [Test]
        public void DupOperation_DuplicatesStringCorrectly()
        {
            // Arrange
            Globals.op_stack.Add("hello");

            // Act
            PSDict.Dup_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[^1], Is.EqualTo("hello"));
            Assert.That(Globals.op_stack[^2], Is.EqualTo("hello"));
        }

        [Test]
        public void DupOperation_DoesNotRemoveOriginalElement()
        {
            // Arrange
            Globals.op_stack.Add(99);

            // Act
            PSDict.Dup_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Contains(99), Is.True);
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void DupOperation_PreservesStackOrder()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Dup_Operation(null);

            // Stack becomes: 1,2,2

            // Assert
            Assert.That(Globals.op_stack[0], Is.EqualTo(1));
            Assert.That(Globals.op_stack[1], Is.EqualTo(2));
            Assert.That(Globals.op_stack[2], Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class ClearOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void ClearOperation_RemovesAllItems_FromStack()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(3);

            // Act
            PSDict.Clear_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void ClearOperation_DoesNotThrow_WhenStackAlreadyEmpty()
        {
            // Arrange
            // stack already empty

            // Act + Assert
            Assert.DoesNotThrow(() => PSDict.Clear_Operation(null));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void ClearOperation_RemovesMixedTypes()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add(new DictNode());

            // Act
            PSDict.Clear_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }
    }
    [TestFixture]
    public class CountOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void CountOperation_PushesZero_WhenStackIsEmpty()
        {
            // Arrange
            // stack is empty

            // Act
            PSDict.Count_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(0));
        }

        [Test]
        public void CountOperation_PushesCorrectCount_WhenStackHasItems()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(20);
            Globals.op_stack.Add(30);

            // Act
            PSDict.Count_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(4));
        }

        [Test]
        public void CountOperation_DoesNotModifyExistingElements()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Count_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[0], Is.EqualTo(1));
            Assert.That(Globals.op_stack[1], Is.EqualTo(2));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(2));
        }

        [Test]
        public void CountOperation_IsDeterministic()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            // Act
            PSDict.Count_Operation(null);
            PSDict.Count_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3)); // second call: 3 items now
        }
    }
    [TestFixture]
    public class SubOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void SubOperation_SubtractsIntegersCorrectly()
        {
            // Arrange: 10 3 -> 10 - 3 = 7
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(3);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(7.0));
        }

        [Test]
        public void SubOperation_SubtractsDoublesCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(5.5);
            Globals.op_stack.Add(2.0);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.5));
        }

        [Test]
        public void SubOperation_RemovesBothOperands()
        {
            // Arrange
            Globals.op_stack.Add(8);
            Globals.op_stack.Add(3);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void SubOperation_PreservesOrderCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(4);

            // Act
            PSDict.Sub_Operation(null);

            // 10 - 4 = 6

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(6.0));
        }

        [Test]
        public void SubOperation_RejectsNonNumericTypes()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Sub operation requires two integers or two floats"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2)); // unchanged
        }

        [Test]
        public void SubOperation_DoesNothing_WhenStackHasLessThanTwoItems()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to sub operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void SubOperation_DoesNotModifyStack_OnTypeError()
        {
            // Arrange
            Globals.op_stack.Add("x");
            Globals.op_stack.Add("y");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sub_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[0], Is.EqualTo("x"));
            Assert.That(Globals.op_stack[1], Is.EqualTo("y"));
        }
    }
    [TestFixture]
    public class MulOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void MulOperation_MultipliesIntegersCorrectly()
        {
            // Arrange: 6 4 -> 24
            Globals.op_stack.Add(6);
            Globals.op_stack.Add(4);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(24.0));
        }

        [Test]
        public void MulOperation_MultipliesDoublesCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(2.5);
            Globals.op_stack.Add(4.0);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(10.0));
        }

        [Test]
        public void MulOperation_RemovesBothOperands()
        {
            // Arrange
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(7);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void MulOperation_PreservesCorrectOrder()
        {
            // Arrange: 10 * 2 = 20
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(20.0));
        }

        [Test]
        public void MulOperation_Fails_WhenNotEnoughItems()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to mul operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void MulOperation_Fails_WhenTypesAreInvalid()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Mul operation requires two integers or two floats"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void MulOperation_DoesNotModifyStack_OnTypeError()
        {
            // Arrange
            Globals.op_stack.Add("x");
            Globals.op_stack.Add("y");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mul_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[0], Is.EqualTo("x"));
            Assert.That(Globals.op_stack[1], Is.EqualTo("y"));
        }
    }
    [TestFixture]
    public class DivOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void DivOperation_DividesIntegersCorrectly()
        {
            // Arrange: 10 / 2 = 5
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Div_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5.0));
        }

        [Test]
        public void DivOperation_DividesDoublesCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(9.0);
            Globals.op_stack.Add(3.0);

            // Act
            PSDict.Div_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.0));
        }

        [Test]
        public void DivOperation_RemovesOperands_EvenOnFailure()
        {
            // Arrange (invalid types)
            Globals.op_stack.Add("a");
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Div_Operation(null);

            // Assert: both removed
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Division requires numeric types"));
        }

        [Test]
        public void DivOperation_DivisionByZero_PrintsError_AndRemovesOperands()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(0);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Div_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Division by zero"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void DivOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Div_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to div operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void DivOperation_PreservesCorrectOrder()
        {
            // Arrange: 20 / 4 = 5
            Globals.op_stack.Add(20);
            Globals.op_stack.Add(4);

            // Act
            PSDict.Div_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5.0));
        }
    }
    [TestFixture]
    public class IdivOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void IdivOperation_DividesIntegersCorrectly()
        {
            // Arrange: 10 / 2 = 5
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void IdivOperation_TruncatesTowardZero()
        {
            // Arrange: 7 / 2 = 3 (integer division)
            Globals.op_stack.Add(7);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3));
        }

        [Test]
        public void IdivOperation_Fails_WhenNotIntegers()
        {
            // Arrange
            Globals.op_stack.Add(10.5);
            Globals.op_stack.Add(2);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Idiv operation requires two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2)); // unchanged
        }

        [Test]
        public void IdivOperation_DivisionByZero_PrintsErrorAndDoesNotChangeStack()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(0);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Division by zero"));

            // important: your code returns early, so stack is unchanged
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void IdivOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to idiv operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void IdivOperation_RemovesBothOperands_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add(20);
            Globals.op_stack.Add(4);

            // Act
            PSDict.Idiv_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class ModOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void ModOperation_ComputesRemainderCorrectly()
        {
            // Arrange: 10 % 3 = 1
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(3);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(1));
        }

        [Test]
        public void ModOperation_HandlesZeroDivisor()
        {
            // Arrange
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(0);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Division by zero"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2)); // no removal due to early return
        }

        [Test]
        public void ModOperation_Fails_WhenTypesAreNotIntegers()
        {
            // Arrange
            Globals.op_stack.Add(10.5);
            Globals.op_stack.Add(2);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Mod operation requires two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void ModOperation_RemovesOperands_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add(7);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void ModOperation_PreservesCorrectValue()
        {
            // Arrange: 20 % 6 = 2
            Globals.op_stack.Add(20);
            Globals.op_stack.Add(6);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(2));
        }

        [Test]
        public void ModOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add(5);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void ModOperation_LeavesStackUnchanged_OnTypeError()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Mod_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[0], Is.EqualTo("a"));
            Assert.That(Globals.op_stack[1], Is.EqualTo("b"));
        }
    }
    [TestFixture]
    public class AbsOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void AbsOperation_ComputesAbsoluteValue_Int()
        {
            // Arrange
            Globals.op_stack.Add(-5);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void AbsOperation_ComputesAbsoluteValue_Float()
        {
            // Arrange
            Globals.op_stack.Add(-3.5f);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.5f));
        }

        [Test]
        public void AbsOperation_ComputesAbsoluteValue_Double()
        {
            // Arrange
            Globals.op_stack.Add(-10.25);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(10.25));
        }

        [Test]
        public void AbsOperation_DoesNotPop_Element()
        {
            // Arrange
            Globals.op_stack.Add(-7);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void AbsOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void AbsOperation_Fails_WhenTypeIsInvalid()
        {
            // Arrange
            Globals.op_stack.Add("not a number");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Abs operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("not a number"));
        }

        [Test]
        public void AbsOperation_ReplacesTopValue_NotPushesNewOne()
        {
            // Arrange
            Globals.op_stack.Add(-42);

            // Act
            PSDict.Abs_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(42));
        }
    }
    [TestFixture]
    public class NegOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void NegOperation_NegatesInteger()
        {
            // Arrange
            Globals.op_stack.Add(5);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(-5));
        }

        [Test]
        public void NegOperation_NegatesNegativeInteger()
        {
            // Arrange
            Globals.op_stack.Add(-10);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(10));
        }

        [Test]
        public void NegOperation_NegatesFloat()
        {
            // Arrange
            Globals.op_stack.Add(3.5f);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(-3.5f));
        }

        [Test]
        public void NegOperation_NegatesDouble()
        {
            // Arrange
            Globals.op_stack.Add(10.25);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(-10.25));
        }

        [Test]
        public void NegOperation_DoesNotPopValue()
        {
            // Arrange
            Globals.op_stack.Add(7);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void NegOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void NegOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add("not a number");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Neg operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("not a number"));
        }

        [Test]
        public void NegOperation_ReplacesTopValue()
        {
            // Arrange
            Globals.op_stack.Add(42);

            // Act
            PSDict.Neg_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(-42));
        }
    }
    [TestFixture]
    public class CeilingOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void CeilingOperation_CeilsFloatCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(3.2f);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(4.0));
        }

        [Test]
        public void CeilingOperation_CeilsDoubleCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(7.01);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(8.0));
        }

        [Test]
        public void CeilingOperation_Integer_RemainsUnchanged()
        {
            // Arrange
            Globals.op_stack.Add(5);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void CeilingOperation_DoesNotPopValue()
        {
            // Arrange
            Globals.op_stack.Add(2.3);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void CeilingOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void CeilingOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add("hello");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Ceiling operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("hello"));
        }

        [Test]
        public void CeilingOperation_ReplacesTopValue()
        {
            // Arrange
            Globals.op_stack.Add(9.1);

            // Act
            PSDict.Ceiling_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(10.0));
        }
    }
    [TestFixture]
    public class RoundOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void RoundOperation_RoundsFloatDown()
        {
            // Arrange
            Globals.op_stack.Add(3.2f);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.0));
        }

        [Test]
        public void RoundOperation_RoundsFloatUp()
        {
            // Arrange
            Globals.op_stack.Add(3.8f);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(4.0));
        }

        [Test]
        public void RoundOperation_RoundsDoubleCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(7.6);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(8.0));
        }

        [Test]
        public void RoundOperation_Integer_RemainsUnchanged()
        {
            // Arrange
            Globals.op_stack.Add(5);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void RoundOperation_DoesNotPopValue()
        {
            // Arrange
            Globals.op_stack.Add(2.7);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void RoundOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void RoundOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add("hello");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Round operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("hello"));
        }

        [Test]
        public void RoundOperation_ReplacesTopValue()
        {
            // Arrange
            Globals.op_stack.Add(4.6);

            // Act
            PSDict.Round_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5.0));
        }
    }
    [TestFixture]
    public class SqrtOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void SqrtOperation_ComputesCorrectSquareRoot_Int()
        {
            // Arrange
            Globals.op_stack.Add(9);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.0));
        }

        [Test]
        public void SqrtOperation_ComputesCorrectSquareRoot_Double()
        {
            // Arrange
            Globals.op_stack.Add(16.0);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(4.0));
        }

        [Test]
        public void SqrtOperation_ComputesCorrectSquareRoot_Float()
        {
            // Arrange
            Globals.op_stack.Add(25.0f);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5.0));
        }

        [Test]
        public void SqrtOperation_DoesNotModifyStackSize()
        {
            // Arrange
            Globals.op_stack.Add(9);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void SqrtOperation_RejectsNegativeInteger()
        {
            // Arrange
            Globals.op_stack.Add(-9);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Cannot take square root of negative number"));

            Assert.That(Globals.op_stack[^1], Is.EqualTo(-9));
        }

        [Test]
        public void SqrtOperation_RejectsNegativeDouble()
        {
            // Arrange
            Globals.op_stack.Add(-4.0);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Cannot take square root of negative number"));

            Assert.That(Globals.op_stack[^1], Is.EqualTo(-4.0));
        }

        [Test]
        public void SqrtOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void SqrtOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add("not a number");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Sqrt operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("not a number"));
        }

        [Test]
        public void SqrtOperation_ReplacesTopValue()
        {
            // Arrange
            Globals.op_stack.Add(49);

            // Act
            PSDict.Sqrt_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(7.0));
        }
    }
    [TestFixture]
    public class FloorOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void FloorOperation_FloorsFloatCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(3.9f);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3.0));
        }

        [Test]
        public void FloorOperation_FloorsDoubleCorrectly()
        {
            // Arrange
            Globals.op_stack.Add(7.99);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(7.0));
        }

        [Test]
        public void FloorOperation_Integer_RemainsUnchanged()
        {
            // Arrange
            Globals.op_stack.Add(5);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void FloorOperation_DoesNotPopValue()
        {
            // Arrange
            Globals.op_stack.Add(9.8);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void FloorOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(), Is.EqualTo("Stack is empty!"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void FloorOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add("not a number");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Round operation requires a numeric type"));

            Assert.That(Globals.op_stack[0], Is.EqualTo("not a number"));
        }

        [Test]
        public void FloorOperation_ReplacesTopValue()
        {
            // Arrange
            Globals.op_stack.Add(6.1);

            // Act
            PSDict.Floor_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(6.0));
        }
    }
    [TestFixture]
    public class LengthOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void LengthOperation_ReturnsStringLength()
        {
            // Arrange
            Globals.op_stack.Add("hello");

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        [Test]
        public void LengthOperation_ReturnsListLength()
        {
            // Arrange
            Globals.op_stack.Add(new List<object> { 1, 2, 3 });

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3));
        }

        [Test]
        public void LengthOperation_PopsOriginalValue()
        {
            // Arrange
            Globals.op_stack.Add("abc");

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(3));
        }

        [Test]
        public void LengthOperation_Fails_WhenTypeInvalid()
        {
            // Arrange
            Globals.op_stack.Add(123); // invalid type

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("length expects a string or array"));

            // original item was removed even on failure (important behavior)
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void LengthOperation_DoesNothing_WhenStackEmpty()
        {
            // Arrange
            // empty stack

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void LengthOperation_ReplacesTopWithLength()
        {
            // Arrange
            Globals.op_stack.Add("abcd");

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(4));
        }

        [Test]
        public void LengthOperation_HandlesEmptyString()
        {
            // Arrange
            Globals.op_stack.Add("");

            // Act
            PSDict.Length_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(0));
        }
    }
    [TestFixture]
    public class MaxLengthOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            Globals.dict_stack.Clear();
        }

        [Test]
        public void MaxLengthOperation_PushesIntMaxValue_WhenUserDictExists()
        {
            // Arrange: need at least 2 dictionaries
            Globals.dict_stack.Add(new DictNode()); // default
            Globals.dict_stack.Add(new DictNode()); // user-defined

            // Act
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void MaxLengthOperation_DoesNotModifyDictStack()
        {
            // Arrange
            Globals.dict_stack.Add(new DictNode());
            Globals.dict_stack.Add(new DictNode());

            int before = Globals.dict_stack.Count;

            // Act
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.dict_stack.Count, Is.EqualTo(before));
        }

        [Test]
        public void MaxLengthOperation_DoesNothing_WhenOnlyDefaultDictExists()
        {
            // Arrange
            Globals.dict_stack.Add(new DictNode()); // only default

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("There are no user defined dictionaries!"));
        }

        [Test]
        public void MaxLengthOperation_DoesNothing_WhenDictStackEmpty()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("There are no user defined dictionaries!"));
        }

        [Test]
        public void MaxLengthOperation_PushesOnlyOneValue()
        {
            // Arrange
            Globals.dict_stack.Add(new DictNode());
            Globals.dict_stack.Add(new DictNode());

            // Act
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void MaxLengthOperation_IsDeterministic()
        {
            // Arrange
            Globals.dict_stack.Add(new DictNode());
            Globals.dict_stack.Add(new DictNode());

            // Act
            PSDict.MaxLength_Operation(null);
            PSDict.MaxLength_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
            Assert.That(Globals.op_stack[0], Is.EqualTo(int.MaxValue));
            Assert.That(Globals.op_stack[1], Is.EqualTo(int.MaxValue));
        }
    }
    [TestFixture]
    public class GetOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void GetOperation_ReturnsCorrectAsciiValue()
        {
            // Arrange: "abc", index 1 -> 'b' -> 98
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(1);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo((int)'b'));
        }

        [Test]
        public void GetOperation_PopsTwoItems_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add(0);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetOperation_IndexOutOfBounds_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(10);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index out of bounds for get operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2)); // unchanged due to early return
        }

        [Test]
        public void GetOperation_NegativeIndex_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(-1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index out of bounds for get operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetOperation_Fails_WhenTopIsNotInteger()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("not index");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Get operation requires an integer index on top of stack"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetOperation_Fails_WhenSecondIsNotString()
        {
            // Arrange
            Globals.op_stack.Add(123);
            Globals.op_stack.Add(0);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Get operation requires a string below the index on stack"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add("abc");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("get requires a string and an index!"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetOperation_ReturnsCorrectCharacterAtZero()
        {
            // Arrange
            Globals.op_stack.Add("xyz");
            Globals.op_stack.Add(0);

            // Act
            PSDict.Get_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo((int)'x'));
        }
    }
    [TestFixture]
    public class GetIntervalOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void GetIntervalOperation_ReturnsCorrectSubstring()
        {
            // Arrange: "hello", index 1, count 3 → "ell"
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(3);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo("ell"));
        }

        [Test]
        public void GetIntervalOperation_PopsThreeItems_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add("abcdef");
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(2);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetIntervalOperation_IndexOutOfBounds_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or count out of bounds for getinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetIntervalOperation_NegativeIndex_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(-1);
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or count out of bounds for getinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetIntervalOperation_NegativeCount_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(-2);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or count out of bounds for getinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetIntervalOperation_Fails_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add("abc");

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetIntervalOperation_DoesNothing_WhenTypeInvalid_Index()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("not index");
            Globals.op_stack.Add(2);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetIntervalOperation_DoesNothing_WhenTypeInvalid_String()
        {
            // Arrange
            Globals.op_stack.Add(123);
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetIntervalOperation_ExactBoundsWorks()
        {
            // Arrange: "abc", index 0, count 3 → "abc"
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(0);
            Globals.op_stack.Add(3);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo("abc"));
        }

        [Test]
        public void GetIntervalOperation_PreservesStackOrder()
        {
            // Arrange
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add(0);
            Globals.op_stack.Add(2);

            // Act
            PSDict.GetInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo("he"));
        }
    }
    [TestFixture]
    public class PutIntervalOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void PutIntervalOperation_ReplacesSubstringCorrectly()
        {
            // Arrange: "hello", index 1, "XY" → "hXYlo"
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add("XY");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo("hXYlo"));
        }

        [Test]
        public void PutIntervalOperation_PopsThreeValues_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add("abcdef");
            Globals.op_stack.Add(2);
            Globals.op_stack.Add("ZZ");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void PutIntervalOperation_IndexOutOfBounds_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(10);
            Globals.op_stack.Add("X");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or substring length out of bounds for putinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_NegativeIndex_PrintsError()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(-1);
            Globals.op_stack.Add("X");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or substring length out of bounds for putinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_TooLongSubstring_PrintsError()
        {
            // Arrange: "abc", index 1, "WXYZ" → too long
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add("WXYZ");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Index or substring length out of bounds for putinterval operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add("abc");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void PutIntervalOperation_Fails_WhenIndexNotInt()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("not index");
            Globals.op_stack.Add("X");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_Fails_WhenTargetNotString()
        {
            // Arrange
            Globals.op_stack.Add(123);
            Globals.op_stack.Add(1);
            Globals.op_stack.Add("X");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_Fails_WhenSubstringNotString()
        {
            // Arrange
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(123);

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void PutIntervalOperation_ReplacesCorrectMiddleSection()
        {
            // Arrange: "12345", index 2, "XX" → "12XX5"
            Globals.op_stack.Add("12345");
            Globals.op_stack.Add(2);
            Globals.op_stack.Add("XX");

            // Act
            PSDict.PutInterval_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo("12XX5"));
        }
    }
    [TestFixture]
    public class EqOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void EqOperation_ReturnsTrue_ForEqualIntegers()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(5);

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void EqOperation_ReturnsFalse_ForDifferentIntegers()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(10);

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void EqOperation_ReturnsTrue_ForEqualStrings()
        {
            // Arrange
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add("hello");

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void EqOperation_ReturnsFalse_ForDifferentStrings()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void EqOperation_PopsTwoItems_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(1);

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void EqOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to eq operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void EqOperation_HandlesMixedTypes_ReturnsFalse()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add("5");

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void EqOperation_ReferenceEqualityWorksForLists()
        {
            // Arrange
            var list = new System.Collections.Generic.List<object> { 1 };
            Globals.op_stack.Add(list);
            Globals.op_stack.Add(list);

            // Act
            PSDict.Eq_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }
    }
    [TestFixture]
    public class NeOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        [Test]
        public void NeOperation_ReturnsFalse_ForEqualIntegers()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(5);

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void NeOperation_ReturnsTrue_ForDifferentIntegers()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(10);

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void NeOperation_ReturnsFalse_ForEqualStrings()
        {
            // Arrange
            Globals.op_stack.Add("hello");
            Globals.op_stack.Add("hello");

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void NeOperation_ReturnsTrue_ForDifferentStrings()
        {
            // Arrange
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void NeOperation_PopsTwoItems_OnSuccess()
        {
            // Arrange
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void NeOperation_DoesNothing_WhenStackTooSmall()
        {
            // Arrange
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to ne operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void NeOperation_HandlesMixedTypes_AsNotEqual()
        {
            // Arrange
            Globals.op_stack.Add(5);
            Globals.op_stack.Add("5");

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void NeOperation_ReferenceEquality_ListCase()
        {
            // Arrange
            var list = new System.Collections.Generic.List<object> { 1 };
            Globals.op_stack.Add(list);
            Globals.op_stack.Add(list);

            // Act
            PSDict.Ne_Operation(null);

            // Assert
            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }
    }
    [TestFixture]
    public class GeOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // NUMERIC TESTS
        // -----------------------

        [Test]
        public void GeOperation_ReturnsTrue_WhenLeftGreater()
        {
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(5);

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GeOperation_ReturnsTrue_WhenEqualNumbers()
        {
            Globals.op_stack.Add(7);
            Globals.op_stack.Add(7);

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GeOperation_ReturnsFalse_WhenLeftLess()
        {
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(8);

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void GeOperation_HandlesMixedNumericTypes()
        {
            Globals.op_stack.Add(5);    // int
            Globals.op_stack.Add(5.0f); // float

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        // -----------------------
        // STRING TESTS
        // -----------------------

        [Test]
        public void GeOperation_ReturnsTrue_WhenStringsEqual()
        {
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("abc");

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GeOperation_ReturnsTrue_WhenLeftLexGreater()
        {
            Globals.op_stack.Add("b");
            Globals.op_stack.Add("a");

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GeOperation_ReturnsFalse_WhenLeftLexSmaller()
        {
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // ERROR / EDGE CASES
        // -----------------------

        [Test]
        public void GeOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Ge_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for ge"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GeOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add(123);
            Globals.op_stack.Add("not a number");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Ge_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("ge requires two numbers or two strings"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void GeOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(9);
            Globals.op_stack.Add(1);

            PSDict.Ge_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class GtOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // NUMERIC TESTS
        // -----------------------

        [Test]
        public void GtOperation_ReturnsTrue_WhenLeftGreater()
        {
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(5);

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GtOperation_ReturnsFalse_WhenEqualNumbers()
        {
            Globals.op_stack.Add(7);
            Globals.op_stack.Add(7);

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void GtOperation_ReturnsFalse_WhenLeftLess()
        {
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(8);

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void GtOperation_HandlesMixedNumericTypes()
        {
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(2.0f);

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        // -----------------------
        // STRING TESTS (based on YOUR implementation)
        // -----------------------

        [Test]
        public void GtOperation_StringCase_ReturnsTrue_WhenLeftLexLessThanRight()
        {
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            PSDict.Gt_Operation(null);

            // NOTE: your code uses < (reversed behavior)
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void GtOperation_StringCase_ReturnsFalse_WhenLeftLexGreater()
        {
            Globals.op_stack.Add("b");
            Globals.op_stack.Add("a");

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void GtOperation_StringCase_ReturnsFalse_WhenEqual()
        {
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("abc");

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // ERROR / EDGE CASES
        // -----------------------

        [Test]
        public void GtOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Gt_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for ge"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void GtOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add(123);
            Globals.op_stack.Add("bad");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Gt_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("gt requires two numbers or two strings"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void GtOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(9);
            Globals.op_stack.Add(1);

            PSDict.Gt_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class LeOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // NUMERIC TESTS
        // -----------------------

        [Test]
        public void LeOperation_ReturnsTrue_WhenLeftLessThanRight()
        {
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(10);

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LeOperation_ReturnsTrue_WhenEqualNumbers()
        {
            Globals.op_stack.Add(7);
            Globals.op_stack.Add(7);

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LeOperation_ReturnsFalse_WhenLeftGreater()
        {
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(3);

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void LeOperation_HandlesMixedNumericTypes()
        {
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(5.0f);

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        // -----------------------
        // STRING TESTS (based on YOUR implementation)
        // -----------------------

        [Test]
        public void LeOperation_StringCase_ReturnsTrue_WhenLeftLexGreaterOrEqual()
        {
            Globals.op_stack.Add("b");
            Globals.op_stack.Add("a");

            PSDict.Le_Operation(null);

            // NOTE: your code uses >= (inverted LE semantics)
            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LeOperation_StringCase_ReturnsTrue_WhenEqualStrings()
        {
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("abc");

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LeOperation_StringCase_ReturnsFalse_WhenLeftLexLess()
        {
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // ERROR / EDGE CASES
        // -----------------------

        [Test]
        public void LeOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Le_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for ge"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void LeOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add(123);
            Globals.op_stack.Add("bad");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Le_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("le requires two numbers or two strings"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void LeOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            PSDict.Le_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class LtOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // NUMERIC TESTS
        // -----------------------

        [Test]
        public void LtOperation_ReturnsTrue_WhenLeftLessThanRight()
        {
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(10);

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LtOperation_ReturnsFalse_WhenLeftGreater()
        {
            Globals.op_stack.Add(10);
            Globals.op_stack.Add(3);

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void LtOperation_ReturnsFalse_WhenEqualNumbers()
        {
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(5);

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void LtOperation_HandlesMixedNumericTypes()
        {
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(5.0f);

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        // -----------------------
        // STRING TESTS
        // -----------------------

        [Test]
        public void LtOperation_ReturnsTrue_WhenLeftLexLess()
        {
            Globals.op_stack.Add("a");
            Globals.op_stack.Add("b");

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void LtOperation_ReturnsFalse_WhenLeftLexGreater()
        {
            Globals.op_stack.Add("b");
            Globals.op_stack.Add("a");

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void LtOperation_ReturnsFalse_WhenStringsEqual()
        {
            Globals.op_stack.Add("abc");
            Globals.op_stack.Add("abc");

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // EDGE CASES
        // -----------------------

        [Test]
        public void LtOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Lt_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for ge"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void LtOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add(123);
            Globals.op_stack.Add("bad");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Lt_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("lt requires two numbers or two strings"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void LtOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(2);

            PSDict.Lt_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class AndOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // BOOLEAN LOGIC TESTS
        // -----------------------

        [Test]
        public void AndOperation_BoolTrueAndTrue_ReturnsTrue()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(true);

            PSDict.And_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void AndOperation_BoolTrueAndFalse_ReturnsFalse()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(false);

            PSDict.And_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void AndOperation_BoolFalseAndFalse_ReturnsFalse()
        {
            Globals.op_stack.Add(false);
            Globals.op_stack.Add(false);

            PSDict.And_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // INTEGER BITWISE TESTS
        // -----------------------

        [Test]
        public void AndOperation_IntBitwise_WorksCorrectly()
        {
            Globals.op_stack.Add(6); // 110
            Globals.op_stack.Add(3); // 011

            PSDict.And_Operation(null);

            // 110 & 011 = 010 = 2
            Assert.That(Globals.op_stack[^1], Is.EqualTo(2));
        }

        [Test]
        public void AndOperation_IntBitwise_ZeroCase()
        {
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(0);

            PSDict.And_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(0));
        }

        // -----------------------
        // STACK BEHAVIOR TESTS
        // -----------------------

        [Test]
        public void AndOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(true);

            PSDict.And_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // ERROR CASES
        // -----------------------

        [Test]
        public void AndOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(true);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.And_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for and"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void AndOperation_Error_WhenMixedTypes()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.And_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("and requires two booleans or two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void AndOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add("true");
            Globals.op_stack.Add("false");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.And_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("and requires two booleans or two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class OrOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // BOOLEAN LOGIC TESTS
        // -----------------------

        [Test]
        public void OrOperation_BoolTrueOrTrue_ReturnsTrue()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(true);

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void OrOperation_BoolTrueOrFalse_ReturnsTrue()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(false);

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        [Test]
        public void OrOperation_BoolFalseOrFalse_ReturnsFalse()
        {
            Globals.op_stack.Add(false);
            Globals.op_stack.Add(false);

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        // -----------------------
        // INTEGER BITWISE TESTS
        // -----------------------

        [Test]
        public void OrOperation_IntBitwise_WorksCorrectly()
        {
            Globals.op_stack.Add(6); // 110
            Globals.op_stack.Add(3); // 011

            PSDict.Or_Operation(null);

            // 110 | 011 = 111 = 7
            Assert.That(Globals.op_stack[^1], Is.EqualTo(7));
        }

        [Test]
        public void OrOperation_IntBitwise_ZeroCase()
        {
            Globals.op_stack.Add(0);
            Globals.op_stack.Add(0);

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(0));
        }

        [Test]
        public void OrOperation_IntBitwise_MixedBits()
        {
            Globals.op_stack.Add(4); // 100
            Globals.op_stack.Add(1); // 001

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(5));
        }

        // -----------------------
        // STACK BEHAVIOR TESTS
        // -----------------------

        [Test]
        public void OrOperation_PopsTwoItems_OnSuccess()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(false);

            PSDict.Or_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // ERROR CASES
        // -----------------------

        [Test]
        public void OrOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(true);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Or_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for or"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        [Test]
        public void OrOperation_Error_WhenMixedTypes()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Or_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("or requires two booleans or two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void OrOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add("true");
            Globals.op_stack.Add("false");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Or_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("or requires two booleans or two integers"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class NotOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // BOOLEAN TESTS
        // -----------------------

        [Test]
        public void NotOperation_BoolTrue_ReturnsFalse()
        {
            Globals.op_stack.Add(true);

            PSDict.Not_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(false));
        }

        [Test]
        public void NotOperation_BoolFalse_ReturnsTrue()
        {
            Globals.op_stack.Add(false);

            PSDict.Not_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(true));
        }

        // -----------------------
        // INTEGER TESTS
        // -----------------------

        [Test]
        public void NotOperation_Int_WorksCorrectly()
        {
            Globals.op_stack.Add(0);

            PSDict.Not_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(~0));
        }

        [Test]
        public void NotOperation_Int_PositiveValue()
        {
            Globals.op_stack.Add(5);

            PSDict.Not_Operation(null);

            Assert.That(Globals.op_stack[^1], Is.EqualTo(~5));
        }

        // -----------------------
        // STACK BEHAVIOR TESTS
        // -----------------------

        [Test]
        public void NotOperation_PopsOneItem_OnSuccess()
        {
            Globals.op_stack.Add(true);

            PSDict.Not_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // ERROR CASES
        // -----------------------

        [Test]
        public void NotOperation_DoesNothing_WhenStackEmpty()
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Not_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack for not"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void NotOperation_Error_WhenInvalidType()
        {
            Globals.op_stack.Add("true");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Not_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("not requires a boolean or an integer"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    
    [TestFixture]
    public class IfOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // TRUE CONDITION
        // -----------------------

        [Test]
        public void IfOperation_ExecutesBlock_WhenConditionTrue()
        {
            // true {12} if  → should push 12
            var block = new List<string> { "12" };

            Globals.op_stack.Add(true);
            Globals.op_stack.Add(block);

            PSDict.If_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(12));
        }

        // -----------------------
        // FALSE CONDITION
        // -----------------------

        [Test]
        public void IfOperation_DoesNotExecute_WhenConditionFalse()
        {
            var block = new List<string> { "12" };

            Globals.op_stack.Add(false);
            Globals.op_stack.Add(block);

            PSDict.If_Operation(null);

            // nothing executed, stack fully consumed
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // TYPE SAFETY
        // -----------------------

        [Test]
        public void IfOperation_Error_WhenConditionNotBool()
        {
            var block = new List<string> { "12" };

            Globals.op_stack.Add(123);
            Globals.op_stack.Add(block);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.If_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("if operation requires a code block and a boolean"));

            // nothing should be popped
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        [Test]
        public void IfOperation_Error_WhenBlockNotList()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add("not_a_block");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.If_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("if operation requires a code block and a boolean"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        // -----------------------
        // STACK UNDERFLOW
        // -----------------------

        [Test]
        public void IfOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(true);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.If_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to if operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // POP BEHAVIOR
        // -----------------------

        [Test]
        public void IfOperation_PopsTwoItems_OnExecution()
        {
            var block = new List<string> { "12" };

            Globals.op_stack.Add(true);
            Globals.op_stack.Add(block);

            PSDict.If_Operation(null);

            // only result remains (or empty if block doesn't push)
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class IfElseOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            Globals.dict_stack.Clear();
            PSDict.PopulateDict();
        }

        // -----------------------
        // TRUE CONDITION → IF BLOCK
        // -----------------------

        [Test]
        public void IfElseOperation_ExecutesIfBlock_WhenConditionTrue()
        {
            var ifBlock = new List<string> { "1" };
            var elseBlock = new List<string> { "2" };

            Globals.op_stack.Add(true);
            Globals.op_stack.Add(ifBlock);
            Globals.op_stack.Add(elseBlock);

            PSDict.IfElse_Operation(null);

            // if branch executes → pushes "1"
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(1));
        }

        // -----------------------
        // FALSE CONDITION → ELSE BLOCK
        // -----------------------

        [Test]
        public void IfElseOperation_ExecutesElseBlock_WhenConditionFalse()
        {
            var ifBlock = new List<string> { "1" };
            var elseBlock = new List<string> { "2" };

            Globals.op_stack.Add(false);
            Globals.op_stack.Add(ifBlock);
            Globals.op_stack.Add(elseBlock);

            PSDict.IfElse_Operation(null);

            // else branch executes → pushes "2"
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
            Assert.That(Globals.op_stack[^1], Is.EqualTo(2));
        }

        // -----------------------
        // TYPE SAFETY
        // -----------------------

        [Test]
        public void IfElseOperation_Error_WhenConditionNotBool()
        {
            var ifBlock = new List<string> { "1" };
            var elseBlock = new List<string> { "2" };

            Globals.op_stack.Add(123);
            Globals.op_stack.Add(ifBlock);
            Globals.op_stack.Add(elseBlock);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.IfElse_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("ifelse operation requires two code blocks and a boolean"));

            // nothing removed
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void IfElseOperation_Error_WhenIfBlockInvalid()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add("not_a_block");
            Globals.op_stack.Add(new List<string> { "2" });

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.IfElse_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("ifelse operation requires two code blocks and a boolean"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        [Test]
        public void IfElseOperation_Error_WhenElseBlockInvalid()
        {
            Globals.op_stack.Add(true);
            Globals.op_stack.Add(new List<string> { "1" });
            Globals.op_stack.Add("not_a_block");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.IfElse_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("ifelse operation requires two code blocks and a boolean"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
        }

        // -----------------------
        // STACK UNDERFLOW
        // -----------------------

        [Test]
        public void IfElseOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(true);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.IfElse_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items on stack to ifelse operation"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // POP BEHAVIOR
        // -----------------------

        [Test]
        public void IfElseOperation_PopsThreeItems_OnExecution()
        {
            var ifBlock = new List<string> { "1" };
            var elseBlock = new List<string> { "2" };

            Globals.op_stack.Add(true);
            Globals.op_stack.Add(ifBlock);
            Globals.op_stack.Add(elseBlock);

            PSDict.IfElse_Operation(null);

            // only result remains
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class ForOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            
        }

        // -----------------------
        // POSITIVE INCREMENT
        // -----------------------

        [Test]
        public void ForOperation_PositiveIncrement_PushesCorrectSequence()
        {
            var block = new List<string>(); // no-op block so only i matters

            Globals.op_stack.Add(1);  // start
            Globals.op_stack.Add(1);  // inc
            Globals.op_stack.Add(3);  // end
            Globals.op_stack.Add(block);

            PSDict.For_Operation(null);

            // expected loop variables: 1,2,3
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
            Assert.That(Globals.op_stack, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        // -----------------------
        // NEGATIVE INCREMENT
        // -----------------------

        [Test]
        public void ForOperation_NegativeIncrement_PushesCorrectSequence()
        {
            var block = new List<string>();

            Globals.op_stack.Add(5);
            Globals.op_stack.Add(-1);
            Globals.op_stack.Add(2);
            Globals.op_stack.Add(block);

            PSDict.For_Operation(null);

            // 5,4,3,2
            Assert.That(Globals.op_stack.Count, Is.EqualTo(4));
            Assert.That(Globals.op_stack, Is.EquivalentTo(new[] { 5, 4, 3, 2 }));
        }

        // -----------------------
        // ZERO INCREMENT ERROR
        // -----------------------

        [Test]
        public void ForOperation_Error_WhenIncrementIsZero()
        {
            var block = new List<string>();

            Globals.op_stack.Add(0);
            Globals.op_stack.Add(0);
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(block);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.For_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Increment cannot be zero"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(4));
        }

        // -----------------------
        // TYPE ERROR
        // -----------------------

        [Test]
        public void ForOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add("bad");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(5);
            Globals.op_stack.Add(new List<string>());

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.For_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Invalid types for for loop"));
        }

        // -----------------------
        // STACK UNDERFLOW
        // -----------------------

        [Test]
        public void ForOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(1);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.For_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items for for loop"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // EXECUTION TEST (REAL BEHAVIOR)
        // -----------------------

        [Test]
        public void ForOperation_ExecutesBlock_ForEachIteration()
        {
            Globals.dict_stack.Clear();
            Globals.op_stack.Clear();
            PSDict.PopulateDict();
            DictNode currentScope = Globals.dict_stack[^1];
            var block = new List<string>();
            block.Add("1");
            block.Add("2");
            block.Add("add");
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(1);
            Globals.op_stack.Add(3);
            Globals.op_stack.Add(block);
           
            PSDict.For_Operation(currentScope);

            // 3 iterations → loop variable pushed 3 times
            Assert.That(Globals.op_stack.Count, Is.EqualTo(6));
            Assert.That(Globals.op_stack, Is.EquivalentTo(new[] { 1, 3, 2, 3, 3,3 }));
        }
    }
    [TestFixture]
    public class RepeatOperationTests
    {
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
        }

        // -----------------------
        // BASIC REPEAT
        // -----------------------

        [Test]
        public void RepeatOperation_ExecutesBlock_CorrectNumberOfTimes()
        {
            var block = new List<string> { "1" };

            Globals.op_stack.Add(3);
            Globals.op_stack.Add(block);

            PSDict.Repeat_Operation(null);

            // 3 executions → 3 outputs/stack entries
            Assert.That(Globals.op_stack.Count, Is.EqualTo(3));
            Assert.That(Globals.op_stack, Does.Contain(1));
        }

        // -----------------------
        // ZERO COUNT
        // -----------------------

        [Test]
        public void RepeatOperation_DoesNothing_WhenCountIsZero()
        {
            var block = new List<string> { "1" };

            Globals.op_stack.Add(0);
            Globals.op_stack.Add(block);

            PSDict.Repeat_Operation(null);

            // 0 iterations → nothing executed, but items popped
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // NEGATIVE COUNT ERROR
        // -----------------------

        [Test]
        public void RepeatOperation_Error_WhenCountNegative()
        {
            var block = new List<string> { "1" };

            Globals.op_stack.Add(-5);
            Globals.op_stack.Add(block);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Repeat_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Count cannot be negative"));

            // nothing should be removed
            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        // -----------------------
        // TYPE ERROR
        // -----------------------

        [Test]
        public void RepeatOperation_Error_WhenInvalidTypes()
        {
            Globals.op_stack.Add("bad");
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Repeat_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Invalid types for repeat loop"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }

        // -----------------------
        // STACK UNDERFLOW
        // -----------------------

        [Test]
        public void RepeatOperation_DoesNothing_WhenStackTooSmall()
        {
            Globals.op_stack.Add(5);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Repeat_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Not enough items for repeat loop"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // EXECUTION VERIFICATION
        // -----------------------

        [Test]
        public void RepeatOperation_ExecutesExactlyNTimes()
        {
            var block = new List<string>();
            block.Add("4");

            Globals.op_stack.Add(4);
            Globals.op_stack.Add(block);

            PSDict.Repeat_Operation(null);

            // 4 iterations → 4 executions
            Assert.That(Globals.op_stack.Count, Is.EqualTo(4));
        }

        // -----------------------
        // POP BEHAVIOR
        // -----------------------

        [Test]
        public void RepeatOperation_PopsTwoItems_OnExecution()
        {
            var block = new List<string> { "1" };

            Globals.op_stack.Add(2);
            Globals.op_stack.Add(block);

            PSDict.Repeat_Operation(null);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class PrintOperationTests
    {
        private TextWriter originalOut;
        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            Globals.dict_stack.Clear();
            PSDict.PopulateDict();
            originalOut = Console.Out;
        }
        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalOut);
        }
        // -----------------------
        // VALID PRINT
        // -----------------------

        [Test]
        public void PrintOperation_PrintsString_AndPopsStack()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            Globals.op_stack.Add("hello");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Print_Operation(currentScope);

            Assert.That(sw.ToString().Trim(), Is.EqualTo("hello"));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // INVALID TYPE
        // -----------------------

        [Test]
        public void PrintOperation_Error_WhenTopNotString()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            Globals.op_stack.Add(123);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Print_Operation(currentScope);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("print can only be used on strings"));

            // nothing should be popped
            Assert.That(Globals.op_stack.Count, Is.EqualTo(1));
        }

        // -----------------------
        // EMPTY STACK
        // -----------------------

        [Test]
        public void PrintOperation_Error_WhenStackEmpty()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.Print_Operation(currentScope);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Stack is empty!"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // POP BEHAVIOR
        // -----------------------

        [Test]
        public void PrintOperation_PopsExactlyOneItem()
        {
            DictNode currentScope = Globals.dict_stack[^1];
            Globals.op_stack.Add("hello");
            using var sw = new StringWriter();
            Console.SetOut(sw);
            PSDict.Print_Operation(currentScope);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }
    }
    [TestFixture]
    public class PostScriptPrintOperationTests
    {
        private TextWriter originalOut;

        [SetUp]
        public void Setup()
        {
            Globals.op_stack.Clear();
            Globals.dict_stack.Clear();
            PSDict.PopulateDict();
            originalOut = Console.Out;
        }
        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalOut);
        }
        // -----------------------
        // NORMAL PRINT
        // -----------------------

        [Test]
        public void PostScriptPrint_PrintsFormattedValue_AndPopsStack()
        {
            Globals.op_stack.Add(42);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.PostScriptPrint_Operation(null);

            var output = sw.ToString().Trim();

            Assert.That(output, Is.EqualTo(DebugFormatter.PostScriptFormat(42)));
            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // STRING VALUE
        // -----------------------

        [Test]
        public void PostScriptPrint_PrintsFormattedString_AndPops()
        {
            Globals.op_stack.Add("hello");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.PostScriptPrint_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo(DebugFormatter.PostScriptFormat("hello")));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // BOOLEAN VALUE
        // -----------------------

        [Test]
        public void PostScriptPrint_PrintsFormattedBool_AndPops()
        {
            Globals.op_stack.Add(true);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.PostScriptPrint_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo(DebugFormatter.PostScriptFormat(true)));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // EMPTY STACK
        // -----------------------

        [Test]
        public void PostScriptPrint_PrintsError_WhenStackEmpty()
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            PSDict.PostScriptPrint_Operation(null);

            Assert.That(sw.ToString().Trim(),
                Is.EqualTo("Stack is empty!"));

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }

        // -----------------------
        // POP BEHAVIOR
        // -----------------------

        [Test]
        public void PostScriptPrint_PopsExactlyOneItem()
        {
           
            DictNode currentScope = Globals.dict_stack[^1];
            Globals.op_stack.Add(99);
            using var sw = new StringWriter();
            Console.SetOut(sw); 
            PSDict.PostScriptPrint_Operation(currentScope);

            Assert.That(Globals.op_stack.Count, Is.EqualTo(0));
        }
    }

}