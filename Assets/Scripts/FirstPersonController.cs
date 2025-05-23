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

  void Start()
  {
    targetPos = transform.position;
    targetRotation = transform.rotation;
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    HandleInput();

    if (isMoving)
    {
      transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
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
    if (isMoving || isRotating) return;

    // --- Rotation ---
    if (Input.GetKeyDown(KeyCode.A))
    {
      targetRotation *= Quaternion.Euler(0, -rotationAngle, 0);
      isRotating = true;
      return; // ← Drehung hat Priorität, danach keine Bewegung
    }

    if (Input.GetKeyDown(KeyCode.D))
    {
      targetRotation *= Quaternion.Euler(0, rotationAngle, 0);
      isRotating = true;
      return;
    }

    // --- Bewegung ---
    Vector3 direction = Vector3.zero;

    if (Input.GetKeyDown(KeyCode.W)) direction = transform.forward;
    else if (Input.GetKeyDown(KeyCode.S)) direction = -transform.forward;
    else return; // ← Kein Bewegungsinput → abbrechen

    direction = RoundToGrid(direction);
    direction.y = 0;

    if (direction != Vector3.zero)
    {
      Vector3 destination = transform.position + direction * gridSize;

      Vector3 rayOrigin = transform.position + Vector3.up * 0.3f;
      //Debug.DrawRay(rayOrigin, direction * (gridSize + 0.3f), Color.red, 5f);
      bool hit = Physics.Raycast(rayOrigin, direction, gridSize + 0.1f);
      //Debug.Log("Raycast hit wall: " + hit);
      if (!hit)
      {
        targetPos = destination;
        isMoving = true;
      }
    }

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