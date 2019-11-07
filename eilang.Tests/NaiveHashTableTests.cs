using Xunit;

namespace eilang.Tests
{
    public class NaiveHashTableTests
    {
        class HashString : IHaveHash
        {
            public string Value { get; }

            public HashString(string value)
            {
                Value = value;
            }
            public int GetHash()
            {
                return Value[0];
            }

            public override string ToString()
            {
                return GetHash().ToString();
            }

            public override bool Equals(object obj)
            {
                return string.Equals((obj as HashString)?.Value, Value);
            }
        }

        [Fact]
        public void TestHashTable()
        {
            var hashTable = new NaiveHashTable<HashString, int>();
            var str = new HashString("hello");
            hashTable.Insert(new HashString("hello"), 1);

            Assert.True(hashTable.Get(new HashString("hello")) == 1);
            
            hashTable.Insert(new HashString("hallååå"), 5);
            Assert.True(hashTable.Get(new HashString("hallååå")) == 5);
        }
    }
}