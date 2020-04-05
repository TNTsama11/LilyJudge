using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using LitJson;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [HideInInspector]
    public bool isTouch=false;

    public GameObject showCard;
    public Image cardImg;
    public Button enterBtn;
    public Button cancelBtn;
    public Button cardBtn;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject leftText;
    public GameObject rightText;
    public GameObject maskImg;
    public Transform leftDoorP1;
    public Transform leftDoorp2;
    public Transform rightDoorp1;
    public Transform rightDoorp2;
    public CanvasGroup leftImg;
    public CanvasGroup rightImg;
    public GameObject bookNameImg;
    public Text bookNameText;
    public GameObject lostBooksNameText;
    public Transform booksNameContent;
    public Transform booksIndex;
    public Button arrow;
    public Transform booksIndexInP;
    public Transform booksIndexOutP;
    private bool isShowImg = false;
    private bool isBooksIndexShow = false;

    void Awake()
    {
        Instance = this;
        enterBtn.onClick.AddListener(OnEnterBtnClick);
        cancelBtn.onClick.AddListener(OnCancelBtnClick);
        cardBtn.onClick.AddListener(OnCardBtnClick);
        arrow.onClick.AddListener(OnArrowClick);
    }

    private void OnArrowClick()
    {
        if (!isBooksIndexShow)
        {
            isBooksIndexShow = true;
            booksIndex.DOMove(booksIndexInP.position, 0.5f).OnComplete(()=>{
                arrow.transform.DOScaleY(-1f, 0.1f);
            });
        }
        else
        {
            booksIndex.DOMove(booksIndexOutP.position, 0.5f).OnComplete(() => {
                arrow.transform.DOScaleY(1f, 0.1f).OnComplete(()=> {
                    isBooksIndexShow = false;
                });
            });
        }
    }

    private void OnCardBtnClick()
    {
        ShowImg();
    }

    private void Start()
    {
        enterBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        StartCoroutine(GetLostBooksName("Books.json"));
    }


    private void OnCancelBtnClick()
    {
        showCard.SetActive(false);
        showCard.GetComponent<RectTransform>().localScale = Vector3.zero;
        cardImg.fillAmount = 0f;
        enterBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        bookNameImg.SetActive(false);
        isShowImg = false;
    }

    private void OnEnterBtnClick()
    {
        cancelBtn.gameObject.SetActive(false);
        enterBtn.gameObject.SetActive(false);
        LilyJudgeAnim();
        GameManager.Instance.SelectCard();
        GameManager.Instance.PlayEnterMusic();
    }

    public void ShowCard(int id)
    {
        SetCard(id);
        showCard.SetActive(true);
        showCard.GetComponent<RectTransform>().DOScale(1f, 0.5f).SetEase(Ease.InOutCubic);
    }

    private void ShowImg()
    {
        if (isShowImg)
        {
            return;
        }
        isShowImg = true;
        cardImg.GetComponent<Image>().DOFillAmount(1f, 1.5f).SetEase(Ease.InOutSine).OnComplete(() => {
            enterBtn.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(true);
            bookNameImg.SetActive(true);
            StartCoroutine(GetBooksInfo("Books.json"));
        });
    }

    private IEnumerator GetBooksInfo(string file)
    {
        string url = Application.streamingAssetsPath;
        UnityWebRequest www = UnityWebRequest.Get(url+ "/" + file);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            bookNameText.text = "无法找到书名";
        }
        else
        {
            BooksReceive receive = JsonMapper.ToObject<BooksReceive>(www.downloadHandler.text);
            foreach (var item in receive.Data)
            {
                if (item.ID == GameManager.Instance.curId)
                {
                    bookNameText.text = item.Name;
                    break;
                }
            }
        }
    }

    private IEnumerator GetLostBooksName(string file)
    {
        string url = Application.streamingAssetsPath;
        UnityWebRequest www = UnityWebRequest.Get(url + "/" + file);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            BooksReceive receive = JsonMapper.ToObject<BooksReceive>(www.downloadHandler.text);            
            string text = "";
            List<string> nameList = new List<string>();
            foreach (var item in receive.Data)
            {
                foreach(var selected in GameManager.Instance.userDatas)
                {                  
                    if (item.ID == selected.Anime)
                    {
                        text = "0";
                        break;
                    }
                }
                if (text != "0")
                {
                    text = item.Name;
                    nameList.Add(text);                    
                }
                else
                {
                    text = "";
                }
            }
            System.Random random = new System.Random();
            List<string> newNameList = new List<string>();
            foreach(var item in nameList)
            {
                newNameList.Insert(random.Next(newNameList.Count), item);
            }
            foreach(var item in newNameList)
            {
                GameObject obj = Instantiate(lostBooksNameText, booksNameContent);
                obj.GetComponent<Text>().text = item;
            }
        }
        }

    private void SetCard(int id)
    {
        Sprite sprite= Resources.Load("img/" + id.ToString(), typeof(Sprite)) as Sprite;
        if (sprite == null)
        {
            return;
        }
        cardImg.sprite = sprite;
    }

    private void LilyJudgeAnim()
    {
        PlayerPrefs.SetInt("Num", -1);
        leftDoor.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(6.5f);
        leftDoor.transform.DOMove(leftDoorP1.position, 0.8f).SetEase(Ease.InExpo).SetDelay(6.5f).OnComplete(()=> {
            rightDoor.GetComponent<CanvasGroup>().DOFade(1f, 1f);
            rightDoor.transform.DOMove(rightDoorp1.position, 0.8f).SetEase(Ease.InExpo).SetDelay(0.3f).OnComplete(()=> {
                maskImg.GetComponent<CanvasGroup>().DOFade(1f, 1f).OnComplete(()=> {
                    leftDoor.transform.DOMove(leftDoorp2.position, 0.4f).SetDelay(0.5f).SetEase(Ease.OutCirc);
                    rightDoor.transform.DOMove(rightDoorp2.position, 0.4f).SetDelay(0.5f).SetEase(Ease.OutCirc).OnComplete(()=> {
                       
                        leftText.AddComponent<Button>().onClick.AddListener(ReturnMenu);
                        rightText.AddComponent<Button>().onClick.AddListener(ReturnMenu);
                    });
                    leftImg.DOFade(0f, 1.5f).SetEase(Ease.InCirc);
                    rightImg.DOFade(0f, 1.5f).SetEase(Ease.InCirc).OnComplete(() => {
                        GameManager.Instance.isSakura = true;
                        leftText.GetComponent<CanvasGroup>().DOFade(0, 1f).SetEase(Ease.InExpo).SetLoops(2, LoopType.Yoyo);
                        rightText.GetComponent<CanvasGroup>().DOFade(0, 1f).SetEase(Ease.InExpo).SetLoops(2, LoopType.Yoyo);
                    });
                });
            });
        });
    }

    private void ReturnMenu()
    {
        Application.Quit();
    }

    void Update()
    {
        
    }
}
