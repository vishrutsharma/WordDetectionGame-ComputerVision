using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bridge : MonoBehaviour
{
    [SerializeField] private ObjectDetector objectDetector;
    [SerializeField] private bool autoInit;
    [HideInInspector] public bool isBridgeCreated = false;
    private float linkDelayTime = 1;

    private void Start()
    {
        if (autoInit) Init();
    }

    private void OnApplicationQuit()
    {
        Close();
    }


    /// <summary>
    /// Creates a bridge link for communication
    /// Starting point of Ar Plugin Initialization
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateBrigdeLink()
    {
        yield return new WaitForSeconds(linkDelayTime);
        objectDetector.InitProcessing();
        yield return new WaitForEndOfFrame();
        isBridgeCreated = true;
        // StartCoroutine(ReadWord());
    }

    private void DisconnectBridgeLink()
    {
        isBridgeCreated = false;
        objectDetector.Disconnect();
    }

    public void Init()
    {
        StartCoroutine(CreateBrigdeLink());
    }

    public void Close()
    {
        DisconnectBridgeLink();
    }

    private IEnumerator ReadWord()
    {
        string word = "";
        while (true)
        {
            word = GetWordData().Item1;
            Debug.Log("Got Word:" + word);
            yield return new WaitForSeconds(1);
        }
    }

    public bool IsBridgeCreated()
    {
        return isBridgeCreated;
    }

    public bool IsBoardClear()
    {
        return GetWordData().Item1.Length == 0;
    }

    /// <summary>
    /// Return word detected by Ar Plugin
    /// </summary>
    /// <returns></returns>
    public Tuple<string, bool> GetWordData()
    {
        string word = objectDetector.GetArData().word.ToString();
        bool status = objectDetector.GetArData().status;
        return new Tuple<string, bool>(word, true);
    }

}
