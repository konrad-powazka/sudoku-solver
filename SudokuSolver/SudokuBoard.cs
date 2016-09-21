using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class SudokuBoard
    {
        private const int NumberOfQuadrantsInOneDirection = 3;
        private const int NumberOfQuadrantCellsInOneDirection = 3;

        public static readonly int NumberOfBoardCellsInSingleDirection
            = NumberOfQuadrantsInOneDirection*NumberOfQuadrantCellsInOneDirection;

        public static readonly int NumberOfBoardCells
            = NumberOfBoardCellsInSingleDirection*NumberOfBoardCellsInSingleDirection;

        public static readonly IReadOnlyCollection<int> ValidNumbers =
            Enumerable.Range(1, NumberOfBoardCellsInSingleDirection).ToList();

        private readonly int?[,] _numbers =
            new int?[NumberOfBoardCellsInSingleDirection, NumberOfBoardCellsInSingleDirection];

        public int? this[int horizontalIndex, int verticalIndex]
        {
            get
            {
                ValidateIndexes(horizontalIndex, verticalIndex);
                return _numbers[horizontalIndex, verticalIndex];
            }
            set
            {
                if (value.HasValue && !ValidNumbers.Contains(value.Value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                ValidateIndexes(horizontalIndex, verticalIndex);
                _numbers[horizontalIndex, verticalIndex] = value;
            }
        }

        public bool IsComplete()
        {
            return _numbers.Flatten().All(n => n.HasValue);
        }

        public bool IsValid()
        {
            var rowsNumbers = _numbers.GetRows();
            var columnsNumbers = _numbers.GetColumns();
            var quadrantsNumbers = GetQuadrantsNumbers();
            var rowsAndColumnsAndQuadrantsNumbers = rowsNumbers.Concat(columnsNumbers).Concat(quadrantsNumbers);

            var areAllRowsAndColumnsAndQuadrantsValid =
                rowsAndColumnsAndQuadrantsNumbers.All(IsRowOrColumnOrQuadrantValid);

            return areAllRowsAndColumnsAndQuadrantsValid;
        }

        private IEnumerable<IEnumerable<int?>> GetQuadrantsNumbers()
        {
            return Enumerable.Range(0, NumberOfQuadrantsInOneDirection)
                .SelectMany(
                    qvi =>
                        Enumerable.Range(0, NumberOfQuadrantsInOneDirection)
                            .Select(qhi => GetQuadrantNumbers(qhi, qvi)));
        }

        private IEnumerable<int?> GetQuadrantNumbers(int quadrantHorizontalIndex, int quadrantVerticalIndex)
        {
            var verticalNumberStartIndex = quadrantVerticalIndex*NumberOfQuadrantCellsInOneDirection;
            var horizontalNumberStartIndex = quadrantHorizontalIndex*NumberOfQuadrantCellsInOneDirection;

            return
                Enumerable.Range(verticalNumberStartIndex, NumberOfQuadrantCellsInOneDirection)
                    .SelectMany(
                        vni =>
                            Enumerable.Range(horizontalNumberStartIndex, NumberOfQuadrantCellsInOneDirection)
                                .Select(hni => this[vni, hni]));
        }

        public bool IsRowOrColumnOrQuadrantValid(IEnumerable<int?> rowOrColumnOrQuadrantNumbers)
        {
            var notEmptyNumbers = rowOrColumnOrQuadrantNumbers.Where(n => n.HasValue).ToArray();
            var notEmptyDistinctNumbers = notEmptyNumbers.Distinct().ToArray();
            var areAnyNumbersDuplicated = notEmptyNumbers.Length != notEmptyDistinctNumbers.Length;

            return !areAnyNumbersDuplicated;
        }

        private static void ValidateIndexes(int horizontalIndex, int verticalIndex)
        {
            ValidateIndex(horizontalIndex);
            ValidateIndex(verticalIndex);
        }

        private static void ValidateIndex(int index)
        {
            if (index < 0 || index >= NumberOfBoardCellsInSingleDirection)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public SudokuBoard Clone()
        {
            var clonedSudokuBoard = new SudokuBoard();

            for (var verticalIndex = 0; verticalIndex < NumberOfBoardCellsInSingleDirection; verticalIndex++)
            {
                for (var horizontalIndex = 0; horizontalIndex < NumberOfBoardCellsInSingleDirection; horizontalIndex++)
                {
                    clonedSudokuBoard[horizontalIndex, verticalIndex] = this[horizontalIndex, verticalIndex];
                }
            }

            return clonedSudokuBoard;
        }

        // TODO: immutable
        public SudokuBoard Solve()
        {
            var clonedBoard = Clone();
            return SolveInternal(clonedBoard);
        }

        public static SudokuBoard SolveInternal(SudokuBoard sudokuBoard)
        {
            if (!sudokuBoard.IsValid())
            {
                return null;
            }

            var cells = Enumerable.Range(0, NumberOfBoardCellsInSingleDirection)
                .SelectMany(
                    vni =>
                        Enumerable.Range(0, NumberOfBoardCellsInSingleDirection)
                            .Select(hni => new Cell(hni, vni, sudokuBoard._numbers))).ToList();

            Debug.Assert(cells.Count == NumberOfBoardCellsInSingleDirection*NumberOfBoardCellsInSingleDirection);

            var rows = Enumerable.Range(0, NumberOfBoardCellsInSingleDirection).Select(rowIndex =>
            {
                var rowCells = cells.Where(c => c.VerticalIndex == rowIndex).ToList();
                return new SolutionRegion(rowCells);
            }).ToList();

            // TODO: rows and cols are mismatched, imporve just for readability
            var columns = Enumerable.Range(0, NumberOfBoardCellsInSingleDirection).Select(columnIndex =>
            {
                var columnCells = cells.Where(c => c.HorizontalIndex == columnIndex).ToList();
                return new SolutionRegion(columnCells);
            }).ToList();

            var quadrants = Enumerable.Range(0, NumberOfQuadrantsInOneDirection).SelectMany(quadrantHorizontalIndex =>
                Enumerable.Range(0, NumberOfQuadrantsInOneDirection)
                    .Select(
                        quadrantVerticalIndex =>
                            new
                            {
                                QuadrantHorizontalIndex = quadrantHorizontalIndex,
                                QuadrantVerticalIndex = quadrantVerticalIndex
                            })).Select(i =>
                            {
                                var quardantCells =
                                    cells.Where(
                                        c =>
                                            c.HorizontalIndex >=
                                            i.QuadrantHorizontalIndex*NumberOfQuadrantCellsInOneDirection &&
                                            c.HorizontalIndex <
                                            (i.QuadrantHorizontalIndex + 1)*NumberOfQuadrantCellsInOneDirection &&
                                            c.VerticalIndex >=
                                            i.QuadrantVerticalIndex*NumberOfQuadrantCellsInOneDirection &&
                                            c.VerticalIndex <
                                            (i.QuadrantVerticalIndex + 1)*NumberOfQuadrantCellsInOneDirection
                                        ).ToList();

                                return new SolutionRegion(quardantCells);
                            }).ToList();

            Debug.Assert(new[] {rows, columns, quadrants}.All(s => s.Count == NumberOfBoardCellsInSingleDirection));

            var solutionRegions = rows.Concat(columns).Concat(quadrants).ToList();

            var cellsToSolutionRegionsMap = cells.ToDictionary(c => c,
                c => (IReadOnlyCollection<SolutionRegion>) solutionRegions.Where(r => r.Cells.Contains(c)).ToList());

            Debug.Assert(cellsToSolutionRegionsMap.Values.All(s => s.Count == 3));

            var cellsToProcess = cells.Where(c => !c.Number.HasValue).ToList();
            var processedCellsBacktrackingInfos = new Stack<CellBacktractingInfo>();

            while (true)
            {
                var currentCellToProcess =
                    cellsToProcess.OrderBy(c => GetCellPossibleValues(c, cellsToSolutionRegionsMap[c]).Count)
                        .FirstOrDefault();

                if (currentCellToProcess == null)
                {
                    return sudokuBoard;
                }

                Debug.Assert(!currentCellToProcess.Number.HasValue);

                var currentCellToProcessPossibleValues = GetCellPossibleValues(currentCellToProcess,
                    cellsToSolutionRegionsMap[currentCellToProcess]);

                var currentCellToProcessBacktrackingInfo = new CellBacktractingInfo(currentCellToProcess,
                    currentCellToProcessPossibleValues);

                do
                {
                    var currentCellToProcessPossibleNumber =
                        currentCellToProcessBacktrackingInfo.GetNextPossibleNumber();

                    if (!currentCellToProcessPossibleNumber.HasValue)
                    {
                        currentCellToProcessBacktrackingInfo.Cell.Number = null;

                        if (!processedCellsBacktrackingInfos.Any())
                        {
                            return null;
                        }

                        currentCellToProcessBacktrackingInfo = processedCellsBacktrackingInfos.Pop();
                        cellsToProcess.Add(currentCellToProcessBacktrackingInfo.Cell);
                        continue;
                    }

                    currentCellToProcessBacktrackingInfo.Cell.Number = currentCellToProcessPossibleNumber;
                    processedCellsBacktrackingInfos.Push(currentCellToProcessBacktrackingInfo);
                    cellsToProcess.Remove(currentCellToProcessBacktrackingInfo.Cell);
                    break;
                } while (true);
            }
        }

        private static IReadOnlyCollection<int> GetCellPossibleValues(Cell cell,
            IReadOnlyCollection<SolutionRegion> cellSolutionRegions)
        {
            if (cell.Number.HasValue)
            {
                return new int[0];
            }

            return
                ValidNumbers.Except(
                    cellSolutionRegions.SelectMany(
                        r => r.Cells.Where(c => c.Number.HasValue).Select(c => c.Number.Value)))
                    .ToList();
        }

        private class Cell
        {
            private readonly int?[,] _numbers;

            public Cell(int horizontalIndex, int verticalIndex, int?[,] numbers)
            {
                _numbers = numbers;
                HorizontalIndex = horizontalIndex;
                VerticalIndex = verticalIndex;
            }

            public int HorizontalIndex { get; }
            public int VerticalIndex { get; }

            public int? Number
            {
                get { return _numbers[HorizontalIndex, VerticalIndex]; }
                set { _numbers[HorizontalIndex, VerticalIndex] = value; }
            }
        }

        private class CellBacktractingInfo
        {
            private readonly Queue<int> _possibleValuesQueue;

            public CellBacktractingInfo(Cell cell, IReadOnlyCollection<int> possibleValues)
            {
                Cell = cell;
                _possibleValuesQueue = new Queue<int>(possibleValues);
            }

            public Cell Cell { get; }

            public int? GetNextPossibleNumber()
            {
                if (!_possibleValuesQueue.Any())
                {
                    return null;
                }

                return _possibleValuesQueue.Dequeue();
            }
        }

        private struct SolutionRegion
        {
            public SolutionRegion(IReadOnlyCollection<Cell> cells)
            {
                if (cells.Count != NumberOfBoardCellsInSingleDirection)
                {
                    throw new ArgumentException();
                }
                Cells = cells;
            }

            public IReadOnlyCollection<Cell> Cells { get; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var rowIndex = 0; rowIndex < NumberOfBoardCellsInSingleDirection; rowIndex++)
            {
                if (rowIndex % NumberOfQuadrantCellsInOneDirection == 0)
                {
                    PrintSeparatorsRow(sb);
                    sb.AppendLine();
                }

                for (var colIndex = 0; colIndex < NumberOfBoardCellsInSingleDirection; colIndex++)
                {
                    if (colIndex % NumberOfQuadrantCellsInOneDirection == 0)
                    {
                        sb.Append("|");
                    }

                    var number = this[rowIndex, colIndex];
                    sb.Append(number);
                }

                sb.Append("|");
                sb.AppendLine();
            }

            PrintSeparatorsRow(sb);

            return sb.ToString();
        }

        private void PrintSeparatorsRow(StringBuilder sb)
        {
            for (var index = 0;
                index < NumberOfBoardCellsInSingleDirection + NumberOfQuadrantsInOneDirection + 1;
                index++)
            {
                sb.Append("-");
            }
        }
    }
}