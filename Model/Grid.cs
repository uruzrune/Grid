using System.Data;

namespace Grid.Model
{
    public class Grid<T> : IEquatable<Grid<T>>
        where T : class
    {
        public Guid Id { get; }

        public (int Width, int Height) Size { get; private set; }

        public (int x, int y) Origin { get; private set; }

        public GridType GridType { get; }

        public T? DefaultValue { get; }

        public bool AllowsSubgrids { get; }

        private readonly Dictionary<(int, int), GridDataCell<T>> _grid;

        private readonly Dictionary<(int, int), Grid<T>> _subgrids;

        public Grid((int, int) size, GridType? gridType = null, T? defaultValue = default, (int, int) origin = default, bool allowSubgrids = true)
        {
            Id = Guid.NewGuid();
            Size = size;
            Origin = origin;
            GridType = gridType ?? GridType.Square;
            DefaultValue = defaultValue;
            AllowsSubgrids = allowSubgrids;

            _grid = new Dictionary<(int, int), GridDataCell<T>>();
            _subgrids = new Dictionary<(int, int), Grid<T>>();
        }

        public GridDataCell<T> Get(int x, int y, T? defaultValue = default)
        {
            return Get((x, y), defaultValue);
        }

        public GridDataCell<T> Get((int x, int y) position, T? defaultValue = default)
        {
            if (IsOutOfBounds((position.x, position.y)))
            {
                throw new ArgumentException($"position {position} is out of bounds of {Size}!");
            }

            // if the point isn't present throughout the grid and subgrids, add it to the top level
            if (!Contains(position))
            {
                _grid[position] = new GridDataCell<T>(this, position, CheckDefaultValue(defaultValue));
            }

            // top level
            if (_grid.ContainsKey(position))
            {
                return _grid[position];
            }

            // subgrid
            var subgrid = _subgrids.Values.First(sg => sg.Contains(CalculateRelativePosition(position, sg.Origin)));
            return subgrid.Get(CalculateRelativePosition(position, subgrid.Origin));
        }

        public GridDataCell<T> Add(int x, int y, T value)
        {
            return Add((x, y), value);
        }

        public GridDataCell<T> Add((int, int) position, T value)
        {
            return Add(new GridDataCell<T>(this, position, value));
        }

        public GridDataCell<T> Add(GridDataCell<T> cell)
        {
            if (IsOutOfBounds(cell.Position))
            {
                throw new ArgumentException($"position {cell.Position} is out of bounds of {Size}!");
            }

            _grid[cell.Position] = cell;

            return _grid[cell.Position];
        }

        public void Add(Grid<T> subgrid)
        {
            if (subgrid == null)
            {
                throw new ArgumentException("subgrid is null!");
            }

            if (!AllowsSubgrids)
            {
                throw new InvalidOperationException("AllowsSubgrid = false!");
            }

            if (GridType != subgrid.GridType)
            {
                throw new InvalidOperationException($"grid types don't match! {GridType} & {subgrid.GridType}");
            }

            if (_subgrids.ContainsKey(subgrid.Origin))
            {
                throw new ArgumentException($"a subgrid already exists at origin ({subgrid.Origin.x},{subgrid.Origin.y})!");
            }

            var positions = GetPositions(true);
            if (positions.Contains(subgrid.Origin))
            {
                throw new ArgumentException($"starting position {subgrid.Origin} is contained by a subgrid!");
            }

            if (subgrid.GetPositions(true).Intersect(positions).Any())
            {
                throw new ArgumentException("grid intersects with subgrids!");
            }

            _subgrids.Add(subgrid.Origin, subgrid);
        }

        public bool Contains((int x, int y) position)
        {
            if (position.x < 0 || position.y < 0 || position.x >= Size.Width || position.y >= Size.Height)
            {
                return false;
            }

            return _grid.ContainsKey(position) ||
                _subgrids.Values.Any(sg => sg.Contains(CalculateRelativePosition(position, sg.Origin)));
        }

        public IEnumerable<(int, int)> GetPositions(bool addOrigin = false)
        {
            var results = _grid.Keys
                .Union(_subgrids.SelectMany(sg => sg.Value.GetPositions(addOrigin)));

            return addOrigin
                ? results
                    .Select(position => (position.Item1 + Origin.x, position.Item2 + Origin.y))
                : results;
        }

        public GridDataCell<T>? GetNeighbor(GridDataCell<T> source, GridDirection direction)
        {
            if (source == null)
            {
                throw new ArgumentException("source cell can't be null!");
            }

            if (!IsSameGrid(source.Grid))
            {
                throw new ArgumentException("source cell not in grid!");
            }

            if (!GridDirection.Directions[GridType].ContainsKey(direction))
            {
                throw new InvalidOperationException($"grid direction {direction} is not valid for grid type {GridType}!");
            }

            var (x, y) = GridDirection.Directions[GridType][direction];
            var newPosition = (source.Position.x + x, source.Position.y + y);

            return IsOutOfBounds(newPosition)
                ? null
                : Get(newPosition);
        }

        public override string ToString()
        {
            return $"{Id}:{Size}:{Origin}:{GridType}:{DefaultValue}:{AllowsSubgrids}";
        }

        public bool Equals(Grid<T>? grid)
        {
            return grid != null && grid.Id == Id;
        }

        public override bool Equals(object? target)
        {
            return target is Grid<T> grid && Equals(grid);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private bool IsOutOfBounds((int x, int y) position)
        {
            return position.x >= Size.Width || position.y >= Size.Height || position.x < 0 || position.y < 0;
        }

        private T? CheckDefaultValue(T? defaultValue)
        {
            if (defaultValue == null || defaultValue == default)
            {
                defaultValue = DefaultValue;
            }

            return defaultValue;
        }

        private static (int, int) CalculateRelativePosition((int x, int y) position, (int x, int y) origin)
        {
            return (position.x - origin.x, position.y - origin.y);
        }

        private bool IsSameGrid(Grid<T> grid)
        {
            return this == grid || _subgrids.Values.Any(sg => sg.IsSameGrid(grid));
        }
    }
}
