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
            bool success = parser.TryParse("\\hello", out object result);

            Assert.IsTrue(success);
            Assert.AreEqual("\\hello", result);
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
            Assert.AreEqual("1", list[0]);
            Assert.AreEqual("2", list[1]);
            Assert.AreEqual("3", list[2]);
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

    
}