using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SpellCastSystem : MonoBehaviour
{
    private List<KeyCode> keys;
    private Queue<KeyCode> keysQueue;
    
    private Coroutine castCoroutine;

    private Dictionary<string, int> castedSpells;
    private int timeRemaining = 0;

    private bool isCastStarted;

    [SerializeField] private int castTime = 3;

    private void Start() =>
        DontDestroyOnLoad(this);

    private void Awake()
    {
        castedSpells = new Dictionary<string, int>();

        keysQueue = new Queue<KeyCode>();
        keys = new List<KeyCode>
        {
#region [KEYBINDS -> FIRE]
		    KeyCode.Q, // RANGE
            KeyCode.W, // POWER
            KeyCode.E, // RADIUS
            KeyCode.R, // SPEED
#endregion
            
#region [KEYBINDS -> WATER]
		    KeyCode.A, // RANGE
            KeyCode.S, // POWER
            KeyCode.D, // RADIUS
            KeyCode.F, // SPEED
#endregion
            
#region [KEYBINDS -> EARTH]
            KeyCode.U, // RANGE
            KeyCode.I, // POWER
            KeyCode.O, // RADIUS
            KeyCode.P, // SPEED
#endregion
            
#region [KEYBINDS -> WIND]
		    KeyCode.H, // RANGE
            KeyCode.J, // POWER
            KeyCode.K, // RADIUS
            KeyCode.L  // SPEED
#endregion
        };
    }

    private void Update()
    {
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                keysQueue.Enqueue(key);

                if (keysQueue.Count == 5)
                {
                    var spell = string.Join("-", keysQueue.Select(f => f.ToString()));

                    StopCoroutine(castCoroutine);
                    StartCoroutine(OnSpellCasted(spell));

                    keysQueue.Clear();

                    timeRemaining = 0;
                    isCastStarted = false;

                    return;
                }

                if (!isCastStarted)
                    castCoroutine = StartCoroutine(OnSpellKeyPressed());
            }
        }
    }

    private void OnGUI()
    {
        float x = 0;
        if (isCastStarted)
        {
            GUI.Box(new Rect(0, 0, 35 * 5 + 10, 50), timeRemaining == 0 ? "CAST" : $"CAST: {timeRemaining} сек.");
            foreach (var key in keysQueue)
            {
                GUI.Box(new Rect(5 + x, 25, 25, 25), key.ToString());
                x += 35;
            }
        }
        
        if (castedSpells.Any())
        {
            GUILayout.BeginArea(new Rect(1920 - 300, 0, 300, 1080));
            {
                GUILayout.BeginVertical("Box");
                {
                    foreach (var spell in castedSpells)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            GUILayout.Label($"CASTED SPELL: {spell.Key}");
                            GUILayout.Label($"TIME: {castTime - spell.Value} SEC.");
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.Space(10.0f);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
    }

    private IEnumerator OnSpellCasted(string spell)
    {
        while (true)
        {
            castedSpells.Add(spell, timeRemaining);
            yield return new WaitForSeconds(castTime);
            castedSpells.Remove(spell);

            break;
        }
    }

    private IEnumerator OnSpellKeyPressed()
    {
        float i = .0f;

        isCastStarted = true;
        timeRemaining = castTime;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            i += 1f;
            timeRemaining -= 1;

            if (i >= 3)
            {
                keysQueue.Clear();
                break;
            }
        }
    }
}