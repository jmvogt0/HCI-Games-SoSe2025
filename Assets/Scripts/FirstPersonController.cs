using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class FirstPersonGridMovement : MonoBehaviour
{
  [Header("Speed Settings")]
  public float moveSpeed = 5f;
  public float gridSize = 1f;

  private Vector3 targetPos;
  private bool isMoving = false;
  public float rotationAngle = 90f; // Drehung pro Tastendruck in Grad
  private Quaternion targetRotation; // Zielrotation
  private bool isRotating = false;
  private int pendingTurn = 0; // -1 = links, 1 = rechts, 0 = keine

  [Header("UI")]
  public TextMeshProUGUI speedText; // für TMP

  void Start()
  {
    targetPos = transform.position;
    targetRotation = transform.rotation;
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    UpdateMoveSpeedFromHeartRate();
    UpdateSpeedUI();
    //Debug.Log("MoveSpeed: " + moveSpeed);
    if (!isMoving && !isRotating)
    {
      if (pendingTurn != 0)
      {
        Vector3 checkDir = RoundToGrid(Quaternion.Euler(0, pendingTurn * rotationAngle, 0) * transform.forward);
        checkDir.y = 0;
        Vector3 checkOrigin = transform.position + Vector3.up * 0.3f;
        bool blocked = Physics.Raycast(checkOrigin, checkDir, gridSize + 0.1f);
        if (!blocked)
        {
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
  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Dot"))
    {
      //Debug.Log("Dot collected!");
      // Score erhöhen
      GameManager.Instance.AddScore(10);
      // Dot entfernen
      Destroy(other.gameObject);
    }
    if (other.CompareTag("TeleportLeft"))
    {
      // Teleport zur rechten Seite
      ResetAfterTeleport(new Vector3(24.5f, 0.6f, -14f), Quaternion.Euler(0, -90f, 0));
      //transform.position = new Vector3(25.5f, transform.position.y, 0);
      //targetPos = transform.position;
    }
    if (other.CompareTag("TeleportRight"))
    {
      // Teleport zur linken Seite
      ResetAfterTeleport(new Vector3(2.5f, 0.6f, -14f), Quaternion.Euler(0, 90f, 0));
      //transform.position = new Vector3(1.5f, transform.position.y, 0);
      //targetPos = transform.position;
    }
    if (other.gameObject.CompareTag("Ghost"))
    {
      Debug.Log("Kontakt mit Geist!");
      GameManager.Instance.LoseLife();
      // Optional: Pacman "resetten" oder sterben lassen
      // z. B. zurück zur Startposition
      // transform.position = GameManager.Instance.GetPacmanStartPosition();
    }
  }

  void HandleInput()
  {
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

    float horizontalInput = Input.GetAxis("Horizontal"); // alte Eingabe
    if (horizontalInput < -0.5f)
    {
      Debug.Log("Keyboard/Legacy Controller Left - Pending turn set to -1");
      pendingTurn = -1;
    }
    else if (horizontalInput > 0.5f)
    {
      Debug.Log("Keyboard/Legacy Controller Right - Pending turn set to 1");
      pendingTurn = 1;
    }

    if (Keyboard.current != null)
    {
      if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
      {
        Debug.Log("Controller/Keyboard Left Arrow - Pending turn set to -1");
        pendingTurn = -1;
      }
      else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
      {
        Debug.Log("Controller/Keyboard Right Arrow - Pending turn set to 1");
        pendingTurn = 1;
      }
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

  void UpdateMoveSpeedFromHeartRate()
  {
    if (HeartRateManager.Instance == null)
      return;

    float hrr = HeartRateManager.Instance.GetHRRPercent(); // 0–1

    // Beispiel: Linearer Mapping zwischen 3 (langsam) und 8 (schnell)
    float minSpeed = 1f;
    float maxSpeed = 5f;

    moveSpeed = Mathf.Lerp(minSpeed, maxSpeed, hrr);
  }

  // Neu hinzugefügt für GhostMovement
  /// <summary>
  /// Gibt die aktuelle Bewegungs-/Blickrichtung auf dem Gitter zurück.
  /// </summary>
  public Vector3 GetCurrentDirection()
  {
    Vector3 dir = RoundToGrid(transform.forward);
    dir.y = 0;
    return dir;
  }
  private void UpdateSpeedUI()
  {
    if (speedText != null)
      speedText.text = $"Speed: {moveSpeed:F1}";
  }
  public void StopMovement()
  {
    isMoving = false;
    isRotating = false;
    pendingTurn = 0;
    targetPos = transform.position;
    targetRotation = transform.rotation;
    moveSpeed = 0f;
  }
 
  public void ResetAfterTeleport(Vector3 newPosition, Quaternion? newRotation = null)
  {
    // Bewegung sofort stoppen
    isMoving = false;
    isRotating = false;
    pendingTurn = 0;

    // Position exakt setzen
    transform.position = newPosition;
    targetPos = newPosition;

    // Rotation zurücksetzen, wenn gewünscht
    if (newRotation.HasValue)
    {
      transform.rotation = newRotation.Value;
      targetRotation = newRotation.Value;
    }
    else
    {
      targetRotation = transform.rotation;
    }

    // Optional: Blickrichtung neu definieren
    // Kann helfen, falls du eine Standardausrichtung (z. B. nach rechts) nach jedem Respawn willst
  }
}