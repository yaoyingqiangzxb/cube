using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube
{
    public class MagicCube
    {
        private Dictionary<CubeFace, CubeFaceMap> dicCubeFaceMap;

        private Dictionary<CubeFace, Cell[]> dicCells;

        private int level;

        public MagicCube(int level)
        {
            this.dicCells = new Dictionary<CubeFace, Cell[]>();
            this.level = level;

            foreach (CubeFace key in Enum.GetValues(typeof(CubeFace)))
            {
                this.dicCells[key] = new Cell[level * level];
                for (int i = 0; i < level * level; i++)
                {
                    this.dicCells[key][i] = new Cell()
                    {
                        face = key,
                        row = i / level,
                        col = i % level,
                        value = i.ToString()
                    };
                }
            }



            //初始化CubeFaceMap
            this.dicCubeFaceMap = new Dictionary<CubeFace, CubeFaceMap>()
            {
                { CubeFace.Upper, new CubeFaceMap() { self = CubeFace.Upper } },
                { CubeFace.Left, new CubeFaceMap() { self = CubeFace.Left } },
                { CubeFace.Front, new CubeFaceMap() { self = CubeFace.Front } },
                { CubeFace.Right, new CubeFaceMap() { self = CubeFace.Right } },
                { CubeFace.Back, new CubeFaceMap() { self = CubeFace.Back } },
                { CubeFace.Down, new CubeFaceMap() { self = CubeFace.Down } },
            };

            //设置映射关系
            //Uppper
            this.dicCubeFaceMap[CubeFace.Upper].left = this.dicCubeFaceMap[CubeFace.Left];
            this.dicCubeFaceMap[CubeFace.Upper].up = this.dicCubeFaceMap[CubeFace.Back];
            this.dicCubeFaceMap[CubeFace.Upper].right = this.dicCubeFaceMap[CubeFace.Right];
            this.dicCubeFaceMap[CubeFace.Upper].down = this.dicCubeFaceMap[CubeFace.Front];

            //Left
            this.dicCubeFaceMap[CubeFace.Left].left = this.dicCubeFaceMap[CubeFace.Back];
            this.dicCubeFaceMap[CubeFace.Left].up = this.dicCubeFaceMap[CubeFace.Upper];
            this.dicCubeFaceMap[CubeFace.Left].right = this.dicCubeFaceMap[CubeFace.Front];
            this.dicCubeFaceMap[CubeFace.Left].down = this.dicCubeFaceMap[CubeFace.Down];

            //Front
            this.dicCubeFaceMap[CubeFace.Front].left = this.dicCubeFaceMap[CubeFace.Left];
            this.dicCubeFaceMap[CubeFace.Front].up = this.dicCubeFaceMap[CubeFace.Upper];
            this.dicCubeFaceMap[CubeFace.Front].right = this.dicCubeFaceMap[CubeFace.Right];
            this.dicCubeFaceMap[CubeFace.Front].down = this.dicCubeFaceMap[CubeFace.Down];

            //Right
            this.dicCubeFaceMap[CubeFace.Right].left = this.dicCubeFaceMap[CubeFace.Front];
            this.dicCubeFaceMap[CubeFace.Right].up = this.dicCubeFaceMap[CubeFace.Upper];
            this.dicCubeFaceMap[CubeFace.Right].right = this.dicCubeFaceMap[CubeFace.Back];
            this.dicCubeFaceMap[CubeFace.Right].down = this.dicCubeFaceMap[CubeFace.Down];

            //Back
            this.dicCubeFaceMap[CubeFace.Back].left = this.dicCubeFaceMap[CubeFace.Right];
            this.dicCubeFaceMap[CubeFace.Back].up = this.dicCubeFaceMap[CubeFace.Upper];
            this.dicCubeFaceMap[CubeFace.Back].right = this.dicCubeFaceMap[CubeFace.Left];
            this.dicCubeFaceMap[CubeFace.Back].down = this.dicCubeFaceMap[CubeFace.Down];

            //Down
            this.dicCubeFaceMap[CubeFace.Down].left = this.dicCubeFaceMap[CubeFace.Left];
            this.dicCubeFaceMap[CubeFace.Down].up = this.dicCubeFaceMap[CubeFace.Front];
            this.dicCubeFaceMap[CubeFace.Down].right = this.dicCubeFaceMap[CubeFace.Right];
            this.dicCubeFaceMap[CubeFace.Down].down = this.dicCubeFaceMap[CubeFace.Back];

        }

        public Cell GetCell(CubeFace face, int row, int col)
        {
            if (row < 0 || row >= level || col < 0 || col >= level)
            {
                throw new ArgumentException("Invalid coordinate values.");
            }

            return this.dicCells[face][row * this.level + col];
        }

        public Cell[] GetCellLine(CubeFace cubeFace, int layer, bool isVertical) 
        {
            Cell[] cellLine=new Cell[level];

            if (isVertical)
            {
                for (int i = 0; i < level; i++)
                {
                    cellLine[i] = this.dicCells[cubeFace][i * level + layer];
                }
            }
            else
            {
                Array.Copy(this.dicCells[cubeFace], layer * level, cellLine, 0, level);
            }
            return cellLine;
        }

        public Cell[] GetAdjacentCells(Cell cell, bool isAll=false)
        {
            Cell[] arr = this.dicCells[cell.face];
            Cell[] surroundings;
            int index = Array.IndexOf(arr, cell);

            int row = index / level;
            int col = index % level;

            Cell up = row > 0 ? arr[index - level] : null;
            Cell down = row < arr.Length / level - 1 ? arr[index + level] : null;
            Cell left = col > 0 ? arr[index - 1] : null;
            Cell right = col < level - 1 ? arr[index + 1] : null;
            if (isAll)
            {
                Cell upLeft = row > 0 && col > 0 ? arr[index - level - 1] : null;
                Cell upRight = row > 0 && col < level - 1 ? arr[index - level + 1] : null;
                Cell downLeft = row < arr.Length / level - 1 && col > 0 ? arr[index + level - 1] : null;
                Cell downRight = row < arr.Length / level - 1 && col < level - 1 ? arr[index + level + 1] : null;
                surroundings = new Cell[] { up, upRight, right, downRight, down, downLeft, left, upLeft };
            }
            else
            {
                surroundings = new Cell[] { up, right, down, left };
            }
            return surroundings.Where(c => c != null).ToArray();
        }

        public void RotateFace(CubeFace cubeFace, bool isClockwise = true)
        {
            Cell[] cells = this.dicCells[cubeFace];
            Cell[] cellsTamp = new Cell[cells.Length];
            //行列转置旋转90度
            for (int i = 0; i < cells.Length; i++)
            {
                int row = i / level;
                int col = i % level;

                cellsTamp[row * level + col] = isClockwise ? cells[(level - col - 1) * level + row] : cells[col * level + row];
            }
            //赋值
            for (int i = 0; i < this.dicCells[cubeFace].Length; i++)
            {
                this.dicCells[cubeFace][i].Copy(cellsTamp[i]);
            }
            
        }


        public void RotateLayer(CubeFace cubeFace, int layer, bool isVertical, bool isClockwise) 
        {
            //--
            
            
        }
        //FaceMap
        //U:D,R,F,L
        //L:U,F,D,B
        //F:U,R,D,L
        //R:U,B,D,F
        //B:U,R,D,L
        //D:F,L,B,R

        //LayerMap
        //UV:U,B,D,F
        //UH:U,R,B,L
        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // 在每个面上一行一行打印出方块颜色，没有必要考虑魔方深度
            // 每个面的行和列索引最大值都是 level - 1，所以不需要考虑层级
            for (int i = 0; i < this.level; i++)
            {
                sb.Append(' ', level * 2 + 1);
                // 打印 UP 面
                for (int j = 0; j < this.level; j++)
                {
                    sb.Append(this.GetCell(CubeFace.Upper, i, j).value + " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            // 打印 LEFT、FRONT、RIGHT、BACK 四个面
            for (int i = 0; i < this.level; i++)
            {
                for (int j = 0; j < this.level; j++)
                {
                    sb.Append(this.GetCell(CubeFace.Left, i, j).value + " ");
                }
                sb.Append(" ");
                for (int j = 0; j < this.level; j++)
                {
                    sb.Append(this.GetCell(CubeFace.Front, i, j).value + " ");
                }
                sb.Append(" ");
                for (int j = 0; j < this.level; j++)
                {
                    sb.Append(this.GetCell(CubeFace.Right, i, j).value + " ");
                }
                sb.Append(" ");
                for (int j = 0; j < this.level; j++)
                {
                    sb.Append(this.GetCell(CubeFace.Back, i, j).value + " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            // 打印 DOWN 面
            for (int i = 0; i < this.level; i++)
            {
                sb.Append(' ', level * 2 + 1);
                for (int j = 0; j < this.level; j++)
                {

                    sb.Append(this.GetCell(CubeFace.Down, i, j).value + " ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }


    }

    public class Cell
    {
        public CubeFace face;
        public int row;
        public int col;
        public string value;

        public void Copy(Cell cell)
        {
            this.face = cell.face;
            this.row = cell.row;
            this.col = cell.col;
            this.value = cell.value;
        }

        public void SwapValue(Cell cell) 
        {
            (this.face, cell.face) = ( cell.face, this.face);
            (this.row, cell.row) = (cell.row, this.row);
            (this.col, cell.col) = (cell.col, this.col);
            (this.value, cell.value) = (cell.value, this.value);
        }
    }

    
    public class CubeFaceMap 
    {
        public CubeFace self;

        public CubeFaceMap left;
        public CubeFaceMap up;
        public CubeFaceMap right;
        public CubeFaceMap down;
    }


    public enum CubeFace
    {
        Upper,
        Left,
        Front,
        Right,
        Back,
        Down
    }
}
