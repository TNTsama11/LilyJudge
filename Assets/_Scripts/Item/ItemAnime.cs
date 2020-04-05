using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ItemAnime : MonoBehaviour
{
    private Button btn;
    public int id;
    public Text nameText;
    public GameObject bg;
    void Awake()
    {
        
    }

    void Start()
    {
        this.GetComponent<RectTransform>().localScale = Vector3.zero;
        this.GetComponent<RectTransform>().DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        if (UIController.Instance.isTouch)
        {
            return;
        }
        if (PlayerPrefs.HasKey("Num"))
        {
            int num = PlayerPrefs.GetInt("Num");
            if (num == -1)
            {
                btn.interactable = false;
                ShowToast.MakeToast("不能贪心哦");
                return;
            }
            if (num >= 2)
            {
                btn.interactable = false;
                ShowToast.MakeToast("哦嚯，机会用完了，怎么办呢嘻嘻");
                return;
            }
            else
            {               
                num++;
                ShowToast.MakeToast("2次机会你用掉" + num + "次了");
                PlayerPrefs.SetInt("Num", num);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Num", 1);
            ShowToast.MakeToast("2次机会你用掉1次了");
        }
        UIController.Instance.isTouch = true;
        this.gameObject.GetComponent<RectTransform>().DOScale(0.8f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(()=> {
            UIController.Instance.isTouch = false;
            UIController.Instance.ShowCard(id);
            GameManager.Instance.curId = id;
        });
    }

    public void Selected(string name)
    {
        if (btn == null)
        {
            btn = this.gameObject.GetComponent<Button>();
        }
        this.gameObject.GetComponent<Image>().sprite = Resources.Load("img/" + id.ToString(), typeof(Sprite)) as Sprite;
        btn.interactable = false;
        nameText.text = name;
        nameText.gameObject.SetActive(true);
        bg.SetActive(true);
    }

    void Update()
    {
        
    }
}
