using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public int cardAmount;
    [HideInInspector]
    public string curName;
    [HideInInspector]
    public int curId;
    public static GameManager Instance;
    public GameObject cardPfb;
    [HideInInspector]
    public List<UserData> userDatas = new List<UserData>();
    [HideInInspector]
    public List<GameObject> cards = new List<GameObject>();

    public GameObject effect;
    [HideInInspector]
    public bool isSakura = true;
    private AudioSource audioSource;
    public AudioClip audio1;
    public AudioClip aduio2;
    public AudioClip audio4;

    private bool isMusicIn;
    private bool isMusicOut;
    private Action action;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
    }

    void Start()
    {
        StartCoroutine(GetData());
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audio1;
        audioSource.Play();
        Invoke("PlayStartMusic",2f);
    }

    private void PlayStartMusic()
    {
        audioSource.Stop();
        audioSource.volume = 0f;
        audioSource.clip = aduio2;
        MusicIn();
    }

    private void MusicIn()
    {
        isMusicIn = true;
        audioSource.Play();
    }
    private void MusicOut(Action a=null)
    {
        isMusicOut = true;
        action = a;
    }

    public void PlayEnterMusic()
    {
        MusicOut(()=> {
            audioSource.clip = audio4;
            MusicIn();
        });

    }

    void Update()
    {
        if (isMusicIn)
        {
            audioSource.volume += Time.deltaTime*0.5f;
            if (audioSource.volume >= 1f)
            {
                isMusicIn = false;
            }
        }
        if (isMusicOut)
        {
            audioSource.volume -= Time.deltaTime*0.5f;
            if (audioSource.volume <= 0f)
            {
                isMusicOut = false;
                audioSource.Stop();
                if (action != null)
                {
                    action();
                }
            }
        }

        if (isSakura)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(effect, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 1)
            {
                Instantiate(effect, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Quaternion.identity);
            }
        }

       
    }

    public void LoadScene()
    {
        isSakura = false;
        SceneManager.LoadScene(1);
    }

    public void SelectCard()
    {
        StartCoroutine(Upload(curName, curId));
    }

    private IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://www.lolimakeus.fun:5000/getAnime");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            ReturnData receive = JsonMapper.ToObject<ReturnData>(www.downloadHandler.text);
            Debug.Log(receive.Code);
            if (receive.Code == 1)
            {
                userDatas = receive.Data;
            }
        }
    }

    private IEnumerator Upload(string name,int index)
    {
        WWWForm form = new WWWForm();
        form.AddField("Fatman", name);
        form.AddField("Anime", index);
        UnityWebRequest www = UnityWebRequest.Post("http://www.lolimakeus.fun:5000/saveAnime", form);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Success");
        }
    }

    private void OnSceneLoaded(Scene s,LoadSceneMode l)
    {
        StartCoroutine(LoadCards());      
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator LoadCards()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Content");
        int i = 0;
        while (i<cardAmount)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject obj = Instantiate(cardPfb, go.transform);
            obj.GetComponent<ItemAnime>().id = i;
            foreach (var item in userDatas)
            {                
                if (item.Anime == i)
                {
                    obj.GetComponent<ItemAnime>().Selected(item.Fatman);
                }
                else
                {
                    continue;
                }
            }
            i++;
        }
    }
}
