using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap wallTilemap;

    private Vector3 targetPos;
    private Vector2 moveDirection;
    private Vector2 lastDirection;

    void Start()
    {
        SnapToGrid();
        moveDirection = Vector2.left;
        lastDirection = moveDirection;
        SetNextTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            ChooseNewDirection();
            SetNextTarget();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TeleportLeft"))
        {
            // Teleport zur rechten Seite
            transform.position = new Vector3(26.5f, transform.position.y, 0);
            targetPos = transform.position;
        }

        if (other.CompareTag("TeleportRight"))
        {
            // Teleport zur linken Seite
            transform.position = new Vector3(1.5f, transform.position.y, 0);
            targetPos = transform.position;
        }
    }

    void SnapToGrid()
    {
        Vector3 pos = transform.position;
        transform.position = pos;
        targetPos = pos;
    }

    void SetNextTarget()
    {
        targetPos = transform.position + (Vector3)moveDirection;
    }

    void ChooseNewDirection()
    {
        List<Vector2> possibleDirs = new List<Vector2>();
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions)
        {
            if (dir == -lastDirection) continue; // Rückwärts verboten

            if (CanMove(dir))
                possibleDirs.Add(dir);
        }

        if (possibleDirs.Count > 0)
        {
            moveDirection = possibleDirs[Random.Range(0, possibleDirs.Count)];
            lastDirection = moveDirection;
        }
        else
        {
            // Wenn nur Rückweg offen, dann nehmen
            if (CanMove(-lastDirection))
            {
                moveDirection = -lastDirection;
                lastDirection = moveDirection;
            }
        }
    }

    bool CanMove(Vector2 dir)
    {
        Vector3Int gridPos = wallTilemap.WorldToCell(transform.position + (Vector3)dir);
        return !wallTilemap.HasTile(gridPos);
    }

    public void StopMovement()
    {
        moveSpeed = 0f;
    }
}