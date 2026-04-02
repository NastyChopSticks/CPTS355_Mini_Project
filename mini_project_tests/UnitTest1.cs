using NUnit.Framework;
using Project;


namespace mini_project_tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            
            Assert.AreEqual(5, TestCase.Add(2, 3));
        }
    }
}