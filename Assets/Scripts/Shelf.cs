using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    public Transform[] collectibles; // массив предметов
    public List<Transform> slots = new List<Transform>(); // слоты для размещения предметов
    private List<bool> slotOccupied = new List<bool>(); // состояние занятости слотов
    private Vector3 normal_size;

    void Start()
    {
        foreach (var slot in slots)
        {
            slotOccupied.Add(false); // все слоты изначально свободны
        }
    }

    void Update()
    {
        CheckForItemInFront();
    }

    void CheckForItemInFront()
    {
        foreach (Transform collectible in collectibles)
        {
            if (collectible != null)
            {
                if (collectible.position.x > transform.position.x - transform.localScale.x &&
                    collectible.position.x < transform.position.x + transform.localScale.x &&
                    collectible.gameObject.GetComponent<BoxCollider>().enabled &&
                    collectible.position.z < transform.position.z - transform.localScale.x)// проверяем, находится ли предмет в пределах области шкафа
                {
                    for (int i = 0; i < slotOccupied.Count; i++)// ищем первый свободный слот (при условии что они заполняются по порядку)
                    {
                        if (!slotOccupied[i]) // если слот свободен
                        {
                            Transform slotTransform = slots[i];

                            collectible.gameObject.transform.position = slotTransform.position;
                            collectible.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                            slotOccupied[i] = true; // помечаем слот как занятый
                            normal_size = collectible.gameObject.transform.localScale;
                            Debug.Log("Item placed in slot: " + slotTransform.name);
                            break; // ждем...
                        }
                    }
                }
            }
        }
    }
}