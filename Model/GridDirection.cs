namespace Grid.Model
{
    public sealed class GridDirection
    {
        public string Value { get; }

        private GridDirection(string value) { Value = value; }

        public static GridDirection North { get; } = new("north");
        public static GridDirection Northeast { get; } = new("northeast");
        public static GridDirection East { get; } = new("east");
        public static GridDirection Southeast { get; } = new("southeast");
        public static GridDirection South { get; } = new("south");
        public static GridDirection Southwest { get; } = new("southwest");
        public static GridDirection West { get; } = new("west");
        public static GridDirection Northwest { get; } = new("northwest");

        public static Dictionary<GridType, Dictionary<GridDirection, (int x, int y)>> Directions =>
            new()
            {
                {
                    GridType.Square,
                    new()
                    {
                        { North, (0, -1) },
                        { East, (1, 0) },
                        { South, (0, 1) },
                        { West, (-1, 0) },
                    }
                },
                {
                    GridType.SquareWithDiagonals,
                    new()
                    {
                        { North, (0, -1) },
                        { Northeast, (1, -1) },
                        { East, (1, 0) },
                        { Southeast, (1, 1) },
                        { South, (0, 1) },
                        { Southwest, (-1, 1) },
                        { West, (-1, 0) },
                        { Northwest, (-1, -1) }
                    }
                },
                {
                    GridType.VerticalHex,
                    new()
                    {
                        { North, (0, -1) },
                        { Northeast, (1, 0) },
                        { Southeast, (1, 1) },
                        { South, (0, 1) },
                        { Southwest, (-1, 1) },
                        { Northwest, (-1, 0) }
                    }
                },
                {
                    GridType.HorizontalHex,
                    new()
                    {
                        { East, (1, 0) },
                        { Southeast, (1, 1) },
                        { Southwest, (0, 1) },
                        { West, (-1, 0) },
                        { Northwest, (0, -1) },
                        { Northeast, (1, -1) }
                    }
                }
            };
    }
}
