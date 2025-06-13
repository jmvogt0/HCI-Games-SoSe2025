using UnityEngine;
using System.Collections.Generic;
//https://www.gamersglobal.de/report/pac-man?page=0,3
public class GhostMovement : MonoBehaviour
{
  [Header("Speed Settings")]
  public float normalSpeed = 5f;
  public float tunnelSpeed = 3f;

  [Header("Wall Detection")]
  public LayerMask wallLayerMask;
  public float checkRadius = 0.1f;

  public enum GhostMode { Random, Blinky, Pinky, Inky, Clyde }
  public GhostMode mode = GhostMode.Random;

  [Header("Targets")]
  public Transform chaseTarget;      // z.B. Pacman
  public Transform scatterTarget;    // Eck-Ziel
  public Transform blinkyTransform;  // für Inky

  private bool isInScatterMode = false;
  private float moveSpeed;
  private Vector3 targetPos;
  private Vector3 moveDirection, lastDirection;

  // 4 Grundrichtungen auf XZ-Ebene
  private static readonly Vector3[] directions = {
        Vector3.forward,   // +Z
        Vector3.back,      // -Z
        Vector3.left,      // -X
        Vector3.right      // +X
    };

  void Start()
  {
    moveSpeed = normalSpeed;
    SnapToGrid();
    moveDirection = Vector3.left;
    lastDirection = moveDirection;
    SetNextTarget();
  }

  void Update()
  {
    // Bewegung zum nächsten Grid-Punkt
    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    if (Vector3.Distance(transform.position, targetPos) < 0.01f)
    {
      transform.position = targetPos;
      ChooseNewDirection();
      SetNextTarget();
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("TunnelZone"))
      moveSpeed = tunnelSpeed;
    else if (other.CompareTag("TeleportLeft"))
    {
      transform.position = new Vector3(26.5f, transform.position.y, transform.position.z);
      targetPos = transform.position;
    }
    else if (other.CompareTag("TeleportRight"))
    {
      transform.position = new Vector3(1.5f, transform.position.y, transform.position.z);
      targetPos = transform.position;
    }
  }

  void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("TunnelZone"))
      moveSpeed = normalSpeed;
  }

  void SnapToGrid()
  {
    Vector3 p = transform.position;
    Vector2Int cell = new Vector2Int(
        Mathf.RoundToInt(p.x),
        Mathf.RoundToInt(p.z)
    );
    transform.position = new Vector3(cell.x, p.y, cell.y);
    targetPos = transform.position;
  }

  void SetNextTarget()
  {
    targetPos = transform.position + moveDirection;
  }

  void ChooseNewDirection()
  {
    var possibleDirs = new List<Vector3>();
    foreach (var dir in directions)
    {
      if (dir == -lastDirection) continue;
      if (CanMove(dir))
        possibleDirs.Add(dir);
    }

    if (possibleDirs.Count == 0)
    {
      if (CanMove(-lastDirection))
        moveDirection = -lastDirection;
      // sonst stehen bleiben
    }
    else
    {
      if (mode == GhostMode.Random || chaseTarget == null)
      {
        moveDirection = possibleDirs[Random.Range(0, possibleDirs.Count)];
      }
      else
      {
        // Zielpunkt bestimmen
        Vector3 targetWorld = isInScatterMode
            ? scatterTarget.position
            : chaseTarget.position;

        switch (mode)
        {
          case GhostMode.Blinky:
            // Ziel = Pacman oder Scatter
            break;

          case GhostMode.Pinky:
            // 4 Felder vor Pacman
            // Hole die 3D-Richtung vom FirstPerson-Controller
            var fpgm = chaseTarget.GetComponent<FirstPersonGridMovement>();
            Vector3 pd = fpgm != null
                ? fpgm.GetCurrentDirection()
                : Vector3.zero;
            targetWorld = isInScatterMode
                ? scatterTarget.position
                : chaseTarget.position + pd * 4f;
            break;

          case GhostMode.Clyde:
            float dist = Vector3.Distance(transform.position, chaseTarget.position);
            if (dist >= 8f)
              targetWorld = isInScatterMode ? scatterTarget.position : chaseTarget.position;
            else
              targetWorld = scatterTarget.position;
            break;

          case GhostMode.Inky:
            var fpgm2 = chaseTarget.GetComponent<FirstPersonGridMovement>();
            Vector3 pacDir = fpgm2 != null
                ? fpgm2.GetCurrentDirection()
                : Vector3.forward;
            Vector3 tileAhead = chaseTarget.position + (Vector3)(pacDir * 2f);
            Vector3 V = tileAhead - blinkyTransform.position;
            targetWorld = isInScatterMode
                ? scatterTarget.position
                : blinkyTransform.position + 2f * V;
            break;
        }

        // Kürzeste Distanz wählen
        float shortest = float.PositiveInfinity;
        Vector3 best = possibleDirs[0];
        foreach (var dir in possibleDirs)
        {
          Vector3 check = transform.position + dir;
          float d = Vector3.Distance(check, targetWorld);
          if (d < shortest)
          {
            shortest = d;
            best = dir;
          }
        }
        moveDirection = best;
      }
    }

    lastDirection = moveDirection;
  }

  bool CanMove(Vector3 dir)
  {
    Vector3 checkPos = transform.position + dir;
    // Kreuz aus Linien zeichnen
    float r = checkRadius;
    Debug.DrawLine(checkPos + Vector3.left * r, checkPos + Vector3.right * r, Color.red, 0.1f);
    Debug.DrawLine(checkPos + Vector3.forward * r, checkPos + Vector3.back * r, Color.red, 0.1f);
    return !Physics.CheckSphere(checkPos, checkRadius, wallLayerMask);
  }

  // Hilfsfunktionen
  public void StopMovement() => moveSpeed = 0f;

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
      if (remainingDots <= 60) moveSpeed = normalSpeed * 1.1f;
      if (remainingDots <= 20) moveSpeed = normalSpeed * 1.2f; // Cruise Elroy!
    }
  }

  public void SetScatterMode(bool scatter)
  {
    isInScatterMode = scatter;
    Debug.Log("Ghost " + mode + " is now in " + (scatter ? "scatter" : "chase") + " mode.");
  }
}