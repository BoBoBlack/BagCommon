using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SceneBagType
{
    Put,//向背包中添加物体
    Get//从背包中取出物体
}

public class BagController : MonoBehaviour
{
    /// <summary>
    /// 当前场景背包的操作方式
    /// </summary>
    public SceneBagType mBagType;
    //是否已锁定
    public bool HasLock = false;

    
    public Button button_bagController;

    public GameObject Image_PlanAndBag;
    public GameObject Image_Plan;
    public GameObject Image_Bag;
    public Image Image_BagContent;

    public Button Button_Plan;
    public Button Button_Bag;

    public Button Button_OverPre;

    public Image Image_BagItemDes;
    public Text Text_BagItemDes;
    //背包中已经存在的元素
    public List<BagItem> CurSelectItemList = new List<BagItem>();
    //场景中所有可放入背包的器材
    public List<BagElement> CurElementList = new List<BagElement>();

    //正确且放入背包的器材列表（实时更新）
    public List<BagElement> curRightSelectList = new List<BagElement>();
    //错误且放入背包的器材列表（实时更新）
    public List<BagElement> curWrongSelectList = new List<BagElement>();
    //正确但未放入背包的器材列表（点击准备完成后更新）
    public List<BagElement> curRightNoSelectList = new List<BagElement>();

    // Start is called before the first frame update
    void Start()
    {
        button_bagController.onClick.AddListener(ClickBagController);
        Button_Plan.onClick.AddListener(ClickButtonPlan);
        Button_Bag.onClick.AddListener(ClickButtonBag);
        Button_OverPre.onClick.AddListener(ClickButtonOverPre);
        HideItemDes();
        if(mBagType== SceneBagType.Get)
        {
            Button_OverPre.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Image_BagItemDes.gameObject.activeInHierarchy)
        {
            Image_BagItemDes.transform.position = Input.mousePosition;
        }
    }
    void ClickBagController()
    {
        Image_PlanAndBag.SetActive(!Image_PlanAndBag.activeInHierarchy);
        Image_Plan.SetActive(true);
        Image_Bag.SetActive(false);
    }
    void ClickButtonPlan()
    {
        Image_Plan.SetActive(true);
        Image_Bag.SetActive(false);
    }
    void ClickButtonBag()
    {
        Image_Plan.SetActive(false);
        Image_Bag.SetActive(true);
    }
    /// <summary>
    /// 准备完成
    /// </summary>
    void ClickButtonOverPre()
    {
        if (HasLock||mBagType== SceneBagType.Get) return;
        HasLock = true;
        AutoShowElementInfoOnBag();
        for (int i = 0; i < CurSelectItemList.Count; i++)
        {
            CurSelectItemList[i].ShowResult();
        }
    }
    /// <summary>
    /// 统计器材选择分数
    /// </summary>
    public float GetBagSelectScore()
    {
        float rightScore = 0;
        for (int i = 0; i < curRightSelectList.Count; i++)
        {
            rightScore += curRightSelectList[i].scoreElement;
        }
        float wrongScore = 0;
        for (int i = 0; i < curWrongSelectList.Count; i++)
        {
            wrongScore += curWrongSelectList[i].scoreElement;
        }
        float score = rightScore - wrongScore > 0 ? rightScore - wrongScore : 0;

        return score;
    }
    /// <summary>
    /// 添加物体到背包
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemDes"></param>
    public void AddItemToContent(string itemName,string itemDes,GameObject obj,bool isRight,bool isSelect)
    {
        for (int i = 0; i < CurSelectItemList.Count; i++)
        {
            if (CurSelectItemList[i].Text_Name.text == itemName) return;
        }
        GameObject itemObj = Instantiate((GameObject)Resources.Load("Prefab/UI/Image_BagItem"));
        itemObj.transform.SetParent(Image_BagContent.transform);
        BagItem item = itemObj.GetComponent<BagItem>();
        CurSelectItemList.Add(item);
        item.InitBagItem(itemName, itemDes,this, obj, isRight, isSelect);

        BagElement element = obj.GetComponent<BagElement>();
        if (isRight&&isSelect)
        {
            curRightSelectList.Add(element);
        }
        else if(!isRight&& isSelect)
        {
            curWrongSelectList.Add(element);
        }
        else if(isRight&&!isSelect)
        {
            curRightNoSelectList.Add(element);
        }
    }
    /// <summary>
    /// 通过物体名称移除背包物体
    /// </summary>
    /// <param name="itemName"></param>
    public void RemoveItem(string itemName)
    {
        for (int i = 0; i < CurSelectItemList.Count; i++)
        {
            if (CurSelectItemList[i].Text_Name.text == itemName)
            {
                Destroy(CurSelectItemList[i].gameObject);
                CurSelectItemList.RemoveAt(i);
            }
        }

        for (int i = 0; i < curRightSelectList.Count; i++)
        {
            if(curRightSelectList[i].itemName== itemName)
            {
                curRightSelectList.RemoveAt(i);
            }
        }
        for (int i = 0; i < curWrongSelectList.Count; i++)
        {
            if (curWrongSelectList[i].itemName == itemName)
            {
                curWrongSelectList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 通过物体实体移除背包物体
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveItemByObject(GameObject obj)
    {
        for (int i = 0; i < CurSelectItemList.Count; i++)
        {
            if (CurSelectItemList[i].connectObj == obj)
            {
                Destroy(CurSelectItemList[i].gameObject);
                CurSelectItemList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 清空背包
    /// </summary>
    public void ClearBag()
    {
        for (int i = 0; i < CurSelectItemList.Count; i++)
        {
            Destroy(CurSelectItemList[i].gameObject);
        }
        CurSelectItemList.Clear();
        CurElementList.Clear();
        curRightSelectList.Clear();
        curWrongSelectList.Clear();
        curRightNoSelectList.Clear();
        HasLock = false;
    }

    public void ShowItemDes(string desStr)
    {
        Image_BagItemDes.gameObject.SetActive(true);
        Text_BagItemDes.text = "\u3000" + desStr;
    }
    public void HideItemDes()
    {
        Image_BagItemDes.gameObject.SetActive(false);
        Text_BagItemDes.text = null;
    }

    void AutoShowElementInfoOnBag()
    {
        for (int i = 0; i < CurElementList.Count; i++)
        {
            if(!CurElementList[i].IsRightObj) continue;
            bool hasSelect = false;
            for (int h = 0; h < CurSelectItemList.Count; h++)
            {
                if(CurElementList[i].itemName== CurSelectItemList[h].Text_Name.text)
                {
                    hasSelect = true;
                    break; 
                }
            }
            if (hasSelect)
                continue;
            else
            {
                CurElementList[i].gameObject.SetActive(false);
                AddItemToContent(CurElementList[i].itemName, CurElementList[i].itemDes, CurElementList[i].gameObject, true, false);
            }
        }
    }
}
