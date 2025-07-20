using UnityEngine;

public class CorrectSlot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Получаем компонент для изменения цвета, если он есть
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Этот метод сработает, когда триггер коллайдера коснется другого коллайдера
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, есть ли у объекта компонент DraggableObject
        if (other.GetComponent<DraggableObject>() != null)
        {
            Debug.Log("Фигура " + other.name + " попала в правильный слот!");
            
            // Здесь можно добавить визуальный эффект, например, короткое осветление
            if (spriteRenderer != null)
            {
                // Пример: сделать слот на мгновение белым
                // StartCoroutine(FlashColor(Color.white));
            }
        }
    }
}