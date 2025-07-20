using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    public float spawnInterval = 2f;
    public Transform _startZone; // Spawn Zone (y=350)
    public Transform _endZone; // Spawn Zone (y=350)
    public float[] yOffsets = new float[] { 2, 0, -2 };
    
    
    private float timer = 0f;
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
           // spawnInterval = Random.Range(1f, 3f); // Случайный интервал между спавном
        }
    }
    
    private void SpawnObject()
    {
        if (objectPrefabs.Length == 0) return;
        
        int randomIndex = Random.Range(0, objectPrefabs.Length);
        GameObject newObject = Instantiate(objectPrefabs[randomIndex], _startZone.position, Quaternion.identity);
        DraggableObject draggableObject = newObject.GetComponent<DraggableObject>();
        
        int randomOffsetIndex = Random.Range(0, yOffsets.Length);
        Vector3 targetPos = _endZone.position + new Vector3(0f, yOffsets[randomOffsetIndex], 0f);
        draggableObject.SetData(targetPos);
        
        
        // Настройка начальной позиции с небольшим случайным смещением
        float randomYOffset = yOffsets[randomOffsetIndex];
        newObject.transform.position += new Vector3(0, randomYOffset, 0);
    }
}