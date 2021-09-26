using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    COMMANDO,
    SCOUT,
    MARKSMAN,
    HEAVY,
    NUM_UNIT_TYPES
}

public enum TextType
{
    NEUTRAL,
    VALID,
    INVALID,
    NUM_TEXT_TYPES
}

/// <summary>
/// Used as a singleton to save all parameters that will be used throughout the game, values loaded from editor
/// </summary>
public class GameProperties : MonoBehaviour
{
    private static GameProperties _instance = null;
    public static GameProperties Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(GameProperties)) as GameProperties;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    [SerializeField]
    public GameObject[] unitPrefabs = new GameObject[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    public int[] unitCosts = new int[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    public float[] spawnTimes = new float[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    public Color[] textColors = new Color[(int)TextType.NUM_TEXT_TYPES];

    [SerializeField]
    public Sprite[] unitIcons = new Sprite[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    public AudioClip[] gunSFX = new AudioClip[(int)UnitType.NUM_UNIT_TYPES];
}
