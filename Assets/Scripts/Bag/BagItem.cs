using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BagItem : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    public Image Image_ItemTexture;
    public GameObject Image_Right;
    public GameObject Image_Wrong;
    public GameObject Image_RightNoSelect;
    public Text Text_Name;
    public string ItemDesStr;
    public GameObject connectObj;
    public bool hasSelect;
    public bool isRightObj;
    BagController bagControll;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitBagItem(string name,string desStr,BagController controll,GameObject Obj,bool isRight,bool isSlect)
    {
        Text_Name.text = name;
        ItemDesStr = desStr;
        bagControll = controll;
        connectObj = Obj;
        isRightObj = isRight;
        hasSelect = isSlect;
        Image_ItemTexture.overrideSprite= (Sprite)Resources.Load("Images/BagItemSprite/" + name, typeof(Sprite));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //鼠标移入，显示信息
        if (bagControll != null)
            bagControll.ShowItemDes(ItemDesStr);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bagControll != null && bagControll.HasLock) return;
        if(eventData.button== PointerEventData.InputButton.Right)
        {
            if(bagControll != null && bagControll.mBagType== SceneBagType.Put)
            {
                //右键点击，移出背包
                connectObj.SetActive(true);
                bagControll.RemoveItem(Text_Name.text);
                if (bagControll != null)
                    bagControll.HideItemDes();
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (bagControll != null && bagControll.mBagType == SceneBagType.Get)
            {
                //左键点击，从背包中取出
                if(connectObj!=null)
                {
                    connectObj.gameObject.SetActive(true);
                    connectObj.GetComponent<BagElement>().isPutingToScene = true;
                    bagControll.RemoveItemByObject(connectObj);
                    bagControll.HideItemDes();
                    bagControll.Image_PlanAndBag.SetActive(false);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (bagControll != null)
            bagControll.HideItemDes();
    }

    public void ShowResult()
    {
        if(hasSelect)
        {
            Image_RightNoSelect.SetActive(false);
            if (isRightObj)
            {
                Image_Right.SetActive(true);
                Image_Wrong.SetActive(false);
            }
            else
            {
                Image_Right.SetActive(false);
                Image_Wrong.SetActive(true);
            }
        }
        else
        {
            Image_RightNoSelect.SetActive(true);
            Image_Right.SetActive(false);
            Image_Wrong.SetActive(false);
        }
    }
}
