using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public settings _settings;

    private Camera currentCamera;
    private Image wheatAmount, coinImage;
    private bool isCoinImageBusy;
    private PoolOfGameObjects getCoinForEffect;
    private TextMeshProUGUI moneyTextMesh, currentPack, maxPack;
    [SerializeField] private int money = 0;
    [SerializeField] private int howMuchForOneStack;
    [SerializeField] private int maxBagCapacity;

    // Start is called before the first frame update
    void Start()
    {
        howMuchForOneStack = _settings.howMuchForOneStack;
        maxBagCapacity = _settings.MaxBagCapacity;
        moneyTextMesh = GameObject.Find("Money").GetComponent<TextMeshProUGUI>();
        currentPack = GameObject.Find("current").GetComponent<TextMeshProUGUI>();
        maxPack = GameObject.Find("max").GetComponent<TextMeshProUGUI>();
        maxPack.text = maxBagCapacity.ToString();
        currentPack.text = "0";
        moneyTextMesh.text = "0";

        getCoinForEffect = new PoolOfGameObjects(50, Resources.Load<GameObject>("Coin"), transform);

        coinImage = GameObject.Find("CoinImage").GetComponent<Image>();
        wheatAmount = GameObject.Find("wheatAmount").GetComponent<Image>();
        wheatAmount.fillAmount = 0;

        currentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
    }



    public void SetWheatAmount(int currentAmount)
    {
        wheatAmount.fillAmount = (float)currentAmount / (float)maxBagCapacity;
        currentPack.text = currentAmount.ToString();
    }

    public void SetEffectOfCoins()
    {
        StartCoroutine(coinsFlyToCoinIcon());        
    }

    private IEnumerator AddCoins(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            money += 1;
            moneyTextMesh.text = money.ToString();
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator coinsFlyToCoinIcon()
    {        
        Transform Coin = getCoinForEffect.GetFreeObject().transform;
        Coin.localScale = Vector3.zero;
        Coin.gameObject.SetActive(true);
        Coin.position = currentCamera.WorldToScreenPoint(GameObject.Find("PointOfBarn").transform.position);

        Sequence seq = DOTween.Sequence();

        seq.Join(Coin.DOScale(1, 1));
        seq.Join(Coin.DOMove(new Vector3(coinImage.transform.position.x-150, coinImage.transform.position.y-150, coinImage.transform.position.z), 1));

        yield return new WaitForSeconds(1f);
        Coin.gameObject.SetActive(false);
        StartCoroutine(AddCoins(howMuchForOneStack));

        if (!isCoinImageBusy)
        {
            isCoinImageBusy = true;
            coinImage.transform.DOShakeScale(0.3f);
            yield return new WaitForSeconds(0.3f);
            isCoinImageBusy = false;
        }
    }



}

public class PoolOfGameObjects : MonoBehaviour
{
    private GameObject[] objects;

    public PoolOfGameObjects(int capacity, GameObject _object, Transform _transform)
    {
        objects = new GameObject[capacity];
        for (int i = 0; i < capacity; i++)
        {
            objects[i] = Instantiate(_object, _transform);
            objects[i].SetActive(false);
        }
    }

    public GameObject GetFreeObject()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (!objects[i].activeSelf)
            {
                return objects[i];
            }
        }

        return objects[0];
    }
}
