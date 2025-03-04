using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false; // перетаскивается ли в данный момент обьект
    private bool isLookingAround = true; // производится ли перемещение камеры в данный момент
    private Vector3 offset;
    public float size_increment; // увеличение обьекта при нажатии
    private Vector3 scaling;
    private bool draggable = false; // костыль чтобы обьекты не уменьшались при касаниях по случакйным местам на экране, на мой взгляд не перегружает код
    private GameObject draggable_object;
    private Vector2 first_touch;
    public float Sensitivity;
    public float camera_border_left; //границы камеры
    public float camera_border_right;

    void Start()
    {
        mainCamera = Camera.main;
        scaling =  new Vector3(size_increment, size_increment, size_increment); // увеличение при взаимодействии, теперь в векторах
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    first_touch = touch.position;
                    Ray ray = mainCamera.ScreenPointToRay(touch.position); // через raycast определяем чего коснулся игрок
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.gameObject.CompareTag("Draggable")) // проверяем тэг нажатого предмета
                        {
                            draggable_object = hit.transform.gameObject;
                            draggable_object.GetComponent<Rigidbody>().isKinematic = true; // на время перетаскивания отключаем возможность взаимодействия с физикой, лучше уж пройти через текстуры насквозь, чем застрять в них
                            draggable_object.GetComponent<BoxCollider>().enabled = false;
                            isDragging = true; //тащим...
                            isLookingAround = false; //по сторонам в этот момент не смотрим
                            draggable = true;
                            offset = draggable_object.transform.position - hit.point; 
                            draggable_object.transform.localScale += scaling; //для большей отдачи при взаимодействии немного увеличиваем обьект
                        }
                        else
                        {
                            isLookingAround = true; // если ничего не тащим, значит двигаем сцену (вернее камеру по сцене)
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging) //перенос обьектов
                    {
                        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane + Random.Range(9.0f, 15.0f)));//для визуального разнообразия роняем обьекты на случайном расстоянии от камеры
                        draggable_object.transform.position = mouseWorldPos + offset;//непосредственно перемещение
                    }
                    if (isLookingAround) //движение по сцене
                    {
                        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, 0, mainCamera.nearClipPlane));
                        float newXPosition = mainCamera.transform.position.x - (first_touch.x - touch.position.x) / -Sensitivity;
                        newXPosition = Mathf.Clamp(newXPosition, camera_border_left, camera_border_right);//ограничиваем область
                        mainCamera.transform.position = new Vector3(newXPosition, 5.0f, mainCamera.transform.position.z);//двигаем камеру
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;//отпускаем обьект
                    if (draggable)
                        {
                            draggable_object.transform.localScale -= scaling;
                            draggable = false;
                            draggable_object.GetComponent<Rigidbody>().isKinematic = false;//возвращаем гравитацию и столкновения
                            draggable_object.GetComponent<BoxCollider>().enabled = true;
                        }
                    break;//ждем чуда
            }
        }
    }
}
