using NUnit.Framework;
using Grid.Model;

namespace Grid.Tests
{
    public class GridTests
    {
        private (int, int) _size, _subgridSize;
        private GridType _type = null!;
        private string _defaultValue = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _size = (5, 5);
            _subgridSize = (2, 2);
            _type = GridType.Square;
            _defaultValue = "foo";
        }

        [Test]
        public void Test_CreateGrid()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            Assert.Multiple(() =>
            {
                Assert.That(grid, Is.Not.Null);
                Assert.That(grid.DefaultValue, Is.EqualTo(_defaultValue));
                Assert.That(_size, Is.EqualTo(grid.Size));
            });
        }

        [Test]
        public void Test_CreateGrid_Get_Simple()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            var cell1 = grid.Get((0, 0)); // default value
            var cell2 = grid.Get((1, 1), "bar"); // substitute default value
            var cell3 = grid.Get(0, 0); // should be same as cell1

            Assert.Multiple(() =>
            {
                Assert.That(cell1.CellData, Is.EqualTo(_defaultValue));
                Assert.That(cell2.CellData, Is.EqualTo("bar"));
                Assert.That(cell1, Is.EqualTo(cell3));
            });
        }

        [Test]
        public void Test_CreateGrid_Simple_GetNeihbors()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            var cell = grid.Get((2, 2), "foo");
            var cellNorth = grid.Get((2, 1), "foo");
            var cellEast = grid.Get((3, 2), "foo");
            var cellSouth = grid.Get((2, 3), "foo");
            var cellWest = grid.Get((1, 2), "foo");

            Assert.Multiple(() =>
            {
                Assert.That(grid.GetNeighbor(cell, GridDirection.North), Is.EqualTo(cellNorth));
                Assert.That(grid.GetNeighbor(cell, GridDirection.East), Is.EqualTo(cellEast));
                Assert.That(grid.GetNeighbor(cell, GridDirection.South), Is.EqualTo(cellSouth));
                Assert.That(grid.GetNeighbor(cell, GridDirection.West), Is.EqualTo(cellWest));
            });
        }

        [Test]
        public void Test_CreateGrid_Simple_GetNeihbors_Negative()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            var cell = grid.Get((0, 0), "foo");
            Assert.Multiple(() =>
            {
                Assert.That(grid.GetNeighbor(cell, GridDirection.North), Is.Null);
                Assert.That(grid.GetNeighbor(cell, GridDirection.West), Is.Null);
            });

            cell = grid.Get((4, 4));
            Assert.Multiple(() =>
            {
                Assert.That(grid.GetNeighbor(cell, GridDirection.South), Is.Null);
                Assert.That(grid.GetNeighbor(cell, GridDirection.East), Is.Null);
            });
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_Simple()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);
            var subgrid = new Grid<string>(_subgridSize, _type, _defaultValue, (2, 2));
            var cell1 = subgrid.Add((0, 0), "foo");
            subgrid.Add(cell1);
            grid.Add(subgrid);

            Assert.Multiple(() =>
            {
                Assert.That(grid.Contains((2, 2)), Is.True);
                Assert.That(grid.GetPositions(true), Is.EqualTo(new List<(int, int)> { (2, 2) }));
            });
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_Complex()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);
            var subgrid1 = new Grid<string>(_subgridSize, _type, _defaultValue, (2, 2));
            var subgrid2 = new Grid<string>(_subgridSize, _type, _defaultValue, (0, 0));

            var cell1 = subgrid1.Add((0, 0), "foo");
            subgrid1.Add(cell1);

            var cell2 = subgrid2.Add((0, 0), "bar");
            subgrid2.Add(cell2);

            grid.Add(subgrid1);
            grid.Add(subgrid2);

            var positions = grid.GetPositions(true);

            Assert.That(grid.Contains((2, 2)), Is.True);
            CollectionAssert.AreEquivalent(positions, new[]{ (0, 0), (2, 2)});
            CollectionAssert.DoesNotContain(positions, (1, 1));
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_Complex2()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);
            var subgrid1 = new Grid<string>((4, 4), _type, _defaultValue, (1, 1));
            var subgrid2 = new Grid<string>(_subgridSize, _type, _defaultValue, (2, 2));

            var cell1 = subgrid1.Add((0, 0), "foo");
            subgrid1.Add(cell1);

            var cell2 = subgrid2.Add((0, 0), "bar");
            subgrid2.Add(cell2);

            subgrid1.Add(subgrid2);

            grid.Add(subgrid1);
            var positions = grid.GetPositions(true);

            CollectionAssert.AreEquivalent(positions, new[]{ (1, 1), (3, 3) });
            CollectionAssert.DoesNotContain(positions, (2, 2));
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_Complex3()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);
            var subgrid1 = new Grid<string>(_subgridSize, _type, _defaultValue, (1, 1));
            var subgrid2 = new Grid<string>(_subgridSize, _type, _defaultValue, (3, 3));

            var fourCoords = new[]{ (0, 0), (0, 1), (1, 0), (1, 1) };
            foreach (var coord in fourCoords)
            {
                subgrid1.Add(new GridDataCell<string>(subgrid1, coord, string.Empty));
                subgrid2.Add(new GridDataCell<string>(subgrid2, coord, string.Empty));
            }

            grid.Add(subgrid1);
            grid.Add(subgrid2);

            CollectionAssert.AreEquivalent(grid.GetPositions(true), new[]{ (1, 1), (1, 2), (2, 1), (2, 2), (3, 3), (3, 4), (4, 3), (4, 4) });
        }

        [Test]
        public void Test_CreateGrid_Get_Throws()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            Assert.Throws<ArgumentException>(() => grid.Get(_size));
            Assert.Throws<ArgumentException>(() => grid.Get((0, -1)));
            Assert.Throws<ArgumentException>(() => grid.Get((-1, 0)));
        }

        [Test]
        public void Test_CreateGrid_Add()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            var result = grid.Add((1, 1), "bar");
            var cell = grid.Get((1, 1));

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(cell));
                Assert.That(result.Position, Is.EqualTo(cell.Position));
                Assert.That(result.CellData, Is.EqualTo(cell.CellData));
            });
        }

        [Test]
        public void Test_CreateGrid_Set_Throws()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);

            Assert.Throws<ArgumentException>(() => grid.Add(_size, "bar"));
            Assert.Throws<ArgumentException>(() =>  grid.Add((-1, 1), "bar"));
            Assert.Throws<ArgumentException>(() =>  grid.Add((1, -1), "bar"));
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_Complex_Throws()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue);
            var subgrid1 = new Grid<string>(_subgridSize, _type, _defaultValue, (1, 1));
            var subgrid2 = new Grid<string>(_subgridSize, _type, _defaultValue, (2, 2));

            var fourCoords = new[]{ (0, 0), (0, 1), (1, 0), (1, 1) };
            foreach (var coord in fourCoords)
            {
                subgrid1.Add(new GridDataCell<string>(subgrid1, coord, string.Empty));
                subgrid2.Add(new GridDataCell<string>(subgrid2, coord, string.Empty));
            }

            Assert.Throws<ArgumentException>(() => {
                grid.Add(subgrid1);
                grid.Add(subgrid2);
            });
        }

        [Test]
        public void Test_CreateGrid_WithSubGrid_NotAllowed_Throws()
        {
            var grid = new Grid<string>(_size, _type, _defaultValue, allowSubgrids: false);
            var subgrid = new Grid<string>(_subgridSize, _type, _defaultValue, (1, 1));

            Assert.Throws<InvalidOperationException>(() => grid.Add(subgrid));
        }
    }
}
