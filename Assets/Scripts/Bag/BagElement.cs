using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagElement : MonoBehaviour
{
    public string itemName;
    public string itemDes;
    public bool IsRightObj;

    //每一个器材的分数
    public float scoreElement = 1;

    Transform BagUITrans;

    Vector3 targetPos;

    [HideInInspector]
    public bool isPutingToScene;
    public Transform ScenePutParent;

    public delegate void OnPutToBag();
    public OnPutToBag onPutToBag;
    BagController bagControll;
    private void Start()
    {
        BagUITrans = GameObject.Find("Button_PlanAndBag").transform;
        bagControll = BagUITrans.parent.GetComponent<BagController>();
        if (string.IsNullOrEmpty(itemName)) itemName = gameObject.name;
        bagControll.CurElementList.Add(this);

        if(bagControll.mBagType== SceneBagType.Get)
        {
            bagControll.AddItemToContent(itemName, itemDes, gameObject, IsRightObj, true);
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if(isPutingToScene)
        {
            Vector3 m_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            transform.position = Camera.main.ScreenToWorldPoint(m_MousePos);

            if(Input.GetMouseButtonDown(0))
            {
                PutToScene();
            }
            else if(Input.GetMouseButtonDown(1))
            {
                isPutingToScene = false;
                PutToBag();
            }
        }
    }
    private void OnMouseDown()
    {
        if (bagControll == null || bagControll.HasLock || bagControll.mBagType == SceneBagType.Get) return;
        PutToBag();
    }
    public void PutToBag()
    {
        if (bagControll.HasLock) return;
        gameObject.SetActive(false);
        GameObject go = new GameObject("image");
        go.transform.SetParent(BagUITrans);
        go.AddComponent<Image>().overrideSprite = (Sprite)Resources.Load("Images/BagItemSprite/"+ itemName, typeof(Sprite));
        go.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        go.transform.DOMove(BagUITrans.position, 0.5f);
        go.transform.DOScale(Vector3.zero, 0.5f).onComplete = delegate
        {
            bagControll.AddItemToContent(itemName, itemDes,gameObject, IsRightObj,true);
            onPutToBag?.Invoke();
            Destroy(go);
        };
    }
    public void PutToScene()
    {
        if(ScenePutParent!=null)
        {
            isPutingToScene = false;
            transform.SetPositionAndRotation(ScenePutParent.position, ScenePutParent.rotation);
            transform.parent = ScenePutParent;
        }
        else
        {
            PutToBag();
        }
    }
}
