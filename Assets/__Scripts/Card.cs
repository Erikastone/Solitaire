using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;//масть карты(C,D,H или S)
    public int rank;//достоинсто карты(1-14)
    public Color color = Color.black;//цвет значков
    public string colS = "Black";//или "Red" имя цвета
    //Этот список хранит все игровые объекты Decorator
    public List<GameObject> decoGOs = new List<GameObject>();
    // Этот список хранит все игровые объекты Pip
    public List<GameObject> piGOs = new List<GameObject>();
    public GameObject back;//игровой обьект рубашки карты
    public CardDefinition def;//Извлекается из DeckXML.xml    
    // Список компонентов SpriteRenderer этого и вложенных в него игровых объектов
    public SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        SetSortOrder(0); // Обеспечит правильную сортировку карт
    }
    // Если spriteRenderers не определен, эта функция определит его
    public void PopulateSpriteRenderes()
    {
        // Если spriteRenderers содержит null или пустой список
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            // Получить компоненты SpriteRenderer этого игрового объекта
            // и вложенных в него игровых объектов
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }
    // Инициализирует поле sortingLayerName во всех компонентах SpriteRenderer
    public void SetSortingLayerName(string tSLN)
    {
        PopulateSpriteRenderes();
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }
    // Инициализирует поле sortingOrder всех компонентов SpriteRenderer
    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderes();
        // Выполнить обход всех элементов в списке spriteRenderers
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == this.gameObject)
            {
                // Если компонент принадлежит текущему игровому объекту, это фон
                tSR.sortingOrder = sOrd;// Установить порядковый номер
                                        // для сортировки в sOrd
                continue;
            }
            // Каждый дочерний игровой объект имеет имя
            // Установить порядковый номер для сортировки, в зависимости от имени
            switch (tSR.gameObject.name)
            {
                case "back":
                    // Установить наибольший порядковый номер
                    // для отображения поверх других спрайтов
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face":// если имя "face"
                default:// или же другое
                        // Установить промежуточный порядковый номер
                        // для отображения поверх фона
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }
    public bool faceUp
    {
        get { return (!back.activeSelf); }
        set { back.SetActive(!value); }
    }
    // Виртуальные методы могут переопределяться в подклассах
    // определением методов с теми же именами
    virtual public void OnMouseUpAsButton()
    {
        print(name);// По щелчку эта строка выведет имя карты
    }
}
[System.Serializable]//доступен для правки в инспекторе
public class Decorator
{
    //этот кдасс хранит информацию из DeckXML о каждом значке на карте
    public string type;//значок определяющий достоинство карты, имеет type="pip"
    public Vector3 loc;//метсо положение спрайта на карте
    public bool flip = false;//признак переворота спрайта по вертикали
    public float scale = 1f;//маштаб спрайта

}
[System.Serializable]
public class CardDefinition
{
    //класс хранит информацию о достоинстве карты
    public string face;//изображение лицевой стороны карты
    public int rank;//достоинство карты 1-13
    public List<Decorator> pips = new List<Decorator>();//значки Pips — это список экземпляров Decorator, отображаемых на лицевой стороне
                                                        // карты, например, десять больших значков масти пик на десятке пик
}

