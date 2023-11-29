using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] GameObject cube1;
    [SerializeField] int size = 30;
    [SerializeField] int trackPoints = 40;


    void Start()
    {
        int[,] track = new int[size, size];
        for (int i = 0; i < track.GetLength(0); i++)
        {
            for (int j = 0; j < track.GetLength(1); j++)
            {
                track[i, j] = 0;
            }
        }
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < trackPoints; i++)
        {
            points.Add(new Vector2(Random.Range(0, size + 1), Random.Range(0, size + 1)));
        }
        points = ConvexHull(points, points.Count);

        for (int i = 0; i < points.Count; i++)
        {
            Instantiate(cube1, new Vector3(points[i].x, 0.1f, points[i].y), Quaternion.identity);
            Vector2 start = points[i];
            Vector2 end;
            if (i == points.Count - 1)
                end = points[0];
            else
                end = points[i + 1];

            Debug.Log("Called");
            Pathfinding(start, end, track);
        }

        for (int i = 0; i < track.GetLength(0); i++)
        {
            for (int j = 0; j < track.GetLength(1); j++)
            {
                if (track[i, j] == 1)
                {
                    Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity);
                }
            }
        }

    }
    void Pathfinding(Vector2 start, Vector2 end, int[,] track)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        queue.Enqueue(start);
        visited.Add(start);
        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();
            if (current == end)
            {
                Debug.Log("found path");
                Vector2[] path = ReconstructPath(cameFrom, start, end);
                for (int i = 0; i < path.Length; i++)
                {
                    track[(int)path[i].x, (int)path[i].y] = 1;
                }
                return;
            }
            foreach (Vector2 neighbour in GetNeighbours(current, track))
            {
                if (!visited.Contains(neighbour))
                {
                    queue.Enqueue(neighbour);
                    visited.Add(neighbour);
                    cameFrom[neighbour] = current;
                }
            }
        }

    }
    Vector2[] ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 current = end;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        return path.ToArray();
    }
    Vector2[] GetNeighbours(Vector2 current, int[,] track)
    {
        List<Vector2> neihgbors = new List<Vector2>();
        if (IsPositionValid((int)current.x + 1, (int)current.y, track))
        {
            if (track[(int)current.x + 1, (int)current.y] == 0 || track[(int)current.x + 1, (int)current.y] == 1)
                neihgbors.Add(new Vector2(current.x + 1, current.y));
        }
        if (IsPositionValid((int)current.x - 1, (int)current.y, track))
        {
            if (track[(int)current.x - 1, (int)current.y] == 0 || track[(int)current.x - 1, (int)current.y] == 1)
                neihgbors.Add(new Vector2(current.x - 1, current.y));
        }
        if (IsPositionValid((int)current.x, (int)current.y + 1, track))
        {
            if (track[(int)current.x, (int)current.y + 1] == 0 || track[(int)current.x, (int)current.y + 1] == 1)
                neihgbors.Add(new Vector2(current.x, current.y + 1));
        }
        if (IsPositionValid((int)current.x, (int)current.y - 1, track))
        {
            if (track[(int)current.x, (int)current.y - 1] == 0 || track[(int)current.x, (int)current.y - 1] == 1)
                neihgbors.Add(new Vector2(current.x, current.y - 1));
        }
        return neihgbors.ToArray();
    }

    bool IsPositionValid(int x, int y, int[,] track)
    {
        if (0 <= x && x < track.GetLength(0) && 0 <= y && y < track.GetLength(1))
        {
            return true;
        }
        return false;
    }

    List<Vector2> ConvexHull(List<Vector2> points, int n)
    {
        if (points.Count < 3)
            return null;

        List<Vector2> hull = new List<Vector2>();

        int l = 0;
        for (int i = 1; i < n; i++)
        {
            if (points[i].x < points[l].x)
            {
                l = i;
            }
        }

        int p = l;
        int q = 0;
        while (true)
        {
            hull.Add(points[p]);


            q = (p + 1) % n;

            for (int i = 0; i < n; i++)
            {

                if (orientation(points[p], points[i], points[q]) == 2)
                {
                    q = i;
                }
            }
            p = q;

            if (p == l)
            {
                break;
            }
        }

        return hull;
    }

    int orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        int val = (int)((q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y));

        if (val == 0)
            return 0;
        else if (val > 0)
            return 1;
        else
            return 2;
    }

}

