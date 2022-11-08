namespace Grid.Model
{
    public sealed class GridType
    {
        public string Value { get; }

        private GridType(string value) { Value = value; }

        public static GridType Square { get; } = new("square");
        public static GridType SquareWithDiagonals { get; } = new("squarewithdiagonals");
        public static GridType HorizontalHex { get; } = new("horizontal hex");
        public static GridType VerticalHex { get; } = new("vertical hex");
    }
}
