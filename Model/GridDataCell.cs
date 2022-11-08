namespace Grid.Model
{
    public class GridDataCell<T> : IEquatable<GridDataCell<T>>
        where T : class
    {
        public (int x, int y) Position { get; }

        public T? CellData { get; }

        public Grid<T> Grid { get; }

        public GridDataCell(Grid<T> grid, (int, int) position, T? cellData)
        {
            Grid = grid;
            Position = position;
            CellData = cellData;
        }

        public bool Equals(GridDataCell<T>? other)
        {
            return other != null
                && other.Grid == Grid
                && other.Position == Position
                && other.CellData == CellData;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as GridDataCell<T>);
        }

        public override int GetHashCode()
        {
            return $"{Grid}:{Position}:{CellData}".GetHashCode();
        }
    }
}
