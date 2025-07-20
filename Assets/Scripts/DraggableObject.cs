using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    private float speed;
    private bool isReturning = false;
    private bool isCorrectSlot = false;
    
    public float minSpeed = 1f;
    public float maxSpeed = 5f;


    private Vector3 _currentTargetPosition;
    private void Start()
    {
        originalPosition = transform.position;
        speed = Random.Range(minSpeed, maxSpeed);
    }
    
    private void Update()
    {
        if (!isDragging && !isReturning)
        {
            Vector3 currentPosition = (_currentTargetPosition)
                - transform.position;
            currentPosition.Normalize();
            
            transform.Translate(currentPosition * speed * Time.deltaTime);
            
            float distance = Vector3.Distance(transform.position, _currentTargetPosition);
            
            Debug.Log($"Distance: {distance}");
            // Проверка достижения правого края (Death Zone)
            if (distance < 1f) // Замените на координату вашей Death Zone
            {
              //  _isEndSide = !_isEndSide;
                GameManager.Instance.LoseLife();
                Destroy(gameObject);
            }
        }
    }
    
    private void OnMouseDown()
    {
        isDragging = true;
    }
    
    private void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }
    
    private void OnMouseUp()
    {
        isDragging = false;
        
        if (!isCorrectSlot)
        {
            isReturning = true;
            StartCoroutine(ReturnToOriginalPosition());
        }
    }
    
    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
            yield return null;
        }
        
        transform.position = originalPosition;
        isReturning = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CorrectSlot"))
        {
            isCorrectSlot = true;
            GameManager.Instance.AddScore();
            Destroy(gameObject);
        }
        else if (other.CompareTag("WrongSlot"))
        {
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
            // Здесь можно добавить эффект взрыва
        }
    }

    public void SetData(Vector3 currentTargetPosition)
    {
        _currentTargetPosition = currentTargetPosition;
    }
}