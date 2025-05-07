using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

//https://www.gamersglobal.de/report/pac-man?page=0,3
public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float normalSpeed = 5f;
    public float tunnelSpeed = 3f;
    
    public Tilemap wallTilemap;
    public enum GhostMode { Random, Blinky, Pinky, Inky, Clyde }
    public GhostMode mode = GhostMode.Random;

    public Transform chaseTarget; // z. B. Pacman
    public Transform scatterTarget; // Home Position auf dem Grid
    public Transform blinkyTransform;

    private bool isInScatterMode = false;

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
        if (other.CompareTag("TunnelZone"))
        {
            moveSpeed = tunnelSpeed;
        }
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
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TunnelZone"))
        {
            moveSpeed = normalSpeed;
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
            if (dir == -lastDirection) continue;

            if (CanMove(dir))
                possibleDirs.Add(dir);
        }

        if (possibleDirs.Count == 0)
        {
            if (CanMove(-lastDirection))
            {
                moveDirection = -lastDirection;
                lastDirection = moveDirection;
            }
            return;
        }

        if (mode == GhostMode.Random || chaseTarget == null)
        {
            moveDirection = possibleDirs[Random.Range(0, possibleDirs.Count)];
        }
        else if (mode == GhostMode.Blinky)
        {
            Vector3 targetPosWorld = isInScatterMode ? scatterTarget.position : chaseTarget.position;
            float shortestDist = Mathf.Infinity;
            Vector2 bestDir = possibleDirs[0];

            foreach (Vector2 dir in possibleDirs)
            {
                Vector3 checkPos = transform.position + (Vector3)dir;
                float dist = Vector3.Distance(checkPos, targetPosWorld);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    bestDir = dir;
                }
            }

            moveDirection = bestDir;
        }
        else if (mode == GhostMode.Pinky && chaseTarget != null)
        {
            // Pinky: Ziel = 4 Tiles in Blickrichtung von Pacman
            Vector2 pacmanDir = chaseTarget.GetComponent<PlayerGridMovement>().GetCurrentDirection();
            Vector3 targetPosWorld = isInScatterMode ? scatterTarget.position : chaseTarget.position + (Vector3)(pacmanDir * 4f);

            float shortestDist = Mathf.Infinity;
            Vector2 bestDir = possibleDirs[0];

            foreach (Vector2 dir in possibleDirs)
            {
                Vector3 checkPos = transform.position + (Vector3)dir;
                float dist = Vector3.Distance(checkPos, targetPosWorld);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    bestDir = dir;
                }
            }

            moveDirection = bestDir;
        }
        else if (mode == GhostMode.Clyde && chaseTarget != null && scatterTarget != null)
        {
            float distanceToPacman = Vector3.Distance(transform.position, chaseTarget.position);
            Vector3 targetPosWorld;

            if (distanceToPacman >= 8f)
            {
                // Jagd-Modus (wie Blinky)
                targetPosWorld = isInScatterMode ? scatterTarget.position : chaseTarget.position;
            }
            else
            {
                // Rückzug zur Ecke
                targetPosWorld = scatterTarget.position;
            }

            float shortestDist = Mathf.Infinity;
            Vector2 bestDir = possibleDirs[0];

            foreach (Vector2 dir in possibleDirs)
            {
                Vector3 checkPos = transform.position + (Vector3)dir;
                float dist = Vector3.Distance(checkPos, targetPosWorld);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    bestDir = dir;
                }
            }

            moveDirection = bestDir;
        }
        else if (mode == GhostMode.Inky && chaseTarget != null && blinkyTransform != null)
        {
            // 1. Zielpunkt 2 Tiles vor Pacman
            Vector2 pacmanDir = chaseTarget.GetComponent<PlayerGridMovement>().GetCurrentDirection();
            Vector3 tileAhead = chaseTarget.position + (Vector3)(pacmanDir * 2f);

            // 2. Vektor von Blinky zu diesem Punkt
            Vector3 V = tileAhead - blinkyTransform.position;

            // 3. Ziel = BlinkyPos + 2 * V
            Vector3 targetPosWorld = isInScatterMode ? scatterTarget.position : blinkyTransform.position + 2f * V;

            // 4. Richtung auswählen wie immer
            float shortestDist = Mathf.Infinity;
            Vector2 bestDir = possibleDirs[0];

            foreach (Vector2 dir in possibleDirs)
            {
                Vector3 checkPos = transform.position + (Vector3)dir;
                float dist = Vector3.Distance(checkPos, targetPosWorld);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    bestDir = dir;
                }
            }

            moveDirection = bestDir;
        }

        lastDirection = moveDirection;
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

    public void ReverseDirection()
    {
        moveDirection = -moveDirection;
        lastDirection = moveDirection;
        SetNextTarget();
    }

    public void UpdateSpeedBasedOnDots(int remainingDots)
    {
        if (mode == GhostMode.Blinky)
        {
            if (remainingDots <= 60) moveSpeed = 5.5f;
            if (remainingDots <= 20) moveSpeed = 6.0f; // Cruise Elroy!
        }
    }

    public void SetScatterMode(bool scatter)
    {
        isInScatterMode = scatter;
    }
}