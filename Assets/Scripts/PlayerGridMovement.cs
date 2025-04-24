using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerGridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Tilemap wallTilemap;

    private Vector2 inputDirection = Vector2.zero;
    private Vector2 moveDirection = Vector2.right; // Startbewegung
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        HandleInput();

        if (!isMoving)
        {
            // Richtungswechsel prüfen
            TryChangeDirection();

            // Bewegung fortsetzen, wenn in aktueller Richtung möglich
            if (CanMove(moveDirection))
            {
                MoveToDirection(moveDirection);
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }

    public Vector2 GetCurrentDirection()
    {
        return moveDirection;
    }

    void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0)
            y = 0;

        Vector2 newInput = new Vector2(x, y);

        if (newInput != Vector2.zero)
        {
            inputDirection = newInput;
        }
    }

    void TryChangeDirection()
    {
        if (inputDirection != Vector2.zero && CanMove(inputDirection))
        {
            moveDirection = inputDirection;
        }
    }

    void MoveToDirection(Vector2 dir)
    {
        targetPos = transform.position + (Vector3)dir;
        isMoving = true;

        if (inputDirection != Vector2.zero && CanMove(inputDirection))
        {
            moveDirection = inputDirection;
            UpdateDirectionVisual();
        }
    }

    void UpdateDirectionVisual()
    {
        if (moveDirection == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveDirection == Vector2.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (moveDirection == Vector2.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (moveDirection == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dot"))
        {
            GameManager.Instance.AddScore(10);
            Destroy(other.gameObject);
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
        if (other.gameObject.CompareTag("Ghost"))
        {
            Debug.Log("Kontakt mit Geist!");
            GameManager.Instance.LoseLife();

            // Optional: Pacman "resetten" oder sterben lassen
            // z. B. zurück zur Startposition
            // transform.position = GameManager.Instance.GetPacmanStartPosition();
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