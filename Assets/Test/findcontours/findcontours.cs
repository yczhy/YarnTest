using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FindContours
{
    // 最外侧必须为空
    private int[][] grid;
    private int width;
    private int height;
    private int LNBD; // last new border
    private int NBD; // new border
    private int NewBorderIndex => borders.Count;
    private List<Border> borders = new();

    public int GetPixel(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < width && j < height)
        {
            return grid[i][j];
        }

        // 外侧均视为空像素
        return 0;
    }

    public int this[int i, int j] => GetPixel(i, j);

    public void Reset()
    {
        this.LNBD = 1;
        this.NBD = 1;
        this.borders.Clear();
        // index 0
        this.borders.Add(new Border(-1, BorderType.None, new List<Vector2Int>(0)));
        // index 1
        var framePath = new List<Vector2Int>();

        int i = 0;
        int j = 0;
        for (i = 0; i < height; i++)
        {
            framePath.Add(new(i, j));
        }

        for (j = 1; j < width; j++)
        {
            framePath.Add(new(i, j));
        }

        for (i = height - 2; i >= 0; i--)
        {
            framePath.Add(new(i, j));
        }

        for (j = width - 2; j > 0; j--)
        {
            framePath.Add(new(i, j));
        }

        this.borders.Add(new Border(-1, BorderType.Frame, framePath));
    }
    
    private void PrintGrid()
    {
        var array = grid.Select(arr =>
        {
            var newArr = arr.Select(num => num.ToString("D2"));
            return string.Join(",", newArr);
        }).ToArray();
        Debug.LogError(string.Join("\n", array));
    }
    
    [Conditional("Test")]
    private void TestPrintGrid()
    {
        PrintGrid();
    }

    public void Test()
    {
        grid = new[]
        {
            new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new[] { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
            new[] { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
            new[] { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
            new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };
        height = grid.Length;
        width = grid[0].Length;
        Reset();
        Find();
    }

    public enum BorderType
    {
        None,
        Frame,
        Outer, // 新边界
        Hole, // 新边界
    }

    public readonly struct Border
    {
        public readonly int parent;
        public readonly BorderType type;
        public readonly List<int> children;
        public readonly List<Vector2Int> path;

        public Border(int parent, BorderType borderType, List<Vector2Int> path)
        {
            this.parent = parent;
            this.type = borderType;
            this.children = new();
            this.path = path;
        }
    }

    private readonly Vector2Int[] neighbors =
    {
        new(0, 0), new(0, 1), new(0, 2), new(1, 2), new(2, 2), new(2, 1), new(2, 0), new(1, 0)
    };

    private readonly int[][] indexs =
    {
        new[] { 0, 1, 2 },
        new[] { 7, 9, 3 },
        new[] { 6, 5, 4 },
    };

    private void CheckStartPoint(int i, int j)
    {
        // step 1
        // 寻找轮廓起点
        // 当前像素为空
        int cur = grid[i][j];
        if (cur == 0) return;

        BorderType type = BorderType.None;
        List<Vector2Int> path = null;

        if (cur == 1 && grid[i][j - 1] == 0)
        {
            // 外轮廓
            type = BorderType.Outer;
            NBD++;
            path = GetPath(i, j, i, j - 1);
        }
        else if (cur >= 1 && grid[i][j + 1] == 0)
        {
            // 洞轮廓
            type = BorderType.Hole;
            NBD++;
            if (cur > 1)
            {
                // 设置上一个新边界
                LNBD = cur;
            }

            path = GetPath(i, j, i, j + 1);
        }

        if (type is BorderType.Outer or BorderType.Hole && path?.Count > 0)
        {
            var lastNewBorder = borders[LNBD];
            // 如果上一边界和新边界类型不同
            // 则上一边界为新边界的父边界 （框-外-洞-外-洞）
            // 否则新边界与上一边界共用父边界
            int parent = lastNewBorder.type == type ? lastNewBorder.parent : LNBD;
            borders.Add(new Border(parent, type, path));
            TestPrintGrid();
        }

        int curBorder = Mathf.Abs(grid[i][j]);
        if (curBorder > 1)
        {
            // 设置上一个新边界
            LNBD = curBorder;
        }
    }

    // 时针方向寻找第一个非0像素
    public Vector2Int FindNeighbor(Vector2Int center, Vector2Int start, bool clockwise = true)
    {
        int dir = clockwise ? 1 : -1;
        int tempX = start.x - center.x + 1;
        int tempY = start.y - center.y + 1;
        int startIdx = indexs[tempX][tempY];
        for (int i = 0, l = neighbors.Length; i < l; i++)
        {
            int curIdx = (startIdx + dir * (i + 1) + l) % l;
            Vector2Int neighbor = neighbors[curIdx];
            int x = neighbor[0] + center[0] - 1;
            int y = neighbor[1] + center[1] - 1;
            if (grid[x][y] != 0)
                return new Vector2Int(x, y);
        }

        return new Vector2Int(-1, -1);
    }

    public bool FindNeighborPath(Vector2Int center, Vector2Int start, List<Vector2Int> ret, bool clockwise = false)
    {
        ret.Clear();

        int dir = clockwise ? 1 : -1;
        int tempX = start.x - center.x + 1;
        int tempY = start.y - center.y + 1;
        int startIdx = indexs[tempX][tempY];
        if (startIdx < 0 || startIdx >= neighbors.Length)
        {
            return false;
        }

        for (int i = 0, l = neighbors.Length; i < l; i++)
        {
            int curIdx = (startIdx + dir * (i + 1) + l) % l;
            Vector2Int neighbor = neighbors[curIdx];
            int x = neighbor[0] + center[0] - 1;
            int y = neighbor[1] + center[1] - 1;
            ret.Add(new Vector2Int(x, y));
            if (grid[x][y] != 0)
                return true;
        }

        return false;
    }

    private readonly List<Vector2Int> temp = new();

    private List<Vector2Int> GetPath(int i, int j, int si, int sj)
    {
        List<Vector2Int> ret = new();

        // 起点
        // i j
        int ci = i; // center i
        int cj = j; // center j
        // 搜寻起点
        // si start i
        // sj start j

        // 顺时针寻找边界的终点
        Vector2Int result = FindNeighbor(new Vector2Int(ci, cj), new Vector2Int(si, sj));
        if (result.x < 0)
        {
            // 未找到 该点为孤点
            grid[i][j] = -NBD;
            return ret;
        }

        // 找到

        ret.Add(new(i, j));

        int ei = result.x; // end i
        int ej = result.y; // end j

        si = ei;
        sj = ej;
        // 不断变换中心点逆时针寻找边界的下一个点 直到回到起点
        // 防止死循环
        // while(true)
        int max = width * height;
        for (int k = 0; k < max; k++)
        {
            Vector2Int center = new Vector2Int(ci, cj);
            Vector2Int start = new Vector2Int(si, sj);
            bool success = FindNeighborPath(center, start, temp);
            if (!success)
            {
                Debug.LogError("Impossible Error");
                return temp;
            }

            // 修改像素标记 表示该点已被检测过
            if (grid[ci][cj + 1] == 0 && temp.Contains(new Vector2Int(ci, cj + 1)))
            {
                grid[ci][cj] = -NBD;
                // 设为负值 防止再次扫描时是被认为是洞边界的起点
                // 中心右侧点必须在逆时针搜索中被检测到
                // 防止下列情况 中间一列 右侧为0 但不能修改值
                // 02 02 02 02 02 02 -2
                // -3 00 00 03 00 00 -2
                // -3 00 00 03 00 00 -2
                // 02 02 02 02 02 02 -2
                // 因为其是4号洞边界的起点
                // 02 02 02 02 02 02 -2
                // -3 00 00 -4 00 00 -2
                // -3 00 00 -4 00 00 -2
                // 02 02 02 02 02 02 -2
            }
            else if (grid[ci][cj] == 1)
            {
                grid[ci][cj] = NBD;
            }

            result = temp[^1];
            ret.Add(result);
            if (ci == ei && cj == ej && result.x == i && result.y == j)
            {
                // 回到起点 结束
                return ret;
            }

            // 新起点为原中心 新中心为新找到的点
            si = ci;
            sj = cj;
            ci = result.x;
            cj = result.y;
        }

        return ret;
    }

    private bool Init(int[][] map)
    {
        int w = map.Length;
        if (w == 0) return false;
        int h = map[0].Length;
        if (h == 0 || map.Any(arr => arr.Length < height)) return false;
        width = w + 2;
        height = h + 2;
        grid = new int[height][];
        for (int i = 0; i < height; i++)
        {
            grid[i] = new int[width];
        }

        for (int j = 1, x = 0; x < w; j++, x++)
        {
            for (int i = 1, y = h - 1; y >= 0; i++, y--)
            {
                grid[i][j] = map[x][y] > 0 ? 1 : 0;
            }
        }

        Reset();

        return true;
    }

    private bool Init(bool[][] map)
    {
        int w = map.Length;
        if (w == 0) return false;
        int h = map[0].Length;
        if (h == 0 || map.Any(arr => arr.Length < height)) return false;
        width = w + 2;
        height = h + 2;
        grid = new int[height][];
        for (int i = 0; i < height; i++)
        {
            grid[i] = new int[width];
        }

        for (int j = 1, x = 0; x < w; j++, x++)
        {
            for (int i = 1, y = h - 1; y >= 0; i++, y--)
            {
                grid[i][j] = map[x][y] ? 1 : 0;
            }
        }

        Reset();

        return true;
    }

    public List<Border> Find()
    {
        List<Border> ret = new();

        for (int i = 0; i < height; i++)
        {
            this.LNBD = 1;
            for (int j = 0; j < width; j++)
            {
                CheckStartPoint(i, j);
            }
        }

        int h = height - 2;

        for (int i = 1, l = borders.Count; i < l; i++)
        {
            var border = borders[i];
            var tfPath = new List<Vector2Int>(border.path);
            for (int j = 0, k = tfPath.Count; j < k; j++)
            {
                Vector2Int loc = tfPath[j];
                loc.x--;
                loc.y--;
                (loc.x, loc.y) = (loc.y, loc.x);
                loc.y = h - loc.y;
                tfPath[j] = loc;
            }

            var tfBorder = new Border(border.parent, border.type, tfPath);
            ret.Add(tfBorder);
        }

        return ret;
    }

    public List<Border> Find(bool[][] map)
    {
        if (!Init(map))
        {
            return new();
        }

        return Find();
    }
}