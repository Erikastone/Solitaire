using System.Collections.Generic;
using UnityEngine;
// Класс SlotDef не наследует MonoBehaviour, поэтому для него не требуется
// создавать отдельный файл на С#.
[System.Serializable]
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stragger;
}
public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;// Используется для ускорения доступа к xml
    public Vector2 multiplier;// Смещение от центра раскладки
    // Ссылки SlotDef
    public List<SlotDef> slotDefs;// Все экземпляры SlotDef для рядов 0-3
    public SlotDef drawPile;
    public SlotDef discarPile;
    //хранит имена всех рядов
    public string[] sortingLayerNames = new string[] {"Row0","Row1",
    "Row2","Row3","Discard","Draw"};
    //эта функция вызывается для чтения файла LayoutXML.xml
    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);//Загрузить XML
        xml = xmlr.xml["xml"][0];//И определяется xml для ускорения доступа к XML
        // Прочитать множители, определяющие расстояние между картами
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        // Прочитать слоты
        SlotDef tSD;
        // slotsX используется для ускорения доступа к элементам <slot>
        PT_XMLHashList slotX = xml["slot"];
        for (int i = 0; i < slotX.Count; i++)
        {
            tSD = new SlotDef();// Создать новый экземпляр SlotDef
            if (slotX[i].HasAtt("type"))
            {
                // Если <slot> имеет атрибут type, прочитать его
                tSD.type = slotX[i].att("type");
            }
            else
            {
                // Иначе определить тип как "slot"; это отдельная карта в ряду
                tSD.type = "slot";
            }
            // Преобразовать некоторые атрибуты в числовые значения
            tSD.x = float.Parse(slotX[i].att("x"));
            tSD.y = float.Parse(slotX[i].att("y"));
            tSD.layerID = int.Parse(slotX[i].att("layer"));
            // Преобразовать номер ряда layerlD в текст layerName
            tSD.layerName = sortingLayerNames[tSD.layerID];/*Поле layerName экземпляра SlotDef обеспечивает правильное перекрытие
карт. В двумерных проектах Unity все ресурсы фактически имеют одну и ту
же координату Z, поэтому необходима дополнительная информация о слоях,
чтобы определить, как должны перекрываться спрайты.*/
            switch (tSD.type)
            {// прочитать дополнительные атрибуты, опираясь на тип слота
                case "slot":
                    tSD.faceUp = (slotX[i].att("faceup") == "1");
                    tSD.id = int.Parse(slotX[i].att("id"));
                    if (slotX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotX[i].att("hiddenby").Split(',');
                        foreach (string s in hiding)
                        {
                            tSD.hiddenBy.Add(int.Parse(s));
                        }
                    }
                    slotDefs.Add(tSD);
                    break;
                case "drawpile":
                    tSD.stragger.x = float.Parse(slotX[i].att("xstagger"));
                    drawPile = tSD;
                    break;
                case "discardpile":
                    discarPile = tSD;
                    break;
            }
        }
    }
}
