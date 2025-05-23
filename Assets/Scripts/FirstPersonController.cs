using UnityEngine;

public class FirstPersonGridMovement : MonoBehaviour
{
  public float moveSpeed = 5f;
  public float gridSize = 1f;

  private Vector3 targetPos;
  private bool isMoving = false;
  public float rotationAngle = 90f; // Drehung pro Tastendruck in Grad
  private Quaternion targetRotation; // Zielrotation
  private bool isRotating = false;
  private int pendingTurn = 0; // -1 = links, 1 = rechts, 0 = keine

  void Start()
  {
    targetPos = transform.position;
    targetRotation = transform.rotation;
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    Debug.Log("Update - Checking for pending turn: " + pendingTurn);
    if (!isMoving && !isRotating)
    {
      if (pendingTurn != 0)
      {
        Vector3 checkDir = RoundToGrid(Quaternion.Euler(0, pendingTurn * rotationAngle, 0) * transform.forward);
        checkDir.y = 0;
        Vector3 checkOrigin = transform.position + Vector3.up * 0.3f;
        bool blocked = Physics.Raycast(checkOrigin, checkDir, gridSize + 0.1f);
        Debug.Log("Trying to turn. Direction: " + checkDir + ", Ray hit: " + blocked);
        if (!blocked)
        {
          Debug.Log("Turning " + (pendingTurn == -1 ? "left" : "right"));
          targetRotation *= Quaternion.Euler(0, pendingTurn * rotationAngle, 0);
          isRotating = true;
          pendingTurn = 0;
          return; // nach Drehung in diesem Frame keine Vorwärtsbewegung
        }
      }

      Vector3 direction = RoundToGrid(transform.forward);
      direction.y = 0;
      Vector3 destination = transform.position + direction * gridSize;

      Vector3 rayOrigin = transform.position + Vector3.up * 0.3f;
      bool hit = Physics.Raycast(rayOrigin, direction, gridSize + 0.1f);
      if (!hit)
      {
        targetPos = destination;
        isMoving = true;
      }
    }

    HandleInput();

    if (isMoving)
    {
      transform.position = Vector3.MoveTowards(transform.position, targetPos, (moveSpeed * 0.3f) * Time.deltaTime);
      if (Vector3.Distance(transform.position, targetPos) < 0.01f)
      {
        transform.position = targetPos;
        isMoving = false;
      }
    }

    if (isRotating)
    {
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationAngle * Time.deltaTime * 5f);
      if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
      {
        transform.rotation = targetRotation;
        isRotating = false;
      }
    }
  }

  void HandleInput()
  {
    //if (isMoving || isRotating) return;

    // --- Rotation ---
    if (Input.GetKeyDown(KeyCode.A))
    {
      Debug.Log("Input A - Pending turn set to -1");
      pendingTurn = -1;
    }

    if (Input.GetKeyDown(KeyCode.D))
    {
      Debug.Log("Input D - Pending turn set to 1");
      pendingTurn = 1;
    }

    float horizontalInput = Input.GetAxis("Horizontal");
    if (horizontalInput < -0.5f)
    {
      Debug.Log("Controller Left - Pending turn set to -1");
      pendingTurn = -1;
    }
    else if (horizontalInput > 0.5f)
    {
      Debug.Log("Controller Right - Pending turn set to 1");
      pendingTurn = 1;
    }

    // Bewegung erfolgt automatisch → kein Code hier notwendig
  }

  Vector3 RoundToGrid(Vector3 dir)
  {
    Vector3 d = dir.normalized;
    if (Mathf.Abs(d.x) > Mathf.Abs(d.z))
      return new Vector3(Mathf.Sign(d.x), 0, 0);
    else
      return new Vector3(0, 0, Mathf.Sign(d.z));
  }
}