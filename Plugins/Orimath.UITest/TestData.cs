namespace Orimath.UITest
{
    public class TestData
    {
        public TestData(int id, string value) { Id = id; Value = value; }
        public int Id { get; set; }
        public string Value { get; set; }
        public override string ToString() { return Id + ":" + Value; }
    }
}
